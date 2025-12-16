using AutoMapper;
using CRM.Application.Dtos.Lead;
using CRM.Application.Interfaces.Leads;
using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Services
{
    public sealed class LeadService(
            ILeadRepository leadRepository,
            IRepository<LeadPotentialLevel> leadPotentialLevelRepository,
            IRepository<LeadStage> leadStageRepository,
            IRepository<LeadSource> leadSourceRepository,
            IRepository<Customer> customerRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMemoryCache memoryCache) : ILeadService
    {
        public async Task<Result> AddItemToLeadAsync(int leadId, int productId)
        {
            try
            {
                var lead = await leadRepository.GetLeadByIdAsync(leadId);

                if (lead == null)
                {
                    return Result.Failure(new Error("LEAD_NOT_FOUND", $"Không tìm thấy lead với Id: {leadId}"));
                }

                if (lead.LeadItems.Any(li => li.ProductId == productId))
                {
                    return Result.Failure(new Error("LEAD_ITEM_EXISTS", $"Sản phẩm với Id: {productId} đã tồn tại trong lead Id: {leadId}"));
                }

                var leadItem = new LeadItem
                {
                    LeadId = leadId,
                    ProductId = productId
                };

                lead.LeadItems.Add(leadItem);

                leadRepository.Update(lead);

                return Result.Success(lead);
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("ADD_LEAD_ITEM_EXCEPTION", $"Lỗi xảy ra khi thêm sản phẩm với Id: {productId} vào lead Id: {leadId}. Chi tiết: {ex.Message}"));
            }
        }

        public async Task<Result<LeadDto>> AddLeadAsync(AddLeadRequest request)
        {
            try
            {
                var lead = new Lead
                {
                    LeadName = request.Name,
                    LeadAddress = request.Address,
                    LeadPhone = request.Phone,
                    LeadEmail = request.Email,
                    LeadSourceId = request.SourceId,
                    LeadPotentialLevelId = request.PotentialLevelId,
                    LeadStageId = 1,
                    EmployeeId = request.EmployeeId,
                    CreateDate = request.StartDate,
                    EndDate = request.EndDate,
                };

                if (request.LeadItems != null && request.LeadItems.Any())
                {
                    lead.LeadItems = request.LeadItems.Select(li => new LeadItem
                    {
                        ProductId = li.ProductId,
                    }).ToList();
                }

                var leadResult = await leadRepository.AddAsync(lead);
                await unitOfWork.SaveChangesAsync();

                var leadDto = mapper.Map<LeadDto>(leadResult);

                if (leadResult != null)
                {
                    //memoryCache.Remove($"Lead_{lead.LeadId}");
                    return Result.Success<LeadDto>(leadDto);
                }

                return Result.Failure<LeadDto>(new Error("ADD_LEAD_FAILED", $"Lỗi xảy ra khi thêm 1 lead"));
            }
            catch (Exception ex)
            {
                return Result.Failure<LeadDto>(new Error("ADD_LEAD_EXCEPTION", $"Lỗi xảy ra khi thêm 1 lead: {ex.Message}"));
            }
        }

        public async Task<Result> DeleteLeadAsync(int leadId)
        {
            try
            {
                var lead = await leadRepository.GetByIdAsync(leadId);

                if (lead == null)
                {
                    return Result.Failure(new("not_found", "không tìm thấy khashc hàng tiềm năng"));
                }

                leadRepository.Remove(lead);

                var deleted = await unitOfWork.SaveChangesAsync();

                if (deleted > 0)
                {
                    return Result.Success();
                }

                return Result.Failure(new("failed", "lỗi"));
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<LeadPotentialLevelDto>> GetAllLeadPotentialLevelsAsync()
        {
            if (memoryCache.TryGetValue("LeadPotentialLevels", out IEnumerable<LeadPotentialLevelDto> cachedLevels))
            {
                return cachedLevels;
            }

            var leadPotentialLevels = await leadPotentialLevelRepository.GetAllAsync();

            var leadPotentialLevelOptions = leadPotentialLevels.Select(lpl => new LeadPotentialLevelDto
            {
                Id = lpl.LeadPotentialLevelId,
                Name = lpl.LeadPotentialLevelName
            });

            memoryCache.Set("LeadPotentialLevels", leadPotentialLevelOptions, TimeSpan.FromHours(1));

            return leadPotentialLevelOptions;
        }

        public async Task<PagedResult<LeadDto>> GetAllLeadsAsync(GetLeadRequest request)
        {
            //if (memoryCache.TryGetValue($"Leads_{request.Keyword}_{request.PageNumber}_{request.PageSize}", out PagedResult<LeadDto> cachedLeads))
            //{
            //    return cachedLeads;
            //}

            var filter = new LeadFilter
            {
                Keyword = request.Keyword,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var leads = await leadRepository.GetLeadsAsync(filter);

            var leadDtos = mapper.Map<IEnumerable<LeadDto>>(leads.Items);

            var pagedResult = new PagedResult<LeadDto>(leadDtos, leads.TotalCount, leads.PageNumber, leads.PageSize);

            //memoryCache.Set($"Leads_{request.Keyword}_{request.PageNumber}_{request.PageSize}", pagedResult, TimeSpan.FromMinutes(1));

            return pagedResult;
        }

        public async Task<IEnumerable<LeadSourceDto>> GetAllLeadSourcesAsync()
        {
            if (memoryCache.TryGetValue("LeadSources", out IEnumerable<LeadSourceDto>? cachedSources))
            {
                return cachedSources;
            }

            var leadSources = await leadSourceRepository.GetAllAsync();

            var leadSourceOptions = leadSources.Select(ls => new LeadSourceDto
            {
                Id = ls.LeadSourceId,
                Name = ls.LeadSourceName
            });

            memoryCache.Set("LeadSources", leadSourceOptions, TimeSpan.FromHours(1));

            return leadSourceOptions;
        }

        public async Task<IEnumerable<LeadStageDto>> GetAllLeadStagesAsync()
        {
            if (memoryCache.TryGetValue("LeadStages", out IEnumerable<LeadStageDto> cachedStages))
            {
                return cachedStages;
            }

            var leadStages = await leadStageRepository.GetAllAsync();

            var leadStageOptions = leadStages.Select(ls => new LeadStageDto
            {
                Id = ls.LeadStageId,
                Name = ls.LeadStageName
            });

            memoryCache.Set("LeadStages", leadStageOptions, TimeSpan.FromHours(1));

            return leadStageOptions;
        }

        public async Task<Result<LeadDto>> GetLeadByIdAsync(int leadId)
        {
            try
            {
                if (memoryCache.TryGetValue($"Lead_{leadId}", out LeadDto? cachedLead))
                {
                    return Result.Success(cachedLead);
                }

                var lead = await leadRepository.GetLeadByIdAsync(leadId);

                if (lead == null)
                {
                    return Result.Failure<LeadDto>(new Error("LEAD_NOT_FOUND", $"Không tìm thấy lead với Id: {leadId}"));
                }

                var leadDto = mapper.Map<LeadDto>(lead);

                memoryCache.Set($"Lead_{leadId}", leadDto, TimeSpan.FromMinutes(10));

                return Result.Success(leadDto);
            }
            catch (Exception ex)
            {
                return Result.Failure<LeadDto>(new Error("GET_LEAD_EXCEPTION", $"Lỗi xảy ra khi lấy lead với Id: {leadId}. Chi tiết: {ex.Message}"));
            }
        }

        public async Task<Result> RemoveProductFromLeadAsync(int leadId, int productId)
        {
            try
            {
                var lead = await leadRepository.GetLeadByIdAsync(leadId);

                if (lead == null)
                {
                    return Result.Failure(new Error("LEAD_NOT_FOUND", $"Không tìm thấy lead với Id: {leadId}"));
                }

                var leadItem = lead.LeadItems.FirstOrDefault(li => li.ProductId == productId);

                if (leadItem == null)
                {
                    return Result.Failure(new Error("LEAD_ITEM_NOT_FOUND", $"Không tìm thấy sản phẩm với Id: {productId} trong lead Id: {leadId}"));
                }

                lead.LeadItems.Remove(leadItem);

                leadRepository.Update(lead);

                var changed = await unitOfWork.SaveChangesAsync();
                if (changed > 0)
                {
                    memoryCache.Remove($"Lead_{lead.LeadId}");
                    return Result.Success();
                }
                else
                {
                    return Result.Failure(new Error("REMOVE_LEAD_ITEM_FAILED", $"Lỗi xảy ra khi xóa sản phẩm với Id: {productId} từ lead Id: {leadId}"));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("REMOVE_LEAD_ITEM_EXCEPTION", $"Lỗi xảy ra khi xóa sản phẩm với Id: {productId} từ lead Id: {leadId}. Chi tiết: {ex.Message}"));
            }
        }

        public async Task<Result<LeadDto>> UpdateLeadAsync(UpdateLeadStageRequest request)
        {
            await unitOfWork.BeginTransactionAsync();
            var lead = await leadRepository.GetLeadByIdAsync(request.Id);

            if (lead == null)
            {
                return Result.Failure<LeadDto>(new Error("LEAD_NOT_FOUND", $"Không tìm thấy lead với Id: {request.Id}"));
            }

            lead.LeadStageId = request.StatusId;

            leadRepository.Update(lead);

            //if (request.StatusId == 3) // Chuyển sang "Đã chuyển đổi"
            //{
            //    // tao 1 customer moi tu lead
            //    var customer = new Customer
            //    {
            //        CustomerName = lead.LeadName,
            //        CustomerAddress = lead.LeadAddress,
            //        CustomerPhone = lead.LeadPhone,
            //        CustomerEmail = lead.LeadEmail,
            //        EmployeeId = lead.EmployeeId,
            //        CreateDate = DateTime.Now,
            //        CreateBy = lead.EmployeeId.ToString()
            //    };

            //    await _customerRepository.AddAsync(customer);
            //}

            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitTransactionAsync();

            memoryCache.Remove($"Lead_{lead.LeadId}");

            var leadDto = mapper.Map<LeadDto>(lead);
            return Result.Success(leadDto);
        }

        public async Task<Result<LeadDto>> UpdateLeadAsync(UpdateLeadRequest request)
        {
            try
            {
                var lead = await leadRepository.GetLeadByIdAsync(request.Id);
                if (lead == null)
                {
                    return Result.Failure<LeadDto>(new Error("LEAD_NOT_FOUND", $"Không tìm thấy lead với Id: {request.Id}"));
                }

                if (string.IsNullOrEmpty(request.Name))
                {
                    return Result.Failure<LeadDto>(new Error("LEAD_NAME_REQUIRED", $"Tên khách hàng tiềm năng không được để trống."));
                }

                if (string.IsNullOrEmpty(request.Email))
                {
                    return Result.Failure<LeadDto>(new Error("LEAD_EMAIL_REQUIRED", $"Email khách hàng tiềm năng không được để trống."));
                }

                if (!string.IsNullOrEmpty(request.Email) && !new EmailAddressAttribute().IsValid(request.Email))
                {
                    return Result.Failure<LeadDto>(new Error("LEAD_EMAIL_INVALID", $"Email khách hàng tiềm năng không đúng định dạng."));
                }

                lead.LeadName = request.Name;
                //lead.LeadCompany = request.Company;
                lead.LeadPhone = request.Phone;
                lead.LeadEmail = request.Email;
                lead.LeadAddress = request.Address;
                lead.LeadPotentialLevelId = request.PotentialLevelId;
                lead.LeadDescription = request.Description;
                lead.LeadStageId = request.StatusId;
                lead.CreateDate = request.StartDate;
                lead.EndDate = request.EndDate;

                leadRepository.Update(lead);
                var changed = await unitOfWork.SaveChangesAsync();
                if (changed > 0)
                {
                    var leadDto = mapper.Map<LeadDto>(lead);
                    memoryCache.Remove($"Lead_{lead.LeadId}");
                    return Result.Success<LeadDto>(leadDto);
                }
                else
                {
                    return Result.Failure<LeadDto>(new Error("UPDATE_LEAD_FAILED", $"Lỗi xảy ra khi cập nhật lead với Id: {request.Id}"));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<LeadDto>(new Error("UPDATE_LEAD_EXCEPTION", $"Lỗi xảy ra khi cập nhật lead với Id: {request.Id}. Chi tiết: {ex.Message}"));
            }
        }

        public async Task<Result<LeadDto>> UpdateLeadStageAsync(int leadId, int stageId)
        {
            try
            {
                var lead = await leadRepository.GetLeadByIdAsync(leadId);

                if (lead == null)
                {
                    return Result.Failure<LeadDto>(new Error("LEAD_NOT_FOUND", $"Không tìm thấy lead với Id: {leadId}"));
                }

                //lead.LeadStageId = stageId;

                //leadRepository.Update(lead);

                await leadRepository.UpdateLeadStageAsync(leadId, stageId);

                var changed = await unitOfWork.SaveChangesAsync();

                var newLead = await leadRepository.GetLeadByIdAsync(leadId);

                var leadDto = mapper.Map<LeadDto>(newLead);

                memoryCache.Remove($"Lead_{lead.LeadId}");
                return Result.Success<LeadDto>(leadDto);

                //if (changed > 0)
                //{
                //    memoryCache.Remove($"Lead_{lead.LeadId}");
                //    return Result.Success<LeadDto>(leadDto);
                //}
                //else
                //{
                //    return Result.Failure<LeadDto>(new Error("UPDATE_LEAD_STAGE_FAILED", $"Lỗi xảy ra khi cập nhật giai đoạn lead"));
                //}
            }
            catch (Exception ex)
            {
                return Result.Failure<LeadDto>(new Error("UPDATE_LEAD_STAGE_EXCEPTION", $"Lỗi xảy ra khi cập nhật giai đoạn lead: {ex.Message}"));
            }
        }
    }
}

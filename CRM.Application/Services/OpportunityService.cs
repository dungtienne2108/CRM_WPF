using AutoMapper;
using CRM.Application.Dtos.Customer;
using CRM.Application.Dtos.Employee;
using CRM.Application.Dtos.Lead;
using CRM.Application.Dtos.Opportunity;
using CRM.Application.Interfaces.Opportunity;
using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace CRM.Application.Services
{
    public class OpportunityService(
        IRepository<OpportunityStage> opportunityStageRepository,
        IOpportunityRepository opportunityRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IMemoryCache memoryCache) : IOpportunityService
    {
        public async Task<Result> AddItemToOpportunityAsync(AddOpportunityItemRequest request)
        {
            try
            {
                var opportunity = await opportunityRepository.GetOpportunityByIdAsync(request.OpportunityId);
                if (opportunity == null)
                {
                    return Result.Failure(new("opportunity_not_found", "Không tìm thấy cơ hội"));
                }

                var existingProductInItems = opportunity.OpportunityItems.FirstOrDefault(i => i.ProductId == request.ProductId);
                if (existingProductInItems != null)
                {
                    return Result.Failure(new("", "Sản phẩm đã tồn tại trong cơ hội"));
                }

                var product = await productRepository.GetByIdAsync(request.ProductId);
                if (product == null)
                {
                    return Result.Failure(new("", "Không tìm thấy sản phẩm"));
                }

                if (product.ProductStatusId == 2)
                {
                    return Result.Failure(new("", "Sản phẩm đã được giữ chỗ"));
                }
                else if (product.ProductStatusId == 3)
                {
                    return Result.Failure(new("", "Sản phẩm đã được bán"));
                }

                opportunity.OpportunityItems.Add(
                    new OpportunityItem
                    {
                        Opportunity = opportunity,
                        ProductId = request.ProductId,
                        SalePrice = request.Price,
                        Quantity = 1
                    });

                //product.ProductStatusId = 2; // chuyển sản phẩm thành đã giữ chỗ (id = 2)

                var added = await unitOfWork.SaveChangesAsync();
                if (added > 0)
                {
                    memoryCache.Remove($"Opportunity_{request.OpportunityId}");
                    memoryCache.Remove($"Products_Opportunity_{request.OpportunityId}");
                    return Result.Success();
                }
                else
                {
                    return Result.Failure(new("", "thêm sản phẩm vào thất bại"));
                }
            }
            catch { throw; }
        }

        public async Task<OpportunityDto> AddOpportunityAsync(AddOpportunityRequest request)
        {
            await unitOfWork.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrWhiteSpace(request.OpportunityName))
                {
                    Log.Warning("Tên cơ hội không được để trống");
                    throw new ArgumentException("Tên cơ hội không được để trống");
                }

                if (request.OpportunityItems == null || !request.OpportunityItems.Any())
                {
                    Log.Warning("Cơ hội phải có ít nhất một sản phẩm");
                    throw new ArgumentException("Cơ hội phải có ít nhất một sản phẩm");
                }

                var productIds = request.OpportunityItems.Select(item => item.ProductId).ToList();
                // Get products without tracking first to validate
                var productsToCheck = (await productRepository.GetProductsByIdsAsync(productIds)).ToList();

                if (productsToCheck.Count != productIds.Count)
                {
                    Log.Warning("Một hoặc nhiều sản phẩm không tồn tại");
                    throw new ArgumentException("Một hoặc nhiều sản phẩm không tồn tại");
                }

                // Kiểm tra trạng thái sản phẩm nếu không phải là "Chưa bán" (id = 1)
                if (productsToCheck.Any(p => p.ProductStatusId != 1))
                {
                    Log.Warning("Một hoặc nhiều sản phẩm đã được giữ chỗ");
                    throw new ArgumentException("Một hoặc nhiều sản phẩm đã được giữ chỗ");
                }

                // Cập nhật trạng thái sản phẩm thành "Đã giữ chỗ" (id = 2)
                // Use dedicated method to avoid tracking conflicts
                await productRepository.UpdateProductStatusByIdsAsync(productIds, 2);

                var opportunity = new Opportunity
                {
                    OpportunityName = request.OpportunityName,
                    OpportunityDescription = request.OpportunityDescription,
                    OpportunityStageId = request.OpportunityStatusId,
                    CustomerId = request.CustomerId,
                    EmployeeId = request.EmployeeId,
                    CreateDate = request.StartDate,
                    EndDate = request.EndDate.HasValue ? DateOnly.FromDateTime(request.EndDate.Value) : default,
                    CreateBy = request.EmployeeId.ToString()
                };

                var opportunityItems = request.OpportunityItems.Select(item => new OpportunityItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    SalePrice = item.Price,
                    ExceptedProfit = item.ExpectedPrice
                }).ToList();

                opportunity.OpportunityItems = opportunityItems;

                await opportunityRepository.AddAsync(opportunity);
                await unitOfWork.SaveChangesAsync();

                //unitOfWork.ClearChangeTracker();

                //memoryCache.Remove($"Opportunity_{opportunity.OpportunityId}");
                await unitOfWork.CommitTransactionAsync();
                await unitOfWork.ReloadEntityAsync(opportunity);
                Log.Information("Tạo cơ hội bán hàng thành công với ID: {OpportunityId}", opportunity.OpportunityId);

                return new OpportunityDto
                {
                    OpportunityId = opportunity.OpportunityId,
                    OpportunityCode = opportunity.OpportunityCode,
                    OpportunityName = opportunity.OpportunityName,
                    OpportunityDescription = opportunity.OpportunityDescription,
                    EndDate = opportunity.EndDate,
                    CreateDate = opportunity.CreateDate,
                    Customer = opportunity.Customer != null ? mapper.Map<CustomerDto>(opportunity.Customer) : null!,
                    Employee = opportunity.Employee != null ? mapper.Map<EmployeeDto>(opportunity.Employee) : null!,
                    OpportunityStatus = opportunity.OpportunityStage != null ? new OpportunityStatusOption
                    {
                        Id = opportunity.OpportunityStage.OpportunityStageId,
                        Name = opportunity.OpportunityStage.OpportunityStageName
                    } : null!,
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Lỗi khi tạo cơ hội bán hàng: {Message}", ex.Message);
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<Result> DeleteOpportunityAsync(int opportunityId)
        {
            try
            {
                var opportunity = await opportunityRepository.GetByIdAsync(opportunityId);

                if (opportunity == null)
                {
                    return Result.Failure(new("not_found", "Không tìm thấy cơ hội bán hàng"));
                }

                opportunityRepository.Remove(opportunity);

                var deleted = await unitOfWork.SaveChangesAsync();

                if (deleted > 0)
                {
                    memoryCache.Remove($"Opportunity_{opportunityId}");
                    return Result.Success();
                }

                return Result.Failure(new("faield", "lỗi"));
            }
            catch { throw; }
        }

        public async Task<PagedResult<OpportunityDto>> GetAllOpportunitiesAsync(GetOpportunityRequest request)
        {
            //if (memoryCache.TryGetValue($"Opportunities_{request.Keyword}_{request.OpportunityStageId}_{request.PageNumber}_{request.PageSize}", out PagedResult<OpportunityDto>? cachedResult))
            //{
            //    return cachedResult;
            //}

            var filter = new OpportunityFilter
            {
                Keyword = request.Keyword,
                OpportunityStageId = request.OpportunityStageId,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var pagedOpportunities = await opportunityRepository.GetAllOpportunitiesAsync(filter);

            var opportunityDtos = mapper.Map<List<OpportunityDto>>(pagedOpportunities.Items);

            var pagedResult = new PagedResult<OpportunityDto>(opportunityDtos, pagedOpportunities.TotalCount, pagedOpportunities.PageNumber, pagedOpportunities.PageSize);

            //memoryCache.Set($"Opportunities_{request.Keyword}_{request.OpportunityStageId}_{request.PageNumber}_{request.PageSize}", pagedResult, TimeSpan.FromMinutes(1));

            return pagedResult;
        }

        public async Task<List<OpportunityStatusOption>> GetAllOpportunityStatusesAsync()
        {
            if (memoryCache.TryGetValue("OpportunityStatuses", out List<OpportunityStatusOption>? cachedStatuses))
            {
                return cachedStatuses;
            }

            var stages = await opportunityStageRepository.GetAllAsync();
            var opportunityStageOptions = stages
                .Select(s => new OpportunityStatusOption
                {
                    Id = s.OpportunityStageId,
                    Name = s.OpportunityStageName
                })
                .ToList();

            memoryCache.Set("OpportunityStatuses", opportunityStageOptions, TimeSpan.FromHours(1));

            return opportunityStageOptions;
        }

        public async Task<Result<IEnumerable<OpportunityDto>>> GetOpportunitiesByCustomerIdAsync(int customerId)
        {
            try
            {
                var oppotunities = await opportunityRepository.GetOpportunitiesByCustomerIdAsync(customerId);

                if (oppotunities.Count() == 0)
                {
                    return Result.Failure<IEnumerable<OpportunityDto>>(new("NOT_FOUND", "Không có cơ hội nào gắn với khashc hàng"));
                }

                var opportunitiesDto = mapper.Map<List<OpportunityDto>>(oppotunities);

                return opportunitiesDto;
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<OpportunityDto>>(new("FAILED", ex.Message));
            }
        }

        public async Task<Result<OpportunityDto>> GetOpportunityByIdAsync(int id)
        {
            try
            {
                if (memoryCache.TryGetValue($"Opportunity_{id}", out OpportunityDto? cachedOpportunity))
                {
                    return Result.Success(cachedOpportunity);
                }

                var opportunity = await opportunityRepository.GetOpportunityByIdAsync(id);

                if (opportunity == null)
                {
                    return Result.Failure<OpportunityDto>(new Error("NOT_FOUND", "Cơ hội không tồn tại"));
                }

                var opportunityDto = mapper.Map<OpportunityDto>(opportunity);

                memoryCache.Set($"Opportunity_{id}", opportunityDto, TimeSpan.FromMinutes(10));

                return Result.Success(opportunityDto);
            }
            catch (Exception ex)
            {
                return Result.Failure<OpportunityDto>(new Error("RETRIEVE_FAILED", $"Lấy thông tin cơ hội thất bại: {ex.Message}"));
            }
        }

        public async Task<Result> RemoveProductFromOpportunityAsync(int opportunityId, int productId)
        {
            try
            {
                var opportunity = await opportunityRepository.GetOpportunityByIdAsync(opportunityId);
                if (opportunity == null)
                {
                    return Result.Failure(new("OPPORTUNITY_NOT_FOUND", "Cơ hội không tồn tại"));
                }

                var itemToRemove = opportunity.OpportunityItems.FirstOrDefault(i => i.ProductId == productId);
                if (itemToRemove == null)
                {
                    return Result.Failure(new("ITEM_NOT_FOUND", "Sản phẩm không tồn tại trong cơ hội"));
                }

                opportunity.OpportunityItems.Remove(itemToRemove);
                var removed = await unitOfWork.SaveChangesAsync();
                if (removed > 0)
                {
                    memoryCache.Remove($"Opportunity_{opportunityId}");
                    return Result.Success();
                }
                else
                {
                    return Result.Failure(new("REMOVE_FAILED", "Xóa sản phẩm khỏi cơ hội thất bại"));
                }
            }
            catch { throw; }
        }

        public async Task<Result<OpportunityDto>> UpdateOpportunityAsync(UpdateOpportunityRequest request)
        {
            try
            {
                var opportunity = await opportunityRepository.GetByIdAsync(request.OpportunityId);
                if (opportunity == null)
                {
                    return Result.Failure<OpportunityDto>(new Error("OPPORTUNITY_NOT_FOUND", "Cơ hội không tồn tại"));
                }

                opportunity.OpportunityName = request.OpportunityName;
                opportunity.CreateDate = request.StartDate;
                opportunity.EndDate = request.EndDate.HasValue ? DateOnly.FromDateTime(request.EndDate.Value) : DateOnly.MaxValue;
                opportunity.OpportunityDescription = request.Description;

                opportunityRepository.Update(opportunity);
                var updated = await unitOfWork.SaveChangesAsync();
                if (updated == 0)
                {
                    return Result.Failure<OpportunityDto>(new Error("UPDATE_FAILED", "Cập nhật cơ hội thất bại"));
                }
                var updatedOpportunity = mapper.Map<OpportunityDto>(opportunity);

                memoryCache.Remove($"Opportunity_{request.OpportunityId}");

                return Result.Success(updatedOpportunity);
            }
            catch (Exception ex)
            {
                return Result.Failure<OpportunityDto>(new Error("UPDATE_FAILED", $"Cập nhật cơ hội thất bại: {ex.Message}"));
            }
        }

        public async Task<Result<OpportunityDto>> UpdateOpportunityStageAsync(int opportunityId, int newStageId)
        {
            try
            {
                var opportunity = await opportunityRepository.GetOpportunityByIdAsync(opportunityId);

                if (opportunity == null)
                {
                    return Result.Failure<OpportunityDto>(new Error("OPPORTUNITY_NOT_FOUND", "Cơ hội không tồn tại"));
                }

                //opportunity.OpportunityStageId = newStageId;

                //opportunityRepository.Update(opportunity);

                await opportunityRepository.UpdateOpportunityStageAsync(opportunityId, newStageId);


                if (opportunity.OpportunityStageId == 1 || opportunity.OpportunityStageId == 4)
                {
                    var productIds = opportunity.OpportunityItems
                        .Where(item => item.ProductId.HasValue)
                        .Select(item => item.ProductId!.Value)
                        .Distinct()
                        .ToList();

                    //var products = await productRepository.GetProductsByIdsAsync(productIds);

                    //// chuyển thành đã giữ chỗ (id = 2)
                    //foreach (var product in products)
                    //{
                    //    product.ProductStatusId = 2;
                    //}

                    await productRepository.UpdateProductStatusByIdsAsync(productIds, 2);
                }

                var updated = await unitOfWork.SaveChangesAsync();

                //await unitOfWork.ReloadEntityAsync(opportunity);

                //if (updated == 0)
                //{
                //    return Result.Failure<OpportunityDto>(new Error("UPDATE_FAILED", "Cập nhật trạng thái cơ hội thất bại"));
                //}

                var newOpportunity = await opportunityRepository.GetOpportunityByIdAsync(opportunityId);

                var updatedOpportunity = mapper.Map<OpportunityDto>(newOpportunity);

                memoryCache.Remove($"Opportunity_{opportunityId}");

                return Result.Success(updatedOpportunity);
            }
            catch (Exception ex)
            {
                return Result.Failure<OpportunityDto>(new Error("UPDATE_FAILED", $"Cập nhật trạng thái cơ hội thất bại: {ex.Message}"));
            }
        }
    }
}

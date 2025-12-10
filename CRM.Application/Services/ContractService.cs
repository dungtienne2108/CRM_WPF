using AutoMapper;
using CRM.Application.Dtos.Contract;
using CRM.Application.Interfaces.Contract;
using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.Extensions.Caching.Memory;

namespace CRM.Application.Services
{
    public class ContractService(
        IProductRepository productRepository,
        IContractRepository contractRepository,
        IDepositRepository depositRepository,
        IEmployeeRepository employeeRepository,
        IRepository<ContractStage> contractStageRepository,
        IRepository<ContractType> contractTypeRepository,
        IRepository<InstallmentSchedule> installmentScheduleRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IMemoryCache memoryCache) : IContractService
    {
        public async Task<Result<int>> CreateContractAsync(CreateContractRequest request)
        {
            try
            {
                var contractType = await contractTypeRepository.GetByIdAsync(request.ContractTypeId);
                if (contractType == null)
                {
                    return Result.Failure<int>(new("Contract_type_not_found.", "Không tìm thấy loại hợp đồng"));
                }

                var contractStage = await contractStageRepository.GetByIdAsync(request.ContractStageId);
                if (contractStage == null)
                {
                    return Result.Failure<int>(new("Contract_stage_not_found.", "Không tìm thấy giai đoạn hợp đồng"));
                }

                var deposit = await depositRepository.GetByIdAsync(request.DepositId);
                if (deposit == null)
                {
                    return Result.Failure<int>(new("Deposit_not_found.", "Không tìm thấy đặt cọc"));
                }

                var employee = await employeeRepository.GetByIdAsync(request.EmployeeId);
                if (employee == null)
                {
                    return Result.Failure<int>(new("Employee_not_found.", "Không tìm thấy nhân viên"));
                }

                if (request.StartDate >= request.EndDate)
                {
                    return Result.Failure<int>(new("Invalid_dates.", "Ngày bắt đầu phải trước ngày kết thúc"));
                }

                if (request.Amount < 0)
                {
                    return Result.Failure<int>(new("Invalid_total_amount.", "Tổng số tiền phải lớn hơn hoặc bằng 0"));
                }

                if (request.Amount < deposit.DepositCost)
                {
                    return Result.Failure<int>(new("Invalid_amount_vs_deposit.", "Tổng số tiền phải lớn hơn hoặc bằng số tiền đặt cọc"));
                }

                var product = await productRepository.GetByIdAsync(request.ProductId);

                if (product == null)
                {
                    return Result.Failure<int>(new("Product_not_found.", "Sản phẩm không tồn tại."));
                }

                if (request.Amount < product.ProductPrice)
                {
                    return Result.Failure<int>(new("Invalid_amount_vs_product_price.", "Tổng số tiền phải lớn hơn hoặc bằng giá sản phẩm"));
                }

                var contract = new Contract
                {
                    ContractNumber = request.ContractNumber,
                    ContractName = request.ContractName,
                    ContractTypeId = request.ContractTypeId,
                    ContractStageId = request.ContractStageId,
                    CustomerId = request.CustomerId,
                    Seller = request.Seller,
                    AmountBeforeTax = request.AmountBeforeTax,
                    AmountAfterTax = request.AmountAfterTax,
                    EmployeeId = request.EmployeeId,
                    DepositId = request.DepositId,
                    Tax = request.Tax,
                    Amount = request.Amount,
                    ContractStartDate = DateOnly.FromDateTime(request.StartDate),
                    ContractEndDate = DateOnly.FromDateTime(request.EndDate),
                    ProductId = request.ProductId,
                };

                deposit.IsCreatedContract = true;
                await contractRepository.AddAsync(contract);

                var installmentSchedule = new InstallmentSchedule
                {
                    Contract = contract,
                    DueDate = DateOnly.FromDateTime(request.EndDate),
                    Amount = deposit.DepositCost.HasValue ? deposit.DepositCost.Value : 0,
                    Status = "Chờ thanh toán",
                    InstallmentName = "Đặt cọc",
                    ContractValuePercentage = deposit.DepositCost.HasValue ?
                        deposit.DepositCost.Value / request.Amount
                        : 0,
                    IsDeposited = true,
                };

                await installmentScheduleRepository.AddAsync(installmentSchedule);

                // chuyển sản phẩm sang đã bán
                product.ProductStatusId = 3; // Đã bán

                var added = await unitOfWork.SaveChangesAsync();
                if (added <= 0)
                {
                    return Result.Failure<int>(new("Create_contract_failed.", "Tạo hợp đồng thất bại"));
                }

                //memoryCache.Remove($"Contract_{contract.ContractId}");

                return Result.Success(contract.ContractId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Result> DeleteContractAsync(int contractId)
        {
            try
            {
                var contract = await contractRepository.GetByIdAsync(contractId);

                if (contract == null)
                {
                    return Result.Failure(new("Contract_not_found.", "Không tìm thấy hợp đồng"));
                }

                contractRepository.Remove(contract);
                var deleted = await unitOfWork.SaveChangesAsync();
                if (deleted <= 0)
                {
                    return Result.Failure(new("Delete_contract_failed.", "Xóa hợp đồng thất bại"));
                }
                return Result.Success();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Result<ContractDto>> GetContractByIdAsync(int contractId)
        {
            try
            {
                //if (memoryCache.TryGetValue($"Contract_{contractId}", out ContractDto cachedContract))
                //{
                //    return Result.Success(cachedContract);
                //}

                var contract = await contractRepository.GetContractByIdAsync(contractId);

                if (contract == null)
                {
                    return Result.Failure<ContractDto>(new("Contract_not_found.", "Không tìm thấy hợp đồng"));
                }

                var contractDto = mapper.Map<ContractDto>(contract);

                //memoryCache.Set($"Contract_{contractId}", contractDto, TimeSpan.FromMinutes(10));

                return Result.Success<ContractDto>(contractDto);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedResult<ContractDto>> GetContractsAsync(GetContractRequest request)
        {
            try
            {
                //if (memoryCache.TryGetValue($"Contracts_{request.Keyword}_{request.PageNumber}_{request.PageSize}", out PagedResult<ContractDto> cachedContracts))
                //{
                //    return cachedContracts;
                //}

                var filter = new ContractFilter
                {
                    Keyword = request.Keyword,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,

                };

                var contracts = await contractRepository.GetContractsAsync(filter);

                var contractDtos = mapper.Map<IEnumerable<ContractDto>>(contracts.Items);

                //memoryCache.Set($"Contracts_{request.Keyword}_{request.PageNumber}_{request.PageSize}", new PagedResult<ContractDto>(contractDtos, contracts.TotalCount, contracts.PageNumber, contracts.PageSize), TimeSpan.FromMinutes(1));

                return new(contractDtos, contracts.TotalCount, contracts.PageNumber, contracts.PageSize);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Result<IEnumerable<ContractDto>>> GetContractsByCustomerIdAsync(int customerId)
        {
            try
            {
                var contracts = await contractRepository.GetContractsByCustomerIdAsync(customerId);

                if (contracts.Count() == 0)
                {
                    return Result.Failure<IEnumerable<ContractDto>>(new("NOT_FOUND", "Không thấy hợp đồng nào của khashc hàng"));
                }

                var contractDtos = mapper.Map<List<ContractDto>>(contracts);

                return contractDtos;
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<ContractDto>>(new("FAILED", ex.Message));
            }
        }

        public async Task<Result<IEnumerable<ContractStageOption>>> GetContractStagesAsync()
        {
            try
            {
                if (memoryCache.TryGetValue("ContractStages", out IEnumerable<ContractStageOption>? cachedStages))
                {
                    return Result.Success(cachedStages);
                }

                var stages = await contractStageRepository.GetAllAsync();
                var stageOptions = stages
                    .Select(s => new ContractStageOption
                    {
                        Id = s.ContractStageId,
                        Name = s.ContractStageName
                    });

                memoryCache.Set("ContractStages", stageOptions, TimeSpan.FromHours(1));

                return Result.Success(stageOptions);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Result<IEnumerable<ContractTypeOption>>> GetContractTypesAsync()
        {
            try
            {
                if (memoryCache.TryGetValue("ContractTypes", out IEnumerable<ContractTypeOption>? cachedTypes))
                {
                    return Result.Success(cachedTypes);
                }

                var types = await contractTypeRepository.GetAllAsync();

                var typesOptions = types
                    .Select(t => new ContractTypeOption
                    {
                        Id = t.ContractTypeId,
                        Name = t.ContractTypeName
                    });

                memoryCache.Set("ContractTypes", typesOptions, TimeSpan.FromHours(1));

                return Result.Success(typesOptions);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Result> RemoveContractImageAsync(int contractId, int documentId)
        {
            try
            {
                var contract = await contractRepository.GetContractByIdAsync(contractId);

                if (contract == null)
                {
                    return Result.Failure(new("Contract_not_found.", "Không tìm thấy hợp đồng"));
                }

                var document = contract.ContractDocuments.FirstOrDefault(d => d.Id == documentId);

                if (document == null)
                {
                    return Result.Failure(new("Image_not_found.", "Không tìm thấy ảnh hợp đồng"));
                }

                contract.ContractDocuments.Remove(document);

                contractRepository.Update(contract);

                var updated = await unitOfWork.SaveChangesAsync();

                if (updated <= 0)
                {
                    return Result.Failure(new("Remove_contract_image_failed.", "Xóa ảnh hợp đồng thất bại"));
                }

                return Result.Success();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Result<ContractDto>> UpdateContractAsync(UpdateContractRequest request)
        {
            try
            {
                var contract = await contractRepository.GetByIdAsync(request.Id);
                if (contract == null)
                {
                    return Result.Failure<ContractDto>(new("Contract_not_found.", "Không tìm thấy hợp đồng"));
                }

                contract.ContractName = request.Name;
                contract.ContractStageId = request.ContractStageId;
                contract.ContractTypeId = request.ContractTypeId;
                contract.AmountAfterTax = request.AmountAfterTax;
                contract.AmountBeforeTax = request.AmountBeforeTax;
                contract.Tax = request.Tax;
                contract.Amount = request.Amount;
                contract.ContractStartDate = DateOnly.FromDateTime(request.StartDate);
                contract.ContractEndDate = DateOnly.FromDateTime(request.EndDate);
                contract.ContractDescription = request.Description;

                contractRepository.Update(contract);

                var updated = await unitOfWork.SaveChangesAsync();
                if (updated <= 0)
                {
                    return Result.Failure<ContractDto>(new("Update_contract_failed.", "Cập nhật hợp đồng thất bại"));
                }
                //memoryCache.Remove($"Contract_{contract.ContractId}");
                var contractDto = mapper.Map<ContractDto>(contract);

                return Result.Success(contractDto);
            }
            catch (Exception ex)
            {
                return Result.Failure<ContractDto>(new("update_contract_failed", "Cập nhật hợp đồng thất bại :" + ex.Message));
            }
        }

        public async Task<Result> UploadContractImageAsync(int contractId, ContractDocumentDto contractDocument)
        {
            try
            {
                var contract = await contractRepository.GetByIdAsync(contractId);

                if (contract == null)
                {
                    return Result.Failure(new("Contract_not_found.", "Không tìm thấy hợp đồng"));
                }

                //contract.ContractImages.Add(new ContractDocument
                //{
                //    ImageUrl = imageUrl,
                //    ContractId = contractId,
                //    CreatedAt = DateTime.UtcNow,
                //});

                contract.ContractDocuments.Add(new ContractDocument
                {
                    FileName = contractDocument.FileName,
                    FilePath = contractDocument.FilePath,
                    FileSize = contractDocument.FileSize,
                    ContentType = contractDocument.ContentType,
                    ContractId = contractId,
                    CreatedAt = DateTime.UtcNow,
                });

                contractRepository.Update(contract);

                var updated = await unitOfWork.SaveChangesAsync();

                if (updated <= 0)
                {
                    return Result.Failure(new("Upload_contract_image_failed.", "Tải ảnh hợp đồng thất bại"));
                }

                return Result.Success();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

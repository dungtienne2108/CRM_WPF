using AutoMapper;
using CRM.Application.Dtos.Deposit;
using CRM.Application.Interfaces.Deposit;
using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.Extensions.Caching.Memory;

namespace CRM.Application.Services
{
    public class DepositService(
        IDepositRepository depositRepository,
        IOpportunityRepository opportunityRepository,
        ICustomerRepository customerRepository,
        IContactRepository contactRepository,
        IEmployeeRepository employeeRepository,
        IContractRepository contractRepository,
        IProductRepository productRepository,
        IRepository<InstallmentSchedule> installmentScheduleRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IMemoryCache memoryCache) : IDepositService
    {
        public async Task<Result<int>> CreateDepositAsync(CreateDepositRequest request)
        {
            try
            {
                var opportunity = await opportunityRepository.GetByIdAsync(request.OpportunityId);

                if (opportunity == null)
                {
                    return Result.Failure<int>(new("OPPORTUNITY_NOT_FOUND", "Cơ hội không tồn tại"));
                }

                if (opportunity.OpportunityStageId != 4) // phải là thành công
                {
                    return Result.Failure<int>(new("INVALID_OPPORTUNITY_STAGE", "Chỉ có thể tạo đặt cọc cho cơ hội ở trạng thái 'Thành công'"));
                }

                var customer = await customerRepository.GetByIdAsync(request.CustomerId);
                if (customer == null)
                {
                    return Result.Failure<int>(new("CUSTOMER_NOT_FOUND", "Khách hàng không tồn tại"));
                }

                var product = await productRepository.GetProductByIdAsync(request.ProductId);
                if (product == null)
                {
                    return Result.Failure<int>(new("PRODUCT_NOT_FOUND", "Sản phẩm không tồn tại"));
                }

                var existingDepositByProduct = await depositRepository.GetDepositByProductIdAsync(product.ProductId);
                // nếu đặt cọc đã tồn tại nhưng hết hạn thì cho phép đặt cọc lại và xóa đặt cọc cũ
                if (existingDepositByProduct != null)
                {
                    if (existingDepositByProduct.EndDate < DateTime.UtcNow)
                    {
                        // hết hạn xóa và cho đặt lại
                        //depositRepository.Remove(existingDepositByProduct);
                        existingDepositByProduct.IsDeleted = true;
                        depositRepository.Update(existingDepositByProduct);
                    }
                    else
                    {
                        // còn hiệu lực không cho đặt
                        return Result.Failure<int>(new("DUPLICATE_DEPOSIT_PRODUCT", "Sản phẩm đã được người khác đặt cọc."));
                    }
                }


                if (request.ContactId.HasValue)
                {
                    var contact = await contactRepository.GetByIdAsync(request.ContactId.Value);
                    if (contact == null)
                    {
                        return Result.Failure<int>(new("CONTACT_NOT_FOUND", "Liên hệ không tồn tại"));
                    }
                }

                var employee = await employeeRepository.GetByIdAsync(request.EmployeeId);
                if (employee == null)
                {
                    return Result.Failure<int>(new("EMPLOYEE_NOT_FOUND", "Nhân viên không tồn tại"));
                }

                if (request.EndDate < request.StartDate)
                {
                    return Result.Failure<int>(new("INVALID_DATE_RANGE", "Ngày kết thúc phải sau ngày bắt đầu"));
                }

                if (request.DepositCosts < 0)
                {
                    return Result.Failure<int>(new("INVALID_DEPOSIT_COST", "Chi phí đặt cọc không được âm"));
                }

                var deposit = new Deposit
                {
                    OpportunityId = request.OpportunityId,
                    CustomerId = request.CustomerId,
                    EmployeeId = request.EmployeeId,
                    ContactId = request.ContactId,
                    ProductId = request.ProductId,
                    DepositName = request.DepositName,
                    CreateDate = request.StartDate,
                    EndDate = request.EndDate,
                    DepositCost = request.DepositCosts,
                    Description = request.Description
                };

                await depositRepository.AddAsync(deposit);

                // cập nhật trạng thái sản phẩm để tránh đặt cọc trùng
                //product.ProductStatusId = 4;
                //productRepository.Update(product);
                await productRepository.UpdateProductStatusByIdAsync(product.ProductId, 4);

                var added = await unitOfWork.SaveChangesAsync();

                if (added > 0)
                {
                    //memoryCache.Remove($"Deposit_{deposit.DepositId}");
                    return Result.Success<int>(deposit.DepositId);
                }

                return Result.Failure<int>(new("CREATE_DEPOSIT_FAILED", "Tạo đặt cọc không thành công"));
            }
            catch (Exception ex)
            {
                return Result.Failure<int>(new("CREATE_DEPOSIT_FAILED", $"Lỗi khi tạo đặt cọc : {ex.Message}"));
            }
        }

        public async Task<Result> DeleteDepositAsync(int depositId)
        {
            try
            {
                var deposit = await depositRepository.GetByIdAsync(depositId);

                if (deposit == null)
                {
                    return Result.Failure(new("not_found", "không thấy đặt cọc"));
                }

                depositRepository.Remove(deposit);

                var deleted = await unitOfWork.SaveChangesAsync();

                if (deleted > 0)
                {
                    return Result.Success();
                }

                return Result.Failure(new("failed", "lỗi"));
            }
            catch { throw; }
        }

        public async Task<Result<DepositDto>> GetDepositByIdAsync(int depositId)
        {
            try
            {
                var deposit = await depositRepository.GetDepositByIdAsync(depositId);

                if (deposit == null)
                {
                    return Result.Failure<DepositDto>(new("DEPOSIT_NOT_FOUND", "Không tìm thấy đặt cọc"));
                }

                var depositDto = mapper.Map<DepositDto>(deposit);

                //memoryCache.Set($"Deposit_{depositId}", depositDto, TimeSpan.FromMinutes(10));

                return Result.Success<DepositDto>(depositDto);
            }
            catch (Exception ex)
            {
                return Result.Failure<DepositDto>(new("GET_DEPOSIT_FAILED", $"Lỗi khi lấy đặt cọc: {ex.Message}"));
            }
        }

        public async Task<PagedResult<DepositDto>> GetDepositsAsync(GetDepositRequest request)
        {
            try
            {
                var filter = new DepositFilter
                {
                    Keyword = request.Keyword,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    IsCreatedContract = request.IsCreatedContract
                };

                var depositsPaged = await depositRepository.GetDepositsAsync(filter);

                var depositDtos = mapper.Map<IEnumerable<DepositDto>>(depositsPaged.Items);

                var pagedResult = new PagedResult<DepositDto>(depositDtos, depositsPaged.TotalCount, depositsPaged.PageNumber, depositsPaged.PageSize);

                //memoryCache.Set($"Deposits_{request.Keyword}_{request.PageNumber}_{request.PageSize}_{request.IsCreatedContract}", pagedResult, TimeSpan.FromMinutes(1));

                return pagedResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách đặt cọc: {ex.Message}", ex);
            }
        }

        public async Task<Result<IEnumerable<DepositDto>>> GetDepositsByCustomerIdAsync(int customerId)
        {
            try
            {
                var deposits = await depositRepository.GetDepositsByCustomerIdAsync(customerId);

                var depositDtos = mapper.Map<IEnumerable<DepositDto>>(deposits);

                return Result.Success<IEnumerable<DepositDto>>(depositDtos);
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<DepositDto>>(new("GET_DEPOSITS_FAILED", $"Lỗi khi lấy đặt cọc của khách hàng: {ex.Message}"));
            }
        }

        public async Task<Result<DepositDto>> UpdateDepositAsync(UpdateDepositRequest request)
        {
            try
            {
                var deposit = await depositRepository.GetByIdAsync(request.DepositId);
                if (deposit == null)
                {
                    return Result.Failure<DepositDto>(new("DEPOSIT_NOT_FOUND", "Đặt cọc không tồn tại"));
                }

                deposit.DepositName = request.DepositName;
                deposit.DepositCost = request.Amount;
                deposit.CreateDate = request.StartDate;
                deposit.EndDate = request.EndDate;
                deposit.Description = request.Description;
                deposit.ContactId = request.ContactId;

                // Cập nhật lại số tiền đặt cọc trong hợp đồng và lịch thanh toán nếu có
                var contract = await contractRepository.GetContractByDepositIdAsync(deposit.DepositId);

                if (contract != null)
                {
                    var installmentSchedules = await installmentScheduleRepository.FindAsync(
                    s => s.ContractId == contract.ContractId);

                    if (installmentSchedules.Count() > 0)
                    {
                        var isDepositSchedule = installmentSchedules
                            .FirstOrDefault(s => s.IsDeposited);

                        if (isDepositSchedule != null)
                        {
                            isDepositSchedule.Amount = request.Amount;
                        }
                    }
                }

                depositRepository.Update(deposit);
                var updated = await unitOfWork.SaveChangesAsync();
                if (updated > 0)
                {
                    //memoryCache.Remove($"Deposit_{deposit.DepositId}");
                    var updatedDeposit = await depositRepository.GetDepositByIdAsync(deposit.DepositId);
                    var depositDto = mapper.Map<DepositDto>(updatedDeposit);
                    return Result.Success<DepositDto>(depositDto);
                }
                else
                {
                    return Result.Failure<DepositDto>(new("UPDATE_DEPOSIT_FAILED", "Cập nhật đặt cọc không thành công"));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<DepositDto>(new("UPDATE_DEPOSIT_FAILED", $"Lỗi khi cập nhật đặt cọc: {ex.Message}"));
            }
        }
    }
}
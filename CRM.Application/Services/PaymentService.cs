using AutoMapper;
using CRM.Application.Dtos.Payment;
using CRM.Application.Interfaces.Payment;
using CRM.Domain.Filters;
using CRM.Domain.Interfaces;
using CRM.Domain.Models;
using CRM.Shared.Results;
using Microsoft.Extensions.Caching.Memory;

namespace CRM.Application.Services
{
    public class PaymentService(
        IPaymentRepository paymentRepository,
        IRepository<InstallmentSchedule> paymentScheduleRepository,
        IRepository<PaymentMethod> paymentMethodRepository,
        IInvoiceRepository invoiceRepository,
        IContractRepository contractRepository,
        IUnitOfWork unitOfWork,
        IMemoryCache memoryCache,
        IMapper mapper
       ) : IPaymentService
    {
        public async Task<Result<int>> CreateInvoiceAsync(CreateInvoiceRequest request)
        {
            try
            {
                var contract = await contractRepository.GetByIdAsync(request.ContractId);
                if (contract == null)
                {
                    return Result.Failure<int>(new("CONTRACT_NOT_FOUND", "Hợp đồng không tồn tại"));
                }

                var installmentSchedule = await paymentScheduleRepository.GetByIdAsync(request.PaymentScheduleId);
                if (installmentSchedule == null || installmentSchedule.ContractId != request.ContractId)
                {
                    return Result.Failure<int>(new("PAYMENT_SCHEDULE_NOT_FOUND", "Kế hoạch thanh toán không tồn tại cho hợp đồng này"));
                }

                if (request.Amount > installmentSchedule.Amount)
                {
                    return Result.Failure<int>(new("AMOUNT_EXCEEDS_SCHEDULED_AMOUNT", "Số tiền hóa đơn vượt quá số tiền kế hoạch thanh toán"));
                }

                var invoice = new Invoice
                {
                    ContractId = request.ContractId,
                    InstallmentScheduleId = request.PaymentScheduleId,
                    CreateDate = request.InvoiceDate,
                    TotalAmount = request.Amount,
                    DueDate = DateOnly.FromDateTime(request.DueDate),
                    Status = InvoiceStatus.Pending
                };

                await invoiceRepository.AddAsync(invoice);
                var added = await unitOfWork.SaveChangesAsync();

                if (added > 0)
                {
                    memoryCache.Remove($"Invoice_{invoice.InvoiceId}");
                    return Result.Success(invoice.InvoiceId);
                }
                else
                {
                    return Result.Failure<int>(new("CREATE_INVOICE_FAILED", "Tạo hóa đơn thất bại"));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<int>(new("CREATE_INVOICE_FAILED", $"Tạo hóa đơn thất bại: {ex.Message}"));
            }
        }

        public async Task<Result<int>> CreatePaymentAsync(CreatePaymentRequest request)
        {
            try
            {
                var invoice = await invoiceRepository.GetByIdAsync(request.InvoiceId);
                if (invoice == null)
                {
                    return Result.Failure<int>(new("INVOICE_NOT_FOUND", "Hóa đơn không tồn tại"));
                }

                if (invoice.Status == InvoiceStatus.Paid)
                {
                    return Result.Failure<int>(new("INVOICE_ALREADY_PAID", "Hóa đơn đã được thanh toán"));
                }

                if (request.Amount > invoice.TotalAmount)
                {
                    return Result.Failure<int>(new("AMOUNT_EXCEEDS_INVOICE_TOTAL", "Số tiền thanh toán vượt quá tổng số tiền hóa đơn"));
                }

                var payment = new Payment
                {
                    InvoiceId = request.InvoiceId,
                    CustomerId = request.CustomerId,
                    PaymentMethodId = request.PaymentMethodId,
                    Amount = request.Amount,
                    PaymentDate = request.PaymentDate,
                    Description = request.Description,
                    RemainAmount = request.RemainAmount
                };

                await paymentRepository.AddAsync(payment);

                var added = await unitOfWork.SaveChangesAsync();
                if (added > 0)
                {
                    memoryCache.Remove($"Payment_{payment.PaymentId}");
                    return Result.Success(payment.PaymentId);
                }
                else
                {
                    return Result.Failure<int>(new("CREATE_PAYMENT_FAILED", "Tạo thanh toán thất bại"));
                }

            }
            catch (Exception ex)
            {
                return Result.Failure<int>(new("CREATE_PAYMENT_FAILED", $"Tạo thanh toán thất bại: {ex.Message}"));
            }
        }

        public async Task<Result<int>> CreatePaymentScheduleAsync(CreatePaymentScheduleRequest request)
        {
            try
            {
                var contract = await contractRepository.GetByIdAsync(request.ContractId);

                if (contract == null)
                {
                    return Result.Failure<int>(new("CONTRACT_NOT_FOUND", "Hợp đồng không tồn tại"));
                }

                // tìm những kế hoạch thanh toán đã có cho hợp đồng 
                var existingSchedules = await paymentScheduleRepository.FindAsync(ps => ps.ContractId == request.ContractId);
                var totalAmount = existingSchedules.Sum(ps => ps.Amount);

                if (totalAmount + request.Amount > contract.AmountAfterTax)
                {
                    return Result.Failure<int>(new("AMOUNT_EXCEEDS_CONTRACT_VALUE", "Tổng số tiền kế hoạch thanh toán vượt quá giá trị hợp đồng"));
                }

                var installmentSchedule = new InstallmentSchedule
                {
                    ContractId = request.ContractId,
                    InstallmentName = request.InstallmentName,
                    Amount = request.Amount,
                    ContractValuePercentage = request.ContractValuePercentage,
                    DueDate = DateOnly.FromDateTime(request.DueDate),
                    Status = "Chưa thanh toán"
                };

                await paymentScheduleRepository.AddAsync(installmentSchedule);

                var added = await unitOfWork.SaveChangesAsync();

                if (added > 0)
                {
                    memoryCache.Remove($"PaymentSchedules_Contract_{request.ContractId}");
                    return Result.Success(installmentSchedule.InstallmentId);
                }
                else
                {
                    return Result.Failure<int>(new("CREATE_PAYMENT_SCHEDULE_FAILED", "Tạo kế hoạch thanh toán thất bại"));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<int>(new("CREATE_PAYMENT_SCHEDULE_FAILED", $"Tạo kế hoạch thanh toán thất bại: {ex.Message}"));

            }
        }

        public async Task<Result> DeletePaymentAsync(int paymentId)
        {
            try
            {
                var payment = await paymentRepository.GetByIdAsync(paymentId);

                if (payment == null)
                {
                    return Result.Failure(new("not_found", "Không thấy thanh toán"));
                }

                paymentRepository.Remove(payment);

                var deleted = await unitOfWork.SaveChangesAsync();

                if (deleted > 0)
                {
                    memoryCache.Remove($"Payment_{paymentId}");
                    return Result.Success();
                }

                return Result.Failure(new("failed", "lỗi"));
            }
            catch { throw; }
        }

        public async Task<Result<InvoiceDto>> GetInvoiceByIdAsync(int invoiceId)
        {
            try
            {
                if (memoryCache.TryGetValue($"Invoice_{invoiceId}", out InvoiceDto cachedInvoice))
                {
                    return Result.Success(cachedInvoice);
                }

                var invoice = await invoiceRepository.GetInvoiceByIdAsync(invoiceId);
                if (invoice == null)
                {
                    return Result.Failure<InvoiceDto>(new("invoice_not_found", "Không tìm thấy hóa đơn"));
                }

                var invoiceDto = mapper.Map<InvoiceDto>(invoice);

                memoryCache.Set($"Invoice_{invoiceId}", invoiceDto, TimeSpan.FromMinutes(5));

                return Result.Success(invoiceDto);
            }
            catch
            {
                throw;
            }
        }

        public async Task<PagedResult<InvoiceDto>> GetInvoicesAsync(GetInvoiceRequest request)
        {
            try
            {
                var invoiceFilter = new InvoiceFilter
                {
                    Keyword = request.Keyword,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    IsPaid = request.IsPaid
                };

                var invoices = await invoiceRepository.GetInvoicesAsync(invoiceFilter);

                var invoiceDtos = mapper.Map<IEnumerable<InvoiceDto>>(invoices.Items);

                return new PagedResult<InvoiceDto>(invoiceDtos, invoices.TotalCount, invoices.PageNumber, invoices.PageSize);
            }
            catch (Exception)
            {
                return new PagedResult<InvoiceDto>(Enumerable.Empty<InvoiceDto>(), 0, request.PageNumber, request.PageSize);
            }
        }

        public async Task<Result<PaymentDto>> GetPaymentByIdAsync(int paymentId)
        {
            try
            {
                if (memoryCache.TryGetValue($"Payment_{paymentId}", out PaymentDto cachedPayment))
                {
                    return Result.Success(cachedPayment);
                }

                var payment = await paymentRepository.GetPaymentByIdAsync(paymentId);
                if (payment == null)
                {
                    return Result.Failure<PaymentDto>(new("payment_not_found", "Không tìm thấy thanh toán"));
                }

                var paymentDto = mapper.Map<PaymentDto>(payment);

                memoryCache.Set($"Payment_{paymentId}", paymentDto, TimeSpan.FromMinutes(5));

                return Result.Success(paymentDto);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Result<IEnumerable<PaymentMethodOption>>> GetPaymentMethodOptionsAsync()
        {
            try
            {
                if (memoryCache.TryGetValue("PaymentMethodOptions", out IEnumerable<PaymentMethodOption>? cachedOptions))
                {
                    return Result.Success(cachedOptions);
                }

                var paymentMethods = await paymentMethodRepository.GetAllAsync();
                var paymentMethodOptions = paymentMethods.Select(pm => new PaymentMethodOption
                {
                    Id = pm.PaymentMethodId,
                    Name = pm.PaymentMethodName
                });

                memoryCache.Set("PaymentMethodOptions", paymentMethodOptions, TimeSpan.FromHours(1));

                return Result.Success(paymentMethodOptions);
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<PaymentMethodOption>>(new("GET_PAYMENT_METHODS_FAILED", $"Lấy phương thức thanh toán thất bại: {ex.Message}"));
            }
        }

        public async Task<PagedResult<PaymentDto>> GetPaymentsAsync(GetPaymentRequest request)
        {
            try
            {
                //if (memoryCache.TryGetValue($"Payments_{request.Keyword}_{request.PageNumber}_{request.PageSize}", out PagedResult<PaymentDto> cachedPayments))
                //{
                //    return cachedPayments;
                //}

                var paymentFilter = new PaymentFilter
                {
                    Keyword = request.Keyword,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                var payments = await paymentRepository.GetPaymentsAsync(paymentFilter);

                var paymentDtos = mapper.Map<IEnumerable<PaymentDto>>(payments.Items);

                var pagedResult = new PagedResult<PaymentDto>(paymentDtos, payments.TotalCount, payments.PageNumber, payments.PageSize);

                //memoryCache.Set($"Payments_{request.Keyword}_{request.PageNumber}_{request.PageSize}", pagedResult, TimeSpan.FromMinutes(1));

                return pagedResult;
            }
            catch (Exception)
            {
                return new PagedResult<PaymentDto>(Enumerable.Empty<PaymentDto>(), 0, request.PageNumber, request.PageSize);
            }
        }

        public async Task<Result<IEnumerable<PaymentScheduleDto>>> GetPaymentSchedulesByContractIdAsync(int contractId)
        {
            try
            {
                if (memoryCache.TryGetValue($"PaymentSchedules_Contract_{contractId}", out IEnumerable<PaymentScheduleDto>? cachedSchedules))
                {
                    return Result.Success(cachedSchedules);
                }

                var paymentSchedules = await paymentScheduleRepository.FindAsync(ps => ps.ContractId == contractId);

                var paymentScheduleDtos = paymentSchedules.Select(ps => new PaymentScheduleDto
                {
                    Id = ps.InstallmentId,
                    ContractId = ps.ContractId,
                    InstallmentName = ps.InstallmentName,
                    Amount = ps.Amount,
                    ContractValuePercentage = ps.ContractValuePercentage,
                    DueDate = ps.DueDate.Value.ToDateTime(TimeOnly.MinValue),
                    Status = ps.Status
                });

                memoryCache.Set($"PaymentSchedules_Contract_{contractId}", paymentScheduleDtos, TimeSpan.FromMinutes(1));

                return Result.Success(paymentScheduleDtos);
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<PaymentScheduleDto>>(new("GET_PAYMENT_SCHEDULES_FAILED", $"Lấy kế hoạch thanh toán thất bại: {ex.Message}"));
            }
        }

        public async Task<Result<InvoiceDto>> UpdateInvoiceAsync(UpdateInvoiceRequest request)
        {
            try
            {
                var invoice = await invoiceRepository.GetByIdAsync(request.Id);
                if (invoice == null)
                {
                    return Result.Failure<InvoiceDto>(new("invoice_not_found", "Không tìm thấy hóa đơn"));
                }

                invoice.DueDate = DateOnly.FromDateTime(request.DueDate);
                invoice.TotalAmount = request.Amount;
                invoice.Status = request.Status;

                invoiceRepository.Update(invoice);
                var updated = await unitOfWork.SaveChangesAsync();

                if (updated > 0)
                {
                    var invoiceDto = mapper.Map<InvoiceDto>(invoice);
                    memoryCache.Remove($"Invoice_{invoice.InvoiceId}");
                    return Result.Success(invoiceDto);
                }
                else
                {
                    return Result.Failure<InvoiceDto>(new("update_invoice_failed", "Cập nhật hóa đơn thất bại"));
                }
            }
            catch
            {
                throw;
            }
        }
    }
}

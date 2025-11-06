using AutoMapper;
using CRM.Domain.Models;

namespace CRM.Application.Mappings
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile()
        {
            CreateMap<Invoice, Dtos.Payment.InvoiceDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.InvoiceId))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.InvoiceCode))
                .ForMember(dest => dest.ContractName, opt => opt.MapFrom(src => src.Contract.ContractName))
                .ForMember(dest => dest.ContractId, opt => opt.MapFrom(s => s.ContractId))
                .ForMember(d => d.InstallmentScheduleId, o => o.MapFrom(s => s.InstallmentScheduleId))
                .ForMember(d => d.InstallmentScheduleName, o => o.MapFrom(s => s.InstallmentSchedule.InstallmentName))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate.HasValue ? src.DueDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsPaid ? "Đã thanh toán" : "Chưa thanh toán"))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Contract.CustomerId))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Contract.Customer.CustomerName));
        }
    }
}

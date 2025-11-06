using AutoMapper;
using CRM.Application.Dtos.Payment;
using CRM.Domain.Models;

namespace CRM.Application.Mappings
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentDto>()
                .ForMember(d => d.Id, s => s.MapFrom(p => p.PaymentId))
                .ForMember(d => d.Code, s => s.MapFrom(p => p.PaymentCode))
                .ForMember(d => d.CustomerId, s => s.MapFrom(p => p.CustomerId))
                .ForMember(d => d.CustomerName, s => s.MapFrom(p => p.Customer != null ? p.Customer.CustomerName : string.Empty))
                .ForMember(d => d.Amount, s => s.MapFrom(p => p.Amount))
                .ForMember(d => d.PaymentDate, s => s.MapFrom(p => p.PaymentDate))
                .ForMember(d => d.Description, s => s.MapFrom(p => p.Description))
                .ForMember(d => d.InvoiceId, s => s.MapFrom(p => p.InvoiceId))
                .ForMember(d => d.InvoiceName, s => s.MapFrom(p => p.Invoice != null ? p.Invoice.InvoiceCode : string.Empty))
                .ForMember(d => d.PaymentMethodId, s => s.MapFrom(p => p.PaymentMethodId))
                .ForMember(d => d.PaymentMethod, s => s.MapFrom(p => p.PaymentMethod != null ? p.PaymentMethod.PaymentMethodName : string.Empty));
        }
    }
}

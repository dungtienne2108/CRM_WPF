using AutoMapper;
using CRM.Application.Dtos.Customer;
using CRM.Domain.Models;

namespace CRM.Application.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerDto>()
                .ForMember(d => d.Id, s => s.MapFrom(src => src.CustomerId))
                .ForMember(d => d.Code, s => s.MapFrom(src => src.CustomerCode))
                .ForMember(d => d.Name, s => s.MapFrom(src => src.CustomerName))
                .ForMember(d => d.CustomerIdentityCard, s => s.MapFrom(src => src.CustomerIdentityCard))
                .ForMember(d => d.TypeId, s => s.MapFrom(src => src.CustomerTypeId))
                .ForMember(d => d.TypeName, s => s.MapFrom(src => src.CustomerType.CustomerTypeName))
                .ForMember(d => d.Phone, s => s.MapFrom(src => src.CustomerPhone))
                .ForMember(d => d.Email, s => s.MapFrom(src => src.CustomerEmail))
                .ForMember(d => d.Gender, s => s.MapFrom(src => src.Gender.GenderName))
                .ForMember(d => d.Address, s => s.MapFrom(src => src.CustomerAddress))
                .ForMember(d => d.Description, s => s.MapFrom(src => src.CustomerDescription))
                .ForMember(d => d.LeadId, s => s.MapFrom(src => src.LeadId));

        }
    }
}

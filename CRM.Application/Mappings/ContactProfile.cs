using AutoMapper;
using CRM.Application.Dtos.Contact;
using CRM.Domain.Models;

namespace CRM.Application.Mappings
{
    public class ContactProfile : Profile
    {
        public ContactProfile()
        {
            CreateMap<Contact, ContactDto>()
                .ForMember(d => d.Id, s => s.MapFrom(s => s.ContactId))
                .ForMember(d => d.Name, s => s.MapFrom(s => s.ContactName))
                .ForMember(d => d.Phone, s => s.MapFrom(s => s.ContactPhone))
                .ForMember(d => d.Email, s => s.MapFrom(s => s.ContactEmail))
                .ForMember(d => d.Address, s => s.MapFrom(s => s.ContactEmail))
                .ForMember(d => d.SalutationId, s => s.MapFrom(s => s.ContactSalutationId))
                .ForMember(d => d.Salutation, s => s.MapFrom(s => s.ContactSalutation.ContactSalutationName != null ? s.ContactSalutation.ContactSalutationName : null))
                .ForMember(d => d.CreatedDate, s => s.MapFrom(s => s.CreateDate))
                .ForMember(d => d.Description, s => s.MapFrom(s => s.ContactDescription))
                .ForMember(d => d.CustomerId, s => s.MapFrom(s => s.CustomerContacts.FirstOrDefault().CustomerId));
        }
    }
}

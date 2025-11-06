using AutoMapper;
using CRM.Application.Dtos.Employee;
using CRM.Domain.Models;

namespace CRM.Application.Mappings
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDto>()
                .ForMember(d => d.Id, s => s.MapFrom(src => src.EmployeeId))
                .ForMember(d => d.Name, s => s.MapFrom(src => src.EmployeeName))
                .ForMember(d => d.Code, s => s.MapFrom(src => src.EmployeeCode))
                .ForMember(d => d.IdentityCard, s => s.MapFrom(src => src.EmployeeIdentityCard))
                .ForMember(d => d.Email, s => s.MapFrom(src => src.EmployeeEmail))
                .ForMember(d => d.PhoneNumber, s => s.MapFrom(src => src.EmployeePhone))
                .ForMember(d => d.GenderId, s => s.MapFrom(src => src.GenderId))
                .ForMember(d => d.GenderName, s => s.MapFrom(src => src.Gender.GenderName))
                .ForMember(d => d.Address, s => s.MapFrom(src => src.EmployeeAddress))
                .ForMember(d => d.EmployeeTypeId, s => s.MapFrom(src => src.EmployeeLevelId))
                .ForMember(d => d.EmployeeTypeName, s => s.MapFrom(src => src.EmployeeLevel.EmployeeLevelName))
                .ForMember(d => d.Birthday, s => s.MapFrom(src => src.EmployeeBirthDay.Value.ToDateTime(TimeOnly.MinValue)))
                .ForMember(d => d.Description, s => s.MapFrom(src => src.EmployeeDescription));
        }
    }
}

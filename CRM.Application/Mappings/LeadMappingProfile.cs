using AutoMapper;
using CRM.Application.Dtos.Lead;
using CRM.Domain.Models;

namespace CRM.Application.Mappings
{
    public class LeadMappingProfile : Profile
    {
        public LeadMappingProfile()
        {
            CreateMap<Lead, LeadDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.LeadId))
                .ForMember(d => d.Code, opt => opt.MapFrom(s => s.LeadCode))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.LeadName))
                .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.LeadPhone))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.LeadEmail))
                .ForMember(d => d.Address, opt => opt.MapFrom(s => s.LeadAddress))
                .ForMember(d => d.PotentialLevel, opt => opt.MapFrom(s => s.LeadPotentialLevel.LeadPotentialLevelName))
                .ForMember(d => d.PotentialLevelId, opt => opt.MapFrom(s => s.LeadPotentialLevelId))
                .ForMember(d => d.Source, opt => opt.MapFrom(s => s.LeadSource.LeadSourceName))
                .ForMember(d => d.SourceId, opt => opt.MapFrom(s => s.LeadSourceId))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.LeadStage.LeadStageName))
                .ForMember(d => d.StatusId, opt => opt.MapFrom(s => s.LeadStageId))
                .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee.EmployeeName))
                .ForMember(d => d.EmployeeId, opt => opt.MapFrom(s => s.EmployeeId))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.LeadDescription))
                .ForMember(d => d.StartDate, opt => opt.MapFrom(s => s.CreateDate))
                .ForMember(d => d.EndDate, opt => opt.MapFrom(s => s.EndDate))
                .ForMember(d => d.Products, opt => opt.MapFrom(s => s.LeadItems.Select(li => li.Product).ToList()));
        }
    }
}

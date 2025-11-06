using AutoMapper;
using CRM.Application.Dtos.Project;
using CRM.Domain.Models;

namespace CRM.Application.Mappings
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dest => dest.ProjectCode, opt => opt.MapFrom(src => src.ProjectCode))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.ProjectName))
                .ForMember(dest => dest.ProjectAddress, opt => opt.MapFrom(src => src.ProjectAddress))
                .ForMember(dest => dest.ProjectStartDate, opt => opt.MapFrom(src => src.ProjectStartDate))
                .ForMember(dest => dest.ProjectEndDate, opt => opt.MapFrom(src => src.ProjectEndDate))
                .ForMember(dest => dest.ProjectStatus, opt => opt.MapFrom(src => src.ProjectStatus))
                .ForMember(dest => dest.ProjectDescription, opt => opt.MapFrom(src => src.ProjectDescription))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.CreateBy, opt => opt.MapFrom(src => src.CreateBy))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
        }
    }
}

using AutoMapper;
using CRM.Application.Dtos.Lead;
using CRM.Application.Dtos.Opportunity;
using CRM.Domain.Models;

namespace CRM.Application.Mappings
{
    public class OpportunityProfile : Profile
    {
        public OpportunityProfile()
        {
            CreateMap<OpportunityItem, OpportunityItemDto>()
                .ForMember(dest => dest.OpportunityId, opt => opt.MapFrom(src => src.OpportunityId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : null))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Project.ProjectName : null))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.SalePrice))
                .ForMember(dest => dest.ProductStatusId, opt => opt.MapFrom(src => src.Product.ProductStatusId))
                .ForMember(dest => dest.ProductStatus, opt => opt.MapFrom(src => src.Product.ProductStatus.ProductStatusName));

            CreateMap<OpportunityStage, OpportunityStatusOption>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OpportunityStageId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OpportunityStageName));

            CreateMap<Opportunity, OpportunityDto>()
                .ForMember(dest => dest.OpportunityId, opt => opt.MapFrom(src => src.OpportunityId))
                .ForMember(dest => dest.OpportunityCode, opt => opt.MapFrom(src => src.OpportunityCode))
                .ForMember(dest => dest.OpportunityName, opt => opt.MapFrom(src => src.OpportunityName))
                .ForMember(dest => dest.OpportunityDescription, opt => opt.MapFrom(src => src.OpportunityDescription))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.ExpectedPrice, opt => opt.MapFrom(src => src.OpportunityItems.Sum(item => item.ExceptedProfit)))
                .ForMember(dest => dest.RealPrice, opt => opt.MapFrom(src => src.OpportunityItems.Sum(item => item.Quantity * item.SalePrice)))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.OpportunityItems.Select(item => item.Product).ToList()))
                .ForMember(dest => dest.OpportunityItems, opt => opt.MapFrom(src => src.OpportunityItems))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.OpportunityStatus, opt => opt.MapFrom(src => src.OpportunityStage))
                .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
                .ForMember(dest => dest.IsDeposited, opt => opt.MapFrom(src => src.IsDeposited));
        }
    }
}

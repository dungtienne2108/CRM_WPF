using AutoMapper;
using CRM.Application.Dtos.Project;
using CRM.Domain.Models;

namespace CRM.Application.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.ProductCode))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.ProductFloors, opt => opt.MapFrom(src => src.ProductFloors))
                .ForMember(dest => dest.ProductAddress, opt => opt.MapFrom(src => src.ProductAddress))
                .ForMember(dest => dest.ProductNumber, opt => opt.MapFrom(src => src.ProductNumber))
                .ForMember(dest => dest.ProductArea, opt => opt.MapFrom(src => src.ProductArea))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.ProductPrice))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.CreateBy, opt => opt.MapFrom(src => src.CreateBy))
                .ForMember(dest => dest.ProductTypeId, opt => opt.MapFrom(src => src.ProductTypeId))
                .ForMember(dest => dest.ProductTypeName, opt => opt.MapFrom(src => src.ProductType != null ? src.ProductType.ProductTypeName : string.Empty))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.ProjectName : string.Empty))
                .ForMember(dest => dest.ProductStatusId, opt => opt.MapFrom(src => src.ProductStatusId))
                .ForMember(dest => dest.ProductStatusName, opt => opt.MapFrom(src => src.ProductStatus != null ? src.ProductStatus.ProductStatusName : string.Empty));
        }
    }
}

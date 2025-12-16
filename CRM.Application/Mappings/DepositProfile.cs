using AutoMapper;
using CRM.Application.Dtos.Deposit;
using CRM.Domain.Models;

namespace CRM.Application.Mappings
{
    public class DepositProfile : Profile
    {
        public DepositProfile()
        {
            CreateMap<Deposit, DepositDto>()
                .ForMember(d => d.Id, s => s.MapFrom(d => d.DepositId))
                .ForMember(d => d.Code, s => s.MapFrom(d => d.DepositCode))
                .ForMember(d => d.Name, s => s.MapFrom(d => d.DepositName))
                .ForMember(d => d.Amount, s => s.MapFrom(d => d.DepositCost))
                .ForMember(d => d.CustomerId, s => s.MapFrom(d => d.CustomerId.HasValue ? d.CustomerId.Value : 0))
                .ForMember(d => d.CustomerName, s => s.MapFrom(d => d.Customer != null ? d.Customer.CustomerName : string.Empty))
                .ForMember(d => d.OpportunityId, s => s.MapFrom(d => d.OpportunityId.HasValue ? d.OpportunityId.Value : 0))
                .ForMember(d => d.OpportunityName, s => s.MapFrom(d => d.Opportunity != null ? d.Opportunity.OpportunityName : string.Empty))
                .ForMember(d => d.StartDate, s => s.MapFrom(d => d.CreateDate.HasValue ? d.CreateDate.Value : DateTime.MinValue))
                .ForMember(d => d.EndDate, s => s.MapFrom(d => d.EndDate.HasValue ? d.EndDate.Value : DateTime.MaxValue))
                .ForMember(d => d.Description, s => s.MapFrom(d => d.Description))
                .ForMember(d => d.EmployeeId, s => s.MapFrom(d => d.EmployeeId.HasValue ? d.EmployeeId.Value : 0))
                .ForMember(d => d.EmployeeName, s => s.MapFrom(d => d.Employee != null ? d.Employee.EmployeeName : string.Empty))
                .ForMember(d => d.ProjectId, s => s.MapFrom(d => d.Product != null && d.Product.Project != null ? d.Product.Project.ProjectId : 0))
                .ForMember(d => d.ProjectName, s => s.MapFrom(d => d.Product != null && d.Product.Project != null ? d.Product.Project.ProjectName : string.Empty))
                .ForMember(d => d.ProductId, s => s.MapFrom(d => d.ProductId))
                .ForMember(d => d.ProductName, s => s.MapFrom(d => d.Product != null ? d.Product.ProductName : string.Empty))
                .ForMember(d => d.ProductPrice, s => s.MapFrom(d => d.Product != null ? d.Product.ProductPrice : 0));

        }
    }
}

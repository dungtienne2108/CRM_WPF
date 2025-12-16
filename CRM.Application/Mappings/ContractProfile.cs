using AutoMapper;
using CRM.Application.Dtos.Contract;
using CRM.Domain.Models;

namespace CRM.Application.Mappings
{
    public class ContractProfile : Profile
    {
        public ContractProfile()
        {
            CreateMap<Contract, ContractDto>()
                .ForMember(d => d.Id, s => s.MapFrom(c => c.ContractId))
                .ForMember(d => d.Code, s => s.MapFrom(c => c.ContractCode))
                .ForMember(d => d.Name, s => s.MapFrom(c => c.ContractName))
                .ForMember(d => d.Number, s => s.MapFrom(c => c.ContractNumber))
                .ForMember(d => d.CustomerId, s => s.MapFrom(c => c.CustomerId))
                .ForMember(d => d.CustomerName, s => s.MapFrom(c => c.Customer != null ? c.Customer.CustomerName : null))
                .ForMember(d => d.AmountAfterTax, s => s.MapFrom(c => c.AmountAfterTax))
                .ForMember(d => d.Tax, s => s.MapFrom(c => c.Tax))
                .ForMember(d => d.AmountBeforeTax, s => s.MapFrom(c => c.AmountBeforeTax))
                .ForMember(d => d.Amount, s => s.MapFrom(c => c.Amount))
                .ForMember(d => d.TypeId, s => s.MapFrom(c => c.ContractTypeId))
                .ForMember(d => d.Type, s => s.MapFrom(c => c.ContractType != null ? c.ContractType.ContractTypeName : null))
                .ForMember(d => d.StatusId, s => s.MapFrom(c => c.ContractStageId))
                .ForMember(d => d.Status, s => s.MapFrom(c => c.ContractStage != null ? c.ContractStage.ContractStageName : null))
                .ForMember(d => d.StartDate, s => s.MapFrom(c => c.ContractStartDate.ToDateTime(new TimeOnly(0, 0))))
                .ForMember(d => d.EndDate, s => s.MapFrom(c => c.ContractEndDate.ToDateTime(new TimeOnly(0, 0))))
                .ForMember(d => d.ProductId, s => s.MapFrom(c => c.ProductId))
                .ForMember(d => d.DepositId, s => s.MapFrom(c => c.DepositId))
                .ForMember(d => d.Description, s => s.MapFrom(c => c.ContractDescription))
                .ForMember(d => d.EmployeeId, s => s.MapFrom(c => c.EmployeeId))
                .ForMember(d => d.EmployeeName, s => s.MapFrom(c => c.Employee != null ? c.Employee.EmployeeName : null))
                .ForMember(d => d.Documents, s => s.MapFrom(c => c.ContractDocuments))
                .ForMember(d => d.PaidAmount, s => s.MapFrom(c => c.InstallmentSchedules != null ? c.InstallmentSchedules.Sum(i => i.Amount) : 0))
                .ForMember(d => d.RemainingAmount, s => s.MapFrom(c => c.AmountAfterTax - (c.InstallmentSchedules != null ? c.InstallmentSchedules.Sum(i => i.Amount) : 0)))
                ;

            CreateMap<ContractDocument, ContractDocumentDto>()
                .ForMember(d => d.Id, s => s.MapFrom(ci => ci.Id))
                .ForMember(d => d.ContractId, s => s.MapFrom(ci => ci.ContractId))
                .ForMember(d => d.FilePath, s => s.MapFrom(ci => ci.FilePath))
                .ForMember(d => d.FileName, s => s.MapFrom(ci => ci.FileName))
                .ForMember(d => d.FileSize, s => s.MapFrom(ci => ci.FileSize))
                .ForMember(d => d.ContentType, s => s.MapFrom(ci => ci.ContentType))
                ;
        }
    }
}

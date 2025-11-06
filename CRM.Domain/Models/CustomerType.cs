namespace CRM.Domain.Models;

public partial class CustomerType
{
    public int CustomerTypeId { get; set; }

    public string CustomerTypeCode { get; set; } = null!;

    public string CustomerTypeName { get; set; } = null!;
}

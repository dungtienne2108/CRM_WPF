namespace CRM.Domain.Models;

public partial class PaymentOption
{
    public int PaymentOptionId { get; set; }

    public string PaymentOptionCode { get; set; } = null!;

    public string PaymentOptionName { get; set; } = null!;

}

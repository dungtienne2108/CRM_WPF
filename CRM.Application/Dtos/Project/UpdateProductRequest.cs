namespace CRM.Application.Dtos.Project
{
    public class UpdateProductRequest
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int ProductFloors { get; set; }

        public decimal? ProductArea { get; set; }

        public decimal? ProductPrice { get; set; }

        public int ProductTypeId { get; set; }

        public int ProductStatusId { get; set; }

    }
}

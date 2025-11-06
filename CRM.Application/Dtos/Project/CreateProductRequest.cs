namespace CRM.Application.Dtos.Project
{
    public class CreateProductRequest
    {
        public int ProjectId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Floors { get; set; }
        public decimal Area { get; set; }
        public decimal Price { get; set; }
        public int StatusId { get; set; }
        public int TypeId { get; set; }
    }
}

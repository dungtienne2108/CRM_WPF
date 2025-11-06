namespace CRM.Application.Dtos.Project
{
    public class CreateProjectRequest
    {
        public string ProjectName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProjectAddress { get; set; }
        public decimal ProjectArea { get; set; }
    }
}

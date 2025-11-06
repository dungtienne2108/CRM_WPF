namespace CRM.Application.Dtos.Project
{
    public class GetProjectRequest
    {
        public string? Keyword { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

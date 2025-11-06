namespace CRM.Application.Dtos.Contract
{
    public class ContractDocumentDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public required string FileName { get; set; }
        public long? FileSize { get; set; }
        public string? ContentType { get; set; }
        public required string FilePath { get; set; }
    }
}

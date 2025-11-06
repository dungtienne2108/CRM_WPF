using System.ComponentModel.DataAnnotations;

namespace CRM.Domain.Models
{
    public class ContractDocument
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ContractId { get; set; }
        [Required]
        public required string FileName { get; set; }
        public string? ContentType { get; set; }
        public long? FileSize { get; set; }
        [Required]
        public required string FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Contract Contract { get; set; } = null!;
    }
}

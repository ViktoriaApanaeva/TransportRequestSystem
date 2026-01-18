using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportRequestSystem.Models;

public class StatusHistory
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int ApplicationId { get; set; }
    [ForeignKey("ApplicationId")]
    public Application? Application { get; set; }
    [Required]
    [MaxLength(50)]
    public string? OldStatus { get; set; }
    [Required]
    [MaxLength(50)]
    public string? NewStatus { get; set; }
    [Required]
    [MaxLength(100)]
    public string? ChangedBy { get; set; }
    [Required]
    [MaxLength(500)]
    public string? Comment { get; set; }
    [Required]
    public DateTime ChangedDate { get; set; } = DateTime.Now;
}

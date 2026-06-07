using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportRequestSystem.Models
{
    public class Driver
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Табельный номер")]
        [StringLength(50)]
        public string PersonnelNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "ФИО")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Телефон")]
        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Display(Name = "Категория прав")]
        [StringLength(20)]
        public string? LicenseCategory { get; set; }

        [Display(Name = "Активен")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Примечание")]
        [StringLength(500)]
        public string? Notes { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Дата обновления")]
        public DateTime? UpdatedAt { get; set; }

        // Навигационное свойство
        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
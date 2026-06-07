using System.ComponentModel.DataAnnotations;

namespace TransportRequestSystem.Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Гос. номер")]
        [StringLength(20)]
        public string PlateNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Марка")]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Модель")]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Display(Name = "Цвет")]
        [StringLength(30)]
        public string? Color { get; set; }

        [Display(Name = "Грузоподъемность (кг)")]
        public int? Capacity { get; set; }

        [Display(Name = "Кол-во мест")]
        public int? SeatsCount { get; set; }

        [Display(Name = "Активно")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Примечание")]
        [StringLength(500)]
        public string? Notes { get; set; }

        // Навигационное свойство 
        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
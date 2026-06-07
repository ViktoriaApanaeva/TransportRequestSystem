using System.ComponentModel.DataAnnotations;

namespace TransportRequestSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(50)]
        public string Role { get; set; } = "User"; // User, Dispatcher, Admin

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }

        // Навигационные свойства
        public virtual ICollection<Application> CreatedApplications { get; set; } = new List<Application>();
        public virtual ICollection<Application> DispatchedApplications { get; set; } = new List<Application>();
    }
}
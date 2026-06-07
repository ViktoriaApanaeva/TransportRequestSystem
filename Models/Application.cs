using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportRequestSystem.Models
{
    public class Application
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Номер заявки")]
        [StringLength(50)]
        public string Number { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Статус")]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.CreatedOrModified;

        [Required]
        [Display(Name = "Дата заявки")]
        [DataType(DataType.Date)]
        public DateTime ApplicationDate { get; set; } = DateTime.Today;

        // Раздел заказчика
        [Required]
        [Display(Name = "Организационная единица")]
        [StringLength(100)]
        public string OrganizationUnit { get; set; } = string.Empty;

        [Required]
        [Display(Name = "ФИО ответственного")]
        [StringLength(100)]
        public string ResponsiblePerson { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Телефон")]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Цель поездки")]
        [StringLength(200)]
        public string Purpose { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Количество пассажиров")]
        [Range(1, 50)]
        public int Passengers { get; set; } = 1;

        [Required]
        [Display(Name = "Маршрут")]
        [StringLength(500)]
        public string Route { get; set; } = string.Empty;

        [Display(Name = "Примечание")]
        [StringLength(1000)]
        public string? Notes { get; set; } = string.Empty;

        // Раздел диспетчера (заполняется автоматически из авторизованного диспетчера)
        [Display(Name = "ФИО диспетчера")]
        [StringLength(100)]
        public string? DispatcherName { get; set; }

        [Display(Name = "Телефон диспетчера")]
        [Phone]
        [StringLength(20)]
        public string? DispatcherPhone { get; set; }

        [Display(Name = "ФИО водителя")]
        [StringLength(100)]
        public string? DriverName { get; set; }

        [Display(Name = "Телефон водителя")]
        [Phone]
        [StringLength(20)]
        public string? DriverPhone { get; set; }

        [Display(Name = "Марка ТС")]
        [StringLength(50)]
        public string? VehicleBrand { get; set; }

        [Display(Name = "Гос. номер")]
        [StringLength(20)]
        public string? VehicleNumber { get; set; }

        [Display(Name = "Цвет ТС")]
        [StringLength(30)]
        public string? VehicleColor { get; set; }

        [Display(Name = "Примечание диспетчера")]
        [StringLength(1000)]
        public string? DispatcherNotes { get; set; }

        [Display(Name = "Дата начала поездки")]
        public DateTime? TripStart { get; set; }

        [Display(Name = "Дата окончания поездки")]
        public DateTime? TripEnd { get; set; }

        // Внешние ключи
        [Column(TypeName = "integer")]
        public int? CreatedByUserId { get; set; }

        [ForeignKey("CreatedByUserId")]
        public virtual User? CreatedByUser { get; set; }

        [Column(TypeName = "integer")]
        public int? DispatcherUserId { get; set; }

        [ForeignKey("DispatcherUserId")]
        public virtual User? DispatcherUser { get; set; }

        public int? DriverId { get; set; }

        [ForeignKey("DriverId")]
        public virtual Driver? Driver { get; set; }

        public int? VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public virtual Vehicle? Vehicle { get; set; }

        // Системные поля
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Навигационные свойства
        public virtual ICollection<StatusHistory> StatusHistories { get; set; } = new List<StatusHistory>();


        // Генератор номера заявки
        public static string GenerateNumber()
        {
            return $"Z-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        }
    }


    public class ApplicationFilter
    {
        [Display(Name = "Дата с")]
        [DataType(DataType.Date)]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "Дата по")]
        [DataType(DataType.Date)]
        public DateTime? DateTo { get; set; }

        [Display(Name = "Организационная единица")]
        public string? OrganizationUnit { get; set; }

        [Display(Name = "Статусы")]
        public List<ApplicationStatus> SelectedStatuses { get; set; } = new();
    }

    public enum ApplicationStatus
    {
        [Display(Name = "Создана")]
        CreatedOrModified,

        [Display(Name = "Утверждена")]
        Approved,

        [Display(Name = "Назначено ТС")]
        AssignedToVehicle,

        [Display(Name = "Исполняется")]
        InProgress,

        [Display(Name = "Исполнена")]
        Completed,

        [Display(Name = "Не исполнена")]
        NotCompleted,

        [Display(Name = "Отклонена диспетчером")]
        RejectedByDispatcher,

        [Display(Name = "Отклонена руководителем")]
        RejectedByDirector,

        [Display(Name = "Удалена")]
        Deleted
    }
}
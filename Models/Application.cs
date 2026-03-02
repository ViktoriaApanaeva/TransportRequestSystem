using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace TransportRequestSystem.Models
{
    public class Application
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Номер заявки")]
        public string Number { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Статус")]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.CreatedOrModified;

        // Дата (раздел фильтров)
        [Required]
        [Display(Name = "Дата")]
        [DataType(DataType.Date)]
        public DateTime ApplicationDate { get; set; } = DateTime.Today;
        
        [Display(Name = "Начало поездки")]
        public DateTime? TripStart { get; set; }

        [Display(Name = "Окончание поездки")]
        public DateTime? TripEnd { get; set; }

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
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Цель поездки")]
        [StringLength(200)]
        public string Purpose { get; set; } = string.Empty;

        [Display(Name = "Кол-во пассажиров")]
        [StringLength(20)]
        public string? Passengers { get; set; } 

        [Required]
        [Display(Name = "Маршрут")]
        [StringLength(500)]
        public string Route { get; set; } = string.Empty;

        [Display(Name = "Примечание")]
        [StringLength(1000)]
        public string? Notes { get; set; }

        // Поля диспетчера
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

        // Системные поля
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public List<StatusHistory> StatusHistory { get; set; } = new();

        public static string GenerateNumber()
        {
            return $"{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        }
    }

    public enum ApplicationStatus
    {
        [Display(Name = "Удалена")]
        Deleted,

        [Display(Name = "Отклонена руководителем")]
        RejectedByDirector,

        [Display(Name = "Отклонена диспетчером")]
        RejectedByDispatcher,

        [Display(Name = "Назначено ТС")]
        AssignedToVehicle,

        [Display(Name = "Создана/Изменена")]
        CreatedOrModified,

        [Display(Name = "Не исполнена")]
        NotCompleted,

        [Display(Name = "Исполнена")]
        Completed,

        [Display(Name = "Исполняется")]
        InProgress,

        [Display(Name = "Утверждена")]
        Approved
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
        public List<ApplicationStatus> SelectedStatuses { get; set; } = new List<ApplicationStatus>();
    }

    
}
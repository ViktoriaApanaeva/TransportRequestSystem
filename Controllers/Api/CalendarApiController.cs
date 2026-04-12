using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportRequestSystem.Data;
using TransportRequestSystem.Models;

namespace TransportRequestSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CalendarApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CalendarApi
        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var events = await _context.Applications
                .Select(a => new
                {
                    id = a.Id,
                    title = $"{a.TripStart:HH:mm} {a.OrganizationUnit ?? a.Number}",
                    start = a.TripStart,
                    end = a.TripEnd,
                    color = GetStatusColor(a.Status),
                    number = a.Number,
                    organizationUnit = a.OrganizationUnit,
                    route = a.Route,
                    driverName = a.DriverName,
                    vehicle = $"{a.VehicleBrand} {a.VehicleNumber}",
                    status = a.Status.ToString(),
                    statusName = GetStatusName(a.Status)
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/CalendarApi/
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var application = await _context.Applications
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.Number,
                    a.OrganizationUnit,
                    a.ResponsiblePerson,
                    a.Phone,
                    a.Purpose,
                    a.Route,
                    a.Passengers,
                    a.Notes,
                    a.DriverName,
                    a.DriverPhone,
                    a.VehicleBrand,
                    a.VehicleNumber,
                    a.VehicleColor,
                    start = a.TripStart,
                    end = a.TripEnd,
                    status = a.Status.ToString()
                })
                .FirstOrDefaultAsync();

            if (application == null)
                return NotFound();

            return Ok(application);
        }

        // GET: api/CalendarApi/day/{date}
        [HttpGet("day/{date}")]
        public async Task<IActionResult> GetDayDetails(DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            var applications = await _context.Applications
                .OrderBy(a => a.TripStart)
                .Select(a => new
                {
                    a.Id,
                    a.Number,
                    a.OrganizationUnit,
                    a.ResponsiblePerson,
                    a.Phone,
                    a.Purpose,
                    a.Route,
                    startTime = a.TripStart,
                    endTime = a.TripEnd,
                    status = a.Status.ToString(),
                    statusName = GetStatusName(a.Status)
                })
                .ToListAsync();

            return Ok(applications);
        }

        private static string GetStatusColor(ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.Approved => "#28a745",
                ApplicationStatus.InProgress => "#ffc107",
                ApplicationStatus.Completed => "#17a2b8",
                ApplicationStatus.CreatedOrModified => "#6f42c1",
                ApplicationStatus.RejectedByDispatcher => "#dc3545",
                ApplicationStatus.RejectedByDirector => "#dc3545",
                ApplicationStatus.AssignedToVehicle => "#007bff",
                ApplicationStatus.NotCompleted => "#6c757d",
                _ => "#6c757d"
            };
        }

        private static string GetStatusName(ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.Approved => "Утверждена",
                ApplicationStatus.InProgress => "Исполняется",
                ApplicationStatus.Completed => "Исполнена",
                ApplicationStatus.CreatedOrModified => "Создана",
                ApplicationStatus.RejectedByDispatcher => "Отклонена диспетчером",
                ApplicationStatus.RejectedByDirector => "Отклонена руководителем",
                ApplicationStatus.AssignedToVehicle => "Назначено ТС",
                ApplicationStatus.NotCompleted => "Не исполнена",
                _ => status.ToString()
            };
        }
    }
}
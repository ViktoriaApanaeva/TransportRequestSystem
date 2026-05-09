using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportRequestSystem.Data;
using TransportRequestSystem.Models;

namespace TransportRequestSystem.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsDispatcher()
        {
            return User.IsInRole("Dispatcher");
        }

        // Главная страница с фильтрами
        public async Task<IActionResult> Index(DateTime? DateFrom, DateTime? DateTo, string OrganizationUnit, List<ApplicationStatus> SelectedStatuses,
                                       bool reset = false, bool selectAll = false)
        {
            if (reset)
            {
                return RedirectToAction("Index");
            }
            if (selectAll)
            {
                SelectedStatuses = Enum.GetValues<ApplicationStatus>().Cast<ApplicationStatus>().ToList();
            }
            ;

            var query = _context.Applications
                .Include(a => a.StatusHistory)
                .AsQueryable();

            // Применяем фильтры с преобразованием в UTC
            if (DateFrom.HasValue)
            {
                var dateFromUtc = DateTime.SpecifyKind(DateFrom.Value.Date, DateTimeKind.Utc);
                query = query.Where(a => a.ApplicationDate >= dateFromUtc);
            }

            if (DateTo.HasValue)
            {
                var dateToUtc = DateTime.SpecifyKind(DateTo.Value.Date.AddDays(1), DateTimeKind.Utc);
                query = query.Where(a => a.ApplicationDate <= dateToUtc);
            }

            if (!string.IsNullOrWhiteSpace(OrganizationUnit))
                query = query.Where(a => a.OrganizationUnit.Contains(OrganizationUnit));

            if (SelectedStatuses != null && SelectedStatuses.Any())
            {
                query = query.Where(a => SelectedStatuses.Contains(a.Status));
            }

            var applications = await query.ToListAsync();

            var filter = new ApplicationFilter
            {
                DateFrom = DateFrom,
                DateTo = DateTo,
                OrganizationUnit = OrganizationUnit,
                SelectedStatuses = SelectedStatuses ?? new List<ApplicationStatus>()
            };
            ViewBag.Filter = filter;
            ViewBag.Dispatchers = await _context.Dispatchers.ToListAsync();
            ViewBag.Drivers = await _context.Drivers.ToListAsync();
            return View(applications);
        }

        // Возвращает модальное окно для создания заявки
        public IActionResult GetCreateModel()
        {
            var model = new Application
            {
                Number = Application.GenerateNumber(),
                ApplicationDate = DateTime.Today,
                Status = ApplicationStatus.CreatedOrModified
            };
            return PartialView("_CreateModal", model);
        }

        // Создание новой заявки
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Application application, string actionType)
        {
            try
            {
                application.Id = 0;
                application.Number = Application.GenerateNumber();
                application.CreatedAt = DateTime.UtcNow;
                application.ApplicationDate = DateTime.SpecifyKind(application.ApplicationDate, DateTimeKind.Utc);

                if (application.TripStart.HasValue)
                    application.TripStart = DateTime.SpecifyKind(application.TripStart.Value, DateTimeKind.Utc);
                if (application.TripEnd.HasValue)
                    application.TripEnd = DateTime.SpecifyKind(application.TripEnd.Value, DateTimeKind.Utc);

                application.Status = actionType switch
                {
                    "approve" => ApplicationStatus.Approved,
                    "reject" => ApplicationStatus.RejectedByDispatcher,
                    _ => ApplicationStatus.CreatedOrModified
                };

                _context.Applications.Add(application);
                await _context.SaveChangesAsync();

                // Запись в историю статусов
                var history = new StatusHistory
                {
                    ApplicationId = application.Id,
                    NewStatus = application.Status.ToString(),
                    ChangedBy = User.Identity.Name ?? "System",
                    ChangedDate = DateTime.UtcNow
                };
                _context.StatusHistory.Add(history);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Заявка {application.Number} создана!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Получение данных заявки для редактирования (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
                return NotFound();

            // Преобразуем даты в UTC для корректной передачи в JSON
            if (application.TripStart.HasValue)
                application.TripStart = DateTime.SpecifyKind(application.TripStart.Value, DateTimeKind.Utc);
            if (application.TripEnd.HasValue)
                application.TripEnd = DateTime.SpecifyKind(application.TripEnd.Value, DateTimeKind.Utc);
            if (application.CreatedAt != DateTime.MinValue)
                application.CreatedAt = DateTime.SpecifyKind(application.CreatedAt, DateTimeKind.Utc);
            if (application.ApplicationDate != DateTime.MinValue)
                application.ApplicationDate = DateTime.SpecifyKind(application.ApplicationDate, DateTimeKind.Utc);

            return Json(application);
        }

        // Сохранение изменений заявки (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Application application, string actionType)
        {
            if (id != application.Id)
                return NotFound();

            try
            {
                var existingApp = await _context.Applications.FindAsync(id);
                if (existingApp == null)
                    return NotFound();

                var oldStatus = existingApp.Status;

                // Обновляем поля с преобразованием дат в UTC
                existingApp.ApplicationDate = DateTime.SpecifyKind(application.ApplicationDate, DateTimeKind.Utc);
                existingApp.TripStart = application.TripStart.HasValue
                    ? DateTime.SpecifyKind(application.TripStart.Value, DateTimeKind.Utc)
                    : null;
                existingApp.TripEnd = application.TripEnd.HasValue
                    ? DateTime.SpecifyKind(application.TripEnd.Value, DateTimeKind.Utc)
                    : null;
                existingApp.OrganizationUnit = application.OrganizationUnit;
                existingApp.ResponsiblePerson = application.ResponsiblePerson;
                existingApp.Phone = application.Phone;
                existingApp.Purpose = application.Purpose;
                existingApp.Passengers = application.Passengers ?? string.Empty;
                existingApp.Route = application.Route;
                existingApp.Notes = application.Notes ?? string.Empty;

                if (IsDispatcher())
                {
                    existingApp.DispatcherName = application.DispatcherName;
                    existingApp.DispatcherPhone = application.DispatcherPhone;
                    existingApp.DriverName = application.DriverName;
                    existingApp.DriverPhone = application.DriverPhone;
                    existingApp.VehicleBrand = application.VehicleBrand;
                    existingApp.VehicleNumber = application.VehicleNumber;
                    existingApp.VehicleColor = application.VehicleColor;
                    existingApp.DispatcherNotes = application.DispatcherNotes;
                }

                existingApp.UpdatedAt = DateTime.UtcNow;

                // Обновляем статус если нужно
                if (IsDispatcher())
                {
                    if (actionType == "approve")
                        existingApp.Status = ApplicationStatus.Approved;
                    else if (actionType == "reject")
                        existingApp.Status = ApplicationStatus.RejectedByDispatcher;
                }

                // Если статус изменился, добавляем запись в историю
                if (oldStatus != existingApp.Status)
                {
                    var history = new StatusHistory
                    {
                        ApplicationId = existingApp.Id,
                        OldStatus = oldStatus.ToString(),
                        NewStatus = existingApp.Status.ToString(),
                        ChangedBy = User.Identity.Name ?? "System",
                        ChangedDate = DateTime.UtcNow
                    };
                    _context.StatusHistory.Add(history);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Заявка {existingApp.Number} обновлена!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Изменение статуса заявки
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, ApplicationStatus status)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
                return NotFound();

            var oldStatus = application.Status;
            application.Status = status;
            application.UpdatedAt = DateTime.UtcNow;

            await AddStatusHistory(id, oldStatus.ToString(), status.ToString(), "Система");
            await _context.SaveChangesAsync();

            TempData["Success"] = "Статус изменен!";
            return RedirectToAction(nameof(Edit), new { id });
        }

        // Вспомогательный метод для добавления записи в историю
        private async Task AddStatusHistory(int appId, string oldStatus, string newStatus, string changedBy)
        {
            var history = new StatusHistory
            {
                ApplicationId = appId,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ChangedBy = changedBy,
                ChangedDate = DateTime.UtcNow
            };
            _context.StatusHistory.Add(history);
            await _context.SaveChangesAsync();
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportRequestSystem.Data;
using TransportRequestSystem.Models;
using System.Security.Claims;

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

        private async Task<User?> GetCurrentUserAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return null;

            return await _context.Users.FindAsync(userId);
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

            var query = _context.Applications
                .Include(a => a.StatusHistories)
                .Include(a => a.Driver)
                .Include(a => a.Vehicle)
                .Include(a => a.CreatedByUser)
                .Include(a => a.DispatcherUser)
                .AsQueryable();

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
            ViewBag.Drivers = await _context.Drivers.Where(d => d.IsActive).ToListAsync();
            ViewBag.Vehicles = await _context.Vehicles.Where(v => v.IsActive).ToListAsync();

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
                // Получаем текущего пользователя
                var currentUser = await GetCurrentUserAsync();
                if (currentUser == null)
                {
                    TempData["Error"] = "Пользователь не авторизован";
                    return RedirectToAction(nameof(Index));
                }

                application.Id = 0;
                application.Number = Application.GenerateNumber();
                application.CreatedAt = DateTime.UtcNow;
                application.ApplicationDate = DateTime.SpecifyKind(application.ApplicationDate, DateTimeKind.Utc);

                // Автоматически заполняем ФИО и телефон ответственного из профиля пользователя
                application.ResponsiblePerson = currentUser.FullName ?? currentUser.Username;
                application.Phone = currentUser.Phone ?? "-";
                application.CreatedByUserId = currentUser.Id;

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

                // Если диспетчер утверждает/отклоняет, заполняем его данные
                if (IsDispatcher() && (actionType == "approve" || actionType == "reject"))
                {
                    application.DispatcherName = currentUser.FullName ?? currentUser.Username;
                    application.DispatcherPhone = currentUser.Phone ?? "-";
                    application.DispatcherUserId = currentUser.Id;
                }

                _context.Applications.Add(application);
                await _context.SaveChangesAsync();

                // Запись в историю
                var history = new StatusHistory
                {
                    ApplicationId = application.Id,
                    NewStatus = application.Status.ToString(),
                    ChangedBy = currentUser.FullName ?? currentUser.Username,
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
            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return NotFound();

            var result = new
            {
                id = application.Id,
                number = application.Number,
                status = (int)application.Status,
                applicationDate = application.ApplicationDate,
                tripStart = application.TripStart,
                tripEnd = application.TripEnd,
                organizationUnit = application.OrganizationUnit,
                responsiblePerson = application.ResponsiblePerson,
                phone = application.Phone,
                purpose = application.Purpose,
                passengers = application.Passengers,
                route = application.Route,
                notes = application.Notes ?? "",
                dispatcherName = application.DispatcherName,
                dispatcherPhone = application.DispatcherPhone,
                driverName = application.DriverName,
                driverPhone = application.DriverPhone,
                vehicleBrand = application.VehicleBrand,
                vehicleNumber = application.VehicleNumber,
                vehicleColor = application.VehicleColor,
                dispatcherNotes = application.DispatcherNotes ?? ""
            };

            return Json(result);
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
                var currentUser = await GetCurrentUserAsync();

                // Обновляем основные поля
                existingApp.ApplicationDate = DateTime.SpecifyKind(application.ApplicationDate, DateTimeKind.Utc);
                existingApp.TripStart = application.TripStart.HasValue
                    ? DateTime.SpecifyKind(application.TripStart.Value, DateTimeKind.Utc)
                    : null;
                existingApp.TripEnd = application.TripEnd.HasValue
                    ? DateTime.SpecifyKind(application.TripEnd.Value, DateTimeKind.Utc)
                    : null;
                existingApp.OrganizationUnit = application.OrganizationUnit;
                existingApp.Purpose = application.Purpose;
                existingApp.Passengers = application.Passengers;
                existingApp.Route = application.Route;
                existingApp.Notes = application.Notes ?? string.Empty;

                // Если пользователь - диспетчер
                if (IsDispatcher() && currentUser != null)
                {
                    // Автоматически заполняем ФИО и телефон диспетчера
                    existingApp.DispatcherName = currentUser.FullName ?? currentUser.Username;
                    existingApp.DispatcherPhone = currentUser.Phone ?? "-";
                    existingApp.DispatcherUserId = currentUser.Id;

                    // Сохраняем примечание диспетчера
                    existingApp.DispatcherNotes = application.DispatcherNotes;

                    // Обновляем статус если нужно
                    if (actionType == "approve")
                        existingApp.Status = ApplicationStatus.Approved;
                    else if (actionType == "reject")
                        existingApp.Status = ApplicationStatus.RejectedByDispatcher;
                }

                // Обновляем информацию о водителе (если выбран)
                if (application.DriverId.HasValue && application.DriverId > 0)
                {
                    var driver = await _context.Drivers.FindAsync(application.DriverId.Value);
                    if (driver != null)
                    {
                        existingApp.DriverId = driver.Id;
                        existingApp.DriverName = driver.FullName;
                        existingApp.DriverPhone = driver.Phone ?? "-";
                    }
                }
                else if (!string.IsNullOrEmpty(application.DriverName))
                {
                    // Если ввели вручную ФИО водителя
                    existingApp.DriverId = null;
                    existingApp.DriverName = application.DriverName;
                    existingApp.DriverPhone = application.DriverPhone ?? "-";
                }

                // Обновляем информацию о ТС
                if (application.VehicleId.HasValue && application.VehicleId > 0)
                {
                    var vehicle = await _context.Vehicles.FindAsync(application.VehicleId.Value);
                    if (vehicle != null)
                    {
                        existingApp.VehicleId = vehicle.Id;
                        existingApp.VehicleBrand = vehicle.Brand;
                        existingApp.VehicleNumber = vehicle.PlateNumber;
                        existingApp.VehicleColor = vehicle.Color;
                    }
                }
                else if (!string.IsNullOrEmpty(application.VehicleBrand))
                {
                    existingApp.VehicleId = null;
                    existingApp.VehicleBrand = application.VehicleBrand;
                    existingApp.VehicleNumber = application.VehicleNumber;
                    existingApp.VehicleColor = application.VehicleColor;
                }

                existingApp.UpdatedAt = DateTime.UtcNow;

                // Если статус изменился, добавляем запись в историю
                if (oldStatus != existingApp.Status)
                {
                    var history = new StatusHistory
                    {
                        ApplicationId = existingApp.Id,
                        OldStatus = oldStatus.ToString(),
                        NewStatus = existingApp.Status.ToString(),
                        ChangedBy = currentUser?.FullName ?? currentUser?.Username ?? "System",
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportRequestSystem.Data;
using TransportRequestSystem.Models;

namespace TransportRequestSystem.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Главная страница с фильтрами
        public async Task<IActionResult> Index(ApplicationFilter filter, bool selectAll = false, bool reset = false)
        {
            if (reset)
                return RedirectToAction(nameof(Index));

            if (selectAll)
                filter.SelectedStatuses = Enum.GetValues<ApplicationStatus>().ToList();

            var query = _context.Applications
                .Include(a => a.StatusHistory)
                .Where(a => a.Status != ApplicationStatus.Deleted)
                .AsQueryable();

            // Применяем фильтры
            if (filter.DateFrom.HasValue)
                query = query.Where(a => a.ApplicationDate >= filter.DateFrom);

            if (filter.DateTo.HasValue)
                query = query.Where(a => a.ApplicationDate <= filter.DateTo);

            if (!string.IsNullOrEmpty(filter.OrganizationUnit))
                query = query.Where(a => a.OrganizationUnit.Contains(filter.OrganizationUnit));

            if (filter.SelectedStatuses?.Any() == true)
                query = query.Where(a => filter.SelectedStatuses.Contains(a.Status));

            var applications = await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            ViewBag.Filter = filter ?? new ApplicationFilter();
            ViewBag.Applications = applications;
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
                    ChangedDate = DateTime.Now
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

        // Получение данных заявки для редактирования
        public async Task<IActionResult> Edit(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            return application == null ? NotFound() : Json(application);
        }

        // Сохранение изменений заявки
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

                // Обновляем поля
                existingApp.ApplicationDate = application.ApplicationDate;
                existingApp.TripStart = application.TripStart;
                existingApp.TripEnd = application.TripEnd;
                existingApp.OrganizationUnit = application.OrganizationUnit;
                existingApp.ResponsiblePerson = application.ResponsiblePerson;
                existingApp.Phone = application.Phone;
                existingApp.Purpose = application.Purpose;
                existingApp.Passengers = application.Passengers ?? string.Empty;
                existingApp.Route = application.Route;
                existingApp.Notes = application.Notes ?? string.Empty;
                existingApp.DispatcherName = application.DispatcherName;
                existingApp.DispatcherPhone = application.DispatcherPhone;
                existingApp.DriverName = application.DriverName;
                existingApp.DriverPhone = application.DriverPhone;
                existingApp.VehicleBrand = application.VehicleBrand;
                existingApp.VehicleNumber = application.VehicleNumber;
                existingApp.VehicleColor = application.VehicleColor;
                existingApp.DispatcherNotes = application.DispatcherNotes;
                existingApp.UpdatedAt = DateTime.UtcNow;

                // Обновляем статус если нужно
                if (actionType == "approve")
                    existingApp.Status = ApplicationStatus.Approved;
                else if (actionType == "reject")
                    existingApp.Status = ApplicationStatus.RejectedByDispatcher;

                // Если статус изменился, добавляем запись в историю
                if (oldStatus != existingApp.Status)
                {
                    var history = new StatusHistory
                    {
                        ApplicationId = existingApp.Id,
                        OldStatus = oldStatus.ToString(),
                        NewStatus = existingApp.Status.ToString(),
                        ChangedBy = User.Identity.Name ?? "System",
                        ChangedDate = DateTime.Now
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

        // Удаление заявки (soft delete) удалено до востребования
        //[HttpPost]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var application = await _context.Applications.FindAsync(id);
        //    if (application == null)
        //        return NotFound();

        //    var oldStatus = application.Status.ToString();
        //    application.Status = ApplicationStatus.Deleted;
        //    application.UpdatedAt = DateTime.Now;

        //    await AddStatusHistory(id, oldStatus, "Удалена", "Система");
        //    await _context.SaveChangesAsync();

        //    TempData["Success"] = "Заявка удалена!";
        //    return RedirectToAction(nameof(Index));
        //}

        // Изменение статуса заявки
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, ApplicationStatus status)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
                return NotFound();

            var oldStatus = application.Status;
            application.Status = status;
            application.UpdatedAt = DateTime.Now;

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
                ChangedDate = DateTime.Now
            };
            _context.StatusHistory.Add(history);
            await _context.SaveChangesAsync();
        }
    }
}
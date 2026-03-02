using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TransportRequestSystem.Data;
using TransportRequestSystem.Models;
using TransportRequestSystem.Pages.Applications;

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
            {
                return RedirectToAction(nameof(Index));
            }

            if (selectAll)
            {
                filter.SelectedStatuses = Enum.GetValues<ApplicationStatus>().ToList();
            }

            var query = _context.Applications
                .Include(a => a.StatusHistory)
                .Where(a => a.Status != ApplicationStatus.Deleted)
                .AsQueryable();

            // Фильтры
            if (filter.DateFrom.HasValue)
                query = query.Where(a => a.ApplicationDate >= filter.DateFrom);

            if (filter.DateTo.HasValue)
                query = query.Where(a => a.ApplicationDate <= filter.DateTo);

            if (!string.IsNullOrEmpty(filter.OrganizationUnit))
                query = query.Where(a => a.OrganizationUnit.Contains(filter.OrganizationUnit));

            if (filter.SelectedStatuses != null && filter.SelectedStatuses.Any())
                query = query.Where(a => filter.SelectedStatuses.Contains(a.Status));

            var applications = await query.OrderByDescending(a => a.CreatedAt).ToListAsync();

            ViewBag.Filter = filter ?? new ApplicationFilter();
            return View(applications);
        }

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

        // GET: Создание заявки 
        public IActionResult Create()
        {
            var application = new Application
            {
                Number = Application.GenerateNumber(),
                Status = ApplicationStatus.CreatedOrModified,
                ApplicationDate = DateTime.Today
            };

            // Если это AJAX запрос, возвращаем частичное представление
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_CreateModal", application);
            }

            return View(application);
        }
        // POST: Создание заявки
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Application application)
        {
            try
            {
                // Генерируем номер ПЕРВЫМ ДЕЛОМ
                if (string.IsNullOrEmpty(application.Number))
                {
                    application.Number = Application.GenerateNumber();
                    Console.WriteLine($"Generated number: {application.Number}");
                }

                application.Id = 0;

                // Конвертируем даты
                application.ApplicationDate = DateTime.SpecifyKind(application.ApplicationDate, DateTimeKind.Utc);
                if (application.TripStart.HasValue)
                    application.TripStart = DateTime.SpecifyKind(application.TripStart.Value, DateTimeKind.Utc);
                if (application.TripEnd.HasValue)
                    application.TripEnd = DateTime.SpecifyKind(application.TripEnd.Value, DateTimeKind.Utc);

                application.CreatedAt = DateTime.UtcNow;
                application.Status = ApplicationStatus.CreatedOrModified;

                // Проверяем ModelState ПОСЛЕ установки номера
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage);
                    Console.WriteLine("ModelState errors: " + string.Join(", ", errors));
                    TempData["Error"] = "Ошибка валидации: " + string.Join(", ", errors);
                    return RedirectToAction(nameof(Index));
                }

                _context.Applications.Add(application);
                await _context.SaveChangesAsync();

                Console.WriteLine($"SUCCESS! Saved application with ID: {application.Id}");
                TempData["Success"] = $"Заявка {application.Number} успешно создана!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner: {ex.InnerException.Message}");
                TempData["Error"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }


        // Редактирование заявки (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
                return NotFound();

            return Json(application); // Возвращаем JSON
        }

        // Редактирование заявки (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Application application)
        {
            if (id != application.Id)
                return NotFound();
            ModelState.Remove("Number");
            ModelState.Remove("Status");

            try
            {
                var existingApp = await _context.Applications.FindAsync(id);
                if (existingApp == null)
                    return NotFound();

                // Обновляем поля
                existingApp.Number = application.Number; 
                existingApp.Status = application.Status;
                existingApp.ApplicationDate = application.ApplicationDate;
                existingApp.TripStart = application.TripStart;
                existingApp.TripEnd = application.TripEnd;
                existingApp.OrganizationUnit = application.OrganizationUnit;
                existingApp.ResponsiblePerson = application.ResponsiblePerson;
                existingApp.Phone = application.Phone;
                existingApp.Purpose = application.Purpose;
                existingApp.Passengers = application.Passengers;
                existingApp.Route = application.Route;
                existingApp.Notes = application.Notes;
                existingApp.DispatcherName = application.DispatcherName;
                existingApp.DispatcherPhone = application.DispatcherPhone;
                existingApp.DriverName = application.DriverName;
                existingApp.DriverPhone = application.DriverPhone;
                existingApp.VehicleBrand = application.VehicleBrand;
                existingApp.VehicleNumber = application.VehicleNumber;
                existingApp.VehicleColor = application.VehicleColor;
                existingApp.DispatcherNotes = application.DispatcherNotes;
                existingApp.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Заявка обновлена!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
        // Удаление заявки
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
                return NotFound();

            var oldStatus = application.Status.ToString();
            application.Status = ApplicationStatus.Deleted;
            application.UpdatedAt = DateTime.Now;

            await AddStatusHistory(id, oldStatus, "Удалена", "Система");
            await _context.SaveChangesAsync();

            TempData["Success"] = "Заявка удалена!";
            return RedirectToAction(nameof(Index));
        }

        // Изменение статуса
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

        // Вспомогательный метод для истории статусов
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
        }

    }
}
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
        //private readonly CreateModel _createModel;

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
            if (ModelState.IsValid)
            {
                application.CreatedAt = DateTime.Now;
                _context.Add(application);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Заявка успешно создана!";

                return RedirectToAction(nameof(Index));
            }

            // Если есть ошибки, возвращаем форму с ошибками
            return PartialView("_CreateModal", application);
        }


        // Редактирование заявки (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var application = await _context.Applications
                .Include(a => a.StatusHistory)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return NotFound();

            return View(application);
        }

        // Редактирование заявки (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Application application)
        {
            if (id != application.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var oldApp = await _context.Applications.FindAsync(id);
                if (oldApp == null)
                    return NotFound();

                var oldStatus = oldApp.Status;

                // Сохраняем неизменяемые поля
                application.CreatedAt = oldApp.CreatedAt;
                application.Number = oldApp.Number;
                application.UpdatedAt = DateTime.Now;

                _context.Entry(oldApp).CurrentValues.SetValues(application);

                // История изменений статуса
                if (oldStatus != application.Status)
                {
                    await AddStatusHistory(id, oldStatus.ToString(),
                        application.Status.ToString(), "Система");
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Заявка обновлена!";
                return RedirectToAction(nameof(Index));
            }

            return View(application);
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
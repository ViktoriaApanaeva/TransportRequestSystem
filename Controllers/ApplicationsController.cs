using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<IActionResult> Index(ApplicationFilter filter)
        {
            var query = _context.Applications
                .Where(a => a.Status != ApplicationStatus.Deleted)
                .AsQueryable();

            if (filter.DateFrom.HasValue)
            {
                query = query.Where(a => a.ApplicationDate >= filter.DateFrom.Value);
            }
            if (filter.DateTo.HasValue)
            {
                query = query.Where(a => a.ApplicationDate <= filter.DateTo.Value);
            }
            if (!string.IsNullOrEmpty(filter.OrganizationUnit))
            {
                query = query.Where(a => a.OrganizationUnit.Contains(filter.OrganizationUnit));
            }
            if (filter.SelectedStatuses != null && filter.SelectedStatuses.Any())
            {
                query = query.Where(a => filter.SelectedStatuses.Contains(a.Status));
            }
            var applications = await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            ViewBag.Filter = filter;
            return View(applications);
        }

        public IActionResult Create()
        {
            var application = new Models.Application
            {
                Number = Application.GenerateNumber(),
            };
            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Application application)
        {
            if (ModelState.IsValid)
            {
                application.Number = Models.Application.GenerateNumber();
                application.CreatedAt = DateTime.Now;
                application.Status = ApplicationStatus.CreatedOrModified;

                _context.Add(application);
                await _context.SaveChangesAsync();
                 
                // Создаем запись в истории статусов
                await CreateStatusHistory(application.Id, null, application.Status.ToString(),
                    User.Identity?.Name ?? "System", "Создание заявки");

                return RedirectToAction(nameof(Index));
            }
            return View(application);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var application = await _context.Applications
                .Include(a => a.StatusHistory)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.Application application)
        {
            if (id != application.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldStatus = await _context.Applications
                        .Where(a => a.Id == id)
                        .Select(a => a.Status)
                        .FirstOrDefaultAsync();

                    application.UpdatedAt = DateTime.Now;
                    _context.Update(application);

                    // Если статус изменился, добавляем запись в историю
                    if (oldStatus != application.Status)
                    {
                        await CreateStatusHistory(id, oldStatus.ToString(),
                            application.Status.ToString(),
                            User.Identity?.Name ?? "System",
                            "Редактирование заявки");
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationExists(application.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application != null)
            {
                application.Status = ApplicationStatus.Deleted;
                application.UpdatedAt = DateTime.Now;

                await CreateStatusHistory(id, application.Status.ToString(),
                    ApplicationStatus.Deleted.ToString(),
                    User.Identity?.Name ?? "System",
                    "Удаление заявки");

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            return await ChangeStatus(id, ApplicationStatus.Approved, "Утвердить");
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            return await ChangeStatus(id, ApplicationStatus.RejectedByDirector, "Отклонение заявки руководителем");
        }

        [HttpPost]
        public async Task<IActionResult> AssignVehicle(int id)
        {
            return await ChangeStatus(id, ApplicationStatus.AssignedToVehicle, "Назначение ТС");
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            return await ChangeStatus(id, ApplicationStatus.Completed, "Исполнение");
        }

        private async Task<IActionResult> ChangeStatus(int id, ApplicationStatus newStatus, string comment)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            var oldStatus = application.Status;
            application.Status = newStatus;
            application.UpdatedAt = DateTime.Now;

            await CreateStatusHistory(id, oldStatus.ToString(), newStatus.ToString(),
                User.Identity?.Name ?? "System", comment);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id });
        }

        private async Task CreateStatusHistory(int applicationId, string? oldStatus,
            string newStatus, string changedBy, string comment)
        {
            var history = new StatusHistory
            {
                ApplicationId = applicationId,
                OldStatus = oldStatus ?? "Не установлен",
                NewStatus = newStatus,
                ChangedBy = changedBy,
                Comment = comment,
                ChangedDate = DateTime.Now
            };

            _context.StatusHistory.Add(history);
        }

        private bool ApplicationExists(int id)
        {
            return _context.Applications.Any(e => e.Id == id);
        }
    }

    public class DashboardViewModel
    {
        public int TotalApplications { get; set; }
        public int TodayApplications { get; set; }
        public int PendingApplications { get; set; }
        public int UrgentApplications { get; set; }
        public List<Models.Application> RecentApplications { get; set; } = new();
    }
}

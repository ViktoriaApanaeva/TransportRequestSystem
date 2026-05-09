using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TransportRequestSystem.Data;
using TransportRequestSystem.Models;

namespace TransportRequestSystem.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsController(ApplicationDbContext context)
        {
            _context = context;
            ExcelPackage.License.SetNonCommercialPersonal("Transport Request System");
        }

        public async Task<IActionResult> Index(DateTime? dateFrom, DateTime? dateTo)
        {
            // По умолчанию последние 30 дней (в UTC)
            dateFrom ??= DateTime.UtcNow.AddDays(-30).Date;
            dateTo ??= DateTime.UtcNow.Date;

            // Преобразуем в UTC для запроса к БД
            var fromUtc = DateTime.SpecifyKind(dateFrom.Value.Date, DateTimeKind.Utc);
            var toUtc = DateTime.SpecifyKind(dateTo.Value.Date.AddDays(1), DateTimeKind.Utc);

            var query = _context.Applications
                .Where(a => a.Status != ApplicationStatus.Deleted)
                .Where(a => a.CreatedAt >= fromUtc && a.CreatedAt <= toUtc);

            var applications = await query.ToListAsync();

            // Статистика по дням
            var dailyStats = applications
                .GroupBy(a => a.CreatedAt.Date)
                .ToDictionary(g => g.Key.ToString("dd.MM"), g => g.Count());

            // Распределение по статусам
            var statusStats = applications
                .GroupBy(a => GetStatusName(a.Status))
                .ToDictionary(g => g.Key, g => g.Count());

            int periodDays = (dateTo.Value.Date - dateFrom.Value.Date).Days + 1;

            // Группировка заявок по виду транспорта (на основе количества пассажиров)
            var vehicleTypeStats = applications
                .Where(a => !string.IsNullOrEmpty(a.Passengers))
                .Select(a => new { a.Passengers, CalculatedType = GetVehicleTypeByPassengers(a.Passengers) })
                .Where(x => x.CalculatedType != "Не определен")
                .GroupBy(x => x.CalculatedType)
                .Select(g => new
                {
                    VehicleType = g.Key,
                    AveragePerDay = Math.Round((double)g.Count() / periodDays, 1)
                })
                .OrderByDescending(x => x.AveragePerDay)
                .ToDictionary(x => x.VehicleType, x => x.AveragePerDay);

            // Использование транспорта (по брендам)
            var vehicleStats = applications
                .Where(a => !string.IsNullOrEmpty(a.VehicleBrand))
                .GroupBy(a => a.VehicleBrand)
                .ToDictionary(g => g.Key, g => g.Count());

            // Среднее время выполнения
            var completedApps = applications
                .Where(a => a.Status == ApplicationStatus.Completed && a.TripEnd.HasValue)
                .Select(a => (a.TripEnd.Value - a.CreatedAt).TotalHours)
                .ToList();

            double avgTime = completedApps.Any() ? Math.Round(completedApps.Average(), 1) : 0;

            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;
            ViewBag.DailyStats = dailyStats;
            ViewBag.StatusStats = statusStats;
            ViewBag.VehicleTypeStats = vehicleTypeStats;
            ViewBag.VehicleStats = vehicleStats;
            ViewBag.AvgTime = avgTime;
            ViewBag.TotalApps = applications.Count;

            return View(applications);
        }

        [HttpGet]
        public async Task<IActionResult> ExportSimple(DateTime? dateFrom, DateTime? dateTo)
        {
            try
            {
                var from = dateFrom ?? DateTime.UtcNow.AddDays(-30).Date;
                var to = dateTo ?? DateTime.UtcNow.Date;

                var fromUtc = DateTime.SpecifyKind(from.Date, DateTimeKind.Utc);
                var toUtc = DateTime.SpecifyKind(to.Date.AddDays(1), DateTimeKind.Utc);

                var applications = await _context.Applications
                    .Where(a => a.Status != ApplicationStatus.Deleted)
                    .Where(a => a.CreatedAt >= fromUtc && a.CreatedAt <= toUtc)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();

                if (!applications.Any())
                {
                    TempData["Error"] = "Нет данных за выбранный период";
                    return RedirectToAction(nameof(Index));
                }

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Заявки");

                string[] headers = {
                    "№ заявки", "Дата создания", "Статус", "Организация",
                    "Ответственный", "Телефон", "Цель", "Маршрут",
                    "Пассажиры", "Водитель", "Транспорт", "Дата поездки", "Примечание"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                for (int i = 0; i < applications.Count; i++)
                {
                    var app = applications[i];
                    int row = i + 2;

                    worksheet.Cells[row, 1].Value = app.Number;
                    worksheet.Cells[row, 2].Value = app.CreatedAt.ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cells[row, 3].Value = GetStatusName(app.Status);
                    worksheet.Cells[row, 4].Value = app.OrganizationUnit;
                    worksheet.Cells[row, 5].Value = app.ResponsiblePerson;
                    worksheet.Cells[row, 6].Value = app.Phone;
                    worksheet.Cells[row, 7].Value = app.Purpose;
                    worksheet.Cells[row, 8].Value = app.Route;
                    worksheet.Cells[row, 9].Value = app.Passengers;
                    worksheet.Cells[row, 10].Value = app.DriverName;
                    worksheet.Cells[row, 11].Value = string.IsNullOrEmpty(app.VehicleBrand) ? "-" : $"{app.VehicleBrand} {app.VehicleNumber}";
                    worksheet.Cells[row, 12].Value = app.TripStart?.ToString("dd.MM.yyyy HH:mm") ?? "-";
                    worksheet.Cells[row, 13].Value = app.Notes;
                }

                worksheet.Cells.AutoFitColumns();

                string fileName = $"Заявки_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                byte[] fileContents = package.GetAsByteArray();

                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                TempData["Error"] = $"Ошибка: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private string GetStatusName(ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.CreatedOrModified => "Создана",
                ApplicationStatus.Approved => "Утверждена",
                ApplicationStatus.InProgress => "Исполняется",
                ApplicationStatus.Completed => "Исполнена",
                ApplicationStatus.RejectedByDispatcher => "Отклонена диспетчером",
                ApplicationStatus.RejectedByDirector => "Отклонена руководителем",
                ApplicationStatus.AssignedToVehicle => "Назначено ТС",
                ApplicationStatus.NotCompleted => "Не исполнена",
                ApplicationStatus.Deleted => "Удалена",
                _ => status.ToString()
            };
        }

        private string GetVehicleTypeByPassengers(string passengers)
        {
            if (string.IsNullOrEmpty(passengers)) return "Не определен";

            if (int.TryParse(passengers, out int count))
            {
                if (count <= 4) return "Легковой автомобиль";
                if (count <= 8) return "Микроавтобус";
                if (count <= 16) return "Грузовой фургон";
                return "Автобус";
            }

            return "Не определен";
        }
    }
}
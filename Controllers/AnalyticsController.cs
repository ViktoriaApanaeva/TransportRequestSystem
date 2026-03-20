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
            // Настройка лицензии EPPlus 
            ExcelPackage.License.SetNonCommercialPersonal("Transport Request System");
        }

        // Главная страница аналитики
        public async Task<IActionResult> Index(DateTime? dateFrom, DateTime? dateTo)
        {
            // По умолчанию последние 30 дней
            dateFrom ??= DateTime.Today.AddDays(-30);
            dateTo ??= DateTime.Today;

            var query = _context.Applications
                .Where(a => a.Status != ApplicationStatus.Deleted)
                .Where(a => a.CreatedAt >= dateFrom && a.CreatedAt <= dateTo.Value.AddDays(1));

            var applications = await query.ToListAsync();

            // Статистика по дням
            var dailyStats = applications
                .GroupBy(a => a.CreatedAt.Date)
                .ToDictionary(g => g.Key.ToString("dd.MM"), g => g.Count());

            // Распределение по статусам
            var statusStats = applications
                .GroupBy(a => GetStatusName(a.Status))
                .ToDictionary(g => g.Key, g => g.Count());

            // Использование транспорта
            var vehicleStats = applications
                .Where(a => !string.IsNullOrEmpty(a.VehicleBrand))
                .GroupBy(a => $"{a.VehicleBrand} {a.VehicleNumber}")
                .ToDictionary(g => g.Key, g => g.Count());

            // Среднее время выполнения
            var completedApps = applications
                .Where(a => a.Status == ApplicationStatus.Completed && a.UpdatedAt.HasValue)
                .Select(a => (a.UpdatedAt.Value - a.CreatedAt).TotalHours);

            double avgTime = completedApps.Any() ? completedApps.Average() : 0;

            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;
            ViewBag.DailyStats = dailyStats;
            ViewBag.StatusStats = statusStats;
            ViewBag.VehicleStats = vehicleStats;
            ViewBag.AvgTime = Math.Round(avgTime, 1);
            ViewBag.TotalApps = applications.Count;

            return View(applications);
        }

        // Экспорт в Excel
        [HttpGet]
        public async Task<IActionResult> ExportSimple(DateTime? dateFrom, DateTime? dateTo)
        {
            try
            {
                var from = dateFrom ?? DateTime.Today.AddDays(-30);
                var to = dateTo ?? DateTime.Today;

                // Получаем данные
                var applications = await _context.Applications
                    .Where(a => a.Status != ApplicationStatus.Deleted)
                    .Where(a => a.CreatedAt >= from && a.CreatedAt <= to.AddDays(1))
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();

                // Проверяем, есть ли данные
                if (!applications.Any())
                {
                    TempData["Error"] = "Нет данных за выбранный период";
                    return RedirectToAction(nameof(Index));
                }

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Заявки");

                // Заголовки
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

                // Данные
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

                // Формируем имя файла
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
    }
}
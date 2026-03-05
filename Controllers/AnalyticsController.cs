using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
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
        [HttpPost]
        public async Task<IActionResult> ExportToExcel(ReportFilter filter)
        {
            var query = _context.Applications
                .Include(a => a.StatusHistory)
                .Where(a => a.Status != ApplicationStatus.Deleted)
                .AsQueryable();

            if (filter.DateFrom.HasValue)
                query = query.Where(a => a.CreatedAt >= filter.DateFrom);

            if (filter.DateTo.HasValue)
                query = query.Where(a => a.CreatedAt <= filter.DateTo.Value.AddDays(1));

            if (!string.IsNullOrEmpty(filter.OrganizationUnit))
                query = query.Where(a => a.OrganizationUnit.Contains(filter.OrganizationUnit));

            if (filter.SelectedStatuses?.Any() == true)
                query = query.Where(a => filter.SelectedStatuses.Contains(a.Status));

            var applications = await query.OrderByDescending(a => a.CreatedAt).ToListAsync();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Заявки");

            // Заголовки
            worksheet.Cells[1, 1].Value = "№ заявки";
            worksheet.Cells[1, 2].Value = "Дата создания";
            worksheet.Cells[1, 3].Value = "Статус";
            worksheet.Cells[1, 4].Value = "Организация";
            worksheet.Cells[1, 5].Value = "Ответственный";
            worksheet.Cells[1, 6].Value = "Телефон";
            worksheet.Cells[1, 7].Value = "Цель";
            worksheet.Cells[1, 8].Value = "Маршрут";
            worksheet.Cells[1, 9].Value = "Пассажиры";
            worksheet.Cells[1, 10].Value = "Водитель";
            worksheet.Cells[1, 11].Value = "Транспорт";
            worksheet.Cells[1, 12].Value = "Дата поездки";
            worksheet.Cells[1, 13].Value = "Примечание";

            // Стиль заголовков
            using (var range = worksheet.Cells[1, 1, 1, 13])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
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
                worksheet.Cells[row, 11].Value = $"{app.VehicleBrand} {app.VehicleNumber}";
                worksheet.Cells[row, 12].Value = app.TripStart?.ToString("dd.MM.yyyy HH:mm") ?? "-";
                worksheet.Cells[row, 13].Value = app.Notes;

                // Альтернативная подсветка строк
                if (i % 2 == 0)
                {
                    worksheet.Cells[row, 1, row, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1, row, 13].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }
            }

            // Авто-подбор ширины колонок
            worksheet.Cells.AutoFitColumns();

            // Создаем отдельный лист со статистикой
            var statsSheet = package.Workbook.Worksheets.Add("Статистика");

            statsSheet.Cells[1, 1].Value = "Показатель";
            statsSheet.Cells[1, 2].Value = "Значение";
            statsSheet.Cells[1, 1, 1, 2].Style.Font.Bold = true;

            statsSheet.Cells[2, 1].Value = "Всего заявок";
            statsSheet.Cells[2, 2].Value = applications.Count;

            statsSheet.Cells[3, 1].Value = "Среднее время выполнения (часов)";
            var completed = applications.Where(a => a.Status == ApplicationStatus.Completed && a.UpdatedAt.HasValue);
            double avgHours = completed.Any()
                ? completed.Average(a => (a.UpdatedAt.Value - a.CreatedAt).TotalHours)
                : 0;
            statsSheet.Cells[3, 2].Value = Math.Round(avgHours, 1);

            statsSheet.Cells[4, 1].Value = "Период";
            statsSheet.Cells[4, 2].Value = $"{filter.DateFrom:dd.MM.yyyy} - {filter.DateTo:dd.MM.yyyy}";

            // Статистика по статусам
            statsSheet.Cells[6, 1].Value = "Статус";
            statsSheet.Cells[6, 2].Value = "Количество";
            statsSheet.Cells[6, 1, 6, 2].Style.Font.Bold = true;

            int statusRow = 7;
            foreach (var status in Enum.GetValues<ApplicationStatus>())
            {
                var count = applications.Count(a => a.Status == status);
                if (count > 0)
                {
                    statsSheet.Cells[statusRow, 1].Value = GetStatusName(status);
                    statsSheet.Cells[statusRow, 2].Value = count;
                    statusRow++;
                }
            }

            statsSheet.Cells.AutoFitColumns();

            // Формируем имя файла
            string fileName = $"Заявки_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            byte[] fileContents = package.GetAsByteArray();

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // Экспорт краткого отчета
        [HttpPost]
        public async Task<IActionResult> ExportSummary(ReportFilter filter)
        {
            var query = _context.Applications
                .Where(a => a.Status != ApplicationStatus.Deleted)
                .AsQueryable();

            if (filter.DateFrom.HasValue)
                query = query.Where(a => a.CreatedAt >= filter.DateFrom);

            if (filter.DateTo.HasValue)
                query = query.Where(a => a.CreatedAt <= filter.DateTo.Value.AddDays(1));

            var applications = await query.ToListAsync();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Сводка");

            // Заголовок отчета
            worksheet.Cells[1, 1].Value = "Отчет по заявкам";
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[2, 1].Value = $"Период: {filter.DateFrom:dd.MM.yyyy} - {filter.DateTo:dd.MM.yyyy}";
            worksheet.Cells[2, 1].Style.Font.Size = 12;

            // Основные показатели
            worksheet.Cells[4, 1].Value = "Основные показатели";
            worksheet.Cells[4, 1].Style.Font.Bold = true;

            worksheet.Cells[5, 1].Value = "Всего заявок:";
            worksheet.Cells[5, 2].Value = applications.Count;

            worksheet.Cells[6, 1].Value = "Выполнено:";
            worksheet.Cells[6, 2].Value = applications.Count(a => a.Status == ApplicationStatus.Completed);

            worksheet.Cells[7, 1].Value = "В работе:";
            worksheet.Cells[7, 2].Value = applications.Count(a => a.Status == ApplicationStatus.InProgress);

            worksheet.Cells[8, 1].Value = "Ожидают:";
            worksheet.Cells[8, 2].Value = applications.Count(a => a.Status == ApplicationStatus.CreatedOrModified);

            worksheet.Cells[9, 1].Value = "Отменено:";
            worksheet.Cells[9, 2].Value = applications.Count(a => a.Status == ApplicationStatus.RejectedByDispatcher
                                                                 || a.Status == ApplicationStatus.RejectedByDirector);

            // Статистика по транспорту
            var vehicleGroups = applications
                .Where(a => !string.IsNullOrEmpty(a.VehicleBrand))
                .GroupBy(a => a.VehicleBrand)
                .Select(g => new { Brand = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5);

            worksheet.Cells[11, 1].Value = "Популярный транспорт";
            worksheet.Cells[11, 1].Style.Font.Bold = true;

            int vehicleRow = 12;
            foreach (var item in vehicleGroups)
            {
                worksheet.Cells[vehicleRow, 1].Value = item.Brand;
                worksheet.Cells[vehicleRow, 2].Value = item.Count;
                vehicleRow++;
            }

            worksheet.Cells.AutoFitColumns();

            byte[] fileContents = package.GetAsByteArray();
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Сводка_{DateTime.Now:yyyyMMdd}.xlsx");
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
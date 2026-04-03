using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "SystemAdmin")]
    public class SystemAdminController : Controller
    {
        private readonly SystemAdminAnalyticsService _analyticsService;

        public SystemAdminController(SystemAdminAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        // GET: Главная панель системного администратора
        public async Task<IActionResult> Dashboard(DateTime? start, DateTime? end)
        {
            var startDate = start ?? DateTime.Now.AddMonths(-1);
            var endDate = end ?? DateTime.Now;

            // Получаем все отчёты
            var doctorReport = await _analyticsService.GetDoctorAnalyticsReportAsync(startDate, endDate);
            var serviceReport = await _analyticsService.GetServicePopularityReportAsync(startDate, endDate);
            var patientReport = await _analyticsService.GetPatientActivityReportAsync(startDate, endDate);
            var systemSummary = await _analyticsService.GetSystemSummaryAsync(startDate, endDate);
            var hourlyLoad = await _analyticsService.GetHourlyLoadReportAsync(startDate, endDate);

            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
            ViewBag.DoctorReport = doctorReport;
            ViewBag.ServiceReport = serviceReport;
            ViewBag.PatientReport = patientReport;
            ViewBag.SystemSummary = systemSummary;
            ViewBag.HourlyLoad = hourlyLoad;

            return View();
        }

        // GET: Детальный отчёт по врачам
        public async Task<IActionResult> DoctorReport(DateTime? start, DateTime? end)
        {
            var startDate = start ?? DateTime.Now.AddMonths(-1);
            var endDate = end ?? DateTime.Now;

            var report = await _analyticsService.GetDoctorAnalyticsReportAsync(startDate, endDate);

            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
            ViewBag.Report = report;

            return View(report);
        }

        // GET: Отчёт по популярности услуг
        public async Task<IActionResult> ServiceReport(DateTime? start, DateTime? end)
        {
            var startDate = start ?? DateTime.Now.AddMonths(-1);
            var endDate = end ?? DateTime.Now;

            var report = await _analyticsService.GetServicePopularityReportAsync(startDate, endDate);

            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
            ViewBag.Report = report;

            return View(report);
        }

        // GET: Отчёт по активности пациентов
        public async Task<IActionResult> PatientReport(DateTime? start, DateTime? end)
        {
            var startDate = start ?? DateTime.Now.AddMonths(-1);
            var endDate = end ?? DateTime.Now;

            var report = await _analyticsService.GetPatientActivityReportAsync(startDate, endDate);

            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
            ViewBag.Report = report;

            return View(report);
        }

        // GET: Почасовая загрузка
        public async Task<IActionResult> HourlyLoadReport(DateTime? start, DateTime? end)
        {
            var startDate = start ?? DateTime.Now.AddMonths(-1);
            var endDate = end ?? DateTime.Now;

            var report = await _analyticsService.GetHourlyLoadReportAsync(startDate, endDate);

            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
            ViewBag.Report = report;

            return View(report);
        }

        // POST: Экспорт отчёта (CSV)
        [HttpPost]
        public async Task<IActionResult> ExportDoctorReport(DateTime start, DateTime end)
        {
            var report = await _analyticsService.GetDoctorAnalyticsReportAsync(start, end);
            
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("DoctorId,FullName,TotalAppointments,CompletedAppointments,CancelledAppointments,TotalRevenue,AverageCheck,UtilizationRate");
            
            foreach (var item in report)
            {
                csv.AppendLine($"{item.DoctorId},{item.FullName},{item.TotalAppointments},{item.CompletedAppointments},{item.CancelledAppointments},{item.TotalRevenue:F2},{item.AverageCheck:F2},{item.UtilizationRate:F2}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"doctor_report_{start:yyyyMMdd}_{end:yyyyMMdd}.csv");
        }
    }
}

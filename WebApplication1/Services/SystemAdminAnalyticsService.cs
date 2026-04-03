using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    /// <summary>
    /// Сервис для системного администратора - предоставляет сложную аналитику и отчётность
    /// </summary>
    public class SystemAdminAnalyticsService
    {
        private readonly MyDbContext _context;

        public SystemAdminAnalyticsService(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Сложный SELECT запрос для системного администратора
        /// Возвращает комплексную аналитику по системе:
        /// - Загрузка врачей по часам и дням
        /// - Статистика записей (новые, отменённые, завершённые)
        /// - Популярность медицинских услуг
        /// - Конверсия онлайн-записи
        /// - Информация о пациентах и их активности
        /// </summary>
        public IQueryable<DoctorAnalyticsDto> GetDoctorAnalyticsReport(DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateOnly.FromDateTime(DateTime.Today).AddDays(-30).ToDateTime(TimeOnly.MinValue);
            var end = endDate ?? DateTime.Now;
            var startDateOnly = DateOnly.FromDateTime(start);
            var endDateOnly = DateOnly.FromDateTime(end);

            // Сложный SELECT запрос с множественными JOIN и агрегацией
            var query = from doctor in _context.Doctors
                        join specialty in _context.Specialties on doctor.SpecializationId equals specialty.SpecializationId
                        select new DoctorAnalyticsDto
                        {
                            DoctorId = doctor.DoctorId,
                            DoctorFullName = doctor.FullName,
                            SpecializationName = specialty.Name,
                            
                            // Общее количество рабочих дней
                            TotalWorkDays = _context.Schedules
                                .Where(s => s.DoctorId == doctor.DoctorId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly)
                                .Select(s => s.Date)
                                .Distinct()
                                .Count(),
                            
                            // Общее количество рабочих часов
                            TotalWorkHours = _context.Schedules
                                .Where(s => s.DoctorId == doctor.DoctorId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly)
                                .Sum(s => (decimal)(s.EndTime.Hour - s.StartTime.Hour + (s.EndTime.Minute - s.StartTime.Minute) / 60.0)),
                            
                            // Количество записей всего
                            TotalAppointments = _context.RecordingSlots
                                .Where(s => s.DoctorId == doctor.DoctorId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly)
                                .Count(),
                            
                            // Количество завершённых записей (есть запись в Appointment)
                            CompletedAppointments = _context.RecordingSlots
                                .Where(s => s.DoctorId == doctor.DoctorId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly 
                                         && s.Appointment != null)
                                .Count(),
                            
                            // Количество отменённых записей (PatientId = null после бронирования)
                            CancelledAppointments = _context.RecordingSlots
                                .Where(s => s.DoctorId == doctor.DoctorId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly 
                                         && s.PatientId == null)
                                .Count(),
                            
                            // Общая выручка врача
                            TotalRevenue = _context.RecordingSlots
                                .Where(s => s.DoctorId == doctor.DoctorId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly 
                                         && s.Appointment != null)
                                .Sum(s => (decimal?)s.Service.Price) ?? 0,
                            
                            // Средняя стоимость приёма
                            AverageAppointmentCost = _context.RecordingSlots
                                .Where(s => s.DoctorId == doctor.DoctorId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly 
                                         && s.Appointment != null && s.Service.Price > 0)
                                .Average(s => (decimal?)s.Service.Price) ?? 0,
                            
                            // Количество уникальных пациентов
                            UniquePatients = _context.RecordingSlots
                                .Where(s => s.DoctorId == doctor.DoctorId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly 
                                         && s.PatientId != null)
                                .Select(s => s.PatientId)
                                .Distinct()
                                .Count(),
                            
                            // Загрузка по дням недели (понедельник)
                            MondayAppointments = _context.RecordingSlots
                                .Where(s => s.DoctorId == doctor.DoctorId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly 
                                         && s.Date.DayOfWeek == DayOfWeek.Monday)
                                .Count(),
                            
                            // Загрузка по дням недели (вторник)
                            TuesdayAppointments = _context.RecordingSlots
                                .Where(s => s.DoctorId == doctor.DoctorId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly 
                                         && s.Date.DayOfWeek == DayOfWeek.Tuesday)
                                .Count(),
                            
                            // Загрузка по дням недели (среда)
                            WednesdayAppointments = _context.RecordingSlots
                                .Where(s => s.DoctorId == doctor.DoctorId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly 
                                         && s.Date.DayOfWeek == DayOfWeek.Wednesday)
                                .Count(),
                            
                            // Загрузка по дням недели (четверг)
                            ThursdayAppointments = _context.RecordingSlots
                                .Where(s => s.DoctorId == doctor.DoctorId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly 
                                         && s.Date.DayOfWeek == DayOfWeek.Thursday)
                                .Count(),
                            
                            // Загрузка по дням недели (пятница)
                            FridayAppointments = _context.RecordingSlots
                                .Where(s => s.DoctorId == doctor.DoctorId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly 
                                         && s.Date.DayOfWeek == DayOfWeek.Friday)
                                .Count(),
                            
                            // Процент заполненности расписания
                            ScheduleUtilizationPercent = _context.Schedules
                                .Where(s => s.DoctorId == doctor.DoctorId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly)
                                .Any() 
                                ? (_context.RecordingSlots
                                    .Where(s => s.DoctorId == doctor.DoctorId 
                                             && s.Date >= startDateOnly && s.Date <= endDateOnly 
                                             && s.PatientId != null)
                                    .Count() * 100.0 / 
                                   (_context.Schedules
                                    .Where(s => s.DoctorId == doctor.DoctorId 
                                             && s.Date >= startDateOnly && s.Date <= endDateOnly)
                                    .Sum(s => (double)(s.EndTime.Hour - s.StartTime.Hour)) * 2)) // 2 записи в час
                                : 0
                        };

            return query;
        }

        /// <summary>
        /// Отчёт по популярности медицинских услуг
        /// </summary>
        public IQueryable<ServicePopularityDto> GetServicePopularityReport(DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateOnly.FromDateTime(DateTime.Today).AddMonths(-3).ToDateTime(TimeOnly.MinValue);
            var end = endDate ?? DateTime.Now;
            var startDateOnly = DateOnly.FromDateTime(start);
            var endDateOnly = DateOnly.FromDateTime(end);

            var query = from service in _context.ServicesClinics
                        select new ServicePopularityDto
                        {
                            ServiceId = service.ServiceId,
                            ServiceName = service.Name,
                            Price = service.Price,
                            TotalBookings = _context.RecordingSlots
                                .Where(s => s.ServiceId == service.ServiceId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly)
                                .Count(),
                            UniquePatients = _context.RecordingSlots
                                .Where(s => s.ServiceId == service.ServiceId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly 
                                         && s.PatientId != null)
                                .Select(s => s.PatientId)
                                .Distinct()
                                .Count(),
                            TotalRevenue = _context.RecordingSlots
                                .Where(s => s.ServiceId == service.ServiceId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly)
                                .Sum(s => (decimal?)service.Price) ?? 0,
                            AvgBookingsPerDay = _context.RecordingSlots
                                .Where(s => s.ServiceId == service.ServiceId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly)
                                .Count() / 
                                (double)((endDateOnly.DayNumber - startDateOnly.DayNumber) + 1),
                            PopularityRank = 0 // Будет рассчитано отдельно
                        };

            return query.OrderByDescending(s => s.TotalBookings);
        }

        /// <summary>
        /// Отчёт по активности пациентов
        /// </summary>
        public IQueryable<PatientActivityDto> GetPatientActivityReport(DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateOnly.FromDateTime(DateTime.Today).AddMonths(-6).ToDateTime(TimeOnly.MinValue);
            var end = endDate ?? DateTime.Now;
            var startDateOnly = DateOnly.FromDateTime(start);
            var endDateOnly = DateOnly.FromDateTime(end);

            var query = from patient in _context.Patients
                        select new PatientActivityDto
                        {
                            PatientId = patient.PatientId,
                            FullName = patient.FullName,
                            Email = patient.Email,
                            Phone = patient.Phone,
                            BirthDate = patient.BirthDate,
                            InsuranceNumber = patient.InsuranceNumber,
                            TotalVisits = _context.RecordingSlots
                                .Where(s => s.PatientId == patient.PatientId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly)
                                .Count(),
                            LastVisitDate = _context.RecordingSlots
                                .Where(s => s.PatientId == patient.PatientId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly)
                                .Max(s => (DateOnly?)s.Date),
                            TotalSpent = _context.RecordingSlots
                                .Where(s => s.PatientId == patient.PatientId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly)
                                .Sum(s => (decimal?)s.Service.Price) ?? 0,
                            UniqueDoctors = _context.RecordingSlots
                                .Where(s => s.PatientId == patient.PatientId 
                                         && s.Date >= startDateOnly && s.Date <= endDateOnly)
                                .Select(s => s.DoctorId)
                                .Distinct()
                                .Count(),
                            RegistrationDate = patient.BirthDate // Можно заменить на дату регистрации если есть
                        };

            return query.OrderByDescending(p => p.TotalVisits);
        }

        /// <summary>
        /// Сводная статистика по системе за период
        /// </summary>
        public SystemSummaryDto GetSystemSummary(DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateOnly.FromDateTime(DateTime.Today).AddMonths(-1).ToDateTime(TimeOnly.MinValue);
            var end = endDate ?? DateTime.Now;
            var startDateOnly = DateOnly.FromDateTime(start);
            var endDateOnly = DateOnly.FromDateTime(end);

            var totalPatients = _context.Patients.Count();
            var totalDoctors = _context.Doctors.Count();
            var totalServices = _context.ServicesClinics.Count();
            var totalSpecialties = _context.Specialties.Count();

            var slotsInPeriod = _context.RecordingSlots
                .Where(s => s.Date >= startDateOnly && s.Date <= endDateOnly)
                .ToList();

            var totalAppointments = slotsInPeriod.Count;
            var bookedAppointments = slotsInPeriod.Count(s => s.PatientId != null);
            var completedAppointments = slotsInPeriod.Count(s => s.Appointment != null);
            var cancelledAppointments = totalAppointments - bookedAppointments;

            var totalRevenue = slotsInPeriod
                .Where(s => s.Appointment != null)
                .Sum(s => (decimal?)s.Service.Price) ?? 0;

            var avgAppointmentsPerDay = totalAppointments / 
                (double)((endDateOnly.DayNumber - startDateOnly.DayNumber) + 1);

            var conversionRate = totalPatients > 0 
                ? (bookedAppointments * 100.0 / (totalPatients * ((endDateOnly.DayNumber - startDateOnly.DayNumber) + 1))) 
                : 0;

            // Топ врачей по количеству записей
            var topDoctors = _context.Doctors
                .Join(_context.RecordingSlots,
                    d => d.DoctorId,
                    s => s.DoctorId,
                    (d, s) => new { Doctor = d, Slot = s })
                .Where(x => x.Slot.Date >= startDateOnly && x.Slot.Date <= endDateOnly)
                .GroupBy(x => x.Doctor)
                .Select(g => new TopPerformerDto
                {
                    Id = g.Key.DoctorId,
                    Name = g.Key.FullName,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            // Топ услуг по популярности
            var topServices = _context.ServicesClinics
                .Join(_context.RecordingSlots,
                    svc => svc.ServiceId,
                    s => s.ServiceId,
                    (svc, s) => new { Service = svc, Slot = s })
                .Where(x => x.Slot.Date >= startDateOnly && x.Slot.Date <= endDateOnly)
                .GroupBy(x => x.Service)
                .Select(g => new TopPerformerDto
                {
                    Id = g.Key.ServiceId,
                    Name = g.Key.Name,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            return new SystemSummaryDto
            {
                PeriodStart = startDateOnly,
                PeriodEnd = endDateOnly,
                TotalPatients = totalPatients,
                TotalDoctors = totalDoctors,
                TotalServices = totalServices,
                TotalSpecialties = totalSpecialties,
                TotalAppointments = totalAppointments,
                BookedAppointments = bookedAppointments,
                CompletedAppointments = completedAppointments,
                CancelledAppointments = cancelledAppointments,
                TotalRevenue = totalRevenue,
                AvgAppointmentsPerDay = avgAppointmentsPerDay,
                ConversionRate = conversionRate,
                TopDoctors = topDoctors,
                TopServices = topServices
            };
        }

        /// <summary>
        /// Отчёт по загрузке врачей по часам (детализация)
        /// </summary>
        public IQueryable<HourlyLoadDto> GetHourlyLoadReport(int doctorId, DateOnly date)
        {
            var query = from schedule in _context.Schedules
                        where schedule.DoctorId == doctorId && schedule.Date == date
                        select new HourlyLoadDto
                        {
                            Date = schedule.Date,
                            StartHour = schedule.StartTime.Hour,
                            EndHour = schedule.EndTime.Hour,
                            ScheduledMinutes = (schedule.EndTime.Hour - schedule.StartTime.Hour) * 60 + 
                                             (schedule.EndTime.Minute - schedule.StartTime.Minute),
                            BookedSlots = _context.RecordingSlots
                                .Where(s => s.DoctorId == schedule.DoctorId && s.Date == schedule.Date)
                                .Count(s => s.PatientId != null),
                            FreeSlots = _context.RecordingSlots
                                .Where(s => s.DoctorId == schedule.DoctorId && s.Date == schedule.Date)
                                .Count(s => s.PatientId == null),
                            UtilizationPercent = _context.RecordingSlots
                                .Where(s => s.DoctorId == schedule.DoctorId && s.Date == schedule.Date)
                                .Any()
                                ? (_context.RecordingSlots
                                    .Where(s => s.DoctorId == schedule.DoctorId && s.Date == schedule.Date)
                                    .Count(s => s.PatientId != null) * 100.0 / 
                                   _context.RecordingSlots
                                    .Where(s => s.DoctorId == schedule.DoctorId && s.Date == schedule.Date)
                                    .Count())
                                : 0
                        };

            return query;
        }
    }

    #region DTO Classes

    /// <summary>
    /// DTO для аналитики по врачам
    /// </summary>
    public class DoctorAnalyticsDto
    {
        public int DoctorId { get; set; }
        public string DoctorFullName { get; set; } = string.Empty;
        public string SpecializationName { get; set; } = string.Empty;
        public int TotalWorkDays { get; set; }
        public decimal TotalWorkHours { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageAppointmentCost { get; set; }
        public int UniquePatients { get; set; }
        public int MondayAppointments { get; set; }
        public int TuesdayAppointments { get; set; }
        public int WednesdayAppointments { get; set; }
        public int ThursdayAppointments { get; set; }
        public int FridayAppointments { get; set; }
        public double ScheduleUtilizationPercent { get; set; }
    }

    /// <summary>
    /// DTO для популярности услуг
    /// </summary>
    public class ServicePopularityDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int TotalBookings { get; set; }
        public int UniquePatients { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AvgBookingsPerDay { get; set; }
        public int PopularityRank { get; set; }
    }

    /// <summary>
    /// DTO для активности пациентов
    /// </summary>
    public class PatientActivityDto
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateOnly BirthDate { get; set; }
        public string? InsuranceNumber { get; set; }
        public int TotalVisits { get; set; }
        public DateOnly? LastVisitDate { get; set; }
        public decimal TotalSpent { get; set; }
        public int UniqueDoctors { get; set; }
        public DateOnly RegistrationDate { get; set; }
    }

    /// <summary>
    /// DTO для сводной статистики системы
    /// </summary>
    public class SystemSummaryDto
    {
        public DateOnly PeriodStart { get; set; }
        public DateOnly PeriodEnd { get; set; }
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalServices { get; set; }
        public int TotalSpecialties { get; set; }
        public int TotalAppointments { get; set; }
        public int BookedAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AvgAppointmentsPerDay { get; set; }
        public double ConversionRate { get; set; }
        public List<TopPerformerDto> TopDoctors { get; set; } = new();
        public List<TopPerformerDto> TopServices { get; set; } = new();
    }

    /// <summary>
    /// DTO для топ исполнителей
    /// </summary>
    public class TopPerformerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    /// <summary>
    /// DTO для почасовой загрузки
    /// </summary>
    public class HourlyLoadDto
    {
        public DateOnly Date { get; set; }
        public int StartHour { get; set; }
        public int EndHour { get; set; }
        public int ScheduledMinutes { get; set; }
        public int BookedSlots { get; set; }
        public int FreeSlots { get; set; }
        public double UtilizationPercent { get; set; }
    }

    #endregion
}

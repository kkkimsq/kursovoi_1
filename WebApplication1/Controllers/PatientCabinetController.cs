using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class PatientCabinetController : Controller
    {
        private readonly MyDbContext _context;

        public PatientCabinetController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Личный кабинет пациента - история записей
        public async Task<IActionResult> Index()
        {
            var email = User.Identity.Name;
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Email == email);

            if (patient == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var appointments = await _context.RecordingSlots
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.Specialization)
                .Include(r => r.Service)
                .Include(r => r.Appointment)
                .Where(r => r.PatientId == patient.PatientId)
                .OrderByDescending(r => r.Date)
                .ThenBy(r => r.StartTime)
                .ToListAsync();

            ViewBag.Patient = patient;
            return View(appointments);
        }

        // POST: Отмена записи
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var email = User.Identity.Name;
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Email == email);

            if (patient == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var slot = await _context.RecordingSlots
                .Include(r => r.Appointment)
                .FirstOrDefaultAsync(r => r.SlotId == id && r.PatientId == patient.PatientId);

            if (slot == null || slot.Date < DateOnly.FromDateTime(DateTime.Now))
            {
                TempData["Error"] = "Невозможно отменить эту запись.";
                return RedirectToAction(nameof(Index));
            }

            // Удаляем связанную запись Appointment если есть
            if (slot.Appointment != null)
            {
                _context.Appointments.Remove(slot.Appointment);
            }

            // Удаляем слот записи
            _context.RecordingSlots.Remove(slot);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Запись успешно отменена.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Перенос записи - выбор новой даты/времени
        public async Task<IActionResult> Reschedule(int id)
        {
            var email = User.Identity.Name;
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Email == email);

            if (patient == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var currentSlot = await _context.RecordingSlots
                .Include(r => r.Doctor)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(r => r.SlotId == id && r.PatientId == patient.PatientId);

            if (currentSlot == null)
            {
                TempData["Error"] = "Запись не найдена.";
                return RedirectToAction(nameof(Index));
            }

            // Получаем свободные слоты для этого врача и услуги
            var availableSlots = await GetAvailableSlotsForDoctor(
                currentSlot.DoctorId, 
                currentSlot.ServiceId,
                currentSlot.Date.AddDays(-7),
                currentSlot.Date.AddDays(30)
            );

            ViewBag.CurrentSlot = currentSlot;
            ViewBag.AvailableSlots = availableSlots;
            return View(currentSlot);
        }

        // POST: Подтверждение переноса записи
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmReschedule(int currentSlotId, int newSlotId)
        {
            var email = User.Identity.Name;
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Email == email);

            if (patient == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var currentSlot = await _context.RecordingSlots
                .FirstOrDefaultAsync(r => r.SlotId == currentSlotId && r.PatientId == patient.PatientId);

            var newSlot = await _context.RecordingSlots
                .FirstOrDefaultAsync(r => r.SlotId == newSlotId && r.PatientId == null);

            if (currentSlot == null || newSlot == null)
            {
                TempData["Error"] = "Ошибка при переносе записи.";
                return RedirectToAction(nameof(Index));
            }

            // Обновляем новый слот данными пациента
            newSlot.PatientId = patient.PatientId;
            
            // Освобождаем старый слот
            currentSlot.PatientId = null;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Запись успешно перенесена.";
            return RedirectToAction(nameof(Index));
        }

        // Вспомогательный метод для получения доступных слотов
        private async Task<List<RecordingSlot>> GetAvailableSlotsForDoctor(
            int doctorId, 
            int serviceId, 
            DateOnly startDate, 
            DateOnly endDate)
        {
            return await _context.RecordingSlots
                .Include(r => r.Doctor)
                .Where(r => r.DoctorId == doctorId
                         && r.ServiceId == serviceId
                         && r.PatientId == null
                         && r.Date >= startDate
                         && r.Date <= endDate)
                .OrderBy(r => r.Date)
                .ThenBy(r => r.StartTime)
                .ToListAsync();
        }
    }
}

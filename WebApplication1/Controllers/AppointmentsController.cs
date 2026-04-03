using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly MyDbContext _context;

        public AppointmentsController(MyDbContext context)
        {
            _context = context;
        }

        // ✅ Если не авторизован — перенаправляем на вход
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account",
                    new { returnUrl = Url.Action("Create", "Appointments") });
            }

            ViewBag.Services = await _context.ServicesClinics.ToListAsync();
            ViewBag.Doctors = await _context.Doctors
                .Include(d => d.Specialization)
                .ToListAsync();
            return View();
        }
        // GET: Appointments/Confirmation
        public IActionResult Confirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecordingSlot slot)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                // Получаем ID пациента из текущей сессии
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Email == User.Identity.Name);

                if (patient != null)
                {
                    slot.PatientId = patient.PatientId;
                    _context.Add(slot);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Confirmation));
                }
            }

            ViewBag.Services = await _context.ServicesClinics.ToListAsync();
            ViewBag.Doctors = await _context.Doctors
                .Include(d => d.Specialization)
                .ToListAsync();
            return View(slot);
        }
    }
    }
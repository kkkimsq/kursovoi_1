using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using Microsoft.AspNetCore.Authorization;

public class DoctorsController : Controller
{
    private readonly MyDbContext _context;

    public DoctorsController(MyDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(int? specialtyId, string searchName)
    {
        var doctorsQuery = _context.Doctors
            .Include(d => d.Specialization)
            .AsQueryable();

        // Фильтрация по специализации
        if (specialtyId.HasValue && specialtyId.Value > 0)
        {
            doctorsQuery = doctorsQuery.Where(d => d.SpecializationId == specialtyId.Value);
        }

        // Поиск по имени врача
        if (!string.IsNullOrWhiteSpace(searchName))
        {
            doctorsQuery = doctorsQuery.Where(d => d.FullName.Contains(searchName));
        }

        // Получаем список всех специализаций для фильтра
        ViewBag.Specialties = await _context.Specialties.ToListAsync();
        ViewBag.CurrentSpecialtyId = specialtyId;
        ViewBag.SearchName = searchName;

        var doctors = await doctorsQuery.ToListAsync();
        return View(doctors);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var doctor = await _context.Doctors
            .Include(d => d.Specialization)
            .FirstOrDefaultAsync(m => m.DoctorId == id);

        if (doctor == null) return NotFound();

        // Получаем услуги, которые оказывает врач (через слоты расписания)
        var services = await _context.RecordingSlots
            .Where(r => r.DoctorId == doctor.DoctorId)
            .Include(r => r.Service)
            .Select(r => r.Service)
            .Distinct()
            .ToListAsync();

        ViewBag.Services = services;

        // Получаем ближайшие свободные слоты для этого врача
        var today = DateOnly.FromDateTime(DateTime.Now);
        var freeSlots = await _context.RecordingSlots
            .Where(r => r.DoctorId == doctor.DoctorId 
                     && r.PatientId == null 
                     && r.Date >= today)
            .Include(r => r.Service)
            .OrderBy(r => r.Date)
            .ThenBy(r => r.StartTime)
            .Take(10)
            .ToListAsync();

        ViewBag.FreeSlots = freeSlots;

        return View(doctor);
    }
}
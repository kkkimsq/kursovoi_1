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
    public async Task<IActionResult> Index()
    {
        var doctors = await _context.Doctors
            .Include(d => d.Specialization)
            .ToListAsync();
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
        return View(doctor);
    }
}
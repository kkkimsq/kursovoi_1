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
    public class ServicesClinicsController : Controller
    {
        private readonly MyDbContext _context;

        public ServicesClinicsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: ServicesClinics
        public async Task<IActionResult> Index()
        {
            return View(await _context.ServicesClinics.ToListAsync());
        }

        // GET: ServicesClinics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicesClinic = await _context.ServicesClinics
                .FirstOrDefaultAsync(m => m.ServiceId == id);
            if (servicesClinic == null)
            {
                return NotFound();
            }

            return View(servicesClinic);
        }

        // GET: ServicesClinics/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServicesClinics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceId,Name,Price")] ServicesClinic servicesClinic)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servicesClinic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(servicesClinic);
        }

        // GET: ServicesClinics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicesClinic = await _context.ServicesClinics.FindAsync(id);
            if (servicesClinic == null)
            {
                return NotFound();
            }
            return View(servicesClinic);
        }

        // POST: ServicesClinics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceId,Name,Price")] ServicesClinic servicesClinic)
        {
            if (id != servicesClinic.ServiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicesClinic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicesClinicExists(servicesClinic.ServiceId))
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
            return View(servicesClinic);
        }

        // GET: ServicesClinics/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicesClinic = await _context.ServicesClinics
                .FirstOrDefaultAsync(m => m.ServiceId == id);
            if (servicesClinic == null)
            {
                return NotFound();
            }

            return View(servicesClinic);
        }

        // POST: ServicesClinics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servicesClinic = await _context.ServicesClinics.FindAsync(id);
            if (servicesClinic != null)
            {
                _context.ServicesClinics.Remove(servicesClinic);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServicesClinicExists(int id)
        {
            return _context.ServicesClinics.Any(e => e.ServiceId == id);
        }
    }
}

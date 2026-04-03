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
    public class RecordingSlotsController : Controller
    {
        private readonly MyDbContext _context;

        public RecordingSlotsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: RecordingSlots
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.RecordingSlots.Include(r => r.Doctor).Include(r => r.Patient).Include(r => r.Service);
            return View(await myDbContext.ToListAsync());
        }

        // GET: RecordingSlots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recordingSlot = await _context.RecordingSlots
                .Include(r => r.Doctor)
                .Include(r => r.Patient)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.SlotId == id);
            if (recordingSlot == null)
            {
                return NotFound();
            }

            return View(recordingSlot);
        }

        // GET: RecordingSlots/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "FullName");
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName");
            ViewData["ServiceId"] = new SelectList(_context.ServicesClinics, "ServiceId", "Name");
            return View();
        }

        // POST: RecordingSlots/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SlotId,DoctorId,PatientId,ServiceId,Date,StartTime")] RecordingSlot recordingSlot)
        {
            ModelState.Remove("Doctor");
            ModelState.Remove("Patient");
            ModelState.Remove("Service");

            if (ModelState.IsValid)
            {
                _context.Add(recordingSlot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "FullName", recordingSlot.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", recordingSlot.PatientId);
            ViewData["ServiceId"] = new SelectList(_context.ServicesClinics, "ServiceId", "Name", recordingSlot.ServiceId);
            return View(recordingSlot);
        }

        // GET: RecordingSlots/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recordingSlot = await _context.RecordingSlots.FindAsync(id);
            if (recordingSlot == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "FullName", recordingSlot.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", recordingSlot.PatientId);
            ViewData["ServiceId"] = new SelectList(_context.ServicesClinics, "ServiceId", "Name", recordingSlot.ServiceId);
            return View(recordingSlot);
        }

        // POST: RecordingSlots/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SlotId,DoctorId,PatientId,ServiceId,Date,StartTime")] RecordingSlot recordingSlot)
        {
            if (id != recordingSlot.SlotId)
            {
                return NotFound();
            }

            ModelState.Remove("Doctor");
            ModelState.Remove("Patient");
            ModelState.Remove("Service");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recordingSlot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecordingSlotExists(recordingSlot.SlotId))
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
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "FullName", recordingSlot.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", recordingSlot.PatientId);
            ViewData["ServiceId"] = new SelectList(_context.ServicesClinics, "ServiceId", "Name", recordingSlot.ServiceId);
            return View(recordingSlot);
        }

        // GET: RecordingSlots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recordingSlot = await _context.RecordingSlots
                .Include(r => r.Doctor)
                .Include(r => r.Patient)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.SlotId == id);
            if (recordingSlot == null)
            {
                return NotFound();
            }

            return View(recordingSlot);
        }

        // POST: RecordingSlots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recordingSlot = await _context.RecordingSlots.FindAsync(id);
            if (recordingSlot != null)
            {
                _context.RecordingSlots.Remove(recordingSlot);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecordingSlotExists(int id)
        {
            return _context.RecordingSlots.Any(e => e.SlotId == id);
        }
    }
}

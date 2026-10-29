using HealthcareManagementSystem.Data;
using HealthcareManagementSystem.Models;
using HealthcareManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareManagementSystem.Controllers;

[Authorize]
public class AppointmentController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    private readonly IHealthcareService _service;

    public AppointmentController(ApplicationDbContext db, UserManager<ApplicationUser> users, IHealthcareService service)
    { _db = db; _users = users; _service = service; }

    [Authorize(Roles="Patient,Receptionist,Admin")]
    public async Task<IActionResult> Index()
    {
        var data = await _db.Appointments.Include(a=>a.Doctor).Include(a=>a.Patient)
            .OrderByDescending(a=>a.AppointmentDate).ToListAsync();
        return View(data);
    }

    [Authorize(Roles="Patient,Receptionist,Admin")]
    [HttpGet]
    public async Task<IActionResult> Create(int? doctorId)
    {
        ViewBag.Doctors = await _db.Doctors.Where(d=>d.IsActive).OrderBy(d=>d.FullName).ToListAsync();
        return View(new Appointment { DoctorId = doctorId ?? 0, AppointmentDate = DateTime.Today.AddDays(1) });
    }

    [Authorize(Roles="Patient,Receptionist,Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Appointment model)
    {
        ModelState.Remove(nameof(model.Patient));
        ModelState.Remove(nameof(model.Doctor));

        if (User.IsInRole("Patient"))
        {
            var user = await _users.GetUserAsync(User);
            var patient = await _db.Patients.FirstOrDefaultAsync(p=>p.UserId==user!.Id);
            if (patient == null) { ModelState.AddModelError("", "Patient profile not found."); }
            else model.PatientId = patient.PatientId;
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Doctors = await _db.Doctors.Where(d=>d.IsActive).ToListAsync();
            return View(model);
        }

        try
        {
            await _service.CreateAppointmentAsync(model);
            TempData["Success"] = "Appointment booked successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch(Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewBag.Doctors = await _db.Doctors.Where(d=>d.IsActive).ToListAsync();
            return View(model);
        }
    }

    [Authorize(Roles="Doctor,Receptionist,Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Status(int id, string status)
    {
        var appointment = await _db.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();
        appointment.Status = status;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}

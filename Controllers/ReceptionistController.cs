using HealthcareManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareManagementSystem.Controllers;

[Authorize(Roles="Receptionist")]
public class ReceptionistController : Controller
{
    private readonly ApplicationDbContext _db;
    public ReceptionistController(Data.ApplicationDbContext db) => _db=db;

    public async Task<IActionResult> Index()
    {
        ViewBag.Patients=await _db.Patients.CountAsync();
        ViewBag.Doctors=await _db.Doctors.CountAsync();
        ViewBag.TodayAppointments=await _db.Appointments.CountAsync(a=>a.AppointmentDate.Date==DateTime.Today);
        return View();
    }
}

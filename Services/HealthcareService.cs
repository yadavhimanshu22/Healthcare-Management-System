using HealthcareManagementSystem.Data;
using HealthcareManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcareManagementSystem.Services;

public class HealthcareService : IHealthcareService
{
    private readonly ApplicationDbContext _db;
    public HealthcareService(ApplicationDbContext db) => _db = db;

    public async Task<bool> IsDoctorSlotAvailableAsync(int doctorId, DateTime date, TimeSpan time)
    {
        return !await _db.Appointments.AnyAsync(a =>
            a.DoctorId == doctorId &&
            a.AppointmentDate.Date == date.Date &&
            a.AppointmentTime == time &&
            a.Status != "Cancelled");
    }

    public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
    {
        if (!await IsDoctorSlotAvailableAsync(appointment.DoctorId, appointment.AppointmentDate, appointment.AppointmentTime))
            throw new InvalidOperationException("This doctor already has an appointment at the selected time.");

        appointment.Status = "Pending";
        _db.Appointments.Add(appointment);
        await _db.SaveChangesAsync();
        return appointment;
    }

    public async Task<List<Doctor>> SearchDoctorsAsync(string? search, string? specialization, int? departmentId)
    {
        var query = _db.Doctors.Include(d => d.Department).Where(d => d.IsActive).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(d => d.FullName.Contains(search) || d.Specialization.Contains(search));

        if (!string.IsNullOrWhiteSpace(specialization))
            query = query.Where(d => d.Specialization == specialization);

        if (departmentId.HasValue)
            query = query.Where(d => d.DepartmentId == departmentId);

        return await query.OrderBy(d => d.FullName).ToListAsync();
    }
}

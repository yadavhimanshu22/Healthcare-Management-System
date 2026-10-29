using HealthcareManagementSystem.Models;

namespace HealthcareManagementSystem.Services;

public interface IHealthcareService
{
    Task<bool> IsDoctorSlotAvailableAsync(int doctorId, DateTime date, TimeSpan time);
    Task<Appointment> CreateAppointmentAsync(Appointment appointment);
    Task<List<Doctor>> SearchDoctorsAsync(string? search, string? specialization, int? departmentId);
}

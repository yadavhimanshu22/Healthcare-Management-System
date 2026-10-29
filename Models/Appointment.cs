using System.ComponentModel.DataAnnotations;

namespace HealthcareManagementSystem.Models;

public class Appointment
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    [Required] public TimeSpan AppointmentTime { get; set; }
    [StringLength(500)] public string? Reason { get; set; }
    [Required] public string Status { get; set; } = "Pending";
    [StringLength(1000)] public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
    public MedicalRecord? MedicalRecord { get; set; }
    public Prescription? Prescription { get; set; }
}

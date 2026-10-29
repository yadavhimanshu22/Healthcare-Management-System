using System.ComponentModel.DataAnnotations;

namespace HealthcareManagementSystem.Models;

public class Patient
{
    public int PatientId { get; set; }
    [Required] public string UserId { get; set; } = "";
    [Required, StringLength(150)] public string FullName { get; set; } = "";
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Phone] public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? BloodGroup { get; set; }
    public string? EmergencyContact { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public ICollection<LabReport> LabReports { get; set; } = new List<LabReport>();
    public ICollection<Bill> Bills { get; set; } = new List<Bill>();
}

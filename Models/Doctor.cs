using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareManagementSystem.Models;

public class Doctor
{
    public int DoctorId { get; set; }

    // ASP.NET Identity User
    [Required]
    public string UserId { get; set; } = "";

    // Basic Information
    [Required, StringLength(150)]
    public string FullName { get; set; } = "";

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = "";

    [Phone, StringLength(20)]
    public string? Phone { get; set; }

    // Professional Information
    [Required, StringLength(100)]
    public string Specialization { get; set; } = "";

    [StringLength(150)]
    public string? Qualification { get; set; }

    [Range(0, 60)]
    public int Experience { get; set; }

    [StringLength(100)]
    public string? LicenseNumber { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Range(0, 99999999)]
    public decimal ConsultationFee { get; set; }

    // Profile
    [StringLength(500)]
    public string? ProfileImage { get; set; }

    // Availability
    [StringLength(200)]
    public string? AvailableDays { get; set; }

    [StringLength(100)]
    public string? AvailableTime { get; set; }

    // Department
    public int? DepartmentId { get; set; }

    public Department? Department { get; set; }

    // System
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // Relationships
    public ICollection<Appointment> Appointments { get; set; }
        = new List<Appointment>();

    public ICollection<MedicalRecord> MedicalRecords { get; set; }
        = new List<MedicalRecord>();
}
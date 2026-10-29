namespace HealthcareManagementSystem.Models;

public class LabReport
{
    public int LabReportId { get; set; }
    public int PatientId { get; set; }
    public int? DoctorId { get; set; }
    public string TestName { get; set; } = "";
    public DateTime TestDate { get; set; } = DateTime.UtcNow;
    public string? Result { get; set; }
    public string? ReferenceRange { get; set; }
    public string? Remarks { get; set; }
    public string? ReportFile { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}

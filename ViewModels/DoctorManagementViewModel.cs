using HealthcareManagementSystem.Models;

namespace HealthcareManagementSystem.ViewModels;

public class DoctorManagementViewModel
{
    public List<Doctor> Doctors { get; set; } = new();

    public List<Department> Departments { get; set; } = new();

    public Doctor Doctor { get; set; } = new();

    public Doctor? SelectedDoctor { get; set; }

    public string ActiveSection { get; set; } = "home";

    public string? SearchTerm { get; set; }

    public string? SelectedSpecialization { get; set; }

    public int? SelectedDepartmentId { get; set; }

    public int TotalDoctors { get; set; }

    public int ActiveDoctors { get; set; }

    public int InactiveDoctors { get; set; }

    public int TotalAppointments { get; set; }

    public int TotalMedicalRecords { get; set; }
}
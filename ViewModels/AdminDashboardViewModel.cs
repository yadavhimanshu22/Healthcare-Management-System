using HealthcareManagementSystem.Models;

namespace HealthcareManagementSystem.ViewModels;

public class AdminDashboardViewModel
{
    // =========================================================
    // MAIN COUNTERS
    // =========================================================

    public int TotalPatients { get; set; }

    public int TotalDoctors { get; set; }

    public int TotalUsers { get; set; }

    public int TotalAppointments { get; set; }


    // =========================================================
    // TODAY'S APPOINTMENTS
    // =========================================================

    public int TodaysAppointments { get; set; }

    public int ActiveAppointments { get; set; }

    public int PendingAppointments { get; set; }

    public int CompletedAppointments { get; set; }

    public int CancelledAppointments { get; set; }


    // =========================================================
    // CLINICAL
    // =========================================================

    public int TotalMedicalRecords { get; set; }

    public int TotalPrescriptions { get; set; }

    public int TotalLabReports { get; set; }

    public int PendingReports { get; set; }


    // =========================================================
    // FINANCIAL
    // =========================================================

    public decimal TotalRevenue { get; set; }

    public decimal TotalBillingAmount { get; set; }


    // =========================================================
    // RECENT DATA
    // =========================================================

    public List<Appointment> RecentAppointments { get; set; }
        = new();

    public List<Doctor> RecentDoctors { get; set; }
        = new();

    public List<Patient> RecentPatients { get; set; }
        = new();


    // =========================================================
    // ADMIN INFORMATION
    // =========================================================

    public string AdminName { get; set; } = "Administrator";

    public string AdminEmail { get; set; } = "";

    public string AdminInitial { get; set; } = "A";
}
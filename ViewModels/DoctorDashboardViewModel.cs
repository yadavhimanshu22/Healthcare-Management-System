using System;

namespace HealthcareManagementSystem.ViewModels
{
    public class DoctorDashboardViewModel
    {
        // =====================================================
        // DOCTOR PROFILE
        // =====================================================

        public string DoctorName { get; set; } = "Doctor";

        public string Specialization { get; set; } = "Medical Specialist";

        public string Email { get; set; } = "";

        public string PhoneNumber { get; set; } = "";


        // =====================================================
        // DASHBOARD STATISTICS
        // =====================================================

        public int TodayAppointments { get; set; }

        public int PendingAppointments { get; set; }

        public int CompletedAppointments { get; set; }

        public int CancelledAppointments { get; set; }

        public int TotalPatients { get; set; }

        public int FollowUpAppointments { get; set; }

        public int PendingReports { get; set; }


        // =====================================================
        // TODAY'S APPOINTMENTS
        // =====================================================

        public List<DoctorAppointmentViewModel> TodayAppointmentList
        {
            get;
            set;
        } = new();


        // =====================================================
        // RECENT PATIENTS
        // =====================================================

        public List<DoctorPatientViewModel> RecentPatients
        {
            get;
            set;
        } = new();
    }


    // =========================================================
    // APPOINTMENT VIEW MODEL
    // =========================================================

    public class DoctorAppointmentViewModel
    {
        public int Id { get; set; }

        public string PatientName { get; set; } = "";

        public string Date { get; set; } = "";

        public string Time { get; set; } = "";

        public string Status { get; set; } = "Pending";

        public string Reason { get; set; } = "";
    }


    // =========================================================
    // PATIENT VIEW MODEL
    // =========================================================

    public class DoctorPatientViewModel
    {
        public int Id { get; set; }

        public string PatientName { get; set; } = "";

        public string Email { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public string LastVisit { get; set; } = "";

        public string Status { get; set; } = "Active";
    }
}
using HealthcareManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareManagementSystem.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        // =====================================================
        // DOCTOR DASHBOARD
        // =====================================================

        [HttpGet]
        public IActionResult Dashboard()
        {
            var model = new DoctorDashboardViewModel
            {
                DoctorName = User.Identity?.Name ?? "Doctor",

                Specialization = "Medical Specialist",

                Email = User.Identity?.Name ?? "",

                PhoneNumber = "",

                // Initial values
                TodayAppointments = 0,
                PendingAppointments = 0,
                CompletedAppointments = 0,
                CancelledAppointments = 0,
                TotalPatients = 0,
                FollowUpAppointments = 0,
                PendingReports = 0
            };

            return View(model);
        }


        // =====================================================
        // DOCTOR PROFILE
        // =====================================================

        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }


        // =====================================================
        // TODAY'S APPOINTMENTS
        // =====================================================

        [HttpGet]
        public IActionResult Appointments()
        {
            return View();
        }


        // =====================================================
        // PATIENT LIST
        // =====================================================

        [HttpGet]
        public IActionResult Patients()
        {
            return View();
        }


        // =====================================================
        // PATIENT HISTORY
        // =====================================================

        [HttpGet]
        public IActionResult PatientHistory()
        {
            return View();
        }


        // =====================================================
        // MEDICAL RECORDS
        // =====================================================

        [HttpGet]
        public IActionResult MedicalRecords()
        {
            return View();
        }


        // =====================================================
        // DIAGNOSIS
        // =====================================================

        [HttpGet]
        public IActionResult Diagnosis()
        {
            return View();
        }


        // =====================================================
        // PRESCRIPTION
        // =====================================================

        [HttpGet]
        public IActionResult Prescriptions()
        {
            return View();
        }


        // =====================================================
        // LABORATORY REPORTS
        // =====================================================

        [HttpGet]
        public IActionResult LaboratoryReports()
        {
            return View();
        }


        // =====================================================
        // FOLLOW-UP APPOINTMENTS
        // =====================================================

        [HttpGet]
        public IActionResult FollowUps()
        {
            return View();
        }


        // =====================================================
        // CONSULTATION NOTES
        // =====================================================

        [HttpGet]
        public IActionResult ConsultationNotes()
        {
            return View();
        }
    }
}
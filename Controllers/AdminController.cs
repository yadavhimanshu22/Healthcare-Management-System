
using HealthcareManagementSystem.Data;
using HealthcareManagementSystem.Models;
using HealthcareManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // =========================================================
        // ADMIN INDEX
        // =========================================================

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Dashboard));
        }

        // =========================================================
        // ADMIN DASHBOARD
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Dashboard(string period = "all")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var adminName = currentUser?.FullName;

            if (string.IsNullOrWhiteSpace(adminName))
            {
                adminName = "Administrator";
            }

            period = period?.ToLowerInvariant() ?? "all";

            if (period != "all" &&
                period != "week" &&
                period != "month" &&
                period != "year")
            {
                period = "all";
            }

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            DateTime? periodStart = null;
            DateTime? periodEnd = null;

            switch (period)
            {
                case "week":
                    int difference =
                        (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;

                    periodStart = today.AddDays(-difference);
                    periodEnd = periodStart.Value.AddDays(7);
                    break;

                case "month":
                    periodStart = new DateTime(today.Year, today.Month, 1);
                    periodEnd = periodStart.Value.AddMonths(1);
                    break;

                case "year":
                    periodStart = new DateTime(today.Year, 1, 1);
                    periodEnd = periodStart.Value.AddYears(1);
                    break;

                default:
                    periodStart = null;
                    periodEnd = null;
                    break;
            }

            var appointmentQuery = _context.Appointments.AsQueryable();

            if (periodStart.HasValue && periodEnd.HasValue)
            {
                appointmentQuery = appointmentQuery.Where(a =>
                    a.AppointmentDate >= periodStart.Value &&
                    a.AppointmentDate < periodEnd.Value);
            }

            var totalUsers = await _userManager.Users.CountAsync();
            var totalPatients = await _context.Patients.CountAsync();
            var totalDoctors = await _context.Doctors.CountAsync();

            var totalAppointments = await appointmentQuery.CountAsync();

            var todaysAppointments = await _context.Appointments.CountAsync(a =>
                a.AppointmentDate >= today &&
                a.AppointmentDate < tomorrow);

            var pendingAppointments = await appointmentQuery.CountAsync(a =>
                a.Status != null && a.Status.ToLower() == "pending");

            var completedAppointments = await appointmentQuery.CountAsync(a =>
                a.Status != null && a.Status.ToLower() == "completed");

            var cancelledAppointments = await appointmentQuery.CountAsync(a =>
                a.Status != null && a.Status.ToLower() == "cancelled");

            var activeAppointments = await appointmentQuery.CountAsync(a =>
                a.Status != null &&
                a.Status.ToLower() != "cancelled" &&
                a.Status.ToLower() != "completed");

            var totalMedicalRecords = await _context.MedicalRecords.CountAsync();
            var totalPrescriptions = await _context.Prescriptions.CountAsync();
            var totalLabReports = await _context.LabReports.CountAsync();

            var pendingReports = 0;

            var totalBillingAmount = await _context.Bills
                .Select(b => (decimal?)b.TotalAmount)
                .SumAsync() ?? 0m;

            var totalRevenue = totalBillingAmount;

            var recentAppointments = await appointmentQuery
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .Take(8)
                .ToListAsync();

            var recentDoctors = await _context.Doctors
                .Include(d => d.Department)
                .OrderByDescending(d => d.CreatedAt)
                .Take(5)
                .ToListAsync();

            var recentPatients = await _context.Patients
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();

            var model = new AdminDashboardViewModel
            {
                TotalPatients = totalPatients,
                TotalDoctors = totalDoctors,
                TotalUsers = totalUsers,
                TotalAppointments = totalAppointments,
                TodaysAppointments = todaysAppointments,
                ActiveAppointments = activeAppointments,
                PendingAppointments = pendingAppointments,
                CompletedAppointments = completedAppointments,
                CancelledAppointments = cancelledAppointments,
                TotalMedicalRecords = totalMedicalRecords,
                TotalPrescriptions = totalPrescriptions,
                TotalLabReports = totalLabReports,
                PendingReports = pendingReports,
                TotalRevenue = totalRevenue,
                TotalBillingAmount = totalBillingAmount,
                RecentAppointments = recentAppointments,
                RecentDoctors = recentDoctors,
                RecentPatients = recentPatients,
                AdminName = adminName
            };

            ViewBag.SelectedPeriod = period;
            ViewData["Title"] = "Admin Dashboard";

            return View(model);
        }

        // =========================================================
        // USERS
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            var userList = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userList.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    FullName = string.IsNullOrWhiteSpace(user.FullName)
                        ? "User"
                        : user.FullName,
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    Role = roles.FirstOrDefault() ?? "User",
                    IsActive = !user.LockoutEnd.HasValue ||
                               user.LockoutEnd.Value <= DateTimeOffset.UtcNow
                });
            }

            ViewData["Title"] = "User Management";

            return View(userList);
        }

        // =========================================================
        // DELETE USER
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction(nameof(Users));
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return RedirectToAction(nameof(Users));
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["Error"] = "Admin account cannot be deleted.";
                return RedirectToAction(nameof(Users));
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "User deleted successfully.";
            }
            else
            {
                TempData["Error"] = string.Join(
                    " ",
                    result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Users));
        }

        // =========================================================
        // TOGGLE USER STATUS
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction(nameof(Users));
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return RedirectToAction(nameof(Users));
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["Error"] = "Admin account cannot be blocked.";
                return RedirectToAction(nameof(Users));
            }

            if (user.LockoutEnd.HasValue &&
                user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                user.LockoutEnd = null;
                user.LockoutEnabled = true;
                TempData["Success"] = "User activated successfully.";
            }
            else
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
                TempData["Success"] = "User blocked successfully.";
            }

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Users));
        }

        // =========================================================
        // PATIENT MANAGEMENT
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Patients(
            string? search = null,
            int? selectedId = null,
            string section = "Home")
        {
            var patientsQuery = _context.Patients.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                patientsQuery = patientsQuery.Where(p =>
                    p.FullName.Contains(search) ||
                    p.Email.Contains(search) ||
                    (p.Phone != null && p.Phone.Contains(search)));
            }

            var patients = await patientsQuery
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            Patient? selectedPatient = null;
            var medicalHistory = new List<MedicalRecord>();

            if (selectedId.HasValue)
            {
                selectedPatient = await _context.Patients
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p =>
                        p.PatientId == selectedId.Value);

                if (selectedPatient != null)
                {
                    medicalHistory = await _context.MedicalRecords
                        .AsNoTracking()
                        .Where(m =>
                            m.PatientId == selectedPatient.PatientId)
                        .OrderByDescending(m => m.CreatedAt)
                        .ToListAsync();
                }
            }

            ViewBag.SelectedPatient = selectedPatient;
            ViewBag.MedicalHistory = medicalHistory;
            ViewBag.SearchTerm = search ?? "";
            ViewBag.Section = section;
            ViewData["Title"] = "Patient Management";

            return View(patients);
        }

        // =========================================================
        // ADD PATIENT
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPatient(
            string fullName,
            string email,
            string phone,
            string password,
            DateTime? dateOfBirth,
            string? gender,
            string? bloodGroup,
            string? emergencyContact,
            string? address)
        {
            fullName = fullName?.Trim() ?? "";
            email = email?.Trim() ?? "";
            phone = phone?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] =
                    "Full name, email and password are required.";

                return RedirectToAction(nameof(Patients),
                    new { section = "add" });
            }

            if (!new System.ComponentModel.DataAnnotations
                    .EmailAddressAttribute().IsValid(email))
            {
                TempData["Error"] =
                    "Please enter a valid email address.";

                return RedirectToAction(nameof(Patients),
                    new { section = "add" });
            }

            if (await _userManager.FindByEmailAsync(email) != null)
            {
                TempData["Error"] =
                    "An account with this email already exists.";

                return RedirectToAction(nameof(Patients),
                    new { section = "add" });
            }

            if (dateOfBirth.HasValue &&
                dateOfBirth.Value.Date > DateTime.Today)
            {
                TempData["Error"] =
                    "Date of birth cannot be in the future.";

                return RedirectToAction(nameof(Patients),
                    new { section = "add" });
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                PhoneNumber = string.IsNullOrWhiteSpace(phone)
                    ? null
                    : phone,
                EmailConfirmed = true
            };

            var createResult =
                await _userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
            {
                TempData["Error"] = string.Join(
                    " ",
                    createResult.Errors.Select(e => e.Description));

                return RedirectToAction(nameof(Patients),
                    new { section = "add" });
            }

            try
            {
                if (!await _roleManager.RoleExistsAsync("Patient"))
                {
                    var roleResult = await _roleManager.CreateAsync(
                        new IdentityRole("Patient"));

                    if (!roleResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            string.Join(" ",
                                roleResult.Errors.Select(e => e.Description)));
                    }
                }

                var roleAssignment =
                    await _userManager.AddToRoleAsync(user, "Patient");

                if (!roleAssignment.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join(" ",
                            roleAssignment.Errors.Select(e => e.Description)));
                }

                var patient = new Patient
                {
                    UserId = user.Id,
                    FullName = fullName,
                    Email = email,
                    Phone = string.IsNullOrWhiteSpace(phone) ? null : phone,
                    DateOfBirth = dateOfBirth,
                    Gender = gender,
                    BloodGroup = bloodGroup,
                    EmergencyContact = emergencyContact,
                    Address = address,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Patient added successfully.";
            }
            catch (Exception)
            {
                await _userManager.DeleteAsync(user);

                TempData["Error"] =
                    "Patient could not be saved. Check the entered data and try again.";
            }

            return RedirectToAction(nameof(Patients));
        }

        // =========================================================
        // EDIT PATIENT
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(
            int patientId,
            string fullName,
            string email,
            string phone,
            DateTime? dateOfBirth,
            string? gender,
            string? bloodGroup,
            string? emergencyContact,
            string? address)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (patient == null)
            {
                TempData["Error"] = "Patient not found.";
                return RedirectToAction(nameof(Patients));
            }

            fullName = fullName?.Trim() ?? "";
            email = email?.Trim() ?? "";
            phone = phone?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] =
                    "Full name and email are required.";

                return RedirectToAction(nameof(Patients),
                    new { selectedId = patientId, section = "edit" });
            }

            if (!new System.ComponentModel.DataAnnotations
                    .EmailAddressAttribute().IsValid(email))
            {
                TempData["Error"] =
                    "Please enter a valid email address.";

                return RedirectToAction(nameof(Patients),
                    new { selectedId = patientId, section = "edit" });
            }

            if (dateOfBirth.HasValue &&
                dateOfBirth.Value.Date > DateTime.Today)
            {
                TempData["Error"] =
                    "Date of birth cannot be in the future.";

                return RedirectToAction(nameof(Patients),
                    new { selectedId = patientId, section = "edit" });
            }

            var duplicate = await _context.Patients.AnyAsync(p =>
                p.PatientId != patientId && p.Email == email);

            var existingUser =
                await _userManager.FindByEmailAsync(email);

            if (duplicate ||
                (existingUser != null &&
                 existingUser.Id != patient.UserId))
            {
                TempData["Error"] =
                    "That email address is already in use.";

                return RedirectToAction(nameof(Patients),
                    new { selectedId = patientId, section = "edit" });
            }

            var user =
                await _userManager.FindByIdAsync(patient.UserId);

            if (user == null)
            {
                TempData["Error"] =
                    "The linked user account was not found. Patient was not changed.";

                return RedirectToAction(nameof(Patients),
                    new { selectedId = patientId, section = "edit" });
            }

            user.FullName = fullName;
            user.Email = email;
            user.UserName = email;
            user.PhoneNumber = string.IsNullOrWhiteSpace(phone)
                ? null
                : phone;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                TempData["Error"] = string.Join(
                    " ",
                    updateResult.Errors.Select(e => e.Description));

                return RedirectToAction(nameof(Patients),
                    new { selectedId = patientId, section = "edit" });
            }

            patient.FullName = fullName;
            patient.Email = email;
            patient.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone;
            patient.DateOfBirth = dateOfBirth;
            patient.Gender = gender;
            patient.BloodGroup = bloodGroup;
            patient.EmergencyContact = emergencyContact;
            patient.Address = address;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Patient updated successfully.";

            return RedirectToAction(nameof(Patients),
                new { selectedId = patientId, section = "profile" });
        }

        // =========================================================
        // DELETE PATIENT
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePatient(int patientId)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (patient == null)
            {
                TempData["Error"] = "Patient not found.";
                return RedirectToAction(nameof(Patients));
            }

            // Preserve linked clinical and financial records.
            var hasRelatedRecords =
                await _context.Appointments.AnyAsync(a =>
                    a.PatientId == patientId) ||
                await _context.MedicalRecords.AnyAsync(m =>
                    m.PatientId == patientId) ||
                await _context.Prescriptions.AnyAsync(p =>
                    p.PatientId == patientId) ||
                await _context.LabReports.AnyAsync(r =>
                    r.PatientId == patientId) ||
                await _context.Bills.AnyAsync(b =>
                    b.PatientId == patientId);

            if (hasRelatedRecords)
            {
                TempData["Error"] =
                    "This patient has linked medical or billing records. " +
                    "Delete is blocked to protect patient history.";

                return RedirectToAction(nameof(Patients));
            }

            var user =
                await _userManager.FindByIdAsync(patient.UserId);

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            if (user != null)
            {
                var deleteUserResult =
                    await _userManager.DeleteAsync(user);

                if (!deleteUserResult.Succeeded)
                {
                    TempData["Error"] =
                        "Patient record was deleted, but the linked login account could not be deleted.";

                    return RedirectToAction(nameof(Patients));
                }
            }

            TempData["Success"] = "Patient deleted successfully.";

            return RedirectToAction(nameof(Patients));
        }

        // =========================================================
        // OTHER ADMIN PAGES
        // =========================================================

        [HttpGet]
        public IActionResult Doctors()
        {
            ViewData["Title"] = "Doctor Management";
            return View();
        }

        [HttpGet]
        public IActionResult Appointments()
        {
            ViewData["Title"] = "Appointment Management";
            return View();
        }

        [HttpGet]
        public IActionResult Records()
        {
            ViewData["Title"] = "Medical Records";
            return View();
        }

        [HttpGet]
        public IActionResult Billing()
        {
            ViewData["Title"] = "Billing Management";
            return View();
        }

        [HttpGet]
        public IActionResult Reports()
        {
            ViewData["Title"] = "Reports & Analytics";
            return View();
        }

        [HttpGet]
        public IActionResult Roles()
        {
            ViewData["Title"] = "Roles & Permissions";
            return View();
        }

        [HttpGet]
        public IActionResult Settings()
        {
            ViewData["Title"] = "System Settings";
            return View();
        }
    }
}

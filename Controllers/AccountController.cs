using HealthcareManagementSystem.Models;
using HealthcareManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }


        // =========================================================
        // LOGIN - GET
        // =========================================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(
            string? email = null,
            string? role = null,
            string? returnUrl = null)
        {
            // If already logged in, open the CURRENT HOME PAGE
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            TempData["OpenLoginPopup"] = "true";

            if (!string.IsNullOrWhiteSpace(email))
            {
                TempData["LoginEmail"] = email.Trim();
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                TempData["LoginRole"] = NormalizeRole(role);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                TempData["ReturnUrl"] = returnUrl;
            }

            // Login popup is opened on the CURRENT Home page
            return RedirectToAction("Index", "Home");
        }


        // =========================================================
        // LOGIN - POST
        // =========================================================

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model,
            string? returnUrl = null,
            bool fromHome = false)
        {
            // -----------------------------------------------------
            // CLEAN EMAIL
            // -----------------------------------------------------

            var email = model.Email?.Trim() ?? string.Empty;


            // -----------------------------------------------------
            // NORMALIZE ROLE
            // -----------------------------------------------------

            var selectedRole = NormalizeRole(model.Role);


            // -----------------------------------------------------
            // EMAIL VALIDATION
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["LoginError"] =
                    "Email address is required.";

                TempData["LoginEmail"] = "";
                TempData["LoginRole"] = selectedRole;
                TempData["OpenLoginPopup"] = "true";

                return RedirectToAction(
                    "Index",
                    "Home");
            }


            // -----------------------------------------------------
            // PASSWORD VALIDATION
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                TempData["LoginError"] =
                    "Password is required.";

                TempData["LoginEmail"] = email;
                TempData["LoginRole"] = selectedRole;
                TempData["OpenLoginPopup"] = "true";

                return RedirectToAction(
                    "Index",
                    "Home");
            }


            // -----------------------------------------------------
            // ROLE VALIDATION
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(selectedRole))
            {
                TempData["LoginError"] =
                    "Please select your role.";

                TempData["LoginEmail"] = email;
                TempData["LoginRole"] = "";
                TempData["OpenLoginPopup"] = "true";

                return RedirectToAction(
                    "Index",
                    "Home");
            }


            // -----------------------------------------------------
            // FIND USER BY EMAIL
            // -----------------------------------------------------

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                TempData["LoginError"] =
                    "Invalid email or password.";

                TempData["LoginEmail"] = email;
                TempData["LoginRole"] = selectedRole;
                TempData["OpenLoginPopup"] = "true";

                return RedirectToAction(
                    "Index",
                    "Home");
            }


            // -----------------------------------------------------
            // CHECK PASSWORD
            // -----------------------------------------------------

            var passwordValid =
                await _userManager.CheckPasswordAsync(
                    user,
                    model.Password);

            if (!passwordValid)
            {
                TempData["LoginError"] =
                    "Invalid email or password.";

                TempData["LoginEmail"] = email;
                TempData["LoginRole"] = selectedRole;
                TempData["OpenLoginPopup"] = "true";

                return RedirectToAction(
                    "Index",
                    "Home");
            }


            // -----------------------------------------------------
            // GET ACTUAL USER ROLES
            // -----------------------------------------------------

            var userRoles =
                await _userManager.GetRolesAsync(user);


            // -----------------------------------------------------
            // NO ROLE ASSIGNED
            // -----------------------------------------------------

            if (userRoles == null || userRoles.Count == 0)
            {
                TempData["LoginError"] =
                    "No role is assigned to this account.";

                TempData["LoginEmail"] = email;
                TempData["LoginRole"] = selectedRole;
                TempData["OpenLoginPopup"] = "true";

                return RedirectToAction(
                    "Index",
                    "Home");
            }


            // -----------------------------------------------------
            // CHECK SELECTED ROLE
            // -----------------------------------------------------

            var actualRole = userRoles
                .FirstOrDefault(r =>
                    r.Equals(
                        selectedRole,
                        StringComparison.OrdinalIgnoreCase));


            // -----------------------------------------------------
            // SUPPORT OLD ADMIN ROLE NAME
            // -----------------------------------------------------

            if (actualRole == null)
            {
                if (
                    selectedRole.Equals(
                        "Administrator",
                        StringComparison.OrdinalIgnoreCase)
                    &&
                    userRoles.Any(r =>
                        r.Equals(
                            "Admin",
                            StringComparison.OrdinalIgnoreCase)))
                {
                    actualRole = "Admin";
                }
                else
                {
                    TempData["LoginError"] =
                        "The selected role does not match your account.";

                    TempData["LoginEmail"] = email;
                    TempData["LoginRole"] = selectedRole;
                    TempData["OpenLoginPopup"] = "true";

                    return RedirectToAction(
                        "Index",
                        "Home");
                }
            }


            // -----------------------------------------------------
            // SIGN IN
            // -----------------------------------------------------

            var result =
                await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    isPersistent: false,
                    lockoutOnFailure: false);


            // =====================================================
            // SUCCESSFUL LOGIN
            // =====================================================

            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "User {Email} logged in successfully as {Role}.",
                    email,
                    selectedRole);


                // -------------------------------------------------
                // PREVENT OLD/CACHED PAGE
                // -------------------------------------------------

                Response.Headers["Cache-Control"] =
                    "no-cache, no-store, must-revalidate";

                Response.Headers["Pragma"] =
                    "no-cache";

                Response.Headers["Expires"] =
                    "0";


                // -------------------------------------------------
                // CLEAR OLD LOGIN DATA
                // -------------------------------------------------

                TempData.Remove("LoginError");
                TempData.Remove("LoginEmail");
                TempData.Remove("LoginRole");
                TempData.Remove("OpenLoginPopup");


                // -------------------------------------------------
                // RETURN URL
                // -------------------------------------------------

                if (!string.IsNullOrWhiteSpace(returnUrl)
                    && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }


                // =================================================
                // ROLE BASED REDIRECTION
                // =================================================

                switch (selectedRole)
                {
                    // -------------------------------------------------
                    // ADMIN
                    // -------------------------------------------------

                    case "Administrator":

                        return RedirectToAction(
                            "Dashboard",
                            "Admin");


                    // -------------------------------------------------
                    // DOCTOR
                    // -------------------------------------------------

                    case "Doctor":

                        return RedirectToAction(
                            "Dashboard",
                            "Doctor");


                    // -------------------------------------------------
                    // PATIENT
                    // IMPORTANT:
                    // Patient will NOT open old patient dashboard.
                    // Patient will open CURRENT Home page.
                    // -------------------------------------------------

                    case "Patient":

                        return RedirectToAction(
                            "Index",
                            "Home");


                    // -------------------------------------------------
                    // RECEPTIONIST
                    // -------------------------------------------------

                    case "Receptionist":

                        return RedirectToAction(
                            "Dashboard",
                            "Receptionist");


                    // -------------------------------------------------
                    // LABORATORY
                    // -------------------------------------------------

                    case "Laboratory":

                        return RedirectToAction(
                            "Dashboard",
                            "Laboratory");


                    // -------------------------------------------------
                    // SAFETY FALLBACK
                    // -------------------------------------------------

                    default:

                        await _signInManager.SignOutAsync();

                        TempData["LoginError"] =
                            "Invalid account role.";

                        TempData["LoginEmail"] = email;
                        TempData["LoginRole"] = selectedRole;
                        TempData["OpenLoginPopup"] = "true";

                        return RedirectToAction(
                            "Index",
                            "Home");
                }
            }


            // =====================================================
            // ACCOUNT LOCKED
            // =====================================================

            if (result.IsLockedOut)
            {
                TempData["LoginError"] =
                    "Your account is temporarily locked. Please try again later.";

                TempData["LoginEmail"] = email;
                TempData["LoginRole"] = selectedRole;
                TempData["OpenLoginPopup"] = "true";

                return RedirectToAction(
                    "Index",
                    "Home");
            }


            // =====================================================
            // NOT ALLOWED
            // =====================================================

            if (result.IsNotAllowed)
            {
                TempData["LoginError"] =
                    "This account is not allowed to sign in.";

                TempData["LoginEmail"] = email;
                TempData["LoginRole"] = selectedRole;
                TempData["OpenLoginPopup"] = "true";

                return RedirectToAction(
                    "Index",
                    "Home");
            }


            // =====================================================
            // LOGIN FAILED
            // =====================================================

            TempData["LoginError"] =
                "Login failed. Please check your email and password.";

            TempData["LoginEmail"] = email;
            TempData["LoginRole"] = selectedRole;
            TempData["OpenLoginPopup"] = "true";

            return RedirectToAction(
                "Index",
                "Home");
        }


        // =========================================================
        // REGISTER - GET
        // =========================================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }


        // =========================================================
        // REGISTER - POST
        // =========================================================

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            // -----------------------------------------------------
            // FULL NAME
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(model.FullName))
            {
                ModelState.AddModelError(
                    "FullName",
                    "Full name is required.");
            }


            // -----------------------------------------------------
            // EMAIL
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError(
                    "Email",
                    "Email is required.");
            }


            // -----------------------------------------------------
            // PHONE NUMBER
            // -----------------------------------------------------

            var phoneNumber =
                model.PhoneNumber?.Trim();

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                ModelState.AddModelError(
                    "PhoneNumber",
                    "Phone number is required.");
            }
            else if (
                phoneNumber.Length != 10 ||
                !phoneNumber.All(char.IsDigit))
            {
                ModelState.AddModelError(
                    "PhoneNumber",
                    "Phone number must contain exactly 10 digits.");
            }


            // -----------------------------------------------------
            // MODEL VALIDATION
            // -----------------------------------------------------

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var email =
                model.Email.Trim();


            // -----------------------------------------------------
            // CHECK EXISTING USER
            // -----------------------------------------------------

            var existingUser =
                await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                ModelState.AddModelError(
                    "Email",
                    "An account with this email already exists.");

                return View(model);
            }


            // -----------------------------------------------------
            // CREATE USER
            // -----------------------------------------------------

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = model.FullName.Trim(),
                PhoneNumber = phoneNumber
            };


            var createResult =
                await _userManager.CreateAsync(
                    user,
                    model.Password);


            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }


            // =====================================================
            // PUBLIC REGISTRATION = PATIENT
            // =====================================================

            const string patientRole = "Patient";


            // -----------------------------------------------------
            // CREATE PATIENT ROLE IF REQUIRED
            // -----------------------------------------------------

            if (!await _roleManager.RoleExistsAsync(patientRole))
            {
                var roleResult =
                    await _roleManager.CreateAsync(
                        new IdentityRole(patientRole));

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);

                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description);
                    }

                    return View(model);
                }
            }


            // -----------------------------------------------------
            // ASSIGN PATIENT ROLE
            // -----------------------------------------------------

            var addRoleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    patientRole);


            if (!addRoleResult.Succeeded)
            {
                foreach (var error in addRoleResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }


            // -----------------------------------------------------
            // DO NOT AUTO LOGIN
            // -----------------------------------------------------

            TempData["RegistrationSuccess"] =
                "Your account has been created successfully. Please sign in.";

            TempData["LoginEmail"] =
                email;

            TempData["LoginRole"] =
                patientRole;

            TempData["OpenLoginPopup"] =
                "true";


            _logger.LogInformation(
                "New Patient account created: {Email}",
                user.Email);


            // After registration, open CURRENT Home page
            return RedirectToAction(
                "Index",
                "Home");
        }


        // =========================================================
        // PROFILE - GET
        // =========================================================

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                await _signInManager.SignOutAsync();

                return RedirectToAction(
                    "Index",
                    "Home");
            }


            // Prevent old profile data from browser cache
            Response.Headers["Cache-Control"] =
                "no-cache, no-store, must-revalidate";

            Response.Headers["Pragma"] =
                "no-cache";

            Response.Headers["Expires"] =
                "0";


            return View(user);
        }


        // =========================================================
        // EDIT PROFILE - GET
        // =========================================================

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                await _signInManager.SignOutAsync();

                return RedirectToAction(
                    "Index",
                    "Home");
            }


            ViewBag.FullName =
                user.FullName;

            ViewBag.Email =
                user.Email;

            ViewBag.PhoneNumber =
                user.PhoneNumber;


            return View(user);
        }


        // =========================================================
        // EDIT PROFILE - POST
        // =========================================================

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(
            string fullName,
            string phoneNumber)
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                await _signInManager.SignOutAsync();

                return RedirectToAction(
                    "Index",
                    "Home");
            }


            // -----------------------------------------------------
            // FULL NAME VALIDATION
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(fullName))
            {
                ModelState.AddModelError(
                    "FullName",
                    "Full name is required.");
            }


            // -----------------------------------------------------
            // PHONE VALIDATION
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                ModelState.AddModelError(
                    "PhoneNumber",
                    "Phone number is required.");
            }
            else
            {
                phoneNumber =
                    phoneNumber.Trim();

                if (
                    phoneNumber.Length != 10 ||
                    !phoneNumber.All(char.IsDigit))
                {
                    ModelState.AddModelError(
                        "PhoneNumber",
                        "Phone number must contain exactly 10 digits.");
                }
            }


            // -----------------------------------------------------
            // MODEL VALIDATION
            // -----------------------------------------------------

            if (!ModelState.IsValid)
            {
                ViewBag.FullName =
                    fullName;

                ViewBag.Email =
                    user.Email;

                ViewBag.PhoneNumber =
                    phoneNumber;

                return View(user);
            }


            // -----------------------------------------------------
            // UPDATE USER
            // -----------------------------------------------------

            user.FullName =
                fullName.Trim();

            user.PhoneNumber =
                phoneNumber.Trim();


            var updateResult =
                await _userManager.UpdateAsync(user);


            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                ViewBag.FullName =
                    user.FullName;

                ViewBag.Email =
                    user.Email;

                ViewBag.PhoneNumber =
                    user.PhoneNumber;

                return View(user);
            }


            // -----------------------------------------------------
            // REFRESH LOGIN COOKIE
            // -----------------------------------------------------

            await _signInManager.RefreshSignInAsync(user);


            TempData["ProfileSuccess"] =
                "Profile updated successfully.";


            // After profile update, return to CURRENT Home page
            return RedirectToAction(
                "Index",
                "Home");
        }


        // =========================================================
        // LOGOUT
        // =========================================================

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();


            // Prevent browser from showing old authenticated page
            Response.Headers["Cache-Control"] =
                "no-cache, no-store, must-revalidate";

            Response.Headers["Pragma"] =
                "no-cache";

            Response.Headers["Expires"] =
                "0";


            return RedirectToAction(
                "Index",
                "Home");
        }


        // =========================================================
        // ACCESS DENIED
        // =========================================================

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        // =========================================================
        // NORMALIZE ROLE
        // =========================================================

        private static string NormalizeRole(
            string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return string.Empty;
            }


            role = role.Trim();


            // -----------------------------------------------------
            // ADMIN
            // -----------------------------------------------------

            if (
                role.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Administrator";
            }


            if (
                role.Equals(
                    "Administrator",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Administrator";
            }


            // -----------------------------------------------------
            // DOCTOR
            // -----------------------------------------------------

            if (
                role.Equals(
                    "Doctor",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Doctor";
            }


            // -----------------------------------------------------
            // PATIENT
            // -----------------------------------------------------

            if (
                role.Equals(
                    "Patient",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Patient";
            }


            // -----------------------------------------------------
            // USER
            // IMPORTANT:
            // Normal User will behave like Patient
            // and open the current Home page.
            // -----------------------------------------------------

            if (
                role.Equals(
                    "User",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Patient";
            }


            // -----------------------------------------------------
            // RECEPTIONIST
            // -----------------------------------------------------

            if (
                role.Equals(
                    "Receptionist",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Receptionist";
            }


            // -----------------------------------------------------
            // LABORATORY
            // -----------------------------------------------------

            if (
                role.Equals(
                    "Laboratory",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Laboratory";
            }


            return string.Empty;
        }
    }
}
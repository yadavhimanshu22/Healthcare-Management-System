using Microsoft.AspNetCore.Mvc;

namespace HealthcareManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        // =========================
        // Home Page
        // URL: /Home/Index
        // =========================
        public IActionResult Index()
        {
            return View();
        }

        // =========================
        // About Page
        // URL: /Home/About
        // =========================
        public IActionResult About()
        {
            return View();
        }

        // =========================
        // Services Page
        // URL: /Home/Services
        // =========================
        public IActionResult Services()
        {
            return View();
        }

        // =========================
        // Contact Page
        // URL: /Home/Contact
        // =========================
        public IActionResult Contact()
        {
            return View();
        }

        // =========================
        // Services Redirect
        // =========================
        public IActionResult OurServices()
        {
            return RedirectToAction("Services");
        }
    }
}
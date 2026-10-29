using Microsoft.AspNetCore.Mvc;

namespace HealthCareSystem.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
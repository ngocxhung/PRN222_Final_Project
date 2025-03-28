using Microsoft.AspNetCore.Mvc;

namespace PRN222_Project.Controllers
{
    public class NotificationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SendNotification()
        {
            return View();
        }
    }
}
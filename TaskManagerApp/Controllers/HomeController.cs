using Microsoft.AspNetCore.Mvc;

namespace TaskManagerApp.Controllers
{
    public class HomeController : Controller
    {
        // Action for the Welcome Page (Index)
        public IActionResult Index()
        {
            return View();
        }

        // Action for the HomePage after "Get Started" click
        public IActionResult HomePage()
        {
            return View();
        }
    }
}

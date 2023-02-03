using Microsoft.AspNetCore.Mvc;

namespace MovieStreaming.Areas.User.Controllers
{ 
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.D = true;
            return View();
        }
    }
}

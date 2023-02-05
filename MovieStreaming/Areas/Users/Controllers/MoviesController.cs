using Microsoft.AspNetCore.Mvc;

namespace MovieStreaming.Areas.Users.Controllers
{
    public class MoviesController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.M = true;
            return View();
        }
    }
}

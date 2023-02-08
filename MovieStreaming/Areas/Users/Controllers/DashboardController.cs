using Microsoft.AspNetCore.Mvc;
using MovieStreaming.Custom.DatabaseHelpers;
using MovieStreaming.Custom.Helpers;
using MovieStreaming.Areas.Admin.Models.User;
using System.Threading.Tasks;
using MovieStreaming.Areas.Admin.Models.Movie;
using System.Linq;

namespace MovieStreaming.Areas.User.Controllers
{ 
    public class DashboardController : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.D = true;
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            Admin.Models.User.User user = (await new Query().SelectSingle<Admin.Models.User.User>($"select Name,Surname,Username,IsSubscribed From [User] where Id={userId}")).Result;
            ViewBag.NewMovies = (await new Query().Select<Movie>("SELECT TOP 5 * from Movie order by Id Desc")).Result.ToList();

            return View(user);
        }
        public async Task<IActionResult> AddSubscription()
        {
            return View();
        }

        public async Task<ActionResult> Subscribe()
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            var createUpdateResult = await new Query().Execute("SubscribeUser @Id,@IsSubscribed", new
            {
                @Id = userId,
                @IsSubscribed = true
            });

            return Json(createUpdateResult);
        }

        public async Task<ActionResult> UnSubscribe()
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            var createUpdateResult = await new Query().Execute("SubscribeUser @Id,@IsSubscribed", new
            {
                @Id = userId,
                @IsSubscribed = false
            });

            return RedirectToAction("Index","Dashboard");
        }
    }
}

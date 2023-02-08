using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieStreaming.Areas.Admin.Models.Movie;
using MovieStreaming.Custom.DatabaseHelpers;
using MovieStreaming.Custom.Helpers;
using MovieStreaming.Custom.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStreaming.Areas.Admin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            Models.User.User currentUser = (await new Query().SelectSingle<Models.User.User>($"select * from [User] where Id={userId}")).Result;
            ViewBag.Total = (await new Query().SelectSingle<int>($"select count(*) from [Movie]")).Result;
            ViewBag.TotalUsers = (await new Query().SelectSingle<int>($"select count(*) from [User]")).Result;
            ViewBag.SubbedUsers = (await new Query().SelectSingle<int>($"select count(*) from [User] where IsSubscribed=1")).Result;
            ViewBag.ActiveComplaints = (await new Query().SelectSingle<int>($"select count(*) from [Complaint] where IsActive=1")).Result;
            if (currentUser.RoleId == 2)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Users" });
            }
            ViewBag.H = true;
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            Models.User.User currentUser = (await new Query().SelectSingle<Models.User.User>($"select * from [User] where Id={userId}")).Result;
            if (currentUser.RoleId == 2)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Users" });
            }
            return View();
        }
        public async Task<IActionResult> GetSurveysByMonth([DataSourceRequest] DataSourceRequest request)
        {
            var data = new List<MovieStats>();
            data = (await new Query().Select<MovieStats>($"select * from GetMoviesByMonth()")).Result.ToList();

            return Json(data);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
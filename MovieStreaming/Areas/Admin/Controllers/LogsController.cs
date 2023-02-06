using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MovieStreaming.Custom.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MovieStreaming.Areas.Admin.Models.ChangeLogs;
using Microsoft.AspNetCore.Authorization;
using MovieStreaming.Custom.Helpers;
using System.Threading.Tasks;
using MovieStreaming.Custom.DatabaseHelpers;

namespace MovieStreaming.Areas.Admin.Controllers
{
    [Authorize]
    public class LogsController : Controller
    {
        private MovieDBContext _context = new MovieDBContext();

        public LogsController(MovieDBContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            Models.User.User currentUser = (await new Query().SelectSingle<Models.User.User>($"select * from [User] where Id={userId}")).Result;
            if (currentUser.RoleId == 2)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Users" });
            }
            ViewBag.CL = true;
            return View();
        }

        public ActionResult Read_Logs([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                var logs = _context.Logs.ToList();

                return Json(logs.ToDataSourceResult(request));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }

        public ActionResult Create_Logs([DataSourceRequest] DataSourceRequest request, ChangeLog log)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    _context.Logs.Add(log);
                    _context.SaveChanges();
                    var _loglist = _context.Logs.ToList();
                    return Json(new[] { log }.ToDataSourceResult(request, ModelState));
                }

                else
                {
                    return Json(_context.Logs.ToList());
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public ActionResult Update_Logs([DataSourceRequest] DataSourceRequest request, ChangeLog log)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Entry(log).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Json(new[] { log }.ToDataSourceResult(request, ModelState));

                }
                else
                {
                    return Json(_context.Logs.ToList());
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public ActionResult Delete_Logs([DataSourceRequest] DataSourceRequest request, ChangeLog log)
        {
            try
            {
                ChangeLog logs = _context.Logs.Find(log.Id);
                if (logs == null)
                {
                    return Json("Role Not Found!");
                }

                _context.Logs.Remove(logs);
                _context.SaveChanges();
                return Json(_context.Logs.ToList());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public async Task<ActionResult> Details(ChangeLog log)
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            Models.User.User currentUser = (await new Query().SelectSingle<Models.User.User>($"select * from [User] where Id={userId}")).Result;
            if (currentUser.RoleId == 2)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Users" });
            }
            ChangeLog logs = _context.Logs.Find(log.Id);

            return View(logs);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieStreaming.Custom.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MovieStreaming.Custom.DatabaseHelpers;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using MovieStreaming.Areas.Admin.Models.Role;
using Microsoft.AspNetCore.Authorization;
using MovieStreaming.Custom.Helpers;

namespace MovieStreaming.Areas.Admin.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private MovieDBContext _context = new MovieDBContext();

        public RoleController(MovieDBContext context)
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
            ViewBag.R = true;
            return View();
        }

        public ActionResult Read_Roles([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                var role = _context.Roles.ToList();

                return Json(role.ToDataSourceResult(request));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }

        public ActionResult Create_Roles([DataSourceRequest] DataSourceRequest request, Role rol)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    _context.Roles.Add(rol);
                    _context.SaveChanges();
                    var _rollist = _context.Roles.ToList();
                    return Json(new[] { rol }.ToDataSourceResult(request, ModelState));
                }

                else
                {
                    return Json(_context.Roles.ToList());
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public ActionResult Update_Roles([DataSourceRequest] DataSourceRequest request, Role rol)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Entry(rol).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Json(new[] { rol }.ToDataSourceResult(request, ModelState));

                }
                else
                {
                    return Json(_context.Roles.ToList());
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public ActionResult Delete_Roles([DataSourceRequest] DataSourceRequest request, Role rol)
        {
            try
            {
                Role role = _context.Roles.Find(rol.Id);
                if (role == null)
                {
                    return Json("Role Not Found!");
                }

                _context.Roles.Remove(role);
                _context.SaveChanges();
                return Json(_context.Roles.ToList());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public async Task<ActionResult> Details(Role rol)
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            Models.User.User currentUser = (await new Query().SelectSingle<Models.User.User>($"select * from [User] where Id={userId}")).Result;
            if (currentUser.RoleId == 2)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Users" });
            }
            Role role = _context.Roles.Find(rol.Id);

            return View(role);
        }
    }
}

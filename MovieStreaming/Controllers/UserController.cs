﻿using Microsoft.AspNetCore.Mvc;
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
using MovieStreaming.Custom.Models.User;

namespace MovieStreaming.Controllers
{
    public class UserController : Controller
    {
        MovieDBContext _context = new MovieDBContext();

        public UserController(MovieDBContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            //var select = from s in _context.Roles select new { s.Id, s.Name };
            //ViewBag.RoleId = select;
            ViewBag.U = true;
            return View();
        }

        public ActionResult Read_Users([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                var user = _context.Users.ToList();

                return Json(user.ToDataSourceResult(request));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }

        public ActionResult Create_Users([DataSourceRequest] DataSourceRequest request, User usr)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    _context.Users.Add(usr);
                    _context.SaveChanges();
                    var _usrlist = _context.Users.ToList();
                    return Json(new[] { usr }.ToDataSourceResult(request, ModelState));
                }

                else
                {
                    return Json(_context.Users.ToList());
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public ActionResult Update_Roles([DataSourceRequest] DataSourceRequest request, User usr)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Entry(usr).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Json(new[] { usr }.ToDataSourceResult(request, ModelState));

                }
                else
                {
                    return Json(_context.Users.ToList());
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public ActionResult Delete_Roles([DataSourceRequest] DataSourceRequest request, User usr)
        {
            try
            {
                User user = _context.Users.Find(usr.Id);
                if (user == null)
                {
                    return Json("User Not Found!");
                }

                _context.Users.Remove(user);
                _context.SaveChanges();
                return Json(_context.Users.ToList());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public ActionResult Details(User usr)
        {
            User user = _context.Users.Find(usr.Id);

            return View(user);
        }

    }
}
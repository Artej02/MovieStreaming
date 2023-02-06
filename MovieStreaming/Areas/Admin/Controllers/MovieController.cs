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
using MovieStreaming.Areas.Admin.Models.Movie;
using System.IO;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using MovieStreaming.Custom.Helpers;

namespace MovieStreaming.Areas.Admin.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        private MovieDBContext _context = new MovieDBContext();
        private readonly IMapper _mapper;
        public IWebHostEnvironment WebHostEnvironment { get; set; }

        public MovieController(MovieDBContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            WebHostEnvironment = webHostEnvironment;
        }

        public async Task<ActionResult> Index(UploadOverviewModel model)
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            Models.User.User currentUser = (await new Query().SelectSingle<Models.User.User>($"select * from [User] where Id={userId}")).Result;
            if (currentUser.RoleId == 2)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Users" });
            }
            if (model.AllowedExtensions == null)
            {
                model = new UploadOverviewModel()
                {
                    AllowedExtensions = new string[] { "mp4" },
                    IsLimited = false
                };
            }
            ViewBag.M = true;
            return View(model);
        }

        public ActionResult Read_Movies([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                var movie = _context.Movies.ToList();

                return Json(movie.ToDataSourceResult(request));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }

        public async Task<ActionResult> Submit(IEnumerable<IFormFile> files)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                    var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                    var physicalPath = Path.Combine("wwwroot", "videos", fileName);

                    // Saving the file
                    using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }

            return Content("");
        }


        public ActionResult Create_Movies([DataSourceRequest] DataSourceRequest request, Movie mov)
            {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Movies.Add(mov);
                    _context.SaveChanges();
                    var _comlist = _context.Movies.ToList();
                    return Json(new[] { mov }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    return Json(_context.Movies.ToList());

                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public ActionResult Update_Movies([DataSourceRequest] DataSourceRequest request, Movie mov)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Entry(mov).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Json(new[] { mov }.ToDataSourceResult(request, ModelState));

                }
                else
                {
                    return Json(_context.Movies.ToList());
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public ActionResult Delete_Movies([DataSourceRequest] DataSourceRequest request, Movie mov)
        {
            try
            {
                Movie movie = _context.Movies.Find(mov.Id);
                if (movie == null)
                {
                    return Json("Movie Not Found!");
                }

                _context.Movies.Remove(movie);
                _context.SaveChanges();
                return Json(_context.Movies.ToList());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public async Task<ActionResult> Details(Movie mov)
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            Models.User.User currentUser = (await new Query().SelectSingle<Models.User.User>($"select * from [User] where Id={userId}")).Result;
            if (currentUser.RoleId == 2)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Users" });
            }
            Movie movie = _context.Movies.Find(mov.Id);

            return View(movie);
        }
    }
}

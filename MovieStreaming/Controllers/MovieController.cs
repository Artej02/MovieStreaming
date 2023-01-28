using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieStreaming.Custom.Models;
using MovieStreaming.Custom.Models.Movie;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MovieStreaming.Custom.DatabaseHelpers;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;

namespace MovieStreaming.Controllers
{
    public class MovieController : Controller
    {
        private MovieDBContext _context = new MovieDBContext();

        public MovieController(MovieDBContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            ViewBag.M = true;
            return View();
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

        public ActionResult Create_Movies([DataSourceRequest] DataSourceRequest request, Movie mov)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    _context.Movies.Add(mov);
                    _context.SaveChanges();
                    var _movlist = _context.Movies.ToList();
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

        public ActionResult Details(Movie mov)
        {
            Movie movie = _context.Movies.Find(mov.Id);

            return View(movie);
        }
    }
}

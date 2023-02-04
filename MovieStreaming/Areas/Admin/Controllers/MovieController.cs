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

namespace MovieStreaming.Areas.Admin.Controllers
{
    public class MovieController : Controller
    {
        private MovieDBContext _context = new MovieDBContext();
        private readonly IMapper _mapper;

        public MovieController(MovieDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

        [HttpPost]
        public IActionResult UploadFile()
        {
            var file = Request.Form.Files[0];
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return Ok(file.FileName);
        }

        public async Task<ActionResult> Create_Movies([DataSourceRequest] DataSourceRequest request, MoviePopupViewModel mov)
            {
            try
            {
                if (ModelState.IsValid)
                {
                    if (mov.Video != null)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(mov.Video.FileName);
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await mov.Video.CopyToAsync(stream);

                        }
                        var movie = _mapper.Map<Movie>(mov);
                        movie.Video = path;
                        _context.Movies.Add(movie);
                        _context.SaveChanges();
                        var _movlist = _context.Movies.ToList();
                        return Json(new[] { mov }.ToDataSourceResult(request, ModelState));
                    }
                    else
                    {
                        return Json(new { error = "Error!" });
                    }
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

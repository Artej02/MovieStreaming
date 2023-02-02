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
using MovieStreaming.Areas.Admin.Models.Complaint;

namespace MovieStreaming.Areas.Admin.Controllers
{
    public class ComplaintContoller : Controller
    {
        private MovieDBContext _context = new MovieDBContext();

        public ComplaintContoller(MovieDBContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            ViewBag.C = true;
            return View();
        }

        public ActionResult Read_Complaints([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                var complaint = _context.Complaints.ToList();

                return Json(complaint.ToDataSourceResult(request));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }

        public ActionResult Create_Complaints([DataSourceRequest] DataSourceRequest request, Complaint com)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    _context.Complaints.Add(com);
                    _context.SaveChanges();
                    var _comlist = _context.Complaints.ToList();
                    return Json(new[] { com }.ToDataSourceResult(request, ModelState));
                }

                else
                {
                    return Json(_context.Complaints.ToList());
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public ActionResult Update_Complaints([DataSourceRequest] DataSourceRequest request, Complaint com)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Entry(com).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Json(new[] { com }.ToDataSourceResult(request, ModelState));

                }
                else
                {
                    return Json(_context.Complaints.ToList());
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public ActionResult Delete_Complaints([DataSourceRequest] DataSourceRequest request, Complaint com)
        {
            try
            {
                Complaint complaint = _context.Complaints.Find(com.Id);
                if (complaint == null)
                {
                    return Json("Complaint Not Found!");
                }

                _context.Complaints.Remove(complaint);
                _context.SaveChanges();
                return Json(_context.Complaints.ToList());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public ActionResult Details(Complaint com)
        {
            Complaint complaint = _context.Complaints.Find(com.Id);

            return View(complaint);
        }
    }
}

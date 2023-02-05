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
using MovieStreaming.Custom.Helpers;
using Microsoft.Extensions.Configuration;

namespace MovieStreaming.Areas.Users.Controllers
{
    public class ComplaintController : Controller
    {
        private MovieDBContext _context = new MovieDBContext();
        private IConfiguration Configuration;

        public ComplaintController(IConfiguration configuration,MovieDBContext context)
        {
            Configuration = configuration;
            _context = context;
        }

        public ActionResult Index()
        {
            ViewBag.C = true;
            return View();
        }
        public ActionResult Form()
        {
            ViewBag.C = true; 
            return View();
        }

        public async Task<ActionResult> GetAllComplaints([DataSourceRequest] DataSourceRequest request)
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            var skip = (request.Page - 1) * request.PageSize;
            var query = $"select * from Complaint where CreatedUserId={userId}";
            var countQuery = $"select count(*) from Ticket";

            string pagination = " OFFSET " + skip + " ROWS FETCH NEXT " + request.PageSize + " ROWS ONLY";
            
            var total = await new Query().SelectSingle<int>(countQuery);
            var result = await new Query().Select<Complaint>(query + pagination);

            return Json(new { Data = result.Result, Total = total.Result });
        }

        public async Task<ActionResult> CreateComplaint([DataSourceRequest] DataSourceRequest request, Complaint ticket)
        {

            bool HasAffected = false;
            var userId = new AuthorizeHelper(HttpContext).GetUserID();

            var createUpdateResult = await new Query().ExecuteAndGetInsId("CreateTicket @Id,@Title,@Description,@TypeId,@SeverityId,@CreatedUserId,@IsActive", new
            {

                @Id = ticket.Id,
                @Title = ticket.Title,
                @Description = ticket.Description,
                @TypeId = ticket.TypeId,
                @SeverityId = ticket.SeverityId,
                @CreatedUserId = userId,
                @IsActive = true

            });
            if (createUpdateResult > 0)
            {

                HasAffected = true;
                Email(userId);

            }
            if (!ModelState.IsValid)
            {
                return this.Json(new DataSourceResult
                {

                    Errors = "Error occurred!"

                });
            }
            return Json(HasAffected);
        }

        private ActionResult Email(int? userID)
        {
            MailHelper mailHelper = new MailHelper(Configuration);
            return Json(mailHelper.SendEmailForTicketCreation(userID));

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

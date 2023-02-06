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
using Microsoft.AspNetCore.Authorization;

namespace MovieStreaming.Areas.Users.Controllers
{
    [Authorize]
    public class ComplaintController : Controller
    {
        private MovieDBContext _context = new MovieDBContext();
        private IConfiguration Configuration;

        public ComplaintController(IConfiguration configuration,MovieDBContext context)
        {
            Configuration = configuration;
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            ViewBag.Type = (await new Query().Select<SelectListItem>($"select Id as [Value], [Name] as [Text] from TicketType")).Result;
            ViewBag.Severity = (await new Query().Select<SelectListItem>($"select Id as [Value], [Name] as [Text] from Severity")).Result;
            ViewBag.C = true;
            return View();
        }
        public ActionResult Form()
        {
            ViewBag.C = true; 
            return View();
        }

        public async Task<ActionResult> Details(Complaint com)
        {
            ViewBag.ComplaintId = com.Id;
            ViewBag.Complaint = (await new Query().Select<SelectListItem>($"select Id as [Value], [Title] as [Text] from Complaint")).Result;
            ViewBag.User = (await new Query().Select<SelectListItem>($"select Id as [Value], [Name] as [Text] from User")).Result;
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
  
            Complaint complaint = _context.Complaints.Find(com.Id);

            return View(complaint);
        }

        public async Task<ActionResult> GetAllComplaints([DataSourceRequest] DataSourceRequest request)
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            var skip = (request.Page - 1) * request.PageSize;
            var query = $"select * from Complaint where CreatedUserId={userId}";
            var countQuery = $"select count(*) from Complaint";

            //string pagination = " OFFSET " + skip + " ROWS FETCH NEXT " + request.PageSize + " ROWS ONLY";
            
            var total = await new Query().SelectSingle<int>(countQuery);
            var result = await new Query().Select<Complaint>(query);

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

        public async Task<ActionResult> CloseComplaint(Complaint complaint, int complaintId)
        {

            var createUpdateResult = await new Query().Execute($"UPDATE [Complaint] SET IsActive = @IsActive WHERE Id = {complaintId}", new

            {

                @IsActive = false

            });

            if (createUpdateResult.HasError)
            {
                return this.Json(new DataSourceResult
                {
                    Errors = "Error occurred! "
                });

            }
            return RedirectToAction("Details", "Complaint", new { complaintId });

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

        public async Task<ActionResult> GetDetails([DataSourceRequest] DataSourceRequest request, int complaintId)
        {
            ViewBag.ComplaintId = complaintId;
            var query = $"select Title,Description from Complaint where Id={complaintId}";
            var result = await new Query().Select<Complaint>(query);

            return Json(new { Data = result.Result });
        }

        public async Task<ActionResult> Reply(int complaintId)
        {
            ViewBag.ComplaintId = complaintId;
            return View();
        }

        public async Task<ActionResult> GetAllReplies([DataSourceRequest] DataSourceRequest request, int complaintId)
        {
            var skip = (request.Page - 1) * request.PageSize;
            var query = $"select * from Reply where ComplaintId = {complaintId}";
            var countQuery = $"select count(*) from Reply";

            string filters = "";
            foreach (var filter in request.Filters)
            {
                filters += KendoGridHelper.ApplyFilter<Reply>(filter);
            }
            string sort = KendoGridHelper.ApplySort<Reply>(request.Sorts, "ID desc");
            string pagination = " OFFSET " + skip + " ROWS FETCH NEXT " + request.PageSize + " ROWS ONLY";
            if (filters.Trim().Length > 0)
                filters = " WHERE " + filters;
            if (sort.Trim().Length > 0)
                sort = " ORDER BY " + sort;
            var total = await new Query().SelectSingle<int>(countQuery + filters);
            var result = await new Query().Select<Reply>(query + filters + sort + pagination);

            return Json(new { Data = result.Result, Total = total.Result });
        }

        public async Task<ActionResult> CreateReply(Reply reply, int complaintId)
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            if (ModelState.IsValid)
            {
                var createUpdateResult = await new Query().Execute("INSERT INTO [Reply] (ComplaintId,Message,UserId,Date,Email) VALUES (@ComplaintId,@Message,@UserId,@Date,@Email)", new

                {
                    @ComplaintId = complaintId,
                    @Message = reply.Message,
                    @UserId = userId,
                    @Date = DateTime.Now,
                    @Email = reply.Email


                });
                if (createUpdateResult.HasError)
                {
                    return this.Json(new DataSourceResult
                    {
                        Errors = "Error occurred! "
                    });

                }
                return Json(createUpdateResult.HasAffected);

            }
            else
            {
                return this.Json(new DataSourceResult
                {
                    Errors = "Error occurred! "
                });

            }
        }
    }
}

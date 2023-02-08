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
using Microsoft.AspNetCore.Authorization;
using MovieStreaming.Custom.Helpers;
using MovieStreaming.Areas.Admin.Models.User;
using Microsoft.Extensions.Configuration;

namespace MovieStreaming.Areas.Admin.Controllers
{
    [Authorize]
    public class ComplaintsController : Controller
    {
        private MovieDBContext _context = new MovieDBContext();
        private IConfiguration Configuration;

        public ComplaintsController(MovieDBContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        public async Task<ActionResult> Index()
        {
            ViewBag.Type = (await new Query().Select<SelectListItem>($"select Id as [Value], [Name] as [Text] from ComplaintType")).Result;
            ViewBag.Severity = (await new Query().Select<SelectListItem>($"select Id as [Value], [Name] as [Text] from Severity")).Result;
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            Models.User.User currentUser = (await new Query().SelectSingle<Models.User.User>($"select * from [User] where Id={userId}")).Result;
            if(currentUser.RoleId == 2)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Users" });
            }
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

        public async Task<ActionResult> CloseComplaint(Complaint complaint)
        {

            var createUpdateResult = await new Query().Execute($"UPDATE [Complaint] SET IsActive = @IsActive WHERE Id = {complaint.Id}", new

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
            return RedirectToAction("Details", "Complaints", new { complaint.Id });

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

        public async Task<ActionResult> Details(Complaint com)
        {
            ViewBag.ComplaintId = com.Id;
            ViewBag.Complaint = (await new Query().Select<SelectListItem>($"select Id as [Value], [Title] as [Text] from Complaint")).Result;
            ViewBag.User = (await new Query().Select<SelectListItem>($"select Id as [Value], [Name] as [Text] from [User]")).Result;
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            Models.User.User currentUser = (await new Query().SelectSingle<Models.User.User>($"select * from [User] where Id={userId}")).Result;
            if (currentUser.RoleId == 2)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Users" });
            }
            Complaint complaint = _context.Complaints.Find(com.Id);

            return View(complaint);
        }
        public async Task<ActionResult> GetDetails([DataSourceRequest] DataSourceRequest request, int complaintId)
        {
            ViewBag.ComplaintId = complaintId;
            var query = $"select Title,Description from Complaint where Id={complaintId}";
            var result = await new Query().Select<Complaint>(query);

            return Json(new { Data = result.Result });
        }

        public ActionResult Reply(int complaintId)
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
            Complaint complaint = (await new Query().SelectSingle<Complaint>($"select * From Complaint where Id={complaintId}")).Result;
            int userID = complaint.CreatedUserId;
            var userName = new AuthorizeHelper(HttpContext).GetUserName();
            if (ModelState.IsValid)
            {
                var createUpdateResult = await new Query().Execute("INSERT INTO [Reply] (ComplaintId,Message,UserId,Date,Email) VALUES (@ComplaintId,@Message,@UserId,@Date,@Email)", new
                {
                    @ComplaintId = complaintId,
                    @Message = reply.Message,
                    @UserId = userId,
                    @Date = DateTime.Now,
                    @Email = userName,
                });
                if (createUpdateResult.HasError)
                {
                    return this.Json(new DataSourceResult
                    {
                        Errors = "Error occurred! "
                    });
                }
                Email(userID); return Json(createUpdateResult.HasAffected);
            }
            else
            {
                return this.Json(new DataSourceResult
                {
                    Errors = "Error occurred! "
                });
            }
        }
        private ActionResult Email(int userID)
        {
            MailHelper mailHelper = new MailHelper(Configuration);

            return Json(mailHelper.SendReplyToEmail(userID));
        }
    }
}

using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MovieStreaming.Areas.Admin.Models.Complaint;
using MovieStreaming.Custom.DatabaseHelpers;
using MovieStreaming.Custom.Helpers;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MovieStreaming.Areas.Users.Controllers
{
    public class ComplaintController : Controller
    {
        private IConfiguration Configuration;
        public ComplaintController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Form()
        {
            return View();
        }
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
}

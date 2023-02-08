using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using MovieStreaming.Custom.DatabaseHelpers;
using MovieStreaming.Custom.Helpers;
using MovieStreaming.Custom.Models.PasswordReset;

namespace MovieStreaming.Controllers
{
    public class ResetPasswordController : Controller
    {
        private IConfiguration Configuration;
        public ResetPasswordController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        public IActionResult ResetPassword(string resetToken)
        {
            ResetPassword resetPassword = new ResetPassword(resetToken);

            return View(resetPassword);
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(RequestResetPassword requestResetPassword)
        {
            var user = (await new Query().SelectSingle<Areas.Admin.Models.User.User>($"Select * From [User] Where Username='{requestResetPassword.Username}'"));
            if (!user.HasData)
            {
                return Json(new { HasError = false, UserNotExist = true, HasAffected = false });
            }
            var token = Guid.NewGuid().ToString();
            var createUpdateResult = await new Query().Execute("CreateResetPasswordRequest @Username,@Token", new
            {
                @Username = requestResetPassword.Username,
                @Token = token
            });
            if (createUpdateResult.HasAffected)
            {
                ResetEmail(token, requestResetPassword.Username);
                return Json(new { HasError = false, UserNotExist = false, HasAffected = true });
            }
            return Json(new { HasError = true, UserNotExist = false, HasAffected = false });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            if (!resetPassword.Password.Equals(resetPassword.ConfirmPassword))
            {
                return Json(new { HasError = false, PasswordDoesNotMatch = true, RequestNotValid = false, HasAffected = false });
            }
            var request = (await new Query().SelectSingle<RequestResetPassword>($"Select * From ResetPasswordRequest Where ResetToken='{resetPassword.resetToken}' and GETDATE()< ExpiryDate"));
            if (!request.HasData)
            {
                return Json(new { HasError = false, PasswordDoesNotMatch = false, RequestNotValid = true, HasAffected = false });
            }
            string hashedPassword = null, salt = null;
            if (resetPassword.Password != null)
            {
                var password = new PasswordHelper(resetPassword.Password);
                hashedPassword = password.Hash;
                salt = password.Salt;
            }
            var createUpdateResult = await new Query().Execute($"Update [User] set Password='{hashedPassword}', Salt='{salt}' where Username='{request.Result.Username}'");
            if (createUpdateResult.HasAffected)
            {
                return Json(new { HasError = false, PasswordDoesNotMatch = false, RequestNotValid = false, HasAffected = true });
            }
            return Json(new { HasError = true, PasswordDoesNotMatch = false, RequestNotValid = false, HasAffected = false });
        }
        public ActionResult ResetEmail(string token, string Username)
        {
            MailHelper mailHelper = new MailHelper(Configuration);

            return Json(mailHelper.SendResetPasswordMailAsync(token, Username));
        }
    }
}

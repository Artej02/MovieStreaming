using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieStreaming.Custom.Helpers;
using MovieStreaming.Custom.DatabaseHelpers;
using MovieStreaming.Custom.Models.User;
using MovieStreaming.Custom.Models.Configuration;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace MovieStreaming.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction(controllerName: "Home", actionName: "Index");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(Login login)
        {

            if (ModelState.IsValid)
            {
                var user = (await new Query().SelectSingle<User>("SELECT * FROM [Users] WHERE Username = @Username", new { @Username = login.Username }));


                if (user.HasError)
                {
                    ModelState.AddModelError(String.Empty, user.ErrorMessage);

                    return Json(new { IsSuccessful = false, FailedValidation = true, IncorrectCredenials = false, WaitingConfirmation = false, IsApproved = false });
                }
                else
                {
                        if (user.HasData && PasswordHelper.Verify(user.Result.Salt, user.Result.Password, login.Password))
                        {
                            if (user.Result.IsApproved == null)
                            {
                                return Json(new { IsSuccessful = false, FailedValidation = false, IncorrectCredenials = false, WaitingConfirmation = true, IsApproved = false });
                            }
                            else if (user.Result.IsApproved == false)
                            {
                                return Json(new { IsSuccessful = false, FailedValidation = false, IncorrectCredenials = false, WaitingConfirmation = false, IsApproved = false });

                            }
                            else
                            {
                                await new AuthorizeHelper(HttpContext).SetAuthentication(user.Result, isPersistent: login.RememberLogin);

                                return Json(new { IsSuccessful = true, FailedValidation = false, IncorrectCredenials = false, WaitingConfirmation = false, IsApproved = true });
                            }
                        }
                        else
                        {

                            return Json(new { IsSuccessful = false, FailedValidation = false, IncorrectCredenials = true, WaitingConfirmation = false, IsApproved = false });
                        }
                }
            }
            else
            {

                return Json(new { IsSuccessful = false, FailedValidation = true, IncorrectCredenials = false, WaitingConfirmation = false, IsApproved = false });//return View(login);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignUp()
        {

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> SignUp(User user)
        {

            if (ModelState.IsValid)
            {


                string hashedPassword = null, salt = null;
                if (user.Password != null)
                {
                    var password = new PasswordHelper(user.Password);
                    hashedPassword = password.Hash;
                    salt = password.Salt;
                }
                bool emailExists = (await new Query().SelectSingle<bool>($"select ISNULL((SELECT 1 FROM [Users] WHERE [Username] = '{user.Username}'), 0)")).Result;

                if (emailExists)
                {
                    return Json(new { IsSuccessful = false, FailedValidation = false, EmailExists = true });
                }
                else
                {
                    var createUpdateResult = await new Query().Execute("CreateUpdateDeleteUsers @CRUDOperation,@Id,@Name,@Surname,@Username,@Password,@Salt,@OrganisationId,@RoleId,@IsApproved,@CreatedUserId,@UpdatedUserId,@CreatedDate,@UpdatedDate", new
                    {
                        @CRUDOperation = (int)CRUDOperation.Create,
                        @Id = user.Id,
                        @Name = user.Name,
                        @Surname = user.Surname,
                        @Username = user.Username,
                        @Password = hashedPassword,
                        @Salt = salt,
                        @RoleId = user.RoleId,
                        @IsApproved = user.IsApproved,
                        @CreatedUserId = user.CreatedUserId,
                        @UpdatedUserId = user.UpdatedUserId,
                        @CreatedDate = DateTime.Now,
                        @UpdatedDate = DateTime.Now
                    });

                    return Json(new { IsSuccessful = true, FailedValidation = false, EmailExists = false, }/*, createUpdateResult.HasAffected*/);


                }

            }
            return Json(new { IsSuccessful = false, FailedValidation = true, EmailExists = false });

        }

        public async Task<ActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}

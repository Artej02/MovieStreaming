﻿using Microsoft.AspNetCore.Mvc;
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
using MovieStreaming.Areas.Admin.Models.Role;
using MovieStreaming.Areas.Admin.Models.User;
using MovieStreaming.Custom.Helpers;
using MovieStreaming.Custom.Models.Configuration;
using MovieStreaming.Custom.Models.LogsModel;

namespace MovieStreaming.Controllers
{
    public class UserController : Controller
    {
        MovieDBContext _context = new MovieDBContext();

        public UserController(MovieDBContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            
            ViewBag.RoleId = (await new Query().Select<SelectListItem>("select Id AS [Value], Name AS Text from Role")).Result;
            ViewBag.U = true;
            return View();
        }

        public ActionResult Read_Users([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                var user = _context.Users.ToList();

                return Json(user.ToDataSourceResult(request));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }

        public async Task<ActionResult> Create_Users([DataSourceRequest] DataSourceRequest request, User user)
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            bool HasAffected = false;
            if (!user.Id.HasValue && user.Password == null)
            {
                ModelState.AddModelError("Password", "Please enter the password");
                return View("AddUsers", user);
            }
            string hashedPassword = null, salt = null;
            if (user.Password != null)
            {
                var password = new PasswordHelper(user.Password);
                hashedPassword = password.Hash;
                salt = password.Salt;
            }
            var userExists = (await new Query().SelectSingle<int>($"select Count(*) from User where Username = '{user.Username}'")).Result;
            if (userExists > 0)
            {
                return this.Json(new DataSourceResult
                {
                    Errors = "This user exists!"
                });
            }
            var createUpdateResult = await new Query().ExecuteAndGetInsId("CreateUpdateDeleteUsers @CRUDOperation,@Id,@Name,@Surname,@Username,@Password,@Salt,@RoleId,@IsApproved,@CreatedUserId,@UpdatedUserId,@CreatedDate,@UpdatedDate,@IsSubscribed", new
            {
                @CRUDOperation = user.Id.HasValue ? (int)CRUDOperation.Update : (int)CRUDOperation.Create,
                @Id = user.Id,
                @Name = user.Name,
                @Surname = user.Surname,
                @Username = user.Username,
                @Password = hashedPassword,
                @Salt = salt,
                @RoleId = user.RoleId,
                @IsApproved = user.IsApproved,
                @CreatedUserId = userId,
                @UpdatedUserId = userId,
                @CreatedDate = DateTime.Now,
                @UpdatedDate = DateTime.Now,
                @IsSubscribed = false
            });
            if (createUpdateResult == 0)
            {
                return this.Json(new DataSourceResult
                {
                    Errors = "Error occurred! "
                });
            }
            else if (createUpdateResult > 0)
            {
                HasAffected = true;
                var afterLogData = (await new Query().SelectSingle<UserLog>($"Select * From User Where Id={createUpdateResult}")).Result;
                var serializedObject = new ChangeLogHelper().SerializeObject(null, afterLogData, (int)ChangeLogTable.Users, userId, (int)ChangeLogAction.Inserte);
                var addLog = new ChangeLogHelper().AddLog(serializedObject);
            }
            return Json(HasAffected);
        }

        public async Task<ActionResult> Update_Users([DataSourceRequest] DataSourceRequest request, User user)
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            var beforeLogData = (await new Query().SelectSingle<UserLog>($"Select * from User where Id={user.Id}")).Result;
            string hashedPassword = null, salt = null;
            if (user.Password != null)
            {
                var password = new PasswordHelper(user.Password);
                hashedPassword = password.Hash;
                salt = password.Salt;
            }
            var createUpdateResult = await new Query().Execute("CreateUpdateDeleteUsers @CRUDOperation,@Id,@Name,@Surname,@Username,@Password,@Salt,@RoleId,@IsApproved,@CreatedUserId,@UpdatedUserId,@CreatedDate,@UpdatedDate,@IsSubscribed", new
            {
                @CRUDOperation = user.Id.HasValue ? (int)CRUDOperation.Update : (int)CRUDOperation.Create,
                @Id = user.Id,
                @Name = user.Name,
                @Surname = user.Surname,
                @Username = user.Username,
                @Password = hashedPassword,
                @Salt = salt,
                @RoleId = user.RoleId,
                @IsApproved = user.IsApproved,
                @CreatedUserId = userId,
                @UpdatedUserId = userId,
                @CreatedDate = DateTime.Now,
                @UpdatedDate = DateTime.Now,
                @IsSubscribed = user.IsSubscribed
            });
            if (createUpdateResult.HasError)
            {
                return this.Json(new DataSourceResult
                {
                    Errors = "Error occurred! "
                });
            }
            else
            {
                var afterLogData = (await new Query().SelectSingle<UserLog>($"Select * From User Where Id={user.Id}")).Result;
                var serializedObject = new ChangeLogHelper().SerializeObject(beforeLogData, afterLogData, (int)ChangeLogTable.Users, userId, (int)ChangeLogAction.Update);
                var addLog = new ChangeLogHelper().AddLog(serializedObject);
            }
            return Json(createUpdateResult.HasAffected);
        }

        public ActionResult Delete_Users([DataSourceRequest] DataSourceRequest request, User usr)
        {
            try
            {
                User user = _context.Users.Find(usr.Id);
                if (user == null)
                {
                    return Json("User Not Found!");
                }

                _context.Users.Remove(user);
                _context.SaveChanges();
                return Json(_context.Users.ToList());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public ActionResult Details(User usr)
        {
            User user = _context.Users.Find(usr.Id);

            return View(user);
        }

    }
}
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieStreaming.Areas.Admin.Models.ViewAuthorization;
using MovieStreaming.Custom.Helpers;
using MovieStreaming.Custom.Models.Configuration;
using MovieStreaming.Custom.Models.LogsModel;
using System;
using System.Threading.Tasks;
using MovieStreaming.Custom.DatabaseHelpers;
using MovieStreaming.Areas.Admin.Models.Role;
using MovieStreaming.Areas.Admin.Models.ViewAuthorization;

namespace UBOResearchTool.Controllers
{
    [Authorize]
    public class ViewAuthorizationController : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.Roles = (await new Query().Select<SelectListItem>($"select Id as [Value], [Name] as [Text] from Role")).Result;
            ViewBag.Views = (await new Query().Select<SelectListItem>($"select Id as [Value], [Name] as [Text] from [View]")).Result;
            ViewBag.AuthorizationType = (await new Query().Select<SelectListItem>($"select Id as [Value], [Type] as [Text] from [AuthorizationType]")).Result;
            ViewBag.VA = true;

            return View();
        }
        public async Task<ActionResult> GetAll([DataSourceRequest] DataSourceRequest request)
        {
            var skip = (request.Page - 1) * request.PageSize;
            var query = $"Select * from GetViewAuthorizations()";
            var countQuery = $"Select count(*) from GetViewAuthorizations()";
            string filters = "";
            foreach (var filter in request.Filters)
            {
                filters += KendoGridHelper.ApplyFilter<ViewAuthorization>(filter);
            }
            string sort = KendoGridHelper.ApplySort<ViewAuthorization>(request.Sorts, "ID desc");
            string pagination = " OFFSET " + skip + " ROWS FETCH NEXT " + request.PageSize + " ROWS ONLY";
            if (filters.Trim().Length > 0)
                filters = " WHERE " + filters;
            if (sort.Trim().Length > 0)
                sort = " ORDER BY " + sort;
            var total = await new Query().SelectSingle<int>(countQuery + filters);
            var result = await new Query().Select<ViewAuthorization>(query + filters + sort + pagination);

            return Json(new { Data = result.Result, Total = total.Result });
        }
        public async Task<ActionResult> Create([DataSourceRequest] DataSourceRequest request, ViewAuthorization authorization)
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            bool HasAffected = false;
            var authorizationExists = (await new Query().SelectSingle<int>($"select Count(*) from ViewAuthorization where ViewID = {authorization.ViewID} and RoleID = {authorization.RoleID}")).Result;
            if (authorizationExists > 0)
            {
                return this.Json(new DataSourceResult
                {
                    Errors = "This authorization exists!"
                });
            }
            var createUpdateResult = await new Query().ExecuteAndGetInsId("CreateUpdateDeleteViewAuthorization @CRDUDOperation,@ID,@ViewID,@RoleID,@AuthorizationTypeID,@CreatedUserId,@UpdatedUserId ", new
            {
                @CRDUDOperation = (int)CRUDOperation.Create,
                @ID = authorization.ID,
                @ViewID = authorization.ViewID,
                @RoleID = authorization.RoleID,
                @AuthorizationTypeID = authorization.AuthorizationTypeID,
                @CreatedUserId = userId,
                @UpdatedUserId = authorization.UpdatedUserId
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
                var afterLogData = (await new Query().SelectSingle<ViewAuthLog>($"Select * From ViewAuthorization Where Id={createUpdateResult}")).Result;
                var serializedObject = new ChangeLogHelper().SerializeObject(null, afterLogData, (int)ChangeLogTable.ViewAuthorization, userId, (int)ChangeLogAction.Inserte);
                var addLog = new ChangeLogHelper().AddLog(serializedObject);
            }

            return Json(HasAffected);
        }
        public async Task<ActionResult> Update([DataSourceRequest] DataSourceRequest request, ViewAuthorization authorization)
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            var createdUserId = (await new Query().SelectSingle<int>($"select CreatedUserId from ViewAuthorization where ID = {authorization.ID}")).Result;
            var beforeLogData = (await new Query().SelectSingle<ViewAuthLog>($"Select * from ViewAuthorization where Id={authorization.ID}")).Result;
            var authorizationExists = (await new Query().SelectSingle<int>($"select Count(*) from ViewAuthorization where ViewID = {authorization.ViewID} and RoleID = {authorization.RoleID} and AuthorizationTypeID = {authorization.AuthorizationTypeID}")).Result;
            if (authorizationExists > 0)
            {
                return this.Json(new DataSourceResult
                {
                    Errors = "This authorization exists!"
                });
            }
            var authorizationExistsDifferentID = (await new Query().SelectSingle<int>($"select Count(*) from ViewAuthorization where ViewID = {authorization.ViewID} and RoleID = {authorization.RoleID} and ID <> {authorization.ID}")).Result;
            if (authorizationExistsDifferentID > 0)
            {
                return this.Json(new DataSourceResult
                {
                    Errors = "This authorization exists!"
                });
            }
            var createUpdateResult = await new Query().Execute("CreateUpdateDeleteViewAuthorization @CRDUDOperation,@ID,@ViewID,@RoleID,@AuthorizationTypeID,@CreatedUserId,@UpdatedUserId ", new
            {
                @CRDUDOperation = (int)CRUDOperation.Update,
                @ID = authorization.ID,
                @ViewID = authorization.ViewID,
                @RoleID = authorization.RoleID,
                @AuthorizationTypeID = authorization.AuthorizationTypeID,
                @CreatedUserId = createdUserId,
                @UpdatedUserId = userId
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
                var afterLogData = (await new Query().SelectSingle<ViewAuthLog>($"Select * From ViewAuthorization Where Id={authorization.ID}")).Result;
                var serializedObject = new ChangeLogHelper().SerializeObject(beforeLogData, afterLogData, (int)ChangeLogTable.ViewAuthorization, userId, (int)ChangeLogAction.Update);
                var addLog = new ChangeLogHelper().AddLog(serializedObject);
            }

            return Json(createUpdateResult.HasAffected);
        }
        public async Task<ActionResult> Delete([DataSourceRequest] DataSourceRequest request, ViewAuthorization authorization)
        {
            var userId = new AuthorizeHelper(HttpContext).GetUserID();
            var viewAuthBeforeDelete = (await new Query().SelectSingle<ViewAuthLog>($"Select * From ViewAuthorization Where Id={authorization.ID}")).Result;
            var deleteResult = await new Query().Execute("CreateUpdateDeleteViewAuthorization @CRDUDOperation,@ID", new
            {
                @CRDUDOperation = (int)CRUDOperation.Delete,
                @ID = authorization.ID
            });
            if (deleteResult.HasError)
            {
                return this.Json(new DataSourceResult
                {
                    Errors = "Error occurred! "
                });
            }
            if (deleteResult.HasAffected)
            {
                var serializedObject = new ChangeLogHelper().SerializeObject(viewAuthBeforeDelete, null, (int)ChangeLogTable.ViewAuthorization, userId, (int)ChangeLogAction.Delete);
                var addLog = new ChangeLogHelper().AddLog(serializedObject);
            }

            return Json(deleteResult.HasAffected);
        }
    }
}

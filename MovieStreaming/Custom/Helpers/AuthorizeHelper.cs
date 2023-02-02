using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MovieStreaming.Custom.DatabaseHelpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using MovieStreaming.Custom.Models.User;

namespace MovieStreaming.Custom.Helpers
{
    public class AuthorizeHelper
    {
        private const string IDUser = "IDUser";
        private const string IDOrganization = "IDOrganization";
        private const string ViewAuthorization = "ViewAuthorization";
        private const string Language = "Language";
        private const string ImagePath = "ImagePath";
        private const string TeamId = "TeamId";

        private HttpContext httpContext;

        public AuthorizeHelper(HttpContext httpContext) => this.httpContext = httpContext;

        public async Task SetAuthentication(User user, bool isPersistent)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Name, String.Join(' ',user.Name,user.Surname)),
                new Claim(IDUser, user.Id.ToString()),

                new Claim("Roli", user.RoleId.ToString()),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()),
                //new Claim(Language, language),
                //new Claim(ImagePath, imagePath),
                //new Claim(ViewAuthorization,JsonConvert.SerializeObject(user.ViewAuthorization, Newtonsoft.Json.Formatting.None))
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                ExpiresUtc = DateTime.Now.AddYears(1),
                AllowRefresh = true
            };

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        //public dynamic GetViewAuthorization()
        //{
        //    return httpContext.User.FindFirst(ViewAuthorization)?.Value;
        //}
        //public Dictionary<int, int> GetViewAuthorization()
        //{
        //    return JsonConvert.DeserializeObject<Dictionary<int, int>>(httpContext.User.FindFirst(ViewAuthorization)?.Value);
        //}
        public int GetUserOrganization()
        {
            return int.Parse(httpContext.User.FindFirst(IDOrganization)?.Value);
        }
        public int GetUserRole()
        {
            return int.Parse(httpContext.User.FindFirst(ClaimTypes.Role)?.Value);
        }
        public int GetUserID()
        {
            return int.Parse(httpContext.User.FindFirst(IDUser)?.Value);
        }
        public int GetOrganisationID()
        {
            return int.Parse(httpContext.User.FindFirst(IDOrganization)?.Value);
        }
        public string GetUserLanguage()
        {
            return (httpContext.User.FindFirst(Language)?.Value.ToString() == null ? "sq-AL" : httpContext.User.FindFirst(Language)?.Value.ToString());
        }
        public string GetUserImagePath()
        {
            return httpContext.User.FindFirst(ImagePath)?.Value.ToString();
        }

        //public async Task<Notification> getLatestRequests()
        //{
        //    Notification notification = new Notification();
        //    var userRole = new AuthorizeHelper(httpContext).GetUserRole();
        //    var userOrganisation = new AuthorizeHelper(httpContext).GetUserOrganization();
        //    notification.UserRole= (await new Query().SelectSingle<string>($"select Name  from Role where Id={userRole}")).Result;

        //    if (userRole == (int)RoleList.Admin)
        //    {
        //        notification.Requests = (await new Query().Select<Requests>($"Select top(3) Name +' '+ Surname as NameSurname,CreatedDate,1 as TypeId from Users where IsApproved is NULL and RoleId={(int)RoleList.View}")).Result.ToList();
        //        notification.RequestsNo = (await new Query().SelectSingle<int>($"Select count(*) from Users where IsApproved is NULL and RoleId={(int)RoleList.View}")).Result;

        //        var questionRequest = (await new Query().Select<Requests>($"Select top(3) u.Name +' '+ u.Surname as NameSurname,rq.CreatedDate,2 as TypeId from RequestQuestion rq inner join  Users u on rq.CreatedUserId=u.Id where rq.IsApproved is NULL")).Result.ToList();
        //        var questionRequestCount = (await new Query().SelectSingle<int>($"Select count(*) from RequestQuestion where IsApproved is NULL")).Result;

        //        notification.Requests.AddRange(questionRequest);
        //        notification.RequestsNo += questionRequestCount;
        //        return notification;
        //    }
        //    else if (userRole == (int)RoleList.View)
        //    {
        //        notification.Requests = (await new Query().Select<Requests>($"Select top(3) Name +' '+ Surname as NameSurname,CreatedDate from Participants where IsApproved is NULL and OrganisationId={userOrganisation}")).Result.ToList();
        //        notification.RequestsNo = (await new Query().SelectSingle<int>($"Select count(*) from Participants where IsApproved is NULL and OrganisationId={userOrganisation}")).Result;

        //        return notification;
        //    }
        //    return new Notification();
        //}

        //public async Task<Models.Role.RoleAccess> GetViewAuthorization(int ViewId)
        //{

        //    return (await new Query().SelectSingle<Models.Role.RoleAccess>($"Select * From GetSingleViewAccess({GetUserRole()},{ViewId})")).Result;

        //}

        //public async Task<List<Models.Role.RoleAccess>> GetViewsAuthorization()
        //{

        //    return (await new Query().Select<Models.Role.RoleAccess>($"Select * From GetViewAccess({GetUserRole()})")).Result.ToList();

        //}
    }
}

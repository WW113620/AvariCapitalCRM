
using AvariCapitalCRM.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AvariCapitalCRM.Web.Attribute
{
    public class AdminLoginAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var isAuthorized = false;
            if (httpContext != null && httpContext.Request.Cookies[LoginHelper.LoginCookieUID] != null
               && httpContext.Request.Cookies[LoginHelper.LoginCookieUserName] != null
               && httpContext.Request.Cookies[LoginHelper.LoginCookieUserType] != null)
            {
                var UserId = httpContext.Request.Cookies[LoginHelper.LoginCookieUID].Value.ToLong(0);
                var UserType = httpContext.Request.Cookies[LoginHelper.LoginCookieUserType].Value.ToInt(0);
                if (UserId > 0 && (UserType == (int)RoleEnums.超级管理员 || UserType == (int)RoleEnums.管理员))
                    isAuthorized = true;
            }

            return isAuthorized;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            string path = filterContext.HttpContext.Request.Url.PathAndQuery;
            var routeValue = new RouteValueDictionary {
                        { "Area", "Admin"},
                        { "Controller", "Account"},
                        { "Action", "Login"}
                    };
            filterContext.Result = new RedirectToRouteResult(routeValue);
        }
    }
}
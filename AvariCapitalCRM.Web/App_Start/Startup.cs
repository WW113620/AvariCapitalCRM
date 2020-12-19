using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Helpers;

namespace AvariCapitalCRM.Web
{
    public partial class Startup
    {
        public static int ExpireHours = 8;
       
        public void ConfigureAuth(IAppBuilder app)
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                CookieName = ConstValue.JuCheapAuthorizeCookieName,
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Login/Index"),
                ExpireTimeSpan = TimeSpan.FromHours(ExpireHours)//8小时cookie过期(默认是14天)
            });
        }
    }


     /// <summary>
    /// IIdentity扩展
    /// </summary>
    public static class IdentityExtention
    {
        /// <summary>
        /// 获取登录的用户ID
        /// </summary>
        /// <param name="identity">IIdentity</param>
        /// <returns></returns>
        public static long GetLoginUserId(this IIdentity identity)
        {
            if (identity != null)
            {
                var claim = (identity as ClaimsIdentity).FindFirst(ConstValue.LoginUserIdKey);
                if (claim != null)
                    return claim.Value.ToLong(0);
            }
            return 0;
        }

        public static int GetLoginRoleType(this IIdentity identity)
        {
            if (identity != null)
            {
                var claim = (identity as ClaimsIdentity).FindFirst(ConstValue.LoginUserRoleType);
                if (claim != null)
                    return claim.Value.ToInt(0);
            }
            return 0;
        }


    }


}
using AvariCapitalCRM.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvariCapitalCRM.Web.Models
{
    public class CommonUtils
    {
        public static void ResetCookieNickName(string nickName)
        {
            if (string.IsNullOrEmpty(nickName))
                CookieHelper.DeleteCookie(LoginHelper.LoginCookieNickName);
            else
            {
                nickName = HttpUtility.UrlEncode(nickName);
                CookieHelper.SetCookie(LoginHelper.LoginCookieNickName, nickName, DateTime.Now.AddHours(Startup.ExpireHours));
            }
        }



    }

}
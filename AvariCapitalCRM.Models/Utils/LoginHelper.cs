using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AvariCapitalCRM.Models.Utils
{
    public class LoginHelper
    {
        public static string LoginPrefix = "ACM";
        public static string LoginCookieUID { get { return $"{LoginPrefix}_Login_UserId"; } }
        public static string LoginCookieUserName { get { return $"{LoginPrefix}_Login_UserName"; } }
        public static string LoginCookieNickName { get { return $"{LoginPrefix}_Login_NickName"; } }
        public static string LoginCookieUserType { get { return $"{LoginPrefix}_Login_UserType"; } }

        public static string UserID
        {
            get { return CookieHelper.GetCookie(LoginHelper.LoginCookieUID); }
        }
        public static string UserName
        {
            get { return CookieHelper.GetCookie(LoginHelper.LoginCookieUserName); }
        }

        public static string NickName
        {
            get
            {
                string nikeName = CookieHelper.GetCookie(LoginHelper.LoginCookieNickName);
                if (string.IsNullOrEmpty(nikeName))
                    return "";
                nikeName = HttpUtility.UrlDecode(nikeName);
                return System.Web.HttpUtility.UrlDecode(nikeName);
            }
        }

        public static int UserType
        {
            get { return CookieHelper.GetCookie(LoginHelper.LoginCookieUserType).ToInt(0); }
        }
        public static string EncryptUserType
        {
            get { return CookieHelper.GetCookie(LoginHelper.LoginCookieUserType); }
        }

        public static string LoginName
        {
            get { return $"{Enum.GetName(typeof(RoleEnums), LoginHelper.UserType)}:{LoginHelper.UserName}"; }
        }


        public static void SetLoginCookie(ViewModels.UserViewModel user, int minute = 120)
        {
            DateTime expireTime = DateTime.Now.AddMinutes(minute);
            CookieHelper.SetCookie(LoginHelper.LoginCookieUID, user.UserId.ToString(), expireTime);
            CookieHelper.SetCookie(LoginHelper.LoginCookieUserName, user.UserName, expireTime);
            CookieHelper.SetCookie(LoginHelper.LoginCookieUserType, user.RoleType.ToString(), expireTime);
        }
        public static void SetLoginEncryptCookie(ViewModels.UserStringViewModel user, string encryKey, int hour = 2)
        {
            string encryUserId = SecureHelper.AESEncrypt(user.UserId.ToString(), encryKey);
            string encryUserName = SecureHelper.AESEncrypt(user.UserName, encryKey);
            string encryUserType = SecureHelper.AESEncrypt(user.RoleType.ToString(), encryKey);
            DateTime expireTime = DateTime.Now.AddHours(hour);
            CookieHelper.SetCookie(LoginHelper.LoginCookieUID, encryUserId, expireTime);
            CookieHelper.SetCookie(LoginHelper.LoginCookieUserName, encryUserName, expireTime);
            CookieHelper.SetCookie(LoginHelper.LoginCookieUserType, encryUserType, expireTime);
        }

        public static void Logout()
        {
            CookieHelper.DeleteCookie(LoginHelper.LoginCookieUID);
            CookieHelper.DeleteCookie(LoginHelper.LoginCookieUserName);
            CookieHelper.DeleteCookie(LoginHelper.LoginCookieUserType);

            if (System.Web.HttpContext.Current.Session != null)
            {
                System.Web.HttpContext.Current.Session.Clear();
            }
        }
    }

}

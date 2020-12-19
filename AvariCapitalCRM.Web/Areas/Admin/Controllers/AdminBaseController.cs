using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvariCapitalCRM.Models.Utils;

namespace AvariCapitalCRM.Web.Areas.Admin.Controllers
{
    public class AdminBaseController : Controller
    {

        //加密秘钥
        public static string AdminLoginEncryKey = "bc5ecb285f3b393b5cc05005efd5a1ee";

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var actionName = filterContext.ActionDescriptor.ActionName;
            if (controllerName.ToLower().Equals("account"))
            {

            }
            else
            {
                if (String.IsNullOrEmpty(this.AdminUserName))
                {
                    //重定向至登录页面
                    // filterContext.Result = RedirectToAction("Login", "Account", new { url = Request.RawUrl });
                    filterContext.Result = RedirectToAction("Login", "Account");
                    return;
                }
            }

        }

        /// <summary>
        /// 获取用户userid
        /// </summary>
        public string AdminUserId
        {
            get
            {
                string encryptUserID = HttpUtility.UrlDecode(AvariCapitalCRM.Models.Utils.LoginHelper.UserID);
                if (!string.IsNullOrWhiteSpace(encryptUserID))
                {
                    try
                    {
                        //解密
                        encryptUserID = SecureHelper.AESDecrypt(encryptUserID, AdminLoginEncryKey);
                        return encryptUserID;
                    }
                    catch
                    {

                    }
                }
                return "";
            }
        }

        /// <summary>
        /// 获取Cookie中的UserName
        /// </summary>
        public string AdminUserName
        {
            get
            {
                string strLogin = HttpUtility.UrlDecode(LoginHelper.UserName);
                if (!string.IsNullOrWhiteSpace(strLogin))
                {
                    try
                    {
                        //解密
                        strLogin = SecureHelper.AESDecrypt(strLogin, AdminLoginEncryKey);
                        return strLogin;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return "";
            }
        }



        /// <summary>
        /// 获取用户角色
        /// </summary>
        public int AdminUserType
        {
            get
            {
                string roleIdEncrypt = HttpUtility.UrlDecode(LoginHelper.EncryptUserType + "");
                if (!string.IsNullOrWhiteSpace(roleIdEncrypt))
                {
                    int i = -1;
                    if (int.TryParse(SecureHelper.AESDecrypt(roleIdEncrypt, AdminLoginEncryKey), out i))
                    {
                        return i;
                    }
                }
                return -1;
            }
        }
    }
}
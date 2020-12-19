using AvariCapitalCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AvariCapitalCRM.Web.Controllers
{
    public class BaseController : Controller
    {

        /// <summary>
        /// 是否登录
        /// </summary>
        protected bool IsLogined { get; set; }

        /// <summary>
        /// 当前登录用户
        /// </summary>
        protected CurrentUserModel CurrentUser { get; private set; }


        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (User.Identity.IsAuthenticated)
            {
                CurrentUser = new CurrentUserModel
                {
                    UserId = User.Identity.GetLoginUserId(),
                    LoginName = User.Identity.Name,
                    LoginType = User.Identity.GetLoginRoleType()
                };
            }

            IsLogined = CurrentUser != null;
        }

        public ActionResult ToError(string msg)
        {
            return RedirectToAction("Error", "Home", new { msg = HttpUtility.UrlEncode(msg) });
        }

        /// <summary>
        /// 获取当前站点域名
        /// </summary>
        /// <returns></returns>
        public string GetSiteUrl()
        {
            string fullUrl = Request.Url.AbsoluteUri;
            string querystring = Request.Url.PathAndQuery;
            string url = fullUrl.Replace(querystring, "");
            return url;
        }

    }

}
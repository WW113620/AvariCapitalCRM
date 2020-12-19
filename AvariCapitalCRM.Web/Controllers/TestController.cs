using log4net;
using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using AvariCapitalCRM.Service.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvariCapitalCRM.Models.Data;
using Qiniu.Util;
using Qiniu.IO;
using Qiniu.Http;
using Qiniu.Common;
using AvariCapitalCRM.Logic;
using Newtonsoft.Json;

namespace AvariCapitalCRM.Web.Controllers
{
    public class TestController : BaseController
    {
        private static ILog logger = LogManager.GetLogger(typeof(HomeController));
        private readonly DataContextEntities _dataContext;
        private readonly RedisHelper _redisHelper;
        private readonly IAccountService _accountService;
        public TestController(DataContextEntities dataContext, RedisHelper redisHelper, IAccountService accountService)
        {
            this._dataContext = dataContext;
            this._redisHelper = redisHelper;
            this._accountService = accountService;
        }

        public ActionResult Index()
        {
            var model = this._accountService.Get(1);

            this._redisHelper.StringSet("test", "哈哈哈哈哈");
            string test = this._redisHelper.StringGet("test");
            return Content("Test");
        }



        [AllowAnonymous]
        public ActionResult NoLogin()
        {
            return Content("无需登录");
        }

        [Authorize]
        public ActionResult TestLogin()
        {
            string userName = CurrentUser.LoginName;
            string username = User.Identity.Name;

            int roleType = CurrentUser.LoginType;// User.Identity.GetLoginRoleType();
            return Content("登录访问");
        }

        // [NonAction]
        public ActionResult Download()
        {
            try
            {
                string AK = ConfigHelper.GetConfigValue("AK");
                string SK = ConfigHelper.GetConfigValue("SK");
                string BtkDomain = ConfigHelper.GetConfigValue("BtkDomain");

                Mac mac = new Mac(AK, SK);
                string rawUrl = "http://qiniusouth.shadou98.com/" + "舞蹈静态之美.zip";
                string saveFile = "D:\\测试七牛云\\舞蹈静态之美2.zip";


                Config.SetZone(ZoneID.US_North, false);
                int expireInSeconds = 3600;
                string accUrl = DownloadManager.CreateSignedUrl(mac, rawUrl, expireInSeconds);
                HttpResult result = DownloadManager.Download(accUrl, saveFile);
            }
            catch (Exception e)
            {
                string mes = e.Message;
            }

            return new EmptyResult();
        }

        public ActionResult TestEmail(string mail)
        {
            if (string.IsNullOrEmpty(mail))
                return Content("邮箱测试");

            var registerUrl = HttpContext.Server.MapPath("~/Templete/EmailTemplate.html");
            var emailTemplete = new EmailTempleteClass
            {
                Title = "Avari Capital Partners New User Account",
                Email = mail,
                Password = "123456",
                LoginUrl = ConfigHelper.GetConfigValue("WebDomainLoginAction").TrimEnd('/')
            };
            var htmlContent = EmailTempleteHelper.ConverToHtml(registerUrl, emailTemplete);
            var sendResult = SendEmailLogic.Send(mail, emailTemplete.Title, htmlContent, true);
            return Content(JsonConvert.SerializeObject(sendResult));
        }



    }
}
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using AvariCapitalCRM.Logic;
using AvariCapitalCRM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AvariCapitalCRM.Models.Data;

namespace AvariCapitalCRM.Web.Controllers
{
    public class LoginController : Controller
    {
        private IAuthenticationManager authenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private static ILog logger = LogManager.GetLogger(typeof(LoginController));
        private readonly AccountLogic _accountLogic;
        private readonly DataContextEntities _dataContext;
        public LoginController(DataContextEntities dataContext, AccountLogic accountLogic)
        {
            this._dataContext = dataContext;
            this._accountLogic = accountLogic;
        }

        #region 登录
        [AllowAnonymous]
        public ActionResult Index(string ReturnUrl, int p = 0)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = ReturnUrl;
            ViewBag.Tab = p;
            return View();
        }

        [HttpPost, AllowAnonymous]
        public JsonResult Login(LoginRequest model)
        {
            UserViewModel userView = new UserViewModel();
            BaseResult result = this._accountLogic.WebLogin(model, ref userView);
            if (result.code != 0)
                return Json(result);

            LoginClaimsIdentity(userView);
            return Json(new BaseResult(0, "登录成功"));
        }

        private void LoginClaimsIdentity(UserViewModel model)
        {

            CommonUtils.ResetCookieNickName(model.NickName);

            var claims = new List<Claim>
                {
                    new Claim(ConstValue.LoginUserIdKey,model.UserId.ToString()),
                    new Claim(ClaimTypes.Name, model.UserName.ToString()),
                    new Claim(ConstValue.LoginUserRoleType, model.RoleType.ToString()),
                };
            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            var pro = new AuthenticationProperties() { IsPersistent = true };
            authenticationManager.SignIn(pro, identity);
        }

        #endregion

        #region 发送短信验证码
        [HttpPost, AllowAnonymous]
        public JsonResult VerifyCode(string phone, int prefix)
        {
            if (string.IsNullOrEmpty(phone))
                return Json(new BaseResult(1, "手机号不能为空"));

            phone = phone.Trim();
            PhoneTypeEnums phoneType = (PhoneTypeEnums)prefix;
            ValidPhoneModel model = this._accountLogic.ValidPhone(phoneType, phone);
            if (!model.IsPhone)
                return Json(new BaseResult(1, "请输入正确的手机号"));

            string code = CaptchaHelper.CreateCode(6);
            string mes = this._accountLogic.SendMessage(code, phone, phoneType);
            if (mes != "发送成功")
                return Json(new BaseResult(1, mes));

            Session["VerifyCode"] = code;
            return Json(new BaseResult(0, code));
        }
        #endregion

        #region 退出
        public ActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
                CookieHelper.DeleteCookie(ConstValue.JuCheapAuthorizeCookieName);
            }
            return RedirectToAction("Index");
        }
        #endregion

    }
}
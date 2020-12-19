
using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using AvariCapitalCRM.Logic;
using AvariCapitalCRM.Service.Accounts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvariCapitalCRM.Models.Data;

namespace AvariCapitalCRM.Web.Areas.Admin.Controllers
{
    public class AccountController : AdminBaseController
    {
        private readonly DataContextEntities _dataContext;
        private readonly AccountLogic _accountLogic;
        private readonly IAccountService _accountService;
        public AccountController(DataContextEntities dataContext, AccountLogic accountLogic, IAccountService accountService)
        {
            this._dataContext = dataContext;
            this._accountLogic = accountLogic;
            this._accountService = accountService;
        }

        #region 登录
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            if (String.IsNullOrEmpty(base.AdminUserName))
                return View();
            else
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult Verify(String UserName, String PassWord, String CaptchCode)
        {

            if (string.IsNullOrEmpty(UserName))
                return Json(new BaseResult(1, "Please enter your user name."));
            if (string.IsNullOrEmpty(PassWord))
                return Json(new BaseResult(1, "Please enter your password."));
            if (string.IsNullOrEmpty(CaptchCode))
                return Json(new BaseResult(1, "Please enter verification code."));

            if (Session["captcha"] == null)
            {
                return Json(new BaseResult(1, "Internal error please contact administrator"));
            }
            String sessionStr = Session["captcha"].ToString();
            if (!CaptchCode.ToLower().Equals(sessionStr.ToLower()))
            {
                return Json(new BaseResult(1, "Verification code error"));
            }

            var user = this._dataContext.Accounts.FirstOrDefault(p => p.UserName == UserName);
            if (user == null)
                return Json(new BaseResult(1, "The user does not exist"));

            if (user.State == (int)UserStatusEnums.待审核)
                return Json(new BaseResult(1, "The user was not approved"));
            if (user.State == (int)UserStatusEnums.已锁定)
                return Json(new BaseResult(1, "The user is locked"));

            var role = this._dataContext.Roles.Find(user.RoleId);
            if (user == null)
                return Json(new BaseResult(1, "Invalid user identity"));

            string encryptedWithSaltPassword = CommonHelper.GetPasswrodWithTwiceEncode(PassWord, user.PasswordSalt);
            if (user.Password != encryptedWithSaltPassword)
                return Json(new BaseResult(1, "Incorrect password"));
            UserStringViewModel userView = new UserStringViewModel
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                RoleType = user.RoleType.ToString()
            };
            LoginHelper.SetLoginEncryptCookie(userView, AdminLoginEncryKey, 8);
            return Json(new BaseResult(0, "OK"));
        }

        #endregion

        #region 获取验证码
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public void GetCaptcha()
        {
            string code = CaptchaHelper.Create(4);
            Session.Remove("captcha");
            Session["captcha"] = code;
            Bitmap bitmap = new Bitmap((int)Math.Ceiling(code.Length * 20.0), 30);
            // 将位图背景填充为白色
            System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(bitmap);
            CaptchaHelper.Create(bitmap, code, ref graph, 0);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            Response.ClearContent();
            Response.ContentType = "image/gif";
            Response.BinaryWrite(ms.ToArray());
        }
        #endregion

        #region 退出
        [HttpGet]
        public ActionResult Logout()
        {
            LoginHelper.Logout();
            return RedirectToAction("Login");
        }
        #endregion

        #region 修改密码
        public ActionResult Password()
        {
            if (String.IsNullOrEmpty(base.AdminUserName))
                return RedirectToAction("Logout");

            var userId = base.AdminUserId.ToLong(0);
            var account = this._dataContext.Accounts.Find(userId);

            if (account == null)
                return RedirectToAction("Logout");

            return View(account);
        }

        [HttpPost]
        public JsonResult ChangePassword(AccountPwdModel model)
        {
            BaseResult result = new BaseResult();
            try
            {
                string userName = base.AdminUserName;
                if (string.IsNullOrEmpty(userName))
                    return Json(new BaseResult(2, "No login"));

                result = this._accountLogic.ChangePassword(model);
            }
            catch (Exception e)
            {
                result = new BaseResult(1, "Exception=" + e.Message);
            }
            return Json(result);
        }
        #endregion

        #region 基本信息
        public ActionResult Info()
        {
            if (String.IsNullOrEmpty(base.AdminUserName))
                return RedirectToAction("Logout");

            string userName = base.AdminUserName;
            var user = this._dataContext.Accounts.FirstOrDefault(p => p.UserName == userName);
            if (user == null)
                return RedirectToAction("Logout");

            return View(user);
        }

        [HttpPost]
        public JsonResult Save(Account model)
        {
            BaseResult result = new BaseResult();
            try
            {
                var account = this._dataContext.Accounts.FirstOrDefault(p => p.UserName == model.UserName);
                if (account == null)
                    return Json(new BaseResult(2, "The user does not exist"));

                account.FirstName = model.FirstName;
                account.LastName = model.LastName;

                if (!string.IsNullOrEmpty(model.Phone))
                {
                    model.Phone = model.Phone.Trim();
                    if (model.Phone != account.Phone)
                    {
                        int count = this._dataContext.Accounts.Count(p => p.UserName == model.Phone);
                        if (count > 0)
                            return Json(new BaseResult(1, "The phone number has been bound"));

                        count = this._dataContext.Accounts.Count(p => p.Phone == model.Phone);
                        if (count > 0)
                            return Json(new BaseResult(1, "The phone number has been bound"));

                        account.Phone = model.Phone;
                    }
                }
                else
                {
                    account.Phone = "";
                }

                if (account.RoleType == (int)RoleEnums.顾问)
                {
                    account.RoleId = model.RoleId;
                }
                //account.Email = model.Email;
                this._dataContext.SaveChanges();
                return Json(new BaseResult(0, "OK"));
            }
            catch (Exception e)
            {
                return Json(new BaseResult(1, "Exception=" + e.Message));
            }
        }
        #endregion

    }
}
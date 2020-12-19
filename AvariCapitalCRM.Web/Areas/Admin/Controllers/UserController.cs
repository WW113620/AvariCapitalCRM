
using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using AvariCapitalCRM.Logic;
using AvariCapitalCRM.Service.Accounts;
using AvariCapitalCRM.Service.BaseCommon;
using AvariCapitalCRM.Web.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvariCapitalCRM.Models.Data;

namespace AvariCapitalCRM.Web.Areas.Admin.Controllers
{
    //[AdminLogin]
    public class UserController : AdminBaseController
    {

        private readonly DataContextEntities _dataContext;
        private readonly AccountLogic _accountLogic;
        private readonly IAccountService _accountService;
        private readonly ICommonService _commonService;
        public UserController(DataContextEntities dataContext, AccountLogic accountLogic, IAccountService accountService, ICommonService commonService)
        {
            this._dataContext = dataContext;
            this._accountLogic = accountLogic;
            this._accountService = accountService;
            this._commonService = commonService;
        }

        #region 客户
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            List<SelectOption> UserStatusList = EnumHelper.EnumToList<UserStatusEnums>();
            ViewBag.UserStatusList = UserStatusList;
            return View();
        }

        [HttpGet]
        public JsonResult GetList(AccountRequest request)
        {
            LayuiPageResult<AccountViewModel> result = new LayuiPageResult<AccountViewModel>() { code = 1 };
            try
            {
                string userName = base.AdminUserName;
                RoleEnums loginType = (RoleEnums)base.AdminUserType;
                request.RoleType = ((int)RoleEnums.客户).ToString();
                var response = this._accountService.GetUserList(request, userName, loginType);
                result = new LayuiPageResult<AccountViewModel>() { code = response.code, msg = response.msg, count = response.page.count, data = response.data };
            }
            catch (Exception e)
            {
                result.msg = e.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Add(long? id, int roleType = (int)RoleEnums.客户)
        {
            List<long> accessIdList = new List<long>();
            Account account = new Account() { Id = 0, RoleType = roleType };
            long Id = id.ToLong(0);
            if (Id > 0)
            {
                account = _dataContext.Accounts.Find(id);
                if (account != null)
                {
                    if (!string.IsNullOrEmpty(account.AccessIds))
                    {
                        List<string> list = account.AccessIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var item in list)
                        {
                            accessIdList.Add(item.ToLong(0));
                        }
                    }
                }
                else
                {
                    account.Id = 0;
                }
            }

            int role = (int)RoleEnums.顾问;
            ViewBag.AdviserList = this._dataContext.Accounts.Where(p => p.RoleType == role).ToList();

            ViewBag.AccessIdList = accessIdList;
            ViewBag.Categories = this._dataContext.Categories.Where(p => p.Level == 1).ToList();

            ViewBag.AdminUserType = base.AdminUserType;

            return View(account);
        }

        [HttpPost]
        public JsonResult Save(Account account)
        {
            if (account.Id > 0)
            {
                var model = this._dataContext.Accounts.Find(account.Id);
                if (model == null)
                    return Json(new BaseResult(1, "The user does not exist"));
                if (model.Email != account.Email)
                {
                    int count = this._dataContext.Accounts.Count(p => p.UserName == account.Email);
                    if (count > 0)
                        return Json(new BaseResult(1, "The email already exists"));
                    count = this._dataContext.Accounts.Count(p => p.Email == account.Email);
                    if (count > 0)
                        return Json(new BaseResult(1, "The email already exists"));
                }

                if (!string.IsNullOrEmpty(account.Phone))
                {
                    account.Phone = account.Phone.Trim();
                    if (model.Phone != account.Phone)
                    {
                        int count = this._dataContext.Accounts.Count(p => p.Phone == account.Phone);
                        if (count > 0)
                            return Json(new BaseResult(1, "Phone number already exists"));
                    }
                }

                model.UserName = account.Email;
                model.Email = account.Email;
                model.FirstName = account.FirstName;
                model.LastName = account.LastName;
                model.Phone = account.Phone;
                model.AccessIds = account.AccessIds;
                model.RoleType = account.RoleType;
                model.RoleId = account.RoleId;
                this._dataContext.SaveChanges();
                return Json(new BaseResult(0, "OK"));
            }
            else
            {
                int count = this._dataContext.Accounts.Count(p => p.UserName == account.Email);
                if (count > 0)
                    return Json(new BaseResult(1, "The email already exists"));
                count = this._dataContext.Accounts.Count(p => p.Email == account.Email);
                if (count > 0)
                    return Json(new BaseResult(1, "The email already exists"));

                if (!string.IsNullOrEmpty(account.Phone))
                {
                    account.Phone = account.Phone.Trim();
                    count = this._dataContext.Accounts.Count(p => p.Phone == account.Phone);
                    if (count > 0)
                        return Json(new BaseResult(1, "Phone number already exists"));
                }
                string password = account.Password;
                var salt = Guid.NewGuid().ToString("N").Substring(12);
                string encryptedWithSaltPassword = CommonHelper.GetPasswrodWithTwiceEncode(account.Password, salt);
                account.Password = encryptedWithSaltPassword;
                account.PasswordSalt = salt;
                account.UserName = account.Email;
                account.State = (int)UserStatusEnums.已通过;
                account.RoleType = account.RoleType;
                account.AddTime = DateTime.Now;
                this._dataContext.Accounts.Add(account);
                this._dataContext.SaveChanges();

                var registerUrl = HttpContext.Server.MapPath("~/Templete/EmailTemplate.html");
                var emailTemplete = new EmailTempleteClass
                {
                    Title = "Avari Capital Partners New User Account",
                    Email = account.Email,
                    Password = password,
                    LoginUrl = ConfigHelper.GetConfigValue("WebDomainLoginAction").TrimEnd('/')
                };
                var htmlContent = EmailTempleteHelper.ConverToHtml(registerUrl, emailTemplete);
                var sendResult = SendEmailLogic.Send(account.Email, emailTemplete.Title, htmlContent, true);

                return Json(new BaseResult(0, "OK"));
            }
        }

        [HttpPost]
        public JsonResult CheckState(long id, int state)
        {
            var account = this._dataContext.Accounts.Find(id);
            if (account == null)
                return Json(new BaseResult(1, "This user does not exist"));
            account.State = state;
            this._dataContext.SaveChanges();
            return Json(new BaseResult(0, "OK"));
        }

        [HttpPost]
        public JsonResult Exist(string text)
        {
            string value = text.Trim();
            int count = this._dataContext.Accounts.Count(p => p.UserName == value);
            if (count > 0)
                return Json(new { code = 0 });

            return Json(new { code = 1 });
        }

        [HttpPost]
        public JsonResult Delete(long id)
        {
            var model = this._dataContext.Accounts.Find(id);
            if (model != null)
            {
                this._dataContext.Accounts.Remove(model);
                this._dataContext.SaveChanges();
            }
            return Json(new BaseResult(0, "OK"));
        }

        #endregion

        #region 顾问
        public ActionResult AdviserList()
        {
            List<SelectOption> UserStatusList = EnumHelper.EnumToList<UserStatusEnums>();
            ViewBag.UserStatusList = UserStatusList;
            return View();
        }

        [HttpGet]
        public JsonResult GetAdviserList(AccountRequest request)
        {
            LayuiPageResult<AccountViewModel> result = new LayuiPageResult<AccountViewModel>() { code = 1 };
            try
            {
                string userName = base.AdminUserName;
                RoleEnums loginType = (RoleEnums)base.AdminUserType;
                request.RoleType = ((int)RoleEnums.顾问).ToString();
                var response = this._accountService.GetUserList(request, userName, loginType);
                result = new LayuiPageResult<AccountViewModel>() { code = response.code, msg = response.msg, count = response.page.count, data = response.data };
            }
            catch (Exception e)
            {
                result.msg = e.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 管理员
        public ActionResult AdminList()
        {
            if (base.AdminUserType != (int)RoleEnums.超级管理员 && base.AdminUserType != (int)RoleEnums.管理员)
                return Content("You do not have access to this page");

            ViewBag.UserType = base.AdminUserType;
            return View();
        }

        [HttpGet]
        public JsonResult GetAdminList(AdminRequest request)
        {
            LayuiPageResult<AccountViewModel> result = new LayuiPageResult<AccountViewModel>() { code = 1 };
            try
            {
                string userName = base.AdminUserName;
                RoleEnums loginType = (RoleEnums)base.AdminUserType;
                var response = this._accountService.GetAdminList(request, userName, loginType);
                result = new LayuiPageResult<AccountViewModel>() { code = response.code, msg = response.msg, count = response.page.count, data = response.data };
            }
            catch (Exception e)
            {
                result.msg = e.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddAdmin()
        {
            if (base.AdminUserType != (int)RoleEnums.超级管理员)
                return Content("You do not have access to this page");
            Account model = new Account();
            return View(model);

        }


        [HttpPost]
        public JsonResult AddSave(Account account)
        {
            account.UserName = account.UserName.Trim();
            int count = this._dataContext.Accounts.Count(p => p.UserName == account.UserName);
            if (count > 0)
                return Json(new BaseResult(1, "The user name already exists"));

            if (!string.IsNullOrEmpty(account.Phone))
            {
                account.Phone = account.Phone.Trim();
                count = this._dataContext.Accounts.Count(p => p.Phone == account.Phone);
                if (count > 0)
                    return Json(new BaseResult(1, "Phone number already exists"));

                count = this._dataContext.Accounts.Count(p => p.UserName == account.Phone);
                if (count > 0)
                    return Json(new BaseResult(1, "Phone number has registered"));
            }
            var salt = Guid.NewGuid().ToString("N").Substring(12);
            string encryptedWithSaltPassword = CommonHelper.GetPasswrodWithTwiceEncode(account.Password, salt);
            account.Password = encryptedWithSaltPassword;
            account.PasswordSalt = salt;
            account.State = (int)UserStatusEnums.已通过;
            account.RoleType = (int)RoleEnums.管理员;
            account.RoleId = 1;
            account.AddTime = DateTime.Now;
            this._dataContext.Accounts.Add(account);
            this._dataContext.SaveChanges();
            return Json(new BaseResult(0, "OK"));
        }


        public ActionResult EditAdmin(long id)
        {

            var model = this._dataContext.Accounts.Find(id);
            if (model == null)
                return Content("Parameters error");

            if (model.UserName != base.AdminUserName)
            {
                var loginUser = this._dataContext.Accounts.FirstOrDefault(u => u.UserName == AdminUserName);
                if (loginUser.RoleType != (int)RoleEnums.超级管理员 && loginUser.RoleType != (int)RoleEnums.管理员)
                    return Content("You can't edit someone else's info");
            }
            return View(model);

        }

        public ActionResult Password(long id)
        {
            var model = this._dataContext.Accounts.Find(id);
            if (model == null)
                return Content("Parameters error");

            if (model.UserName != base.AdminUserName)
            {
                var loginUser = this._dataContext.Accounts.FirstOrDefault(u => u.UserName == AdminUserName);
                if (loginUser.RoleType != (int)RoleEnums.超级管理员 && loginUser.RoleType != (int)RoleEnums.管理员)
                    return Content("You can't change someone else's password");
            }

            return View(model);

        }



        #endregion

        #region 经销商自己登陆
        public ActionResult ClientList()
        {
            if (base.AdminUserType != (int)RoleEnums.超级管理员 && base.AdminUserType != (int)RoleEnums.管理员 && base.AdminUserType != (int)RoleEnums.顾问)
                return Content("You do not have access to this page");

            List<SelectOption> UserStatusList = EnumHelper.EnumToList<UserStatusEnums>();
            ViewBag.UserStatusList = UserStatusList;
            return View();
        }

        [HttpGet]
        public JsonResult GetClientList(AccountRequest request)
        {
            LayuiPageResult<AccountViewModel> result = new LayuiPageResult<AccountViewModel>() { code = 1 };
            try
            {
                string userName = base.AdminUserName;
                RoleEnums loginType = (RoleEnums)base.AdminUserType;
                request.RoleType = ((int)RoleEnums.客户).ToString();
                var response = this._accountService.GetUserList(request, userName, loginType);
                result = new LayuiPageResult<AccountViewModel>() { code = response.code, msg = response.msg, count = response.page.count, data = response.data };
            }
            catch (Exception e)
            {
                result.msg = e.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}
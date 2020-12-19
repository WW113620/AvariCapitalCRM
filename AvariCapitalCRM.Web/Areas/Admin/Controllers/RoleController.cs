using AvariCapitalCRM.Models.Data;
using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using AvariCapitalCRM.Service.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AvariCapitalCRM.Web.Areas.Admin.Controllers
{
    public class RoleController : AdminBaseController
    {
        private readonly DataContextEntities _dataContext;
        private readonly IAccountService _accountService;
        public RoleController(DataContextEntities dataContext, IAccountService accountService)
        {
            this._dataContext = dataContext;
            this._accountService = accountService;
        }

        public ActionResult Index()
        {
            if (base.AdminUserType != (int)RoleEnums.超级管理员&& base.AdminUserType != (int)RoleEnums.管理员)
            {
                return Content("You do not have access to this page");
            }
            return View();
        }

        [HttpGet]
        public JsonResult GetList(RoleRequest request)
        {
            LayuiPageResult<RoleViewModel> result = new LayuiPageResult<RoleViewModel>() { code = 1 };
            try
            {
                var response = this._accountService.GetUserRoleList(request);
                result = new LayuiPageResult<RoleViewModel>() { code = response.code, msg = response.msg, count = response.page.count, data = response.data };
            }
            catch (Exception e)
            {
                result.msg = e.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add(long? id)
        {
            if (base.AdminUserType != (int)RoleEnums.超级管理员&& base.AdminUserType != (int)RoleEnums.管理员)
            {
                return Content("You do not have access to this page");
            }
            Role model = new Role() { Id = 0 };
            long Id = id.ToLong(0);
            if (Id > 0)
            {
                model = this._dataContext.Roles.Find(Id);
                if (model == null)
                    return Content("Parameters error");
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult Submit(Role model)
        {
            if (string.IsNullOrEmpty(model.RoleName))
                return Json(new { code = 1, msg = "The group name cannot be empty" });

            if (string.IsNullOrEmpty(model.Permissions))
                return Json(new { code = 1, msg = "The access categories cannot be empty" });

            if (model.Id > 0)
            {
                var role = this._dataContext.Roles.FirstOrDefault(p => p.Id == model.Id);
                if (role == null)
                    return Json(new { code = 1, msg = "The group does not exist" });
                model.Id = role.Id;
                role.RoleName = model.RoleName;
                role.Permissions = model.Permissions;
                role.Remark = model.Remark;
                role.IsDel = 0;
                this._dataContext.SaveChanges();
            }
            else
            {
                model.IsDel = 0;
                model.AddTime = DateTime.Now;
                this._dataContext.Roles.Add(model);
                this._dataContext.SaveChanges();
            }
            return Json(new { code = 0, materialId = model.Id });
        }


        [HttpPost]
        public JsonResult Delete(long id)
        {
            var model = this._dataContext.Roles.Find(id);
            if (model != null)
            {
                this._dataContext.Roles.Remove(model);

                var userList = this._dataContext.Accounts.Where(p => p.RoleId == id).ToList();
                if (userList != null && userList.Any())
                {
                    foreach (var item in userList)
                    {
                        item.RoleId = 0;
                    }
                }
              
                this._dataContext.SaveChanges();
            }
            return Json(new BaseResult(0, "OK"));
        }


    }
}
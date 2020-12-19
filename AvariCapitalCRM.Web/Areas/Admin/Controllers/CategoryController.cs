using AvariCapitalCRM.Models.Data;
using AvariCapitalCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AvariCapitalCRM.Web.Areas.Admin.Controllers
{
    public class CategoryController : AdminBaseController
    {

        private readonly DataContextEntities _dataContext;

        public CategoryController(DataContextEntities dataContext)
        {
            this._dataContext = dataContext;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetList()
        {
            DataResult<Category> dataResult = new DataResult<Category> { code = 1, msg = "" };
            try
            {
                dataResult.data = this._dataContext.Categories.ToList();
                dataResult.code = 0;
                dataResult.msg = "OK";
            }
            catch (Exception e)
            {
                dataResult.msg = e.Message;
            }
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Edit(long id, string name)
        {
            var model = this._dataContext.Categories.Find(id);
            if (model == null)
                return Json(new BaseResult(1, "The category does not exist or has been deleted"));

            model.Name = name;
            this._dataContext.SaveChanges();
            return Json(new BaseResult(0, "OK"));
        }

        [HttpPost]
        public JsonResult Add(string name, long parentId)
        {
            Category model = this._dataContext.Categories.Find(parentId);
            if (model == null)
            {
                model = new Category();
                model.Id = 0;
                model.Level = 0;
            }

            Category category = new Category { Name = name, ParentId = model.Id, Level = model.Level + 1 };
            this._dataContext.Categories.Add(category);
            this._dataContext.SaveChanges();
            return Json(new BaseResult(0, "OK"));
        }

        [HttpPost]
        public JsonResult Delete(long id)
        {
            var model = this._dataContext.Categories.Find(id);
            if (model != null)
            {
                this._dataContext.Categories.Remove(model);
                this._dataContext.SaveChanges();
            }
            return Json(new BaseResult(0, "OK"));
        }

    }
}
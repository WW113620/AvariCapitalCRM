using AvariCapitalCRM.Models.Data;
using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using AvariCapitalCRM.Models.ViewModels.Charts;
using AvariCapitalCRM.Service.BaseCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AvariCapitalCRM.Web.Areas.Admin.Controllers
{
    public class HomeController : AdminBaseController
    {
        private readonly DataContextEntities _dataContext;
        private readonly ICommonService _commonService;
        public HomeController(DataContextEntities dataContext, ICommonService commonService)
        {
            this._dataContext = dataContext;
            this._commonService = commonService;
        }

        /// <summary>
        /// 登录成功跳转该页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.UserName = base.AdminUserName;
            ViewBag.UserType = base.AdminUserType;
            return View();
        }

        public ActionResult Main()
        {
            int loginType = base.AdminUserType;
            ViewBag.UserType = loginType;

            StatisticsModel model = new StatisticsModel();

            //var files = this._dataContext.MediaFiles.ToList();

            var files = (from a in this._dataContext.MediaFiles
                         join b in this._dataContext.Materials on a.MaterialId equals b.Id
                         select a).ToList();
            if (loginType == (int)RoleEnums.超级管理员 || loginType == (int)RoleEnums.管理员)
            {
                model.Vidoes = files.Count(p => p.MediaType == (int)MediaTypeEnums.视频);
                model.Images = files.Count(p => p.MediaType == (int)MediaTypeEnums.图片);
                model.Documents = files.Count(p => p.MediaType == (int)MediaTypeEnums.文档);
                model.Download = this._dataContext.DownloadLogs.Count();
            }
            else if (loginType == (int)RoleEnums.客户|| loginType == (int)RoleEnums.顾问)
            {
                model.Vidoes = files.Count(p => p.MediaType == (int)MediaTypeEnums.视频);
                model.Images = files.Count(p => p.MediaType == (int)MediaTypeEnums.图片);
                model.Documents = files.Count(p => p.MediaType == (int)MediaTypeEnums.文档);
                model.Download = this._dataContext.DownloadLogs.Count(p => p.UserName == AdminUserName);

                return RedirectToAction("Index", "Materials", new { area="Admin"});
            }



            return View(model);
        }

        #region 数据统计
        [HttpGet]
        public JsonResult GetDownloadStatistics(BaseReqestParams request)
        {
            LayuiPageResult<DownloadStatisticsModel> result = new LayuiPageResult<DownloadStatisticsModel>() { code = 1 };
            try
            {
                string userName = base.AdminUserName;
                RoleEnums loginType = (RoleEnums)base.AdminUserType;
                var response = this._commonService.GetDownloadStatistics(request, userName, loginType);
                result = new LayuiPageResult<DownloadStatisticsModel>() { code = response.code, msg = response.msg, count = response.page.count, data = response.data };
            }
            catch (Exception e)
            {
                result.msg = e.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetChartData()
        {
            ChartResultModel resultModel = new ChartResultModel();
            resultModel.MediaFileLineData = GetMediaFileLineData();
            resultModel.MaterialPieData = GetMaterialPieData();
            resultModel.UserLineData = GetUserLineData();
            return Json(new { code = 0, data = resultModel });
        } 
        #endregion

        #region 折线图 1

        public EchartsData<LineStyleValue> GetMediaFileLineData()
        {
            EchartsData<LineStyleValue> result = new EchartsData<LineStyleValue>() { code = 1, series = new List<LineStyleValue>() };

            var list = this._commonService.GetMediaFileLineData();
            List<HomeChartModels> viewList = new List<HomeChartModels>();

            viewList = list;

            result = new EchartsData<LineStyleValue>()
            {
                code = 0,
                name = "Statistical analysis of files data in the latest month",
                xAxisData = viewList.Select(x => x.ShowDate).ToArray(),
                series = new List<LineStyleValue>
                    {
                        new LineStyleValue() {name="Video",itemStyle=GetItemStyle(), data=viewList.Select(y=>y.VideoCount).ToList() },
                        new LineStyleValue() {name="Picture",itemStyle=GetItemStyle(),data=viewList.Select(y=>y.PictureCount).ToList() },
                        new LineStyleValue() {name="Document",itemStyle=GetItemStyle(),data=viewList.Select(y=>y.DocumentCount).ToList() },
                    }
            };

            return result;
        }

        public ItemStyle GetItemStyle()
        {
            AreaStyle areaStyle = new AreaStyle { type = "default" };
            LineNormal normal = new LineNormal { areaStyle = areaStyle };
            ItemStyle itemStyle = new ItemStyle { normal = normal };
            return itemStyle;
        }
        #endregion

        #region 饼状汇总图
        public EchartsData<KeyValue> GetMaterialPieData()
        {
            EchartsData<KeyValue> result = new EchartsData<KeyValue>() { code = 1, series = new List<KeyValue>() };
            List<KeyValue> list = this._commonService.GetMaterialPieData();

            result = new EchartsData<KeyValue>()
            {
                code = 0,
                name = "Classified data statistics",
                xAxisData = list.Select(x => x.name).ToArray(),
                series = list
            };
            return result;
        }
        #endregion

        #region 用户折线图

        public EchartsData<LineValue> GetUserLineData()
        {
            EchartsData<LineValue> result = new EchartsData<LineValue>() { code = 1, series = new List<LineValue>() };

            var list = this._commonService.GetUserLineData();
            List<ChartModels> viewList = new List<ChartModels>();

            #region 
            DateTime end = DateTime.Now.Date;
            DateTime start = end.AddMonths(-1);
            var dateList = CommonHelper.GetDateList(start, end);

            ChartModels model = null;
            foreach (var item in dateList)
            {
                model = new ChartModels();
                var itemData = list.FirstOrDefault(p => p.ShowDate == item);
                model.ShowDate = item;
                model.UserCount = itemData != null ? itemData.UserCount : 0;
                viewList.Add(model);
            }
            #endregion
            result = new EchartsData<LineValue>()
            {
                code = 0,
                name = "Statistics of new users in the latest month",
                xAxisData = viewList.Select(x => x.ShowDate).ToArray(),
                series = new List<LineValue>
                    {
                        new LineValue() {name="Distributor",data=viewList.Select(y=>y.UserCount).ToList() }
                    }
            };

            return result;
        }
        #endregion

    }
}
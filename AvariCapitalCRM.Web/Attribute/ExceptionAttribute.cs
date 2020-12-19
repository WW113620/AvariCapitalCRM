
using AvariCapitalCRM.Models.ViewModels;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AvariCapitalCRM.Web.Attribute
{
    public class ExceptionAttribute : HandleErrorAttribute
    {
        private static ILog logger = LogManager.GetLogger(typeof(ExceptionAttribute));
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            string controllerName = filterContext.RouteData.GetRequiredString("controller");
            string actionName = filterContext.RouteData.GetRequiredString("action");

            string msg = string.Format
           ("消息类型：{0}<br>消息内容：{1}<br>引发异常的方法：{2}<br>引发异常的对象：{3}<br>堆栈:{4}<br>异常目录：{5}<br>异常文件：{6}",
               filterContext.Exception.GetType().Name,
               filterContext.Exception.Message,
               filterContext.Exception.TargetSite,
               filterContext.Exception.Source,
               filterContext.Exception.StackTrace,
               controllerName,
               actionName);

            logger.Error(msg);
            //如果是ajax或者上传文件处理返回结果
            bool isAjax = filterContext.HttpContext.Request.IsAjaxRequest();
            bool isUpload = filterContext.HttpContext.Request.Files.Count > 0;

            if (isAjax || isUpload)
            {
                //返回统一的Json错误信息
                var scriptSerializer = JsonSerializer.Create(new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                using (var sw = new StringWriter())
                {
                    scriptSerializer.Serialize(sw, new BaseResult { code = 1, msg = filterContext.Exception.Message + filterContext.Exception.StackTrace });
                    filterContext.HttpContext.Response.Write(sw.ToString());
                }
                filterContext.HttpContext.Response.End();
            }
            else
            {
                filterContext.HttpContext.Response.Redirect("/Home/Error", true);
            }
        }
    }
}
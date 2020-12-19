using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AvariCapitalCRM.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //注册 log4net
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\Configs\\log4net.config"));

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutofacConfig.Register();
            AutoMapperConfig.Register();

            System.Threading.Thread LoadServiceData = new System.Threading.Thread(new System.Threading.ThreadStart(LoadQuartzWebservice));
            LoadServiceData.Start();
        }

        private void LoadQuartzWebservice()
        {
            //定义一个定时器，并开启和配置相关属性
            System.Timers.Timer Wtimer = new System.Timers.Timer(1000 * 60);//执行任务的周期
            Wtimer.Elapsed += new System.Timers.ElapsedEventHandler(Wtimer_Elapsed);
            Wtimer.Enabled = true;
            Wtimer.AutoReset = true;
        }

        void Wtimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

        }

        public override void Init()
        {
            //注册事件
            this.AuthenticateRequest += WebApiApplication_AuthenticateRequest;
            base.Init();
        }

        void WebApiApplication_AuthenticateRequest(object sender, EventArgs e)
        {
            //启用 webapi 支持session 会话
            HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }

    }
}

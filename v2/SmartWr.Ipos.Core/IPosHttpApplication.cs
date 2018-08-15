
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SmartWr.Ipos.Core.Settings;
using SmartWr.Ipos.Core.Web.App_Start;

namespace SmartWr.Ipos.Core
{
    public class IPosHttpApplication : HttpApplication
    {
        void Application_Start(object s, EventArgs e)
        {

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.Filter(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            IposConfig.ToggleDbInitializer();
        }

        protected void Application_Error(object s, EventArgs e)
        {
            var ex = Server.GetLastError();
            ErrorConfig.CatchAllErrors(ex, this.Context);
        }
    }
}
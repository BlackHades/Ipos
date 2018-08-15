using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SmartWr.Ipos.Core.Controllers.MvcControllers;
using SmartWr.WebFramework.Library.Infrastructure.IoCs;
using SmartWr.WebFramework.Library.Infrastructure.Logging;

namespace SmartWr.Ipos.Core.Web.App_Start
{
    public static class ErrorConfig
    {
        private static ILogger _logger;

        public static void CatchAllErrors(Exception ex, HttpContext httpContext)
        {
            //get exception status
            var status = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;

            //resolve and log exception
            _logger = EngineContext.Current.Resolve<ILogger>();
            _logger.Log(ex);


            //clear server error and skip IIS custom exception
            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = status;
            httpContext.Response.TrySkipIisCustomErrors = true;


            //return json if request is Ajax 
            if (httpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.Write("{ errorStatus: true, message: \"Sorry, an error has occured on the server.\" }");
                httpContext.Response.End();
            }

            else
            {
                httpContext.Response.ContentType = "text/html; charset=utf-8";

                var routeData = new RouteData();

                routeData.Values["controller"] = "Error";

                if (status == 404)
                    routeData.Values["action"] = "notfound";
                else
                    routeData.Values["action"] = "ServerError";

                IController eController = new ErrorController();
                eController.Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
            }
        }
    }
}
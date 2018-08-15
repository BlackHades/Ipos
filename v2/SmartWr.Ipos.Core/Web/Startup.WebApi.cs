using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace SmartWr.Ipos.Core.Web
{
    public partial  class Startup
    {
        // Web API configuration and services
        public static void ConfigureWebApi(HttpConfiguration config)
        {
            // Use camel case for JSON data.
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Web API routes
            config.MapHttpAttributeRoutes();
            //http://www.strathweb.com/2015/10/global-route-prefixes-with-attribute-routing-in-asp-net-web-api/
            //config.MapHttpAttributeRoutes(new CentralizedPrefixProvider("api"));
            // Web API configuration and services

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}

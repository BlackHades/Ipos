using System.Web.Http;
using AutoMapper;
using Microsoft.Owin;
using Owin;
using SmartWr.Ipos.Core.Web;

[assembly: OwinStartup(typeof(Startup), "Configuration")]
namespace SmartWr.Ipos.Core.Web
{
    public partial class Startup
    {
        public static void Configuration(IAppBuilder app)
        {
            Mapper.Initialize(ConfigureMapper);
            var config = new HttpConfiguration();
            ConfigureComposition(config);
            ConfigureAuth(app);
            ConfigureWebApi(config);
            app.UseWebApi(config);
        }
    }
}

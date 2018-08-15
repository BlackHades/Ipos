using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using SmartWr.WebFramework.Library.Infrastructure.Factory;
using SmartWr.WebFramework.Library.MiddleServices.DataAccess;
using SmartWr.WebFramework.Library.MiddleServices.Factory;
using SmartWr.WebFramework.Library.MiddleServices.Interfaces.Auth;
using System;
using System.Web;

namespace SmartWr.Ipos.Core.Web
{
    public partial class Startup
    {
        public static void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext<IEntitiesContext>(DbContextFactory.CreateApplicationDbContext);
            app.CreatePerOwinContext<IApplicationUserManager>(IdentityFactory.CreateApplicationUserManager);
            app.CreatePerOwinContext<IApplicationRoleManager>(IdentityFactory.CreateApplicationRoleManager);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/auth/login"),
                AuthenticationMode = AuthenticationMode.Active,
                CookieName = "IPosCookie",
                ReturnUrlParameter = "ReturnUrl",
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = context =>
                    {
                        if (!IsAjaxRequest(context.Request))
                            context.Response.Redirect(context.RedirectUri);
                    }
                }
            });
        }

        private static bool IsAjaxRequest(IOwinRequest owinRequest)
        {
            var apiPath = VirtualPathUtility.ToAbsolute("~/api");
            return owinRequest.Uri.LocalPath.ToLower().StartsWith(apiPath);
        }
    }
}
using System.Web;
using Autofac;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SmartWr.Ipos.Core.Context;
using SmartWr.WebFramework.Library.Infrastructure.Factory;
using SmartWr.WebFramework.Library.Infrastructure.Identity;
using SmartWr.WebFramework.Library.Infrastructure.TypeFinder;
using SmartWr.WebFramework.Library.MiddleServices.DataAccess;
using SmartWr.WebFramework.Library.MiddleServices.Interfaces.Auth;

namespace SmartWr.Ipos.Core.IocConfig
{
    public class AuthIocConfig : IDependencyRegistrar
    {
        public int Order
        {
            get { return 1; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.Register(b =>
            {
                return HttpContext.Current.Request.GetOwinContext().Get<IApplicationSignInManager>();
            }).As<IApplicationSignInManager>().InstancePerRequest();

            builder.RegisterType(typeof(ApplicationIdentityUser)).As(typeof(IUser<int>)).InstancePerRequest();

            builder.Register(b =>
            {
                var manager = IdentityFactory.CreateUserManager((IPosDbContext)b.Resolve<IEntitiesContext>());
                return manager;
            }).InstancePerRequest();

            builder.Register(b => IdentityFactory.CreateRoleManager((IPosDbContext)
                b.Resolve<IEntitiesContext>())).InstancePerRequest();

            builder.Register(b =>
            {
                return HttpContext.Current.Request.GetOwinContext().Get<IApplicationRoleManager>();
            }).As<IApplicationRoleManager>().InstancePerRequest();

            builder.Register(b =>
            {
                return HttpContext.Current.Request.GetOwinContext().Get<IApplicationUserManager>();
            }).As<IApplicationUserManager>().InstancePerRequest();


            builder.Register(b =>
            {
                var manager = IdentityFactory.CreateUserManager((IPosDbContext)b.Resolve<IEntitiesContext>());
                return manager;
            }).InstancePerRequest();

            builder.Register(b => IdentityFactory.CreateRoleManager((IPosDbContext)
                b.Resolve<IEntitiesContext>())).InstancePerRequest();
        }

        private IApplicationRoleManager RegisterRoleManager(IComponentContext arg)
        {
            return null;
        }
    }
}
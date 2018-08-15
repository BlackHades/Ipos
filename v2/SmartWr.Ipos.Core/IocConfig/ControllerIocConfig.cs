using System.Reflection;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Nop.Core.Caching;
using SmartWr.Ipos.Core.Controllers;
using SmartWr.Ipos.Core.Messaging;
using SmartWr.Ipos.Core.Settings;
using SmartWr.WebFramework.Library.Infrastructure.TypeFinder;

namespace SmartWr.Ipos.Core.IocConfig
{
    public partial class ControllerIocConfig : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            var assembly = Assembly.GetAssembly(typeof(BaseMvcController));

            builder.RegisterControllers(assembly);
            builder.RegisterApiControllers(assembly);

            //builder.Register(x =>
            //{
            //    return new HttpContextWrapper(HttpContext.Current) as HttpContextBase;
            //});

            //builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>()
            //   .Named<ICacheManager>("nop_cache_per_request").InstancePerLifetimeScope();

            builder.RegisterType<EmailService>().As<IEmailService>();
            builder.RegisterType<SmsService>().As<SmsService>();

            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>()
                .Named<ICacheManager>(AppKeys.MemoryCacheKey).SingleInstance();
        }

        public int Order
        {
            get { return 5; }
        }
    }
}
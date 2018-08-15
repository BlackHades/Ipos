using Autofac;
using SmartWr.Ipos.Core.Context;
using SmartWr.Ipos.Core.Context.Services;
using SmartWr.Ipos.Core.Settings;
using SmartWr.WebFramework.Library.Infrastructure.Logging;
using SmartWr.WebFramework.Library.Infrastructure.TypeFinder;
using SmartWr.WebFramework.Library.MiddleServices.DataAccess;
using SmartWr.WebFramework.Library.MiddleServices.Interfaces.Data;
using SmartWr.WebFramework.Library.MiddleServices.UnitOfWork;

namespace SmartWr.Ipos.Core.IocConfig
{
    public class EFIocConfig : IDependencyRegistrar
    {
        public int Order
        {
            get { return 0; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.Register(x => NLogLogger.Instance).As<ILogger>().InstancePerRequest();

            builder.Register(x =>
            {
                var logger = x.Resolve<ILogger>();
                return new IPosDbContext(AppKeys.NameOrConnectionString, logger);
            }).As<IEntitiesContext>().InstancePerRequest();

            builder.Register(x =>
            {
                var logger = x.Resolve<ILogger>();
                return new IPosDbContext(AppKeys.NameOrConnectionString, logger);
            }).As<IEntitiesContext>().InstancePerRequest();

            builder.Register(x =>
            {
                return new UnitOfWork(x.Resolve<IEntitiesContext>());
            }).As<IUnitOfWork>().InstancePerLifetimeScope();

            builder.RegisterType<IPosReportService>().As<IPosReportService>().InstancePerRequest();
            builder.RegisterType<ProductService>().As<ProductService>().InstancePerRequest();
            builder.RegisterType<CategoryService>().As<CategoryService>().InstancePerRequest();
            builder.RegisterType<OrderDetailService>().As<OrderDetailService>().InstancePerRequest();
            builder.RegisterType<AuditTrailService>().As<AuditTrailService>().InstancePerRequest();
            builder.RegisterType<WasteService>().As<WasteService>().InstancePerRequest();
            builder.RegisterType<CustomerService>().As<CustomerService>().InstancePerRequest();
        }
    }
}
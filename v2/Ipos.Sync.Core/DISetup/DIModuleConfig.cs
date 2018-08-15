using Autofac;
using Ipos.Sync.Core.ApiClient;
using Ipos.Sync.Core.Context;
using Ipos.Sync.Core.Contracts;
using Ipos.Sync.Core.Logics;
using Ipos.Sync.Core.Services;
using Ipos.Sync.Core.UoW;
using Ipos.Sync.StoreDataProviders.Contracts;
using Ipos.Sync.StoreDataProviders.Ipos;
using System;
using System.Configuration;

namespace Ipos.Sync.Core.DISetup
{
    public class DIModuleConfig : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.Register<SyncApiClient>(s =>
                {
                    return new SyncApiClient(ConfigurationManager.AppSettings["SYNC_APP_URL"]);
                }).As<SyncApiClient>();

            var context = new IposSyncContext();
            var uof = new UnitOfWork<Guid>(context);

            builder.RegisterInstance(uof).As<IUnitOfWork<Guid>>();
            builder.RegisterInstance(context).As<IContext>();
            builder.RegisterType<TransactionSyncService>().As<TransactionSyncService>();
            builder.RegisterType<SpoilSyncService>().As<SpoilSyncService>();

            builder.Register<IposMssqlProvider>(c =>
            {
                return new IposMssqlProvider(ConfigurationManager.AppSettings["DATA.CONNECTIONSTRING"]
             , ConfigurationManager.AppSettings["DATA.PROVIDER"]);

            }).As<IStoreDataProvider>();

            builder.Register<IposMssqlProvider>(c =>
            {
                return new IposMssqlProvider(ConfigurationManager.AppSettings["SYNC_STORE.CONNECTIONSTRING"]
             , ConfigurationManager.AppSettings["DATA.PROVIDER"]);

            }).As<ISyncStoreDataProvider>();

            builder.Register<ProcessFreshTransactionWorkflow>(c =>
            {
                var transSvc = c.Resolve<TransactionSyncService>();
                var storeDataProvider = c.Resolve<IStoreDataProvider>();
                var syncStoreDataProvider = c.Resolve<ISyncStoreDataProvider>();

                return new ProcessFreshTransactionWorkflow(transSvc, storeDataProvider, syncStoreDataProvider);

            }).As<ProcessFreshTransactionWorkflow>();

            builder.Register<ProcessSpoilWorkflow>(c =>
            {
                var transSvc = c.Resolve<SpoilSyncService>();
                var storeDataProvider = c.Resolve<IStoreDataProvider>();
                var syncStoreDataProvider = c.Resolve<ISyncStoreDataProvider>();

                return new ProcessSpoilWorkflow(storeDataProvider, transSvc, syncStoreDataProvider);
            }).As<ProcessSpoilWorkflow>();

            base.Load(builder);
        }
    }
}
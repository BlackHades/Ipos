using Autofac;
using Hangfire;
using Ipos.Sync.Core.DISetup;
using Ipos.Sync.Core.Logics;
using Ipos.Sync.Core.Services;
using Ipos.Sync.StoreDataProviders.Contracts;
using Serilog;
using System;
using Topshelf;

namespace Ipos.Sync.Core.Components.JobSchduler
{
    public class IPosServiceControl
    {
        public static void Run(TransactionSyncService transSvc, SpoilSyncService spoilSvc, IStoreDataProvider storeDataProvider)
        {
            HostFactory.Run(x =>
            {
                //x.DependsOnMsSql();
                x.UseSerilog();

                x.SetStartTimeout(TimeSpan.FromSeconds(60));

                x.Service<IposSyncService>(s =>
                {
                    s.ConstructUsing(name => new IposSyncService(transSvc, spoilSvc, storeDataProvider));     //3
                    s.WhenStarted(tc => tc.Start());              //4
                    s.WhenStopped(tc => tc.Stop());
                });

                x.EnableServiceRecovery(recoveryOption =>
                {
                    recoveryOption.RestartService(5);
                });

                x.RunAsLocalSystem();                            //6

                x.SetDescription("Ipos transaction upload to Ipos Analytics.");        //7
                x.SetDisplayName("Ipos Cloud Sync");                       //8
                x.SetServiceName("IposCloudSync");

                x.StartAutomatically();


            });
        }

        public static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .WriteTo.RollingFile(@"C:\Iposlogs\ipossync-service-{Date}.txt")
                .CreateLogger();
        }

        public static void ConfigureHangfire(IContainer container)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("IposSyncContext");

            GlobalConfiguration.Configuration.UseActivator(new IposContainerJobActivator(container));

#if DEBUG

            RecurringJob.AddOrUpdate<ProcessFreshTransactionWorkflow>(workflow => workflow.TriggerOfflineTableSync(), Cron.MinuteInterval(1));

            RecurringJob.AddOrUpdate<ProcessFreshTransactionWorkflow>(workflow => workflow.SyncBatchToCloud(), Cron.MinuteInterval(2));

            RecurringJob.AddOrUpdate<ProcessSpoilWorkflow>(workflow => workflow.TriggerOfflineTableSync(), Cron.MinuteInterval(1));

            RecurringJob.AddOrUpdate<ProcessSpoilWorkflow>(workflow => workflow.SyncBatchToCloud(), Cron.MinuteInterval(2));


#else
            RecurringJob.AddOrUpdate<ProcessFreshTransactionWorkflow>(workflow => workflow.TriggerOfflineTableSync(), Cron.MinuteInterval(45));

            RecurringJob.AddOrUpdate<ProcessFreshTransactionWorkflow>(workflow => workflow.SyncBatchToCloud(), Cron.HourInterval(1));

            RecurringJob.AddOrUpdate<ProcessSpoilWorkflow>(workflow => workflow.TriggerOfflineTableSync(), Cron.HourInterval(24));

            RecurringJob.AddOrUpdate<ProcessSpoilWorkflow>(workflow => workflow.SyncBatchToCloud(), Cron.HourInterval(25));
#endif
        }
    }
}
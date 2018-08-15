using Autofac;
using Hangfire.Logging;
using Ipos.Sync.Core.Components.JobSchduler;
using Ipos.Sync.Core.DISetup;
using Ipos.Sync.Core.Logics;
using Ipos.Sync.Core.Models.Enums;
using Ipos.Sync.Core.Services;
using Ipos.Sync.StoreDataProviders.Contracts;
using Ipos.Sync.StoreDataProviders.Ipos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync
{
    public class Program
    {
        private static IContainer Container { get; set; }
        static void Main(string[] args)
        {
            Console.WriteLine("Ipos Sync started....");
            var builder = new ContainerBuilder();
            builder.RegisterModule<DIModuleConfig>();
            Container = builder.Build();
            var transSvc = Container.Resolve<TransactionSyncService>();
            var storeDataProvider = Container.Resolve<IStoreDataProvider>();
            var spoilSvc = Container.Resolve<SpoilSyncService>();
            var mssqlProvider = Container.Resolve<ISyncStoreDataProvider>();

            IPosServiceControl.ConfigureLogger();
            IPosServiceControl.ConfigureHangfire(Container);
            IPosServiceControl.Run(transSvc, spoilSvc, storeDataProvider);

            //var pendingTxnDataList = transSvc.GetAll(0, 2
            //    , t => t.Id
            //    , t => t.IsSyncReady == true && t.SyncStatus == (Int32)SyncStatus.PENDING
            //    , Core.Enums.OrderBy.Ascending).ToList();

            //var pendingTxnDataList = mssqlProvider.GetRangeUnsyncedTxn(2, (Int32)SyncStatus.PENDING, true);

            //foreach (var p in pendingTxnDataList)
            //{
            //    var txn = transSvc.SyncTransactionWithCloud(p).Result;

            //    if (txn == null)
            //    {
            //        continue;
            //    }

            //    mssqlProvider.UpdateSyncTransaction(txn);
            //};


            //var pendingSpoilDataList = mssqlProvider.GetRangeUnsyncedSpoil(2, (Int32)SyncStatus.PENDING, true);

            //foreach (var p in pendingSpoilDataList)
            //{
            //    var spoil = spoilSvc.SyncTransactionWithCloud(p).Result;

            //    if (spoil == null)
            //    {
            //        continue;
            //    }

            //    mssqlProvider.UpdateSyncedSpoil(spoil);
            //};

            //Task.WaitAll();
            //Console.WriteLine("IPos Sync Exiting.....");
            //Console.Read();

        }
    }
}

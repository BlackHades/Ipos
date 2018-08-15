using Ipos.Sync.Core.Enums;
using Ipos.Sync.Core.Helpers;
using Ipos.Sync.Core.Models;
using Ipos.Sync.Core.Models.Enums;
using Ipos.Sync.Core.Services;
using Ipos.Sync.StoreDataProviders.Contracts;
using Ipos.Sync.StoreDataProviders.Dto;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.Logics
{
    public class ProcessSpoilWorkflow
    {
        private readonly SpoilSyncService _spoilSyncSvc;
        private readonly IStoreDataProvider _storeDataProvider;
        private Object thisLock = new Object();
        private readonly ISyncStoreDataProvider _syncStoreProvider;
        private String FetchCount = ConfigurationManager.AppSettings["FETCH_COUNT"];
        public ProcessSpoilWorkflow(IStoreDataProvider storeDataProvider, SpoilSyncService spoilSyncSvc, ISyncStoreDataProvider syncStoreProvider)
        {
            _spoilSyncSvc = spoilSyncSvc;
            _storeDataProvider = storeDataProvider;
            _syncStoreProvider = syncStoreProvider;
        }

        private List<SpoilDto> GetUnSyncedSpoil(DateTime? date)
        {
            String sqlFormat = date.HasValue ?
                date.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : null;
            return _storeDataProvider.RetrieveUnSyncedSpoils(sqlFormat).ToList();
        }

        public List<Spoil> GetUnSyncedSpoilAfter(Spoil lastSpoil)
        {
            List<SpoilDto> dtoSpoils;

            if (lastSpoil == null)
                dtoSpoils = this.GetUnSyncedSpoil(null);
            else
                dtoSpoils = this.GetUnSyncedSpoil(lastSpoil.CreatedOnUtc);

            var spoilList = new List<Spoil>();
            dtoSpoils.ForEach(c =>
            {
                var trt = Spoil.Create(c);
                trt.MachineName = Environment.MachineName;
                spoilList.Add(trt);
            });

            return spoilList;
        }

        Spoil GetLastRetreivedSpoil()
        {
            return _spoilSyncSvc.GetLastRetrievedSpoil();
        }

        public async Task TriggerOfflineTableSync()
        {
            lock (thisLock)
            {
                var spoilTrt = GetLastRetreivedSpoil();
                var freshSpoils = GetUnSyncedSpoilAfter(spoilTrt);

                freshSpoils.ForEach(trt =>
                {

                    Spoil dbTrt = this.GetSpoilBy(trt.TransactionRefNo);
                        //_spoilSyncSvc.FirstOrDefault(t => t.TransactionRefNo == trt.TransactionRefNo);

                        if (dbTrt == null)
                    {
                        trt.CreatedOnUtc = DateTime.UtcNow;
                        trt.ModifiedOnUtc = DateTime.UtcNow;
                        Log.Warning(String.Format("Spoil with stock ref no ({0}) couldn't be retreive for update.", trt.StockRefNo));
                        Log.Information(String.Format("Switching to adding Spoil with stock ref no ({0}) to sync table as newly created.", trt.StockRefNo));

                            //_spoilSyncSvc.Add(trt);
                            _syncStoreProvider.AddSpoilToSync(
                            Guid.NewGuid(),
                            trt.TransactionRefNo, trt.StockRefNo,
                              trt.StockDetails, trt.StockUnit,
                              trt.StockUnitLeft, trt.IsSyncReady,
                              trt.SyncStatus, trt.SpoilDetails,
                              trt.ReasonSyncFailed, trt.ReportedBy,
                              trt.SyncRefCreatedOn, trt.SyncRefModifiedOn,
                              trt.MachineName, trt.SyncRefNo,
                              trt.SyncDate, trt.SyncFailedCount,
                              trt.RefCreatedDate, trt.RefModifiedDate,
                              trt.Cost, trt.CreatedOnUtc,
                              trt.ModifiedOnUtc, trt.IsDeleted);
                    }
                    else
                    {
                        Spoil.Extend(trt, dbTrt);
                        _syncStoreProvider.UpdateSyncedSpoil(dbTrt.StockRefNo,
                            dbTrt.StockDetails, dbTrt.StockUnit,
                            dbTrt.StockUnitLeft, dbTrt.IsSyncReady,
                            dbTrt.SyncStatus, dbTrt.SpoilDetails,
                            dbTrt.ReportedBy, dbTrt.MachineName,
                            dbTrt.RefCreatedDate, dbTrt.RefModifiedDate,
                            dbTrt.Cost, dbTrt.Id);
                            //_spoilSyncSvc.Update(dbTrt);
                        }
                });
            }

            await Task.FromResult<int>(1);
        }

        private Spoil GetSpoilBy(string transactionRefNo)
        {
            var reader = _syncStoreProvider.GetSpoiltBy(transactionRefNo);

            if (!reader.Read())
                return null;

            Spoil spoil = new Spoil();
            DataReaderHelper.DataReaderToObject(reader, spoil);

            return spoil;
        }

        public void SyncBatchToCloud()
        {
            lock (this)
            {
                List<SpoilDto> pendingSpoilList = _syncStoreProvider.GetRangeUnsyncedSpoil(FetchCount, (Int32)SyncStatus.PENDING, true);

                if (pendingSpoilList == null || pendingSpoilList.Any() == false)
                    return;

                var txnList = _spoilSyncSvc.SyncSpoilsWithCloud(pendingSpoilList).Result;

                if (txnList == null)
                {
                    Log.Warning("Sync Transaction with Cloud return null usually cause will connecting to the cloud");
                    return;
                }

                pendingSpoilList.ForEach(spoil =>
                {
                    _syncStoreProvider.UpdateSyncedSpoil(spoil);
                });
            }
        }

        //public void SyncToCloud()
        //{
        //    lock (thisLock)
        //    {
        //        var pendingTxnDataList = _syncStoreProvider.GetRangeUnsyncedSpoil(FetchCount, (Int32)SyncStatus.PENDING, true);


        //        foreach (var p in pendingTxnDataList)
        //        {
        //            try
        //            {
        //                var txn = _spoilSyncSvc.SyncSpoilsWithCloud(p).Result;
        //                if (txn != null)
        //                    _syncStoreProvider.UpdateSyncedSpoil(txn);
        //            }
        //            catch (Exception e)
        //            {
        //                Log.Error(e.Message);
        //            }
        //        };
        //    }
        //}
    }
}
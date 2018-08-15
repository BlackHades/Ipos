using Ipos.Sync.Core.Helpers;
using Ipos.Sync.Core.Models;
using Ipos.Sync.Core.Models.Enums;
using Ipos.Sync.Core.Services;
using Ipos.Sync.StoreDataProviders.Contracts;
using Ipos.Sync.StoreDataProviders.Dto;
using Ipos.Sync.StoreDataProviders.Ipos;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.Logics
{
    public class ProcessFreshTransactionWorkflow
    {
        private readonly TransactionSyncService _trtSyncSvc;
        private readonly IStoreDataProvider _storeDataProvider;
        private readonly ISyncStoreDataProvider _syncStoreProvider;

        private String FetchCount = ConfigurationManager.AppSettings["FETCH_COUNT"];

        public ProcessFreshTransactionWorkflow(TransactionSyncService trtSyncSvc,
            IStoreDataProvider storeDataProvider, ISyncStoreDataProvider syncStoreProvider)
        {
            _trtSyncSvc = trtSyncSvc;
            _storeDataProvider = storeDataProvider;
            _syncStoreProvider = syncStoreProvider;
        }

        public void SyncBatchToCloud()
        {
            lock (this)
            {
                List<TransactionDto> pendingTxnList = _syncStoreProvider.GetRangeUnsyncedTxn(FetchCount, (Int32)SyncStatus.PENDING, true);

                if (pendingTxnList == null || pendingTxnList.Any() == false)
                    return;

                var txnList = _trtSyncSvc.SyncTransactionWithCloud(pendingTxnList).Result;

                if (txnList == null)
                {
                    Log.Warning("Sync Transaction with Cloud return null usually cause will connecting to the cloud");
                    return;
                }

                pendingTxnList.ForEach(txn =>
                {
                    _syncStoreProvider.UpdateSyncTransaction(txn);
                });
            }
        }
        //public void SyncToCloud()
        //{
        //    lock (this)
        //    {
        //        List<TransactionDto> pendingTxnList = _syncStoreProvider.GetRangeUnsyncedTxn(3, (Int32)SyncStatus.PENDING, true);

        //        pendingTxnList.ForEach(p =>
        //        {
        //            var txn = _trtSyncSvc.SyncTransactionWithCloud(p).Result;

        //            if (txn == null)
        //            {
        //                Log.Warning("Sync Transaction with Cloud return null usually cause will connecting to the cloud");
        //                return;
        //            }

        //            _syncStoreProvider.UpdateSyncTransaction(txn);
        //        });
        //    }
        //}

        private Transaction GetLastRetreivedTransaction()
        {
            return _trtSyncSvc.GetLastTransaction();
        }

        public List<Transaction> GetLatestAfter(Transaction lastTransaction)
        {
            lock (this)
            {
                List<TransactionDto> dtoTransaction;

                if (lastTransaction == null)
                {
                    dtoTransaction = this.GetPOSNewTransction(null, null);
                }
                else
                {
                    dtoTransaction = this.GetPOSNewTransction(lastTransaction.RefModifiedDate, lastTransaction.RefCreatedDate);
                }

                List<Transaction> transactionList = new List<Transaction>();
                dtoTransaction.ForEach(c =>
                {
                    var trt = Transaction.Create(c);
                    trt.CurrencyCode = CurrencyCodeHelper.NGN;
                    trt.MachineName = Environment.MachineName;
                    transactionList.Add(trt);
                });

                return transactionList;
            }
        }

        private List<TransactionDto> GetPOSNewTransction(DateTime? modifiedDate, DateTime? createdDate)
        {
            Int32 rowLimit = 20;
            Int32.TryParse(ConfigurationManager.AppSettings["FETCH_COUNT"], out rowLimit);
            String sqlFormat = createdDate.HasValue ?
                createdDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : null;
            return _storeDataProvider.RetrieveUnSyncedSales(null, sqlFormat, rowLimit).ToList();
        }

        public async Task TriggerOfflineTableSync()
        {
            lock (this)
            {
                var salesTrt = GetLastRetreivedTransaction();
                var freshTransaction = GetLatestAfter(salesTrt);

                freshTransaction.ForEach(trt =>
                {
                    var dbTrt = this.GetTransactionBy(trt.TransactionRefNo);

                    //_trtSyncSvc.FirstOrDefault(t => t.StockRefNo == trt.StockRefNo);

                    if (dbTrt == null)
                    {
                        trt.CreatedOnUtc = DateTime.UtcNow;
                        trt.ModifiedOnUtc = DateTime.UtcNow;
                        Log.Warning(String.Format("Transaction with stock ref no ({0}) couldn't be retreive for update.", trt.StockRefNo));
                        Log.Information(String.Format("Switching to adding transaction with stock ref no ({0}) to sync table as newly created.", trt.StockRefNo));
                        //._trtSyncSvc.Add(trt);
                        _syncStoreProvider.AddTransactionToSync(
                                Guid.NewGuid(), trt.ReasonSyncFailed,
                                trt.TransactionRefNo, trt.CurrencyCode,
                                  trt.Cashier, trt.MachineName,
                                  trt.SyncDate, trt.SyncStatus,
                                  trt.IsSyncReady, trt.StockItemNo,
                                  trt.StockRefNo, trt.StockItemCode,
                                  trt.StockDetails, trt.StockCategoryRefNo,
                                  trt.StockCategoryLine, trt.StockUnitLeft,
                                  trt.StockUnitPurchased, trt.StockUnitAmount,
                                  trt.StockCostAmount, trt.StockDiscountAmount,
                                  trt.RefModifiedDate, trt.RefCreatedDate,
                                  trt.CustomerName, trt.CustomerEmail,
                                  trt.CustomerAddress, trt.CustomerTel,
                                  trt.CustomerGender, trt.CustomerDOB,
                                  trt.Barcode, trt.TaxAmount,
                                  trt.CreatedOnUtc, trt.ModifiedOnUtc,
                                  trt.IsDeleted, trt.SyncFailedCount,
                                  trt.SyncRefNo, trt.SyncRefCreatedOn,
                                  trt.SyncRefModifiedOn, trt.StockReorderUnit);
                    }
                    else
                    {
                        Transaction.Extend(trt, dbTrt);
                        _syncStoreProvider.UpdateSyncTransaction(
                            dbTrt.Id, dbTrt.Barcode,
                        dbTrt.TransactionRefNo, dbTrt.Cashier,
                        dbTrt.CustomerAddress, dbTrt.CustomerDOB,
                        dbTrt.CustomerEmail, dbTrt.CustomerName,
                        dbTrt.CustomerGender, dbTrt.CustomerTel,
                        dbTrt.RefCreatedDate, dbTrt.RefModifiedDate,
                        dbTrt.StockCategoryLine, dbTrt.StockCategoryRefNo,
                        dbTrt.StockCostAmount, dbTrt.StockDetails,
                        dbTrt.StockDiscountAmount, dbTrt.StockItemCode,
                        dbTrt.StockItemNo, dbTrt.StockRefNo,
                        dbTrt.StockUnitAmount, dbTrt.StockUnitLeft,
                        dbTrt.StockUnitPurchased, dbTrt.TaxAmount,
                        dbTrt.IsDeleted, dbTrt.StockReorderUnit,
                        dbTrt.IsSyncReady, dbTrt.SyncStatus);
                    }
                });
            }

            await Task.FromResult<dynamic>(null);
        }

        private Transaction GetTransactionBy(string transactionRefNo)
        {
            var reader = _syncStoreProvider.GetTransactionBy(transactionRefNo);

            if (!reader.Read())
                return null;

            Transaction txn = Transaction.Create(null);
            DataReaderHelper.DataReaderToObject(reader, txn);

            return txn;
        }

        //public void SyncToCloud()
        //{
        //    lock (this)
        //    {
        //        var pendingTxnDataList = _trtSyncSvc.GetAll(0, 2
        //     , t => t.Id
        //     , t => t.IsSyncReady == true && t.SyncStatus == (Int32)SyncStatus.PENDING
        //     , Core.Enums.OrderBy.Ascending).ToList();

        //        foreach (var p in pendingTxnDataList)
        //        {
        //            var txn = _trtSyncSvc.SyncTransactionWithCloud(p).Result;
        //            _trtSyncSvc.UpdateSyncTransaction(txn);
        //        };
        //    }
        //}
    }
}

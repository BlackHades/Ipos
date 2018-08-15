using Ipos.Sync.StoreDataProviders.Dto;
using SmartWr.Ipos.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.StoreDataProviders.Contracts
{
    public interface ISyncStoreDataProvider
    {
        DbDataReader GetLastTransaction();
        DbDataReader GetTransactionBy(string transactionRefNo);
        void AddSpoilToSync(Guid Id, string transactionRefNo, int stockRefNo, string stockDetails, int stockUnit, int? stockUnitLeft, bool isSyncReady, int syncStatus, string spoilDetails, string reasonSyncFailed, string reportedBy, DateTime? syncRefCreatedOn, DateTime? syncRefModifiedOn, string machineName, string syncRefNo, DateTime? syncDate, int syncFailedCount, DateTime? refCreatedDate, DateTime? refModifiedDate, double? cost, DateTime createdOnUtc, DateTime? modifiedOnUtc, bool isDeleted);
        void UpdateSyncedSpoil(Int32 StockRefNo, String StockDetails,
            Int32 StockUnit, Int32? StockUnitLeft, bool IsSyncReady, Int32 SyncStatus,
            String SpoilDetails, String ReportedBy, String MachineName,
            DateTime? RefCreatedDate, DateTime? RefModifiedDate, Double? Cost, Guid Id);
        DbDataReader GetSpoiltBy(string transactionRefNo);
        DbDataReader GetLastSpoil();
        void UpdateSyncTransaction(TransactionDto txn);
        List<TransactionDto> GetRangeUnsyncedTxn(String count, int pending, bool syncReady);
        List<SpoilDto> GetRangeUnsyncedSpoil(String count, int status, bool isSyncReady);
        void UpdateSyncedSpoil(SpoilDto txn);
        void AddTransactionToSync(Guid guid, string reasonSyncFailed, string transactionRefNo, string currencyCode, string cashier, string machineName, DateTime? syncDate, int syncStatus, bool isSyncReady, string stockItemNo, string stockRefNo, string stockItemCode, string stockDetails, string stockCategoryRefNo, string stockCategoryLine, int? stockUnitLeft, int stockUnitPurchased, decimal stockUnitAmount, decimal stockCostAmount, double stockDiscountAmount, DateTime? refModifiedDate, DateTime? refCreatedDate, string customerName, string customerEmail, string customerAddress, string customerTel, int? customerGender, DateTime? customerDOB, string barcode, decimal taxAmount, DateTime createdOnUtc, DateTime? modifiedOnUtc, bool isDeleted, int syncFailedCount, string syncRefNo, DateTime? syncRefCreatedOn, DateTime? syncRefModifiedOn, int stockReorderUnit);
        void UpdateSyncTransaction(Guid Id, string barcode, string transactionRefNo, string cashier, string customerAddress, DateTime? customerDOB, string customerEmail, string customerName, int? customerGender, string customerTel, DateTime? refCreatedDate, DateTime? refModifiedDate, string stockCategoryLine, string stockCategoryRefNo, decimal stockCostAmount, string stockDetails, double stockDiscountAmount, string stockItemCode, string stockItemNo, string stockRefNo, decimal stockUnitAmount, int? stockUnitLeft, int stockUnitPurchased, decimal taxAmount, bool isDeleted, int stockReorderUnit, bool isSyncReady, int syncStatus);
    }
}

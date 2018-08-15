using Ipos.Sync.Core.Contracts;
using Ipos.Sync.StoreDataProviders.Dto;
using IposAnalytics.Logic.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.Models
{
    public class Spoil : BaseEntity<Guid>
    {
        public String TransactionRefNo { get; set; }
        public Int32 StockRefNo { get; set; }
        public String StockDetails { get; set; }
        public Int32 StockUnit { get; set; }
        public Int32? StockUnitLeft { get; set; }
        public Boolean IsSyncReady { get; set; }
        public Int32 SyncStatus { get; set; }
        public String SpoilDetails { get; set; }
        public String ReasonSyncFailed { get; set; }
        public String ReportedBy { get; set; }
        public DateTime? SyncRefCreatedOn { get; set; }
        public DateTime? SyncRefModifiedOn { get; set; }
        public String MachineName { get; set; }
        public String SyncRefNo { get; set; }
        public DateTime? SyncDate { get; set; }
        public Int32 SyncFailedCount { get; set; }
        public DateTime? RefCreatedDate { get; set; }
        public DateTime? RefModifiedDate { get; set; }
        public Double? Cost { get; set; }

        public static Spoil Create(SpoilDto spoilDto)
        {
            if (spoilDto == null)
                throw new ArgumentNullException();

            var spoil = new Spoil();

            if (String.IsNullOrEmpty(spoilDto.ReportedBy))
            {
                spoil.ReasonSyncFailed = "ReportedBy is missing which is required to sync this transaction.; ";
                spoil.IsSyncReady = false;
                spoil.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (spoilDto.StockRefNo <= 0)
            {
                spoil.ReasonSyncFailed += "StockRefNo is missing which is required to sync this transaction.; ";
                spoil.IsSyncReady = false;
                spoil.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (String.IsNullOrEmpty(spoilDto.StockDetails))
            {
                spoil.ReasonSyncFailed = "StockDetails is missing which is required to sync this transaction.; ";
                spoil.IsSyncReady = false;
                spoil.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (spoilDto.StockUnit <= 0)
            {
                spoil.ReasonSyncFailed = "StockUnit  must be greater than zero which is required to sync this transaction.; ";
                spoil.IsSyncReady = false;
                spoil.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (!spoilDto.RefCreatedDate.HasValue)
            {
                spoil.ReasonSyncFailed = "RefCreatedDate was not supplied or invalid.";
                spoil.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
                spoil.IsSyncReady = false;
            }

            spoil.ReportedBy = spoilDto.ReportedBy;
            spoil.StockDetails = spoilDto.StockDetails;
            spoil.StockRefNo = spoilDto.StockRefNo;
            spoil.StockUnit = spoilDto.StockUnit;
            spoil.StockUnitLeft = spoilDto.StockUnitLeft;
            spoil.SpoilDetails = spoilDto.SpoilDetails;
            spoil.IsDeleted = spoilDto.IsDeleted;
            spoil.TransactionRefNo = spoilDto.TransactionRefNo;
            spoil.RefCreatedDate = spoilDto.RefCreatedDate;
            spoil.RefModifiedDate = spoilDto.RefModifiedDate;
            spoil.Cost = spoilDto.Cost;

            spoil.Id = Guid.NewGuid();
           
            if (spoil.SyncStatus != (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED)
            {
                spoil.IsSyncReady = true;
                spoil.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.PENDING;
            }

            return spoil;
        }

        public static Spoil Extend(Spoil trtSource, Spoil trtTarget)
        {
            if (trtSource == null || trtTarget == null)
            {
                throw new ArgumentNullException();
            }

            ValidateTransactionInput(trtSource, trtTarget);

            trtTarget.ReportedBy = trtSource.ReportedBy;
            trtTarget.StockDetails = trtSource.StockDetails;
            trtTarget.StockRefNo = trtSource.StockRefNo;
            trtTarget.StockUnit = trtSource.StockUnit;
            trtTarget.StockUnitLeft = trtSource.StockUnitLeft;
            trtTarget.Cost = trtSource.Cost;
            trtTarget.SpoilDetails = trtSource.SpoilDetails;
            trtTarget.RefCreatedDate = trtSource.RefCreatedDate;
            trtTarget.RefModifiedDate = trtSource.RefModifiedDate;
            trtTarget.IsDeleted = trtSource.IsDeleted;

            if (trtTarget.SyncStatus != (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED)
            {
                trtTarget.IsSyncReady = true;
                trtTarget.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.PENDING;
            }

            return trtTarget;
        }

        private static void ValidateTransactionInput(Spoil trtSource, Spoil trtTarget)
        {
            if (String.IsNullOrEmpty(trtSource.ReportedBy))
            {
                trtTarget.ReasonSyncFailed = "Stock Reporter is missing which is required to sync this transaction.";
                trtTarget.IsSyncReady = false;
                trtTarget.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (trtSource.StockRefNo <= 0)
            {
                trtTarget.ReasonSyncFailed = "Stock reference no is missing which is required to sync this transaction.";
                trtTarget.IsSyncReady = false;
                trtTarget.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (String.IsNullOrEmpty(trtSource.StockDetails))
            {
                trtTarget.ReasonSyncFailed = "Stock details is missing which is required to sync this transaction.";
                trtTarget.IsSyncReady = false;
                trtTarget.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (trtSource.StockUnit <= 0)
            {
                trtTarget.ReasonSyncFailed = "StockUnit must be greater than zero which is required to sync this transaction.";
                trtTarget.IsSyncReady = false;
                trtTarget.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (!trtSource.RefCreatedDate.HasValue)
            {
                trtTarget.ReasonSyncFailed = "RefCreatedDate was not supplied or invalid.";
                trtTarget.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
                trtTarget.IsSyncReady = false;
            }
        }

        public static explicit operator SpoilData(Spoil data)
        {
            return new SpoilData
            {
                TxnNo = data.Id,
                DeviceName = data.MachineName,
                CreatedDate = data.CreatedOnUtc,
                ModifiedDate = data.ModifiedOnUtc
            };
        }
    }
}
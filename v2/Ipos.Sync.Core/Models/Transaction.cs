using Ipos.Sync.Core.Contracts;
using Ipos.Sync.Core.Models.Enums;
using Ipos.Sync.StoreDataProviders.Dto;
using IposAnalytics.Logic.DataContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.Models
{
    public class Transaction : BaseEntity<Guid>
    {
        private Transaction()
        {
        }

        public static explicit operator TxnData(Transaction txnFrom)
        {
            var txnData = new TxnData()
            {
                TxnNo = txnFrom.Id,
                DeviceName = txnFrom.MachineName,
                SalesPerson = txnFrom.Cashier,
                CustomerName = txnFrom.CustomerName,
                CustomerEmail = txnFrom.CustomerEmail,
                CustomerGender = txnFrom.CustomerGender,
                CustomerAddress = txnFrom.CustomerAddress,
                CustomerDOB = txnFrom.CustomerDOB,
                CustomerTel = txnFrom.CustomerTel,
                ReceiptNo = txnFrom.TransactionRefNo.Substring(0, 8), // Currently Ipos generate reciept No from Transaction.Id substring this needs to have a column
                CreatedDate = txnFrom.CreatedOnUtc,
                ModifiedDate = txnFrom.ModifiedOnUtc,
            };

            return txnData;
        }

        public static Transaction Extend(Transaction trtSource, Transaction trtTarget)
        {

            if (trtSource == null || trtTarget == null)
            {
                throw new ArgumentNullException();
            }

            ValidateTransactionInput(trtSource, trtTarget);
            trtTarget.Barcode = trtSource.Barcode;
            trtTarget.TransactionRefNo = trtSource.TransactionRefNo;
            trtTarget.Cashier = trtSource.Cashier;
            trtTarget.CustomerAddress = trtSource.CustomerAddress;
            trtTarget.CustomerDOB = trtSource.CustomerDOB;
            trtTarget.CustomerEmail = trtSource.CustomerEmail;
            trtTarget.CustomerName = trtSource.CustomerName;
            trtTarget.CustomerGender = trtSource.CustomerGender;
            trtTarget.CustomerTel = trtSource.CustomerTel;
            trtTarget.RefCreatedDate = trtSource.RefCreatedDate;
            trtTarget.RefModifiedDate = trtSource.RefModifiedDate;
            trtTarget.StockCategoryLine = trtSource.StockCategoryLine;
            trtTarget.StockCategoryRefNo = trtSource.StockCategoryRefNo;
            trtTarget.StockCostAmount = trtSource.StockCostAmount;
            trtTarget.StockDetails = trtSource.StockDetails;
            trtTarget.StockDiscountAmount = trtSource.StockDiscountAmount;
            trtTarget.StockItemCode = trtSource.StockItemCode;
            trtTarget.StockItemNo = trtSource.StockItemNo;
            trtTarget.StockRefNo = trtSource.StockRefNo;
            trtTarget.StockUnitAmount = trtSource.StockUnitAmount;
            trtTarget.StockUnitLeft = trtSource.StockUnitLeft;
            trtTarget.StockUnitPurchased = trtSource.StockUnitPurchased;
            trtTarget.TaxAmount = trtSource.TaxAmount;
            trtTarget.IsDeleted = trtSource.IsDeleted;
            trtTarget.StockReorderUnit = trtSource.StockReorderUnit;
            //trtTarget.Id = Guid.NewGuid();

            if (trtTarget.SyncStatus != (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED)
            {
                trtTarget.IsSyncReady = true;
                trtTarget.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.PENDING;
            }

            return trtTarget;
        }

        private static void ValidateTransactionInput(Transaction trtSource, Transaction trtTarget)
        {
            if (String.IsNullOrEmpty(trtSource.TransactionRefNo))
            {
                trtTarget.ReasonSyncFailed = "Transaction reference no is missing which is required to sync this transaction.";
                trtTarget.IsSyncReady = false;
                trtTarget.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (String.IsNullOrEmpty(trtSource.StockRefNo))
            {
                trtTarget.ReasonSyncFailed = "Stock reference no is missing which is required to sync this transaction.";
                trtTarget.IsSyncReady = false;
                trtTarget.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (String.IsNullOrEmpty(trtSource.StockItemNo))
            {
                trtTarget.ReasonSyncFailed = "Stock item no is missing which is required to sync this transaction.";
                trtTarget.IsSyncReady = false;
                trtTarget.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (String.IsNullOrEmpty(trtSource.StockDetails))
            {
                trtTarget.ReasonSyncFailed = "Stock details is missing which is required to sync this transaction.";
                trtTarget.IsSyncReady = false;
                trtTarget.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (trtSource.StockUnitPurchased <= 0)
            {
                trtTarget.ReasonSyncFailed = "Stock unit purchase  must be greater than zero which is required to sync this transaction.";
                trtTarget.IsSyncReady = false;
                trtTarget.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (!trtSource.RefCreatedDate.HasValue)
            {
                trtTarget.ReasonSyncFailed = "Empty transaction created.";
                trtTarget.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
                trtTarget.IsSyncReady = false;
            }
        }

        public static Transaction Create(TransactionDto trtValues)
        {
            var trt = new Transaction();

            if (trtValues == null)
            {
                return trt;
            }

            if (String.IsNullOrEmpty(trtValues.TransactionRefNo))
            {
                trt.ReasonSyncFailed = "Transaction reference no is missing which is required to sync this transaction.";
                trt.IsSyncReady = false;
                trt.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (String.IsNullOrEmpty(trtValues.StockRefNo))
            {
                trt.ReasonSyncFailed = "Stock reference no is missing which is required to sync this transaction.";
                trt.IsSyncReady = false;
                trt.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (String.IsNullOrEmpty(trtValues.StockItemNo))
            {
                trt.ReasonSyncFailed = "Stock item no is missing which is required to sync this transaction.";
                trt.IsSyncReady = false;
                trt.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (String.IsNullOrEmpty(trtValues.StockDetails))
            {
                trt.ReasonSyncFailed = "Stock details is missing which is required to sync this transaction.";
                trt.IsSyncReady = false;
                trt.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (trtValues.StockUnitPurchased <= 0)
            {
                trt.ReasonSyncFailed = "Stock unit purchase  must be greater than zero which is required to sync this transaction.";
                trt.IsSyncReady = false;
                trt.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
            }

            if (!trtValues.RefCreatedDate.HasValue)
            {
                trt.ReasonSyncFailed = "Empty transaction created.";
                trt.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED;
                trt.IsSyncReady = false;
            }

            trt.StockReorderUnit = trtValues.StockReorderUnit;
            trt.Barcode = trtValues.Barcode;
            trt.TransactionRefNo = trtValues.TransactionRefNo;
            trt.Cashier = trtValues.Cashier;
            trt.CustomerAddress = trtValues.CustomerAddress;
            trt.CustomerDOB = trtValues.CustomerDOB;
            trt.CustomerEmail = trtValues.CustomerEmail;
            trt.CustomerName = trtValues.CustomerName;

            if (!String.IsNullOrEmpty(trtValues.CustomerGender))
            {
                if (trtValues.CustomerGender.StartsWith("F", StringComparison.InvariantCultureIgnoreCase))
                {
                    trt.CustomerGender = 1;// FEMALE = 1
                }

                else if (trtValues.CustomerGender.StartsWith("M", StringComparison.InvariantCultureIgnoreCase))
                {
                    trt.CustomerGender = 2;// MALE = 2
                }
            }

            trt.CustomerTel = trtValues.CustomerTel;
            trt.RefCreatedDate = trtValues.RefCreatedDate;
            trt.RefModifiedDate = trtValues.RefModifiedDate;
            trt.StockCategoryLine = trtValues.StockCategoryLine;
            trt.StockCategoryRefNo = trtValues.StockCategoryRefNo;
            trt.StockCostAmount = trtValues.StockCostAmount;
            trt.StockDetails = trtValues.StockDetails;
            trt.StockDiscountAmount = trtValues.StockDiscountAmount;
            trt.StockItemCode = trtValues.StockItemCode;
            trt.StockItemNo = trtValues.StockItemNo;
            trt.StockRefNo = trtValues.StockRefNo;
            trt.StockUnitAmount = trtValues.StockUnitAmount;
            trt.StockUnitLeft = trtValues.StockUnitLeft;
            trt.StockUnitPurchased = trtValues.StockUnitPurchased;
            trt.TaxAmount = trtValues.TaxAmount_;
            trt.IsDeleted = trtValues.IsDeleted_;
            trt.Id = Guid.NewGuid();

            if (trt.SyncStatus != (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.REJECTED)
            {
                trt.IsSyncReady = true;
                trt.SyncStatus = (Int32)Ipos.Sync.Core.Models.Enums.SyncStatus.PENDING;
            }

            return trt;
        }

        public Int32 SyncFailedCount { get; set; }
        public String SyncRefNo { get; set; }
        public DateTime? SyncRefCreatedOn { get; set; }
        public DateTime? SyncRefModifiedOn { get; set; }
        public String ReasonSyncFailed { get; set; }
        public String TransactionRefNo { get; set; }
        public String CurrencyCode { get; set; }
        public String Cashier { get; set; }
        public String MachineName { get; set; }
        public DateTime? SyncDate { get; set; }
        public Int32 SyncStatus { get; set; }
        public Boolean IsSyncReady { get; set; }
        public String StockItemNo { get; set; }
        public String StockRefNo { get; set; }
        public String StockItemCode { get; set; }
        public String StockDetails { get; set; }
        public String StockCategoryRefNo { get; set; }
        public String StockCategoryLine { get; set; }
        public Int32 StockReorderUnit { get; set; }
        public Int32? StockUnitLeft { get; set; }
        public Int32 StockUnitPurchased { get; set; }
        public Decimal StockUnitAmount { get; set; }
        public Decimal StockCostAmount { get; set; }
        public Double StockDiscountAmount { get; set; }
        public DateTime? RefModifiedDate { get; set; }
        public DateTime? RefCreatedDate { get; set; }
        public String CustomerName { get; set; }
        public String CustomerEmail { get; set; }
        public String CustomerAddress { get; set; }
        public String CustomerTel { get; set; }
        public Int32? CustomerGender { get; set; }
        public DateTime? CustomerDOB { get; set; }
        public String Barcode { get; set; }
        public Decimal TaxAmount { get; set; }
    }
}

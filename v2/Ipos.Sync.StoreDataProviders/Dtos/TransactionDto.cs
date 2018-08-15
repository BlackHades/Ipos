using IposAnalytics.Logic.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.StoreDataProviders.Dto
{
    public class TransactionDto
    {

        public static explicit operator TxnData(TransactionDto txnFrom)
        {
            var txnData = new TxnData()
            {
                TxnNo = txnFrom.Id,
                DeviceName = txnFrom.MachineName,
                SalesPerson = txnFrom.Cashier,
                CustomerName = txnFrom.CustomerName,
                CustomerEmail = txnFrom.CustomerEmail,

                CustomerAddress = txnFrom.CustomerAddress,
                CustomerDOB = txnFrom.CustomerDOB,
                CustomerTel = txnFrom.CustomerTel,
                ReceiptNo = txnFrom.TransactionRefNo.Substring(0, 8), // Currently Ipos generate reciept No from Transaction.Id substring this needs to have a column
                CreatedDate = txnFrom.CreatedOnUtc,
                ModifiedDate = txnFrom.ModifiedOnUtc,
            };
            txnData.CustomerGender = String.IsNullOrEmpty(txnFrom.CustomerGender) ?
                0 : (txnFrom.CustomerGender.ToUpper() == "F" ? 0 : 1);

            return txnData;
        }
        public Int32 SyncFailedCount { get; set; }
        public String ReasonSyncFailed { get; set; }
        public Nullable<DateTime> SyncRefModifiedOn { get; set; }
        public Nullable<DateTime> SyncRefCreatedOn { get; set; }
        public Boolean IsSyncReady { get; set; }
        public Nullable<DateTime> SyncDate { get; set; }
        public String SyncRefNo { get; set; }
        public Int32 SyncStatus { get; set; }
        public Decimal TaxAmount { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public Guid Id { get; set; }
        public String TransactionRefNo { get; set; }
        public String StockRefNo { get; set; }
        public String StockItemNo { get; set; }
        public String StockItemCode { get; set; }
        public String StockDetails { get; set; }
        public String StockCategoryRefNo { get; set; }
        public String StockCategoryLine { get; set; }
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
        public String CustomerGender { get; set; }
        public String Barcode { get; set; }
        public Decimal TaxAmount_ { get; set; }
        public Boolean IsDeleted_ { get; set; }
        public String Cashier { get; set; }
        public int StockReorderUnit { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }
        public DateTime? CustomerDOB { get;  set; }
        public string MachineName { get; set; }
        public string CurrencyCode { get; internal set; }
        public int StockUnit { get; internal set; }
        public object SpoilDetails { get; internal set; }
        public string ReportedBy { get; internal set; }
        public double Cost { get; internal set; }
        public string Remark { get; internal set; }
    }
}
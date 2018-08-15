using IposAnalytics.Logic.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.StoreDataProviders.Dto
{
    public class SpoilDto
    {

        public static explicit operator SpoilData(SpoilDto data)
        {
            return new SpoilData
            {
                TxnNo = data.Id,
                DeviceName = data.MachineName,
                CreatedDate = data.CreatedOnUtc,
                ModifiedDate = data.ModifiedOnUtc
            };
        }
        public String TransactionRefNo { get; set; }
        public Guid SpoilId { get; set; }
        public Int32 StockRefNo { get; set; }
        public String StockDetails { get; set; }
        public Int32 StockUnit { get; set; }
        public Int32? StockUnitLeft { get; set; }
        public String SpoilDetails { get; set; }
        public string ReportedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? RefCreatedDate { get; set; }
        public DateTime? RefModifiedDate { get; set; }
        public double? Cost { get; set; }
        public Guid Id { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public bool IsSyncReady { get; set; }
        public int SyncStatus { get; set; }
        public string ReasonSyncFailed { get; set; }
        public DateTime? SyncRefCreatedOn { get; set; }
        public DateTime? SyncRefModifiedOn { get; set; }
        public string MachineName { get; set; }
        public string SyncRefNo { get; set; }
        public DateTime? SyncDate { get; set; }
        public int SyncFailedCount { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }
        public byte[] RowVersion { get; set; } = new byte[255];
    }
}
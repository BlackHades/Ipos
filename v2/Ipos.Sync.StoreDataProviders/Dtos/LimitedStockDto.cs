
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.StoreDataProviders.Dto
{
    public class LimitedStockDto
    {
        public String StockRefNo { get; set; }
        public String StockDescription { get; set; }
        public Int32? StockUnitLeft { get; set; }
        public Int32 RestockPoint { get; set; }
        public Boolean IsSyncReady { get; set; }
        public Int32 SyncStatus { get; set; }
        public String ReasonSyncFailed { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.Models.Map
{
    public class LimitedStockMap: EntityTypeConfiguration<LimitedStock>
    {
        public LimitedStockMap()
        {
            this.ToTable("Sync.LimitedStocks");
            this.Property(l => l.StockRefNo).HasMaxLength(50).IsUnicode(false);
            this.Property(l => l.StockDescription).HasMaxLength(250).IsUnicode(false);
            this.Property(l => l.StockDescription).HasMaxLength(250).IsUnicode(false);
        }
    }
}

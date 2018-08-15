using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.Models.Map
{
    public class TransactionMap : EntityTypeConfiguration<Transaction>
    {
        public TransactionMap()
        {
            this.ToTable("Sync.Transactions");
            this.HasKey(t => t.Id);
            this.Property(t => t.TransactionRefNo).HasMaxLength(50).IsUnicode(false);
            this.Property(t => t.CurrencyCode).HasMaxLength(50).IsUnicode(false);
            this.Property(t => t.Cashier).HasMaxLength(150).IsUnicode(false);
            this.Property(t => t.MachineName).HasMaxLength(50).IsUnicode(false);
            this.Property(t => t.StockCategoryLine).HasMaxLength(150).IsUnicode(false);
            this.Property(t => t.StockDetails).HasMaxLength(250).IsUnicode(false);
            this.Property(t => t.StockItemCode).HasMaxLength(50).IsUnicode(false);
            this.Property(t => t.StockRefNo).HasMaxLength(50).IsUnicode(false);
            this.Property(t => t.CustomerName).HasMaxLength(150).IsUnicode(false);
            this.Property(t => t.CustomerEmail).HasMaxLength(50).IsUnicode(false);
            this.Property(t => t.CustomerAddress).HasMaxLength(150).IsUnicode(false);
            this.Property(t => t.CustomerTel).HasMaxLength(50).IsUnicode(false);
            this.Property(t => t.Barcode).HasMaxLength(100).IsUnicode(false);
            this.Property(t => t.SyncRefNo).HasMaxLength(150).IsUnicode(false);
        }
    }
}

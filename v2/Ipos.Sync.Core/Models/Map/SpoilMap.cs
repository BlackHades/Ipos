using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.Models.Map
{
    public class SpoilMap: EntityTypeConfiguration<Spoil>
    {
        public SpoilMap()
        {
            this.ToTable("Sync.Spoils");
            this.Property(s => s.StockDetails).HasMaxLength(250).IsUnicode(false);
            this.Property(s => s.SpoilDetails).HasMaxLength(250).IsUnicode(false);
            this.Property(s => s.TransactionRefNo).HasMaxLength(50).IsUnicode(false);
            this.Property(s => s.MachineName).HasMaxLength(150).IsUnicode(false);

        }
    }
}

using System.Data.Entity.ModelConfiguration;

namespace SmartWr.Ipos.Core.Models.Mapping
{
    public class YearlySalesOrderMap : EntityTypeConfiguration<YearlySalesOrder>
    {
        public YearlySalesOrderMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Year)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("YearlySalesOrder");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Year).HasColumnName("Year");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.EntryDate).HasColumnName("EntryDate");
        }
    }
}

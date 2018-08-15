using System.Data.Entity.ModelConfiguration;

namespace SmartWr.Ipos.Core.Models.Mapping
{
    public class HourlySalesOrderMap : EntityTypeConfiguration<HourlySalesOrder>
    {
        public HourlySalesOrderMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Time)
                .HasMaxLength(50);

            this.Property(t => t.Hour)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("HourlySalesOrder");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Time).HasColumnName("Time");
            this.Property(t => t.Hour).HasColumnName("Hour");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.EntryDate).HasColumnName("EntryDate");
        }
    }
}

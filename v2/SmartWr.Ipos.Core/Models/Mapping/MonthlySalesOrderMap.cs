using System.Data.Entity.ModelConfiguration;

namespace SmartWr.Ipos.Core.Models.Mapping
{
    public class MonthlySalesOrderMap : EntityTypeConfiguration<MonthlySalesOrder>
    {
        public MonthlySalesOrderMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Month)
                .HasMaxLength(50);

            this.Property(t => t.Year)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("MonthlySalesOrder");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Month).HasColumnName("Month");
            this.Property(t => t.Year).HasColumnName("Year");
            this.Property(t => t.EntryDate).HasColumnName("EntryDate");
            this.Property(t => t.Amount).HasColumnName("Amount");
        }
    }
}

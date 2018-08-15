using System.Data.Entity.ModelConfiguration;

namespace SmartWr.Ipos.Core.Models.Mapping
{
    public class WeeklySalesOrderMap : EntityTypeConfiguration<WeeklySalesOrder>
    {
        public WeeklySalesOrderMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.MonthName)
                .HasMaxLength(50);

            this.Property(t => t.Week)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("WeeklySalesOrder");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.MonthName).HasColumnName("MonthName");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.Week).HasColumnName("Week");
            this.Property(t => t.EntryDate).HasColumnName("EntryDate");
        }
    }
}

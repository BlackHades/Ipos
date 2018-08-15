using System.Data.Entity.ModelConfiguration;

namespace SmartWr.Ipos.Core.Models.Mapping
{
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            // Primary Key
            this.HasKey(t => t.OrderUId);

            // Properties
            this.Property(t => t.Remark)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Order");
            this.Property(t => t.OrderUId).HasColumnName("OrderUId");
            this.Property(t => t.EntryDate).HasColumnName("EntryDate");
            this.Property(t => t.User_Id).HasColumnName("User_Id");
            this.Property(t => t.IsDiscounted).HasColumnName("IsDiscounted");
            this.Property(t => t.Total).HasColumnName("Total");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.OrderStatus).HasColumnName("OrderStatus");
            this.Property(t => t.Customer_UId).HasColumnName("Customer_UId");
            this.Property(t => t.PaymentMethod).HasColumnName("PaymentMethod");

            // Relationships
            this.HasRequired(t => t.aspnet_Users)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.User_Id);

            this.HasOptional(t => t.Customer)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.Customer_UId);

            this.Property(t => t.CreatedBy_Id)
               .IsOptional();

            this.HasOptional(t => t.CreatedBy)
               .WithMany()
               .HasForeignKey(p => p.CreatedBy_Id).WillCascadeOnDelete(false);

            Ignore(p => p.Id);
            Ignore(p => p.CreatedOnUtc);
        }
    }
}
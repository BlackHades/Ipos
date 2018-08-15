using System.Data.Entity.ModelConfiguration;

namespace SmartWr.Ipos.Core.Models.Mapping
{
    public class OrderDetailMap : EntityTypeConfiguration<OrderDetail>
    {
        public OrderDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.OrderDetailUId);

            // Properties
            this.Property(t => t.Remarks)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("OrderDetails");
            this.Property(t => t.OrderDetailUId).HasColumnName("OrderDetailUId");
            this.Property(t => t.EntryDate).HasColumnName("EntryDate");
            this.Property(t => t.Discount).HasColumnName("Discount");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.Quantiy).HasColumnName("Quantiy");
            this.Property(t => t.Product_Id).HasColumnName("Product_Id");
            this.Property(t => t.Order_UId).HasColumnName("Order_UId");
            this.Property(t => t.CostPrice).HasColumnName("CostPrice");
            this.Property(t => t.Remarks).HasColumnName("Remarks");

            // Relationships
            this.HasRequired(t => t.Order)
                .WithMany(t => t.OrderDetails)
                .HasForeignKey(d => d.Order_UId);

            this.HasOptional(t => t.Product)
                .WithMany(t => t.OrderDetails)
                .HasForeignKey(d => d.Product_Id);

            this.Property(t => t.CreatedBy_Id)
                .IsOptional();

            this.HasOptional(t => t.CreatedBy)
               .WithMany()
               .HasForeignKey(p => p.CreatedBy_Id).WillCascadeOnDelete(false);

            this.Ignore(p => p.Id);
            this.Ignore(p => p.CreatedOnUtc);
        }
    }
}
using System.Data.Entity.ModelConfiguration;

namespace SmartWr.Ipos.Core.Models.Mapping
{
    public class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            // Primary Key
            this.HasKey(t => t.ProductId);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(150);

            this.Property(t => t.Description)
                .HasMaxLength(250);

            this.Property(t => t.PhotoURL)
                .HasMaxLength(250);

            this.Property(t => t.Extention)
                .HasMaxLength(20);

            this.Property(t => t.FileName)
                .HasMaxLength(150);

            this.Property(t => t.Barcode)
                .HasMaxLength(250);

            this.Property(t => t.Notes)
                .HasMaxLength(250);

            this.Property(t => t.ContentType)
                .HasMaxLength(50);

           
            // Table & Column Mappings
            this.ToTable("Product");
            this.Property(t => t.ProductUId).HasColumnName("ProductUId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.EntryDate).HasColumnName("EntryDate");
            this.Property(t => t.Insert_UId).HasColumnName("Insert_UId");
            this.Property(t => t.Update_UId).HasColumnName("Update_UId");
            this.Property(t => t.PhotoURL).HasColumnName("PhotoURL");
            this.Property(t => t.Extention).HasColumnName("Extention");
            this.Property(t => t.FileName).HasColumnName("FileName");
            this.Property(t => t.IsDiscountable).HasColumnName("IsDiscountable");
            this.Property(t => t.Barcode).HasColumnName("Barcode");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.CostPrice).HasColumnName("CostPrice");
            this.Property(t => t.ReorderLevel).HasColumnName("ReorderLevel");
            this.Property(t => t.ContentType).HasColumnName("ContentType");
            this.Property(t => t.FileSize).HasColumnName("FileSize");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.ExpiryDate).HasColumnName("ExpiryDate");
            this.Property(t => t.CanExpire).HasColumnName("CanExpire");
            this.Property(t => t.Category_UId).HasColumnName("Category_UId");
            this.Property(t => t.IsDiscontinued).HasColumnName("IsDiscontinued");

            // Relationships
            this.HasOptional(t => t.aspnet_Users)
                .WithMany(t => t.Products)
                .HasForeignKey(d => d.Insert_UId);
            this.HasOptional(t => t.Category)
                .WithMany(t => t.Products)
                .HasForeignKey(d => d.Category_UId);

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

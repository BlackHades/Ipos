using System.Data.Entity.ModelConfiguration;

namespace SmartWr.Ipos.Core.Models.Mapping
{
    public class CategoryMap : EntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.CategoryUId);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(150);

            this.Property(t => t.Description)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Category");
            this.Property(t => t.CategoryUId).HasColumnName("CategoryUId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.EntryDate).HasColumnName("EntryDate");

            // Relationships
            this.HasMany(t => t.Products1)
                .WithMany(t => t.Categories)
                .Map(m =>
                    {
                        m.ToTable("Product_Category");
                        m.MapLeftKey("Category_UId");
                        m.MapRightKey("Product_Id");
                    });

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

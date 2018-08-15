using System.Data.Entity.ModelConfiguration;

namespace SmartWr.Ipos.Core.Models.Mapping
{
    public class SpoilMap : EntityTypeConfiguration<Spoil>
    {
        public SpoilMap()
        {
            // Primary Key
            this.HasKey(t => t.SpoilId);

            // Properties
            this.Property(t => t.Title)
                .HasMaxLength(150);

            this.Property(t => t.Description)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Spoil");
            this.Property(t => t.SpoilId).HasColumnName("SpoilId");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Product_Id).HasColumnName("Product_Id");
            this.Property(t => t.EntryDate).HasColumnName("EntryDate");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.User_Id).HasColumnName("User_Id");

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

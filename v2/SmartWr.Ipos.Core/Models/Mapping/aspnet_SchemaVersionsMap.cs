using System.Data.Entity.ModelConfiguration;

namespace SmartWr.Ipos.Core.Models.Mapping
{
    public class aspnet_SchemaVersionsMap : EntityTypeConfiguration<aspnet_SchemaVersions>
    {
        public aspnet_SchemaVersionsMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Feature, t.CompatibleSchemaVersion });

            // Properties
            this.Property(t => t.Feature)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.CompatibleSchemaVersion)
                .IsRequired()
                .HasMaxLength(128);

            // Table & Column Mappings
            this.ToTable("aspnet_SchemaVersions");
            this.Property(t => t.Feature).HasColumnName("Feature");
            this.Property(t => t.CompatibleSchemaVersion).HasColumnName("CompatibleSchemaVersion");
            this.Property(t => t.IsCurrentVersion).HasColumnName("IsCurrentVersion");

            this.Ignore(p => p.Id);
            this.Ignore(p => p.IsDeleted);
            this.Ignore(p => p.CreatedBy);
            this.Ignore(p => p.CreatedBy_Id);
            this.Ignore(p => p.CreatedOnUtc);
            this.Ignore(p => p.ModifiedBy_Id);
            this.Ignore(p => p.ModifiedOnUtc);
            this.Ignore(p => p.ModifiedBy);
        }
    }
}
using System.Data.Entity.ModelConfiguration;

namespace SmartWr.Ipos.Core.Models.Mapping
{
    public class AuditMap : EntityTypeConfiguration<Audit>
    {
        public AuditMap()
        {
            // Primary Key
            this.HasKey(t => t.AuditId);

            // Properties
            this.Property(t => t.Description)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Audit");
            this.Property(t => t.AuditId).HasColumnName("AuditId");
            this.Property(t => t.AuditType).HasColumnName("AuditType");
            this.Property(t => t.EntryDate).HasColumnName("EntryDate");
            this.Property(t => t.User_Id).HasColumnName("User_Id");
            this.Property(t => t.Description).HasColumnName("Description");

            // Relationships
            this.HasOptional(t => t.aspnet_Users)
                .WithMany(t => t.Audits)
                .HasForeignKey(d => d.User_Id);

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
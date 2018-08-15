using System.Data.Entity.ModelConfiguration;

namespace SmartWr.Ipos.Core.Models.Mapping
{
    public class aspnet_UsersMap : EntityTypeConfiguration<aspnet_Users>
    {
        public aspnet_UsersMap()
        {
            // Primary Key
            this.HasKey(t => t.UserId);

            // Properties
            this.Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.LoweredUserName)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.MobileAlias)
                .HasMaxLength(16);

            // Table & Column Mappings
            this.ToTable("aspnet_Users");
            this.Property(t => t.ApplicationId).HasColumnName("ApplicationId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.LoweredUserName).HasColumnName("LoweredUserName");
            this.Property(t => t.MobileAlias).HasColumnName("MobileAlias");
            this.Property(t => t.IsAnonymous).HasColumnName("IsAnonymous");
            this.Property(t => t.LastActivityDate).HasColumnName("LastActivityDate");

            // Relationships
            this.HasRequired(t => t.aspnet_Applications)
                .WithMany(t => t.aspnet_Users)
                .HasForeignKey(d => d.ApplicationId);

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

using System.Data.Entity.ModelConfiguration;

namespace SmartWr.Ipos.Core.Models.Mapping
{
    public class BulkSMMap : EntityTypeConfiguration<BulkSM>
    {
        public BulkSMMap()
        {
            // Primary Key
            this.HasKey(t => t.SMSUId);

            // Properties
            this.Property(t => t.Sender)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("BulkSMS");
            this.Property(t => t.SMSUId).HasColumnName("SMSUId");
            this.Property(t => t.Sender).HasColumnName("Sender");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.Recipients).HasColumnName("Recipients");
            this.Property(t => t.EntryDate).HasColumnName("EntryDate");
            this.Property(t => t.MessageType).HasColumnName("MessageType");
            this.Property(t => t.DeliveryStatus).HasColumnName("DeliveryStatus");
            this.Property(t => t.User_Id).HasColumnName("User_Id");

            // Relationships
            this.HasRequired(t => t.aspnet_Users)
                .WithMany(t => t.BulkSMS)
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

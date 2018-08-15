using System.Data.Entity.ModelConfiguration;

namespace SmartWr.Ipos.Core.Models.Mapping
{
    public class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            // Primary Key
            this.HasKey(t => t.CustomerId);

            // Properties
            this.Property(t => t.LastName)
                .HasMaxLength(150);

            this.Property(t => t.FirstName)
                .HasMaxLength(150);

            this.Property(t => t.Sex)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Email)
                .HasMaxLength(150);

            this.Property(t => t.PhoneNo)
                .HasMaxLength(250);

            this.Property(t => t.Address)
                .HasMaxLength(350);

            this.Property(t => t.Remarks)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Customer");
            this.Property(t => t.CustomerId).HasColumnName("CustomerId");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.Sex).HasColumnName("Sex");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.PhoneNo).HasColumnName("PhoneNo");
            this.Property(t => t.EntryDate).HasColumnName("EntryDate");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Remarks).HasColumnName("Remarks");

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
using System.Data.Entity.ModelConfiguration;
using VT.Data.Entities;

namespace VT.Data.Mapping
{
    internal class ContactPersonConfiguration : EntityTypeConfiguration<ContactPerson>
    {
        public ContactPersonConfiguration(string schema = "wfdb")
        {
            ToTable(schema + ".contactperson");
            HasKey(x => x.ContactPersonId);

            Property(x => x.FirstName).HasColumnName("FirstName").IsRequired().HasMaxLength(50);
            Property(x => x.LastName).HasColumnName("LastName").IsRequired().HasMaxLength(50);
            Property(x => x.MiddleName).HasColumnName("MiddleName").IsOptional().HasMaxLength(50);

            Property(x => x.Email).HasColumnName("Email").IsRequired().HasMaxLength(50);
            Property(x => x.Telephone).HasColumnName("Telephone").IsOptional().HasMaxLength(50);
            Property(x => x.Mobile).HasColumnName("Mobile").IsOptional().HasMaxLength(50);
            Property(x => x.Fax).HasColumnName("Fax").IsOptional().HasMaxLength(50);

            Property(x => x.ContactType).HasColumnName("ContactType").IsOptional().HasMaxLength(50);
        }
    }
}

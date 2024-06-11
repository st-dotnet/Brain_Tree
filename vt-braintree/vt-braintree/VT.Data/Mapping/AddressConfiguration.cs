using System.Data.Entity.ModelConfiguration;
using VT.Data.Entities;

namespace VT.Data.Mapping
{
    internal class AddressConfiguration : EntityTypeConfiguration<Address>
    {
        public AddressConfiguration(string schema = "wfdb")
        {
            ToTable(schema + ".address");
            HasKey(x => x.AddressId);

            Property(x => x.AddressType).HasColumnName("AddressType").IsOptional().HasMaxLength(20);
            Property(x => x.City).HasColumnName("City").IsOptional().HasMaxLength(50);
            Property(x => x.Country).HasColumnName("Country").IsOptional().HasMaxLength(50);
            Property(x => x.Territory).HasColumnName("Territory").IsOptional().HasMaxLength(50);
            Property(x => x.PostalCode).HasColumnName("PostalCode").IsOptional().HasMaxLength(10);
            Property(x => x.StreetAddress).HasColumnName("StreetAddress").IsOptional().HasMaxLength(250);
        }
    }
}

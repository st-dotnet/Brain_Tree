using System.Data.Entity.ModelConfiguration;
using VT.Data.Entities;

namespace VT.Data.Mapping
{
    internal class CustomerConfiguration : EntityTypeConfiguration<Customer>
    {
        public CustomerConfiguration(string schema = "wfdb")
        {
            ToTable(schema + ".customer");
            HasKey(x => x.CustomerId);

            Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);
            Property(x => x.CompanyId).HasColumnName("CompanyId").IsOptional();

            HasOptional(x => x.Company).WithMany(x => x.Customers).HasForeignKey(x => x.CompanyId);
            Property(x => x.GatewayCustomerId).HasColumnName("GatewayCustomerId").IsOptional().HasMaxLength(255);

            Property(x => x.IsDeleted).HasColumnName("IsDeleted").IsOptional();
            Property(x => x.IsCcActive).HasColumnName("IsActive").IsOptional();

            Property(x => x.CustomerJson).HasColumnName("CustomerJson").IsOptional();

            Property(x => x.Token).HasColumnName("Token").IsOptional();
            Property(x => x.ExpireAt).HasColumnName("ExpireAt").IsOptional();

            HasMany(x => x.Addresses)
                .WithMany(x => x.Customers)
                .Map(ca =>
                {
                    ca.MapLeftKey("CustomerId");
                    ca.MapRightKey("AddressId");
                    ca.ToTable("customeraddress");
                });

            HasMany(x => x.ContactPersons)
               .WithMany(x => x.Customers)
               .Map(ca =>
               {
                   ca.MapLeftKey("CustomerId");
                   ca.MapRightKey("ContactPersonId");
                   ca.ToTable("customercontactperson");
               });
        }
    }
}

using System.Data.Entity.ModelConfiguration;
using VT.Data.Entities;

namespace VT.Data.Mapping
{
    internal class CompanyConfiguration : EntityTypeConfiguration<Company>
    {
        public CompanyConfiguration(string schema = "wfdb")
        {
            ToTable(schema + ".company");
            HasKey(x => x.CompanyId);

            Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);
            Property(x => x.MerchantAccountId).HasColumnName("MerchantAccountId").IsOptional().HasMaxLength(255);
            Property(x => x.GatewayCustomerId).HasColumnName("GatewayCustomerId").IsOptional().HasMaxLength(255);
            Property(x => x.ServiceFeePercentage).HasColumnName("ServiceFeePercentage");
            Property(x => x.IsGpsOn).HasColumnName("IsGpsOn").IsOptional();
            Property(x => x.PaymentGatewayType).HasColumnName("PaymentGatewayType").IsRequired();
            Property(x => x.MerchantJson).HasColumnName("MerchantJson").IsOptional();
            Property(x => x.CustomerJson).HasColumnName("CustomerJson").IsOptional();
            Property(x => x.Threshold).HasColumnName("Threshold").IsOptional();
            HasMany(x => x.Addresses)
                .WithMany(x => x.Companies)
                .Map(ca =>
                {
                    ca.MapLeftKey("CompanyId");
                    ca.MapRightKey("AddressId");
                    ca.ToTable("companyaddress");
                });

            HasMany(x => x.ContactPersons)
                .WithMany(x => x.Companies)
                .Map(ca =>
                {
                    ca.MapLeftKey("CompanyId");
                    ca.MapRightKey("ContactPersonId");
                    ca.ToTable("companycontactperson");
                });
        }
    }
}

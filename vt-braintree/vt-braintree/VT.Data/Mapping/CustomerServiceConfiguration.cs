using System.Data.Entity.ModelConfiguration;
using VT.Data.Entities;

namespace VT.Data.Mapping
{
    internal class CustomerServiceConfiguration : EntityTypeConfiguration<CustomerService>
    {
        public CustomerServiceConfiguration(string schema = "wfdb")
        {
            ToTable(schema + ".customerservice");
            HasKey(x => x.CustomerServiceId );

            Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);
            Property(x => x.Description).HasColumnName("Description").IsRequired().HasMaxLength(1000);
            Property(x => x.Cost).HasColumnName("Cost").IsRequired();
            Property(x => x.IsDeleted).HasColumnName("IsDeleted").IsRequired();

            Property(x => x.CustomerId).HasColumnName("CustomerId").IsRequired();
            Property(x => x.CompanyServiceId).HasColumnName("CompanyServiceId").IsOptional();

            HasRequired(x => x.Customer).WithMany(x => x.CustomerServices).HasForeignKey(x => x.CustomerId);
            HasOptional(x => x.CompanyService).WithMany(x => x.CustomerServices).HasForeignKey(x => x.CompanyServiceId);

        }
    }
}

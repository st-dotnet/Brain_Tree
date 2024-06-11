using System.Data.Entity.ModelConfiguration;
using VT.Data.Entities;

namespace VT.Data.Mapping
{
    internal class CompanyServiceConfiguration : EntityTypeConfiguration<CompanyService>
    {
        public CompanyServiceConfiguration(string schema = "wfdb")
        {
            ToTable(schema + ".companyservice");
            HasKey(x => x.CompanyServiceId);

            Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);
            Property(x => x.Description).HasColumnName("Description").IsOptional().HasMaxLength(1000);
            Property(x => x.CompanyId).HasColumnName("CompanyId").IsOptional();
            Property(x => x.IsDeleted).HasColumnName("IsDeleted").IsRequired();

            HasOptional(x => x.Company).WithMany(x => x.CompanyServices).HasForeignKey(x => x.CompanyId);

        }
    }
}

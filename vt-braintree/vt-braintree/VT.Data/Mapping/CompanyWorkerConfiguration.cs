using System.Data.Entity.ModelConfiguration;
using VT.Data.Entities;

namespace VT.Data.Mapping
{
    internal class CompanyWorkerConfiguration : EntityTypeConfiguration<CompanyWorker>
    {
        public CompanyWorkerConfiguration(string schema = "wfdb")
        {
            ToTable(schema + ".companyworker");
            HasKey(x => x.CompanyWorkerId);

            Property(x => x.Email).HasColumnName("Email").IsRequired().HasMaxLength(50);
            Property(x => x.HashedPassword).HasColumnName("HashedPassword").IsRequired().HasMaxLength(100);
            Property(x => x.CompanyId).HasColumnName("CompanyId").IsOptional();

            Property(x => x.FirstName).HasColumnName("FirstName").IsRequired().HasMaxLength(50);
            Property(x => x.MiddleName).HasColumnName("MiddleName").IsOptional().HasMaxLength(50);
            Property(x => x.LastName).HasColumnName("LastName").IsRequired().HasMaxLength(50);
            Property(x => x.IsAdmin).HasColumnName("IsAdmin").IsRequired();
            Property(x => x.IsDeleted).HasColumnName("IsDeleted").IsRequired();

            HasOptional(x => x.Company).WithMany(x => x.CompanyWorkers).HasForeignKey(x => x.CompanyId);
        }
    }
}

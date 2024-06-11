using System.Data.Entity.ModelConfiguration;
using VT.Data.Entities;

namespace VT.Data.Mapping
{
    internal class ServiceRecordItemConfiguration : EntityTypeConfiguration<ServiceRecordItem>
    {
        public ServiceRecordItemConfiguration(string schema = "wfdb")
        {
            ToTable(schema + ".servicerecorditem");
            HasKey(x => x.ServiceRecordItemId);

            Property(x => x.ServiceRecordId).HasColumnName("ServiceRecordId").IsRequired();
            Property(x => x.CustomerId).HasColumnName("CustomerId").IsRequired();
            Property(x => x.CompanyServiceId).HasColumnName("CompanyServiceId").IsOptional();
            Property(x => x.CustomerServiceId).HasColumnName("CustomerServiceId").IsOptional();

            Property(x => x.ServiceName).HasColumnName("ServiceName").IsRequired().HasMaxLength(200);
            Property(x => x.Type).HasColumnName("Type").IsRequired();
            Property(x => x.CostOfService).HasColumnName("CostOfService").IsOptional();

            Property(x => x.StartTime).HasColumnName("StartTime").IsRequired();
            Property(x => x.EndTime).HasColumnName("EndTime").IsRequired();

            Property(x => x.Description).HasColumnName("Description").IsOptional().HasMaxLength(2000);

            HasOptional(x => x.CompanyService).WithMany(x => x.ServiceRecordItems).HasForeignKey(x => x.CompanyServiceId);
            HasOptional(x => x.CustomerService).WithMany(x => x.ServiceRecordItems).HasForeignKey(x => x.CustomerServiceId);
            HasRequired(x => x.ServiceRecord).WithMany(x => x.ServiceRecordItems).HasForeignKey(x => x.ServiceRecordId);
        }
    }
}

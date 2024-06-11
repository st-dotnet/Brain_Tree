using System.Data.Entity.ModelConfiguration;
using VT.Data.Entities;

namespace VT.Data.Mapping
{
    internal class ServiceRecordConfiguration : EntityTypeConfiguration<ServiceRecord>
    {
        public ServiceRecordConfiguration(string schema = "wfdb")
        {
            ToTable(schema + ".servicerecord");
            HasKey(x => x.ServiceRecordId);

            Property(x => x.CustomerId).HasColumnName("CustomerId").IsRequired();
            Property(x => x.CompanyId).HasColumnName("CompanyId").IsOptional();
            Property(x => x.CompanyWorkerId).HasColumnName("CompanyWorkerId").IsRequired();
            Property(x => x.Description).HasColumnName("Description").IsRequired().HasMaxLength(1500);
            Property(x => x.TotalAmount).HasColumnName("TotalAmount").IsOptional();

            Property(x => x.StartTime).HasColumnName("StartTime").IsOptional();
            Property(x => x.EndTime).HasColumnName("EndTime").IsRequired();

            Property(x => x.BilledToCompany).HasColumnName("BilledToCompany").IsRequired();
            Property(x => x.Status).HasColumnName("Status").IsRequired();
            Property(x => x.IsInvoiceSent).HasColumnName("IsInvoiceSent").IsOptional();

            Property(x => x.InvoiceDate).HasColumnName("InvoiceDate").IsOptional();
            Property(x => x.BtTransactionId).HasColumnName("BtTransactionId").IsOptional().HasMaxLength(250);

            Property(x => x.IsVoid).HasColumnName("IsVoid").IsRequired();
            Property(x => x.VoidTime).HasColumnName("VoidTime").IsOptional();


            HasOptional(x => x.Company).WithMany(x => x.ServiceRecords).HasForeignKey(x => x.CompanyId);
            HasRequired(x => x.Customer).WithMany(x => x.ServiceRecords).HasForeignKey(x => x.CustomerId);
            HasRequired(x => x.CompanyWorker).WithMany(x => x.ServiceRecords).HasForeignKey(x => x.CompanyWorkerId);
        }
    }
}

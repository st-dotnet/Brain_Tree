using System.Data.Entity.ModelConfiguration;
using VT.Data.Entities;

namespace VT.Data.Mapping
{
    internal class CommissionConfiguration : EntityTypeConfiguration<Commission>
    {
        public CommissionConfiguration(string schema = "wfdb")
        {
            ToTable(schema + ".commission");
            HasKey(x => x.CommissionId);
            Property(x => x.Date).HasColumnName("Date").IsOptional();
            Property(x => x.ServiceRecordId).HasColumnName("ServiceRecordId").IsOptional();
            Property(x => x.Amount).HasColumnName("Amount").IsOptional();
            Property(x => x.BtTransactionId).HasColumnName("BtTransactionId").IsOptional().HasMaxLength(100);
            Property(x => x.Type).HasColumnName("Type").IsOptional();
            HasRequired(x => x.ServiceRecord).WithMany(x => x.Commissions).HasForeignKey(x => x.ServiceRecordId);
        }
    }
}

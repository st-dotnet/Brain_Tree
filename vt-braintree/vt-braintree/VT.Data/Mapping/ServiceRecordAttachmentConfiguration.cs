using System.Data.Entity.ModelConfiguration;
using VT.Data.Entities;

namespace VT.Data.Mapping
{
    internal class ServiceRecordAttachmentConfiguration : EntityTypeConfiguration<ServiceRecordAttachment>
    {
        public ServiceRecordAttachmentConfiguration(string schema="wfdb")
        {
            ToTable(schema + ".servicerecordattachment");
            HasKey(x => x.ServiceRecordAttachmentId);

            Property(x => x.ServiceRecordItemId).HasColumnName("ServiceRecordItemId").IsRequired();
            Property(x => x.Url).HasColumnName("Url").IsRequired();
            Property(x => x.Description).HasColumnName("Description").IsOptional().HasMaxLength(1500);
            Property(x => x.Type).HasColumnName("Type").IsOptional().HasMaxLength(45);
            Property(x => x.Date).HasColumnName("Date").IsRequired();

            HasRequired(x => x.ServiceRecordItem).WithMany(x => x.ServiceRecordAttachments).HasForeignKey(x => x.ServiceRecordItemId);
        }
    }
}

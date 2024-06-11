using System.Data.Entity.ModelConfiguration;
using VT.Data.Entities;

namespace VT.Data.Mapping
{
    internal class CompanyWorkerCustomerConfiguration : EntityTypeConfiguration<CompanyWorkerCustomer>
    {
        public CompanyWorkerCustomerConfiguration()
        {
            ToTable("companyworkercustomer");

            HasKey(x => new
            {
                x.CompanyWorkerId,
                x.CustomerId
            });

            Property(x => x.CustomerOrder).HasColumnName("CustomerOrder").IsOptional();

            HasRequired(x => x.Customer).WithMany(x => x.CompanyWorkerCustomers).HasForeignKey(x => x.CustomerId);

            HasRequired(x => x.CompanyWorker).WithMany(x => x.AccessibleCustomers).HasForeignKey(x => x.CompanyWorkerId);
        }
    }
}

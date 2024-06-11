using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using VT.Data.Entities;
using VT.Data.Mapping;

namespace VT.Data.Context
{
    public class VerifyTechContext : DbContext, IVerifyTechContext
    {
         #region Constructor

        public VerifyTechContext()
            : base("name=ConnectionString")
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 180;
        }

        public VerifyTechContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        public VerifyTechContext(DbConnection existingConnection)
            : base(existingConnection, true)
        {
        }

        #endregion


        #region Table/Views

        public IDbSet<Address> Addresses { get; set; }  
        public IDbSet<CompanyWorker> CompanyWorkers { get; set; }  
        public IDbSet<Company> Companies { get; set; }  
        public IDbSet<CompanyService> CompanyServices { get; set; }  
        public IDbSet<Customer> Customers { get; set; }
        public IDbSet<CustomerService> CustomerServices { get; set; }
        public IDbSet<ContactPerson> ContactPersons { get; set; }
        public IDbSet<ServiceRecord> ServiceRecords { get; set; }
        public IDbSet<ServiceRecordItem> ServiceRecordItems { get; set; }
        public IDbSet<ServiceRecordAttachment> ServiceRecordAttachments { get; set; }
        public IDbSet<Commission> Commissions { get; set; }
        public IDbSet<CompanyWorkerCustomer> CompanyWorkerCustomers { get; set; } 

        #endregion


        #region Overridable method(s)

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new AddressConfiguration());
            modelBuilder.Configurations.Add(new CompanyConfiguration());
            modelBuilder.Configurations.Add(new CompanyWorkerConfiguration());
            modelBuilder.Configurations.Add(new CompanyServiceConfiguration());
            modelBuilder.Configurations.Add(new CustomerConfiguration());
            modelBuilder.Configurations.Add(new ContactPersonConfiguration());
            modelBuilder.Configurations.Add(new ServiceRecordConfiguration());
            modelBuilder.Configurations.Add(new ServiceRecordItemConfiguration());
            modelBuilder.Configurations.Add(new CustomerServiceConfiguration());
            modelBuilder.Configurations.Add(new ServiceRecordAttachmentConfiguration());
            modelBuilder.Configurations.Add(new CommissionConfiguration());
            modelBuilder.Configurations.Add(new CompanyWorkerCustomerConfiguration());
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message, exception.InnerException);
            }
        }

        #endregion


        public Database Db
        {
            get { return this.Database; }
        }

    }
}

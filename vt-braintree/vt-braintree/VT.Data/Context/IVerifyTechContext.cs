using System;
using System.Data.Entity;
using VT.Data.Entities;

namespace VT.Data.Context
{
    public interface IVerifyTechContext : IDisposable 
    {
        Database Db { get; }
        IDbSet<Address> Addresses { get; set; }
        IDbSet<CompanyWorker> CompanyWorkers { get; set; }
        IDbSet<Company> Companies { get; set; }
        IDbSet<CompanyService> CompanyServices { get; set; }
        IDbSet<Customer> Customers { get; set; }
        IDbSet<CustomerService> CustomerServices { get; set; }
        IDbSet<ContactPerson> ContactPersons { get; set; }
        IDbSet<ServiceRecord> ServiceRecords { get; set; }
        IDbSet<ServiceRecordItem> ServiceRecordItems { get; set; }
        IDbSet<ServiceRecordAttachment> ServiceRecordAttachments { get; set; }
        IDbSet<Commission> Commissions { get; set; }
        IDbSet<CompanyWorkerCustomer> CompanyWorkerCustomers { get; set; }

        int SaveChanges();
    }
}

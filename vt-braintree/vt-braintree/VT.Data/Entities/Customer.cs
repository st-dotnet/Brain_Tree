using System;
using System.Collections.Generic;

namespace VT.Data.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string GatewayCustomerId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsCcActive { get; set; }
        public DateTime? ExpireAt { get; set; }
        public string Token { get; set; }  
        public string CustomerJson { get; set; }
        public virtual Company Company { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<ContactPerson> ContactPersons { get; set; }
        public virtual ICollection<ServiceRecord> ServiceRecords { get; set; }
        public virtual IList<CustomerService> CustomerServices { get; set; }
        public virtual IList<CompanyWorkerCustomer> CompanyWorkerCustomers { get; set; } 
    }
}

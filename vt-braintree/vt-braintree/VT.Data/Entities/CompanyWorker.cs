using System.Collections.Generic;

namespace VT.Data.Entities
{
    public class CompanyWorker
    {
        public int CompanyWorkerId { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public int? CompanyId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool IsAdmin { get; set; }
        public string PasswordSalt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<ServiceRecord> ServiceRecords { get; set; } 
        public virtual ICollection<CompanyWorkerCustomer> AccessibleCustomers { get; set; }
    }
}

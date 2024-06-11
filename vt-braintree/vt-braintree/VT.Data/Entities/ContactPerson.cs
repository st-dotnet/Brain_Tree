using System.Collections.Generic;

namespace VT.Data.Entities
{
    public class ContactPerson
    {
        public int ContactPersonId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string ContactType { get; set; }

        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
    }
}

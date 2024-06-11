using System.Collections.Generic;

namespace VT.Data.Entities
{
    public class Address
    {
        public int AddressId { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Territory { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string AddressType { get; set; }

        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
    }
}

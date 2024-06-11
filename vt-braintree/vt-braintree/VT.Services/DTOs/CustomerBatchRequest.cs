using System.Collections.Generic;

namespace VT.Services.DTOs
{
    public class CustomerBatchRequest
    {
        public int CompanyId { get; set; }

        public List<ImportCustomerRequest> ImportCustomers { get; set; }
    }

    public class ImportCustomerRequest
    {
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }

        public string Status { get; set; }
        public string Reason { get; set; }
    }

}

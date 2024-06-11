using System.Collections.Generic;
using VT.Data;

namespace VT.Services.DTOs
{
    public class CustomerSaveRequest
    {
        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public string ContactFirstName { get; set; }
        public string ContactMiddleName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
        public string ContactMobile { get; set; }
        public PaymentMethod PaymentMethodType { get; set; }

        public List<CustomerServiceSaveRequest> CustomerServices { get; set; }

        public bool IsActive { get; set; }
    }

    public class CustomerServiceSaveRequest
    {
        public int CustomerServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}

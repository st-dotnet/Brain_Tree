using System.Collections.Generic;

namespace VT.Web.Models
{
    public class ImportCustomerListModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Css { get; set; }
        public List<ImportCustomerValidation> Data { get; set; }
    }

    public class ImportCustomerValidation
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
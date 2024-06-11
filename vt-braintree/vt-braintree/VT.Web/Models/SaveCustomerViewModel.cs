using System;
using System.ComponentModel.DataAnnotations;
using VT.Common;

namespace VT.Web.Models
{
    public class SaveCustomerViewModel
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
        public bool HasGatewayCustomer { get; set; }

        public bool IsActive { get; set; }

        //customer service
        public int CompanyServiceId { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
         
    }
    
    public class CustomerServiceViewModel : BaseCustomerServiceViewModel
    {
        public int CustomerServiceId { get; set; }
        [Required(ErrorMessage = "Company service is required.")]
        public int CompanyServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Price is required.")]
        public double Price { get; set; }

        public int ServiceRecordItemId { get; set; }
        public int ServiceRecordId { get; set; }
        public DateTime EndTime { get; set; }
        public string Type { get; set; }
        public Double? CostOfService { get; set; }

        public bool InMemoryDeleted { get; set; }
        public bool HasChanged { get; set; }
        public bool InMemoryAdded { get; set; }
    }

    public class BaseCustomerServiceViewModel
    {
        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
    }

    public class CompanyServiceModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class GatewayCustomerViewModel
    {
        public string ClientToken { get; set; }
        public int CustomerId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string CreditCard { get; set; }
        public string Expiration { get; set; }

        public string Nonce { get; set; }
        public string CcToken { get; set; }
        public string RedirectUrl { get; set; }

        public bool IsCustomerTokenExpired { get; set; }
        public bool IsCustomerNotFound { get; set; }

        public GatewayAccount AccountType { get; set; }
    }
}
using Newtonsoft.Json; 

namespace VT.Services.DTOs.SplashPayments
{
    public class SplashCreateCustomer
    {  

        [JsonProperty(PropertyName = "login")]
        public string LoginId { get; set; }

        [JsonProperty(PropertyName = "first")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "last")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "inactive")]
        public string Inactive { get; set; }

        [JsonProperty(PropertyName = "address1")]
        public string Address1 { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "zip")]
        public string Zip { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "fax")]
        public string Fax { get; set; }

        [JsonProperty(PropertyName = "method")]
        public string Method { get; set; }

        [JsonProperty(PropertyName = "number")]
        public string Number { get; set; }

        [JsonProperty(PropertyName = "cvv")]
        public string Cvv { get; set; }

        [JsonProperty(PropertyName = "expiration")]
        public string Expiration { get; set; }
 
    }

    public class SplashCreateCustomerRequest
    {
        public int CompanyId { get; set; }

        [JsonProperty(PropertyName = "login")]
        public string CustomerLoginId { get; set; }

        [JsonProperty(PropertyName = "first")]
        public string CustomerFirstName { get; set; }

        [JsonProperty(PropertyName = "last")]
        public string CustomerLastName { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string CustomerEmail { get; set; }

        [JsonProperty(PropertyName = "customerinactive")]
        public string CustomerInactive { get; set; }

        [JsonProperty(PropertyName = "address1")]
        public string CustomerAddress1 { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string CustomerCity { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string CustomerState { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string CustomerCountry { get; set; }

        [JsonProperty(PropertyName = "zip")]
        public string CustomerZip { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string CustomerPhone { get; set; }

        [JsonProperty(PropertyName = "fax")]
        public string CustomerFax { get; set; }

        [JsonProperty(PropertyName = "method")]
        public string PaymentMethod { get; set; }

        [JsonProperty(PropertyName = "number")]
        public string PaymentNumber { get; set; }

        [JsonProperty(PropertyName = "cvv")]
        public string PaymentCvv { get; set; }

        [JsonProperty(PropertyName = "expiration")]
        public string PaymentExpiration { get; set; }

        [JsonProperty(PropertyName = "inactive")]
        public string PaymentInactive { get; set; }


        [JsonProperty(PropertyName = "customer")]
        public SplashCreateCustomer Customer { get; set; }

        [JsonProperty(PropertyName = "payment")]
        public CustomerPaymentRequest Payment { get; set; }
    }
     
    public class CustomerPaymentRequest
    {
        [JsonProperty(PropertyName = "method")]
        public string Method { get; set; }

        [JsonProperty(PropertyName = "number")]
        public string Number { get; set; }

        [JsonProperty(PropertyName = "cvv")]
        public string Cvv { get; set; }

        [JsonProperty(PropertyName = "expiration")]
        public string Expiration { get; set; } 
    }

    public class CustomerPayment
    {
        [JsonProperty(PropertyName = "method")]
        public string Method { get; set; }

        [JsonProperty(PropertyName = "number")]
        public string Number { get; set; }

        [JsonProperty(PropertyName = "cvv")]
        public string Cvv { get; set; }

        [JsonProperty(PropertyName = "expiration")]
        public string Expiration { get; set; }

        [JsonProperty(PropertyName = "inactive")]
        public string Inactive { get; set; }
    }

    // create customer combo request
    public class CreateCustomerWithPaymentToken
    {
        [JsonProperty(PropertyName = "customer")]
        public SplashCreateCustomer Customer { get; set; }

        [JsonProperty(PropertyName = "payment")]
        public CustomerPaymentRequest Payment { get; set; }
    }

    public class AddCustomerCreditCardRequest
    {
        public int CompanyId { get; set; }

        public int CustomerId { get; set; }

        [JsonProperty(PropertyName = "customer")]
        public string Customer { get; set; }

        [JsonProperty(PropertyName = "method")]
        public string PaymentMethod { get; set; }

        [JsonProperty(PropertyName = "number")]
        public string PaymentNumber { get; set; }

        [JsonProperty(PropertyName = "cvv")]
        public string PaymentCvv { get; set; }

        [JsonProperty(PropertyName = "expiration")]
        public string PaymentExpiration { get; set; }

        [JsonProperty(PropertyName = "inactive")]
        public string PaymentInactive { get; set; }

        [JsonProperty(PropertyName = "payment")]
        public CustomerPaymentRequest Payment { get; set; }
    }

    public class AddCustomerCreditCard
    {
        [JsonProperty(PropertyName = "customer")]
        public string Customer { get; set; } 

        [JsonProperty(PropertyName = "inactive")]
        public string Inactive { get; set; }

        [JsonProperty(PropertyName = "payment")]
        public CustomerPaymentRequest Payment { get; set; }
    }

    public class DisableCreditCard
    {
        [JsonProperty(PropertyName = "inactive")]
        public string Inactive { get; set; }
    }

    public class SplashUpdateCustomerRequest 
    {
        public int CompanyId { get; set; } 
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerEmail { get; set; }   
    }
}

namespace VT.Services.DTOs.SplashPayments
{
    public class BaseSplashCreateCustomerRequest
    { 
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerEmail { get; set; }

        public string CustomerCreditCard { get; set; }
        public string CustomerExpiration { get; set; }

        public string Nonce { get; set; }
        public string CcToken { get; set; }
        public string RedirectUrl { get; set; }

        public bool IsCustomerTokenExpired { get; set; }
        public bool IsCustomerNotFound { get; set; }
    }

    public class  CreateCustomerRequest : BaseSplashCreateCustomerRequest
    {

    }

    public class UpdateSplashCustomerRequest 
    {
        public int CustomerId { get; set; }
        public int CompanyId { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}

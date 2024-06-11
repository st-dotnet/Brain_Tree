using VT.Common;

namespace VT.Services.DTOs
{
    public class GatewayCustomerRequest
    {
        public string ClientToken { get; set; }
        public int CustomerId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string NonceFromTheClient { get; set; }
        public string CcToken { get; set; }

        public GatewayAccount AccountType { get; set; }

        public string GatewayCustomerId { get; set; }

        // for Splash
        public string Expiration { get; set; }
        public string PaymentMethod { get; set; }
        public string CardOrAccountNumber { get; set; }
        public string Routing { get; set; }
    }
}

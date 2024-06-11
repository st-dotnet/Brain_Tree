namespace VT.Services.DTOs
{
    public class MerchantCreditCardDetailResponse : BaseResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CreditCardNumber { get; set; }
        public string Cvv { get; set; }
        public string Expiration { get; set; }
        public string CcToken { get; set; }
    }

    public class CustomerCreditCardDetailResponse : BaseResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CreditCardNumber { get; set; }
        public string Cvv { get; set; }
        public string Expiration { get; set; }
        public string CcToken { get; set; }
    }

    public class GetGatewayCustomerResponse : BaseResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CreditCardNumber { get; set; }
        public string Cvv { get; set; }
        public string Expiration { get; set; }
        public string CcToken { get; set; }
    } 
}

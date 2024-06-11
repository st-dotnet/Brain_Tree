namespace VT.Services.DTOs
{
    public class MerchantCreditCardDetailRequest
    {
        public int CompanyId { get; set; }
    }

    public class CustomerCreditCardDetailRequest
    {
        public int CustomerId { get; set; }
    }

    public class GetGatewayCustomerRequest
    {
        public bool IsMerchant { get; set; } // internal use
        public int CustomerId { get; set; }
    }
}

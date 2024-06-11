namespace VT.Services.DTOs
{
    public class GatewayTransactionRequest
    {
        public string MerchantId { get; set; }
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public decimal? ServiceFeeAmount { get; set; }
        public string DescriptorName { get; set; }
        public string DescriptorUrl { get; set; }
    }
}
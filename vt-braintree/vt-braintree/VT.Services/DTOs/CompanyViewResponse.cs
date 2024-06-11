using VT.Data;

namespace VT.Services.DTOs
{
    public class CompanyViewResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Users { get; set; }
        public int Services { get; set; }
        public int Customers { get; set; }
        public string MerchantAccountId { get; set; }
        public string GatewayCustomerId { get; set; }
        public int Threshold { get; set; }
        public bool IsGpsOn { get; set; }
        public PaymentGatewayType PaymentGatewayType { get; set; }
    }
}

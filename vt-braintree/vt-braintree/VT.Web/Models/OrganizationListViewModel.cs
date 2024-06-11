using VT.Data;

namespace VT.Web.Models
{
    public class OrganizationListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Users { get; set; }
        public int Services { get; set; }
        public int Customers { get; set; }
        public string MerchantAccountId { get; set; }
        public string GatewayCustomerId { get; set; } 
        public PaymentGatewayType PaymentGatewayType { get; set; } 
        public string MerchantConfiguredCss
        {
            get { return !string.IsNullOrEmpty(this.MerchantAccountId) ? "primary" : "warning"; }
        }
        public string MerchantConfiguredText
        {
            get { return !string.IsNullOrEmpty(this.MerchantAccountId) ? "Yes" : "No"; }
        }

        public string PaymentConfiguredCss
        {
            get { return !string.IsNullOrEmpty(this.GatewayCustomerId) ? "primary" : "warning"; }
        }
        public string PaymentConfiguredText
        {
            get { return !string.IsNullOrEmpty(this.GatewayCustomerId) ? "Yes" : "No"; }
        }
    }
}
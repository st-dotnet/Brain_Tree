namespace VT.Web.Models
{
    public class CustomerListViewModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string GatewayCustomerId { get; set; }
        public bool IsActive { get; set; }

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
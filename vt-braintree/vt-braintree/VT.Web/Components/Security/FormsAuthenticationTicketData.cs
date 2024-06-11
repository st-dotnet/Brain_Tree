using VT.Data;

namespace VT.Web.Components.Security
{
	public class FormsAuthenticationTicketData
	{
		public int UserId { get; set; }
		public string EmailAddress { get; set; }
		public string FullName { get; set; }
		public string[] Roles { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public bool HasMerchantAccount { get; set; }
        public bool HasGatewayCustomer { get; set; }

        public int PaymentGateway { get; set; }
    }
}
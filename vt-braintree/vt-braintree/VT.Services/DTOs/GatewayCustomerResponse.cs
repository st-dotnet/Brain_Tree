using System.Collections.Generic; 

namespace VT.Services.DTOs
{
    public class GatewayCustomerResponse : BaseResponse
    {
        public string ReferenceNumber { get; set; }
        public string MerchantId { get; set; }
        public string AccountId { get; set; }
        public string EntityId { get; set; }
        public string MemberId { get; set; }

        public string AnnualCCSales { get; set; }
        public string Established { get; set; }
        public string MerchantCategoryCode { get; set; }
        public string MerchantNew { get; set; }
        public string Status { get; set; }
        public string TCVersion { get; set; }
        public string EntityLoginId { get; set; }
        public string EntityAddress1 { get; set; }
        public string EntityCity { get; set; }
        public string EntityCountry { get; set; }
        public string EntityEmail { get; set; }
        public string EntityEmployerId { get; set; }
        public string EntityName { get; set; }
        public string EntityPhone { get; set; }
        public string EntityState { get; set; }
        public string EntityType { get; set; }
        public string EntityWebsite { get; set; }
        public string EntityZip { get; set; }
        public string CardOrAccountNumber { get; set; }
        public string AccountsPaymentMethod { get; set; }
        public string AccountsRoutingCode { get; set; }
        public string AccountsPrimary { get; set; }
        public string MemberTitle { get; set; }
        public string MemberDateOfBirth { get; set; }
        public string MemberDriverLicense { get; set; }
        public string MemberDriverLicenseState { get; set; }
        public string MemberEmail { get; set; }
        public string MemberFirstName { get; set; }
        public string MemberLastName { get; set; }
        public string MemberOwnerShip { get; set; }
        public string MemberPrimary { get; set; }
        public string MemberSocialSecurityNumber { get; set; }



    }

    public class DatumAllCustomers
    {
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public string login { get; set; }
        public object merchant { get; set; }
        public string first { get; set; }
        public object middle { get; set; }
        public string last { get; set; }
        public object company { get; set; }
        public string email { get; set; }
        public string fax { get; set; }
        public string phone { get; set; }
        public string country { get; set; }
        public string zip { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public object address2 { get; set; }
        public string address1 { get; set; }
        public int inactive { get; set; }
        public int frozen { get; set; }
    }

    public class Totals
    {
        public int count { get; set; }
    }

    public class Page
    {
        public int current { get; set; }
        public int last { get; set; }
    }

    public class DetailsAllCustomers
    {
        public int requestId { get; set; }
        public Totals totals { get; set; }
        public Page page { get; set; }
    }
    
    public class Response
    {
        public List<DatumAllCustomers> data { get; set; }
        public DetailsAllCustomers details { get; set; }
        public List<object> errors { get; set; }
    }

    public class SplashGatewayAllCustomers
    {
        public Response response { get; set; }
    } 
}

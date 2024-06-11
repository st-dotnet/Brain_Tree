using VT.Data;

namespace VT.Services.DTOs
{
    public class CompanySaveRequest: CompanyPreferencesRequest
    {
        public string Name { get; set; }
        public double ServiceFeePercentage { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string MerchantJson { get; set; }
        public PaymentGatewayType PaymentGatewayType { get; set; }

        public string ContactFirstName { get; set; }
        public string ContactMiddleName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
        public string ContactMobile { get; set; }
    }

    public class CompanyPreferencesRequest
    {
        public int OrganizationId { get; set; }
        public bool IsGpsOn { get; set; }
        public int Threshold { get; set; }
    }
}

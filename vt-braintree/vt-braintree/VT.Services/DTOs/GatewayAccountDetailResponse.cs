namespace VT.Services.DTOs
{
    public class GatewayAccountDetailResponse :BaseResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DateOfBirth { get; set; }
        public string Ssn { get; set; }
        public string IndStreetAddress { get; set; }
        public string IndLocality { get; set; }
        public string IndRegion { get; set; }
        public string IndPostalCode { get; set; }

        public string LegalName { get; set; }
        public string DbaName { get; set; }
        public string TaxId { get; set; }
        public string BusStreetAddress { get; set; }
        public string BusLocality { get; set; }
        public string BusRegion { get; set; }
        public string BusPostalCode { get; set; }

        public string Descriptor { get; set; }
        public string FundEmail { get; set; }
        public string FundMobilePhone { get; set; }
        public string AccountNumber { get; set; }
        public string RoutingNumber { get; set; }
        public bool TosAccepted { get; set; }
        public string Id { get; set; }
        public string MerchantAccountId { get; set; }
        public int CompanyId { get; set; }
    }
}

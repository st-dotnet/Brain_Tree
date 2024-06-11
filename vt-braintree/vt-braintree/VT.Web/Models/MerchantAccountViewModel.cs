using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VT.Common;

namespace VT.Web.Models
{
    public class MerchantAccountViewModel
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
        public string Destination { get; set; }
        public string FundEmail { get; set; }
        public string FundMobilePhone { get; set; }
        public string AccountNumber { get; set; }
        public string RoutingNumber { get; set; }

        public bool TosAccepted { get; set; }
        public string Id { get; set; }
        public string MerchantAccountId { get; set; }
        public int CompanyId { get; set; }
        public string RedirectUrl { get; set; }
    }

    public class MessageViewModel
    {
        public string Title { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public GatewayAccount Type { get; set; }
    }
}
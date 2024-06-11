using System.Collections.Generic;
using Newtonsoft.Json;

namespace VT.Services.DTOs.SplashPayments
{
    public class SplashCreateMerchant
    {
        [JsonProperty(PropertyName = "new")]
        public string MerchantNew { get; set; }

        [JsonProperty(PropertyName = "established")]
        public string Established { get; set; }

        [JsonProperty(PropertyName = "annualCCSales")]
        public string AnnualCCSales { get; set; }

        [JsonProperty(PropertyName = "mcc")]
        public string MerchantCategoryCode { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "tcVersion")]
        public string TCVersion { get; set; }
        
        [JsonProperty(PropertyName = "entity")]
        public SplashCreateEntity Entity { get; set; } 

        [JsonProperty(PropertyName = "members")]
        public List<SplashCreateMember> Members { get; set; }

    }


    public class SplashMerchantRequest
    {
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

    public class SplashCreateMember
    {
        [JsonProperty(PropertyName = "entity")]
        public string EntityId { get; set; }

        [JsonProperty(PropertyName = "merchant")]
        public string MerchantId { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "first")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "last")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "dob")]
        public string DateOfBirth { get; set; }

        [JsonProperty(PropertyName = "ownership")]
        public string OwnerShip { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "ssn")]
        public string SocialSecurityNumber { get; set; }

        [JsonProperty(PropertyName = "dl")]
        public string DriverLicense { get; set; }

        [JsonProperty(PropertyName = "dlstate")]
        public string DriverLicenseState { get; set; }

        [JsonProperty(PropertyName = "primary")]
        public string Primary { get; set; }

        [JsonProperty(PropertyName = "adddress1")]
        public string Address1 { get; set; }

        [JsonProperty(PropertyName = "address2")]
        public string Address2 { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "zip")]
        public string Zip { get; set; }

        [JsonProperty(PropertyName = "fax")]
        public string Fax { get; set; }
    }

    public class SplashCreateEntityRequest
    {
        [JsonProperty(PropertyName = "login")]
        public string LoginId { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "address1")]
        public string Address1 { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "zip")]
        public string Zip { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "ein")]
        public string EmployerId { get; set; }

        [JsonProperty(PropertyName = "website")]
        public string Website { get; set; }

        [JsonProperty(PropertyName = "fax")]
        public string Fax { get; set; }

        [JsonProperty(PropertyName = "address2")]
        public string Address2 { get; set; }

        [JsonProperty(PropertyName = "accounts")]
        public List<SplashCreateAccount> Accounts { get; set; }
    }

    public class SplashCreateAccount
    {
        [JsonProperty(PropertyName = "primary")]
        public string Primary { get; set; }

        [JsonProperty(PropertyName = "account")]
        public SplashAccount Account { get; set; }
    }

    public class SplashCreateAccountRequest
    {
        [JsonProperty(PropertyName = "primary")]
        public string Primary { get; set; }

        [JsonProperty(PropertyName = "method")]
        public string AccountPaymentMethod { get; set; }

        [JsonProperty(PropertyName = "number")]
        public string AccountCardOrAccountNumber { get; set; }

        [JsonProperty(PropertyName = "routing")]
        public string AccountRoutingCode { get; set; }


        [JsonProperty(PropertyName = "account")]
        public SplashAccount Account { get; set; }
    }

    public class SplashAccount
    {
        [JsonProperty(PropertyName = "method")]
        public string PaymentMethod { get; set; }

        [JsonProperty(PropertyName = "number")]
        public string CardOrAccountNumber { get; set; }

        [JsonProperty(PropertyName = "routing")]
        public string RoutingCode { get; set; }
    }
}

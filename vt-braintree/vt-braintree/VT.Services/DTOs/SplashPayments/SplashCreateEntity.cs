using System.Collections.Generic;
using Newtonsoft.Json;

namespace VT.Services.DTOs.SplashPayments
{
    public abstract class BaseSplashCreateEntity
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

    public class SplashCreateEntity : BaseSplashCreateEntity
    { 
    }

    public class SplashUpdateEntity : BaseSplashCreateEntity
    { 
    }
}
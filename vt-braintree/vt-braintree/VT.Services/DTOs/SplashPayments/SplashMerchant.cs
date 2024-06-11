using System.Collections.Generic;
using Newtonsoft.Json;

namespace VT.Services.DTOs.SplashPayments
{
    public abstract class BaseSplashMerchant
    {  
        [JsonProperty(PropertyName = "established")]
        public string Established { get; set; }

        [JsonProperty(PropertyName = "annualCCSales")]
        public string AnnualCCSales { get; set; }

        [JsonProperty(PropertyName = "mcc")]
        public string MerchantCategoryCode { get; set; } 
    }

    public class SplashMerchantCreate : BaseSplashMerchant
    {
        [JsonProperty(PropertyName = "new")]
        public string MerchantNew { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "tcVersion")]
        public string TCVersion { get; set; }
    }

    public class SplashMerchantUpdate : BaseSplashMerchant
    { 
    }
}
using Newtonsoft.Json;

namespace VT.Services.DTOs.SplashPayments
{
    public class BaseSplashAccountRequest
    {
        [JsonProperty(PropertyName = "primary")]
        public string Primary { get; set; }

        [JsonProperty(PropertyName = "method")]
        public string AccountPaymentMethod { get; set; }

        [JsonProperty(PropertyName = "number")]
        public string AccountCardOrAccountNumber { get; set; }

        [JsonProperty(PropertyName = "routing")]
        public string AccountRoutingCode { get; set; }
    }

    public class SplashAccountUpdateRequest : BaseSplashAccountRequest
    {
    }

    public class SplashUpdateAccount : BaseSplashAccountRequest
    {
    }
}

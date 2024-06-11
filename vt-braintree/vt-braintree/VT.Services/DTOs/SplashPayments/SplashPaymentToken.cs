using Newtonsoft.Json;

namespace VT.Services.DTOs.SplashPayments
{
    public class SplashPaymentToken
    {
        [JsonProperty(PropertyName = "method")]
        public string PaymentMethod { get; set; }

        [JsonProperty(PropertyName = "number")] 
        public string CardOrAccountNumber { get; set; }

        [JsonProperty(PropertyName = "routing")]
        public string RoutingCode { get; set; }

    }
}

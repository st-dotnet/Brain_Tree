using Newtonsoft.Json;

namespace VT.Services.DTOs.SplashPayments
{
    public class SplashCreateToken
    {
        [JsonProperty(PropertyName = "customer")]
        public string Customer { get; set; }

        [JsonProperty(PropertyName = "payment")]
        public SplashPaymentToken PaymentMethod { get; set; }

        [JsonProperty(PropertyName = "expiration")]
        public string Expiration { get; set; }
    }
}
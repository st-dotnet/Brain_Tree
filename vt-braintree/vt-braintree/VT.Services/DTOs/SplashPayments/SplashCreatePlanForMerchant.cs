using Newtonsoft.Json;

namespace VT.Services.DTOs.SplashPayments
{
    public class SplashCreatePlanForMerchant
    {
        [JsonProperty(PropertyName = "merchant")]
        public string MerchantId { get; set; }

        [JsonProperty(PropertyName = "schedule")]
        public string Schedule { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }

    public class SplashCreatePlanForMerchantRequest
    {
        [JsonProperty(PropertyName = "merchant")]
        public string MerchantId { get; set; }

        [JsonProperty(PropertyName = "schedule")]
        public string Schedule { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}

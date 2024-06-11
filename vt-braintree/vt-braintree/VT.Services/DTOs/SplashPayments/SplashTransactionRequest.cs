using Newtonsoft.Json;

namespace VT.Services.DTOs.SplashPayments
{ 
    public class SplashTransactionRequest
    {
        public decimal Amount { get; set; }
        public string MerchantId { get; set; }
        public string CustomerJson { get; set; } // customer table 
        public string Descriptor { get; set; }
    }

    public class SplashTransactionResult : BaseResponse
    {
        public string TransactionId { get; set; }
    }

    public class SplashTransaction 
    {
        [JsonProperty(PropertyName = "merchant")]
        public string MerchantId { get; set; }

        [JsonProperty(PropertyName = "login")]
        public string LoginId { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string BankAccount { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "mcc")]
        public string MCC { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "origin")]
        public string Origin { get; set; }

        [JsonProperty(PropertyName = "total")]
        public string Total { get; set; }

        [JsonProperty(PropertyName = "mid")]
        public string Mid { get; set; } 
    }

}

using System.Collections.Generic;
using Newtonsoft.Json;

namespace VT.Services.DTOs.SplashPayments
{
    public class SplashTokenResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string TokenId { get; set; }

        [JsonProperty(PropertyName = "created")]
        public string Created { get; set; }

        [JsonProperty(PropertyName = "modified")]
        public string Modified { get; set; }

        [JsonProperty(PropertyName = "creator")]
        public string Creator { get; set; }

        [JsonProperty(PropertyName = "modifier")]
        public string Modifier { get; set; }

        [JsonProperty(PropertyName = "customer")]
        public string CustomerId { get; set; }

        [JsonProperty(PropertyName = "payment")]
        public string PaymentMethod { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "expiration")]
        public string Expiration { get; set; }

        [JsonProperty(PropertyName = "inactive")]
        public string InActive { get; set; }

        [JsonProperty(PropertyName = "frozen")]
        public string Frozen { get; set; }

    }

    public class DatumToken
    {
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public string customer { get; set; }
        public string token { get; set; }
        public string expiration { get; set; }
        public int inactive { get; set; }
        public int frozen { get; set; }
    }

    public class DetailsToken
    {
        public int requestId { get; set; }
        public Totals totals { get; set; }
        public Page page { get; set; }
    }


    public class ResponseToken
    {
        public List<DatumToken> data { get; set; }
        public DetailsToken details { get; set; }
        public List<object> errors { get; set; }
    }

    public class TokenResponse
    {
        public ResponseToken response { get; set; }
    }
}


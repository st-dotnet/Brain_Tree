using Newtonsoft.Json;

namespace VT.Services.DTOs.SplashPayments
{
    public class BaseSplashMemeberRequest
    {
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
        public string Ownership { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "ssn")]
        public string Ssn { get; set; }

        [JsonProperty(PropertyName = "dl")]
        public string Dl { get; set; }

        [JsonProperty(PropertyName = "dlstate ")]
        public string DlState { get; set; }

        [JsonProperty(PropertyName = "primary")]
        public string Primary { get; set; }
    }

    public class SplashCreateMemberRequest : BaseSplashMemeberRequest
    {
    }

    public class SplashUpdateMember : BaseSplashMemeberRequest
    {
    }
}

using System.Collections.Generic;
using Newtonsoft.Json;

namespace VT.Services.DTOs.SplashPayments
{
    public class SplashMerchantResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string MerchantId { get; set; }

        [JsonProperty(PropertyName = "created")]
        public string Created { get; set; }

        [JsonProperty(PropertyName = "modified")]
        public string Modified { get; set; }

        [JsonProperty(PropertyName = "creator")]
        public string Creator { get; set; }

        [JsonProperty(PropertyName = "modifier")]
        public string Modifier { get; set; }

        [JsonProperty(PropertyName = "lastActivity")]
        public string LastActivity { get; set; }

        [JsonProperty(PropertyName = "entity")]
        public string Entity { get; set; }

        [JsonProperty(PropertyName = "dba")]
        public string DBA { get; set; }

        [JsonProperty(PropertyName = "new")]
        public string MerchantNew { get; set; }

        [JsonProperty(PropertyName = "established")]
        public string Established { get; set; }

        [JsonProperty(PropertyName = "annualCCSales")]
        public string AnnualCCSales { get; set; }

        [JsonProperty(PropertyName = "mcc")]
        public string MerchantCreditCode { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "boarded")]
        public string Boarded { get; set; }

        [JsonProperty(PropertyName = "tcVersion")]
        public string TCVersion { get; set; }

        [JsonProperty(PropertyName = "inactive")]
        public string Inactive { get; set; }
    }

    public class SplashMerchantAccount2
    {
        public string method { get; set; }
        public string number { get; set; }
        public string routing { get; set; }
        public object payment { get; set; }
    }

    public class SplashMerchantAccount
    {
        public SplashMerchantAccount2 account { get; set; }
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public string entity { get; set; }
        public string token { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string primary { get; set; }
        public string status { get; set; }
        public string currency { get; set; }
        public int inactive { get; set; }
        public int frozen { get; set; }
    }

    public class SplashMerchantEntity
    {
        public List<SplashMerchantAccount> accounts { get; set; }
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public string ipCreated { get; set; }
        public string ipModified { get; set; }
        public object clientIp { get; set; }
        public string login { get; set; }
        public object parameter { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string address1 { get; set; }
        public object address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
        public object fax { get; set; }
        public string email { get; set; }
        public string website { get; set; }
        public string ein { get; set; }
        public string currency { get; set; }
        public string custom { get; set; }
        public int inactive { get; set; }
        public int frozen { get; set; }
    }

    public class SplashMerchantMember
    {
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public string merchant { get; set; }
        public string title { get; set; }
        public string first { get; set; }
        public object middle { get; set; }
        public string last { get; set; }
        public string ssn { get; set; }
        public string dob { get; set; }
        public string dl { get; set; }
        public string dlstate { get; set; }
        public string ownership { get; set; }
        public string email { get; set; }
        public object fax { get; set; }
        public object phone { get; set; }
        public object country { get; set; }
        public object zip { get; set; }
        public object state { get; set; }
        public object city { get; set; }
        public object address2 { get; set; }
        public object address1 { get; set; }
        public string primary { get; set; }
        public int inactive { get; set; }
        public int frozen { get; set; }
    }

    public class SplashMerchantDatum
    {
        public SplashMerchantEntity entity { get; set; }
        public List<SplashMerchantMember> members { get; set; }
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public string lastActivity { get; set; }
        public string dba { get; set; }
        public string @new { get; set; }
        public string established { get; set; }
        public string annualCCSales { get; set; }
        public int avgTicket { get; set; }
        public object amex { get; set; }
        public object discover { get; set; }
        public string mcc { get; set; }
        public string status { get; set; }
        public object boarded { get; set; }
        public string tinStatus { get; set; }
        public string tcVersion { get; set; }
        public string tcDate { get; set; }
        public int inactive { get; set; }
        public int frozen { get; set; }
    }

    public class SplashMerchantDetails
    {
        public int requestId { get; set; }
    }

    public class SplashMerchantGatewayResponse
    {
        public List<SplashMerchantDatum> data { get; set; }
        public SplashMerchantDetails details { get; set; }
        public List<SplashError> errors { get; set; }
    }

    public class SplashGatewayMerchantResponse : BaseResponse
    {
        public SplashMerchantGatewayResponse response { get; set; }
    }

    public class SplashError
    { 
        public string field { get; set; }
        public int code { get; set; }
        public int severity { get; set; }
        public string msg { get; set; }
    }
}

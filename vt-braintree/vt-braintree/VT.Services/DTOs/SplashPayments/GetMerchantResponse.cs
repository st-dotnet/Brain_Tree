using System.Collections.Generic;

namespace VT.Services.DTOs.SplashPayments
{
    public class GetMerchantResponse : BaseResponse
    {
        public MerchantResponse response { get; set; }
    }

    public class MerchantDatum
    {
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public object lastActivity { get; set; }
        public string entity { get; set; }
        public object dba { get; set; }
        public int @new { get; set; }
        public int established { get; set; }
        public int annualCCSales { get; set; }
        public int avgTicket { get; set; }
        public object amex { get; set; }
        public object discover { get; set; }
        public string mcc { get; set; }
        public int status { get; set; }
        public int boarded { get; set; }
        public int tinStatus { get; set; }
        public int tcVersion { get; set; }
        public string tcDate { get; set; }
        public int inactive { get; set; }
        public int frozen { get; set; }
    }

    public class MerchantTotals
    {
        public int count { get; set; }
    }

    public class MerchantPage
    {
        public int current { get; set; }
        public int last { get; set; }
    }

    public class MerchantDetails
    {
        public int requestId { get; set; }
        public MerchantTotals totals { get; set; }
        public MerchantPage page { get; set; }
    }

    public class MerchantResponse
    {
        public List<MerchantDatum> data { get; set; }
        public MerchantDetails details { get; set; }
        public List<object> errors { get; set; }
    } 
}

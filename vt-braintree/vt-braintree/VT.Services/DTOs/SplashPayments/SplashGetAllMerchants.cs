using System.Collections.Generic;

namespace VT.Services.DTOs.SplashPayments
{
    public class SplashGetAllMerchants
    {
        public MerchantsResponse response { get; set; }
    }

    public class DatumMerchants
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
        public object mcc { get; set; }
        public int status { get; set; }
        public object boarded { get; set; }
        public int tinStatus { get; set; }
        public int tcVersion { get; set; }
        public string tcDate { get; set; }
        public int inactive { get; set; }
        public int frozen { get; set; }
    }

    public class TotalMerchants
    {
        public int count { get; set; }
    }

    public class PageMerchants
    {
        public int current { get; set; }
        public int last { get; set; }
    }

    public class DetailsMerchants
    {
        public int requestId { get; set; }
        public TotalMerchants totals { get; set; }
        public PageMerchants page { get; set; }
    }

    public class MerchantsResponse
    {
        public List<DatumMerchants> data { get; set; }
        public DetailsMerchants details { get; set; }
        public List<object> errors { get; set; }
    }
}

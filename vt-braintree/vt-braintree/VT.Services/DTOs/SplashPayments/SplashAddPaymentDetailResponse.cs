using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT.Services.DTOs.SplashPayments
{
    public class Payment
    {
        public int method { get; set; }
        public string number { get; set; }
        public string routing { get; set; }
        public object payment { get; set; }
    }

    public class SplashPaymentDetailAddDatum
    {
        public Payment payment { get; set; }
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

    public class SplashPaymentDetailAddDetails
    {
        public int requestId { get; set; }
    }

    public class SplashPaymentDetailAddResponse
    {
        public List<SplashPaymentDetailAddDatum> data { get; set; }
        public SplashPaymentDetailAddDetails details { get; set; }
        public List<SplashError> errors { get; set; }
    }

    public class SplashAddPaymentDetailResponse
    {
        public SplashPaymentDetailAddResponse response { get; set; }
    }
}

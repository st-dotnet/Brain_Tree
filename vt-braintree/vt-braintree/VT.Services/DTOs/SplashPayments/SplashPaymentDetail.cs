using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT.Services.DTOs.SplashPayments
{
    public class PaymentDetailDatum
    {
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public string customer { get; set; }
        public string token { get; set; }
        public string expiration { get; set; }
        public string inactive { get; set; }
        public int frozen { get; set; }
    }

    public class PaymentDetailDetails
    {
        public int requestId { get; set; }
    }

    public class SplashPaymentDetailCustomerResponse
    {
        public List<PaymentDetailDatum> data { get; set; }
        public PaymentDetailDetails details { get; set; }
        public List<SplashError> errors { get; set; }
    }

    public class SplashPaymentDetailResponse
    {
        public SplashPaymentDetailCustomerResponse response { get; set; }
    }
}

using System.Collections.Generic;

namespace VT.Services.DTOs.SplashPayments
{
    public class SplashDeleteCustomerResponse
    {
        public ResponseSplashCustomer response { get; set; }
    }

    public class DatumDeleteSplash
    {
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public string login { get; set; }
        public object merchant { get; set; }
        public string first { get; set; }
        public object middle { get; set; }
        public string last { get; set; }
        public object company { get; set; }
        public string email { get; set; }
        public string fax { get; set; }
        public string phone { get; set; }
        public string country { get; set; }
        public string zip { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public object address2 { get; set; }
        public string address1 { get; set; }
        public int inactive { get; set; }
        public int frozen { get; set; }
    }

    public class DetailsDeleteSplash
    {
        public int requestId { get; set; }
    }

    public class ResponseSplashCustomer
    {
        public List<DatumDeleteSplash> data { get; set; }
        public DetailsDeleteSplash details { get; set; }
        public List<object> errors { get; set; }
    }

}

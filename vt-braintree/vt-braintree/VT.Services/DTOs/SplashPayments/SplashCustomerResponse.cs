using System.Collections.Generic;
using Newtonsoft.Json;

namespace VT.Services.DTOs.SplashPayments
{ 
    public class DatumSplashResponse
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
        public object fax { get; set; }
        public object phone { get; set; }
        public object country { get; set; }
        public object zip { get; set; }
        public object state { get; set; }
        public object city { get; set; }
        public object address2 { get; set; }
        public object address1 { get; set; }
        public string inactive { get; set; }
        public int frozen { get; set; }
    }

    public class DetailsSplashResponse
    {
        public int requestId { get; set; }
    } 

    public class SplashResponse
    {
        public List<DatumSplashResponse> data { get; set; }
        public DetailsSplashResponse details { get; set; }
        public List<SplashError> errors { get; set; }
    }

    public class SplashGatewayCustomerResponse : BaseResponse
    {
        public SplashResponse response { get; set; }
    }

    public class GetSplashCustomerResponse 
    {
        public SplashResponse response { get; set; }
    } 
}

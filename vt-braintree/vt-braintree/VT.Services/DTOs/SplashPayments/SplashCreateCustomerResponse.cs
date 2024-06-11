using System.Collections.Generic;
  
namespace VT.Services.DTOs.SplashPayments
{ 
    public class SplashCreateCustomerResponseCustomer
    {
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public string login { get; set; }
        public string merchant { get; set; }
        public string first { get; set; }
        public object middle { get; set; }
        public string last { get; set; }
        public string company { get; set; }
        public string email { get; set; }
        public string fax { get; set; }
        public string phone { get; set; }
        public string country { get; set; }
        public string zip { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string address2 { get; set; }
        public string address1 { get; set; }
        public string inactive { get; set; }
        public int frozen { get; set; }
    }

    public class SplashCreateCustomerResponsePayment
    {
        public int method { get; set; }
        public string number { get; set; }
        public string routing { get; set; }
        public object payment { get; set; }
    }

    public class SplashCreateCustomerResponseDatum
    {
        public SplashCreateCustomerResponseCustomer customer { get; set; }
        public SplashCreateCustomerResponsePayment payment { get; set; }
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public string token { get; set; }
        public string expiration { get; set; }
        public int inactive { get; set; }
        public int frozen { get; set; }
    }

    public class SplashCreateCustomerResponseDetails
    {
        public int requestId { get; set; }
    }

    public class SplashCreateCustomerResponseResponse
    {
        public List<SplashCreateCustomerResponseDatum> data { get; set; }
        public SplashCreateCustomerResponseDetails details { get; set; }
        public List<SplashError> errors { get; set; }
    }

    public class SplashCreateCustomerResponseObject
    {
        public SplashCreateCustomerResponseResponse response { get; set; }
    }
}

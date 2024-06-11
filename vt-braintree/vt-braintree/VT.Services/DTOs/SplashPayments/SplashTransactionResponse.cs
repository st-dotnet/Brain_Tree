using System.Collections.Generic;

namespace VT.Services.DTOs.SplashPayments
{
    public class SplashTransactionResponse 
    {
        public ResponseTransaction response { get; set; }
    }

    public class DatumTransaction
    {
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public string ipCreated { get; set; }
        public string ipModified { get; set; }
        public string merchant { get; set; }
        public string token { get; set; }
        public object fortxn { get; set; }
        public object batch { get; set; }
        public object subscription { get; set; }
        public string type { get; set; }
        public string expiration { get; set; }
        public string currency { get; set; }
        public object authDate { get; set; }
        public object authCode { get; set; }
        public object captured { get; set; }
        public object settled { get; set; }
        public object settledCurrency { get; set; }
        public object settledTotal { get; set; }
        public int allowPartial { get; set; }
        public string order { get; set; }
        public object description { get; set; }
        public object terminal { get; set; }
        public string origin { get; set; }
        public object tax { get; set; }
        public string total { get; set; }
        public object cashback { get; set; }
        public string authorization { get; set; }
        public string approved { get; set; }
        public int cvv { get; set; }
        public int swiped { get; set; }
        public int emv { get; set; }
        public int signature { get; set; }
        public object unattended { get; set; }
        public string first { get; set; }
        public object middle { get; set; }
        public string last { get; set; }
        public object company { get; set; }
        public string email { get; set; }
        public object address1 { get; set; }
        public object address2 { get; set; }
        public object city { get; set; }
        public object state { get; set; }
        public object zip { get; set; }
        public object country { get; set; }
        public object phone { get; set; }
        public string status { get; set; }
        public int refunded { get; set; }
        public string reserved { get; set; }
        public string checkStage { get; set; }
        public int inactive { get; set; }
        public int frozen { get; set; }
    }

    public class DetailsTransaction
    {
        public int requestId { get; set; }
    }

    public class ResponseTransaction
    {
        public List<DatumTransaction> data { get; set; }
        public DetailsTransaction details { get; set; }
        public List<SplashError> errors { get; set; }
    }
}

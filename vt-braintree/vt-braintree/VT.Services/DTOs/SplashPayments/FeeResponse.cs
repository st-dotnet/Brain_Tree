using System.Collections.Generic;

namespace VT.Services.DTOs.SplashPayments
{
    public class FeeResponse
    {
        public ResponseFee response { get; set; }
    }

    public class DatumFee
    {
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public string entity { get; set; }
        public object forentity { get; set; }
        public object org { get; set; }
        public object fromentity { get; set; }
        public object name { get; set; }
        public string description { get; set; }
        public string schedule { get; set; }
        public int scheduleFactor { get; set; }
        public string start { get; set; }
        public string um { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public int inactive { get; set; }
        public int frozen { get; set; }
    }

    public class FeeDetailsFee
    {
        public int requestId { get; set; }
    }

    public class ResponseFee
    {
        public List<DatumFee> data { get; set; }
        public FeeDetailsFee details { get; set; }
        public List<SplashError> errors { get; set; }
    }
}

using System.Collections.Generic;

namespace VT.Services.DTOs.SplashPayments
{
    public class SplashPlanResponse
    {
        public ResponseTestPlan response { get; set; }
    }

    public class DatumTestPlan
    {
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public string merchant { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string schedule { get; set; }
        public int scheduleFactor { get; set; }
        public string amount { get; set; }
        public int inactive { get; set; }
        public int frozen { get; set; }
    }

    public class DetailsTestPlan
    {
        public int requestId { get; set; }
    }

    public class ResponseTestPlan
    {
        public List<DatumTestPlan> data { get; set; }
        public DetailsTestPlan details { get; set; }
        public List<object> errors { get; set; }
    }
}

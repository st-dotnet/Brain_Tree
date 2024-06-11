using System.Collections.Generic;

namespace VT.Services.DTOs.SplashPayments
{
    public class GetMemberResponse : BaseResponse
    {
        public MemberResponse response { get; set; }
    }

    public class MemberDatum
    {
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public string merchant { get; set; }
        public string title { get; set; }
        public string first { get; set; }
        public object middle { get; set; }
        public string last { get; set; }
        public string ssn { get; set; }
        public string dob { get; set; }
        public string dl { get; set; }
        public string dlstate { get; set; }
        public int ownership { get; set; }
        public string email { get; set; }
        public object fax { get; set; }
        public object phone { get; set; }
        public object country { get; set; }
        public object zip { get; set; }
        public object state { get; set; }
        public object city { get; set; }
        public object address2 { get; set; }
        public object address1 { get; set; }
        public int primary { get; set; }
        public int inactive { get; set; }
        public int frozen { get; set; }
    }

    public class MemberTotals
    {
        public int count { get; set; }
    }

    public class MemberPage
    {
        public int current { get; set; }
        public int last { get; set; }
    }

    public class MemberDetails
    {
        public int requestId { get; set; }
        public MemberTotals totals { get; set; }
        public MemberPage page { get; set; }
    }

    public class MemberResponse
    {
        public List<MemberDatum> data { get; set; }
        public MemberDetails details { get; set; }
        public List<object> errors { get; set; }
    }

}

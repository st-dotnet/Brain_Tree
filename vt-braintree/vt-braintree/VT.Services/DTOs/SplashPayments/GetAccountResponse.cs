using System.Collections.Generic;

namespace VT.Services.DTOs.SplashPayments
{
    public class GetAccountResponse : BaseResponse
    {
        public AccountResponse response { get; set; }
    }

    public class AccountDatum
    {
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public string entity { get; set; }
        public string token { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int primary { get; set; }
        public int status { get; set; }
        public string currency { get; set; }
        public int inactive { get; set; }
        public int frozen { get; set; }
    }

    public class AccountTotals
    {
        public int count { get; set; }
    }

    public class AccountPage
    {
        public int current { get; set; }
        public int last { get; set; }
    }

    public class AccountDetails
    {
        public int requestId { get; set; }
        public AccountTotals totals { get; set; }
        public AccountPage page { get; set; }
    }

    public class AccountResponse
    {
        public List<AccountDatum> data { get; set; }
        public AccountDetails details { get; set; }
        public List<object> errors { get; set; }
    } 
}

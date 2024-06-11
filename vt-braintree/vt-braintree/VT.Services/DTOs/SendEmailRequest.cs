using VT.Common;
using VT.Data.Entities;

namespace VT.Services.DTOs
{
    public class SendEmailRequest : BaseEmailRequest
    {
        public ServiceRecord ServiceRecord { get; set; }
        public bool SetPaidExternal { get; set; }
    }

    public class SendCustomerEmailRequest : BaseEmailRequest
    {
        public ServiceRecord ServiceRecord { get; set; }
        public int CustomerId { get; set; } 
        public string SetCreditCardUrl { get; set; }
    }

    public class SendMultipleServicesEmailRequest : BaseEmailRequest
    {
        public int[] ServiceRecords { get; set; }
        public int CustomerId { get; set; }
        public bool IsBillingFromAdmin { get; set; }
        public bool SetPaidExternal { get; set; }
    }

    public class BaseEmailRequest
    {
        public string PdfTemplate { get; set; }
        public string Template { get; set; }
        public string FromEmail { get; set; }
    }
}

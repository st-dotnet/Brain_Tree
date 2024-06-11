namespace VT.Services.DTOs
{
    public class ChargeCustomerRequest
    {
        public int CompanyId { get; set; }
        public decimal Amount { get; set; }
        public string EmailTemplatePath { get; set; }
        public string ServiceRecordIds { get; set; }
    }
    
    public class ChargeCustomerCcRequest
    {
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string ServiceRecordIds { get; set; }
        public string EmailTemplatePath { get; set; }
        public bool ChargeExternal { get; set; }
    }
    public class GetChargeCustomerRequest
    {
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string ServiceRecordIds { get; set; }
        public string EmailTemplatePath { get; set; }
        public bool ChargeExternal { get; set; }
    
    }
}

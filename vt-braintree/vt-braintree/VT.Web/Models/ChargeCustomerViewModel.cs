namespace VT.Web.Models
{
    public class ChargeCustomerViewModel
    {
        public int CompanyId { get; set; }
        public decimal Amount { get; set; }
        public string ServiceRecordIds { get; set; }
    }

    public class ChargeCustomerAccountViewModel
    {
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string ServiceRecordIds { get; set; }
        public bool ChargeExternal { get; set; }
    }
}
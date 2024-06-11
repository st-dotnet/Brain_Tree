namespace VT.Services.DTOs
{
    public class SaveCustomerServiceRequest
    {
        public int CustomerServiceId { get; set; }
        public int CompanyServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
    }
}

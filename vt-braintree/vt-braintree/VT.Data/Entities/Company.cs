using System.Collections.Generic;

namespace VT.Data.Entities
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public string MerchantAccountId { get; set; }
        public string GatewayCustomerId { get; set; }
        public double ServiceFeePercentage { get; set; }
        public bool IsGpsOn { get; set; }
        public int? Threshold { get; set; }
        public PaymentGatewayType PaymentGatewayType { get; set; }
        public string MerchantJson { get; set; }
        public string CustomerJson { get; set; }

        public virtual ICollection<CompanyWorker> CompanyWorkers { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<ContactPerson> ContactPersons { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }

        public virtual ICollection<CompanyService> CompanyServices { get; set; }
        public virtual ICollection<ServiceRecord> ServiceRecords { get; set; }
    }

    public class MerchantInfo
    {
        public string MerchantId { get; set; }
        public string AccountId { get; set; }
        public string EntityId { get; set; }
        public string MemberId { get; set; }
    }
}

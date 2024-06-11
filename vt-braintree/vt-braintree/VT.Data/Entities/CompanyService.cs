using System.Collections.Generic;

namespace VT.Data.Entities
{
    public class CompanyService
    {
        public int CompanyServiceId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Company Company { get; set; }
        public virtual IList<ServiceRecordItem> ServiceRecordItems { get; set; }
        public virtual IList<CustomerService>  CustomerServices { get; set; }
    }
}

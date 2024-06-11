using System.Collections.Generic;

namespace VT.Data.Entities
{
    public class CustomerService
    {
        //primary key
        public int CustomerServiceId { get; set; }
         
        public int CustomerId { get; set; }
        public int CompanyServiceId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }
        public bool IsDeleted { get; set; }

        //navigation properties
        public virtual Customer Customer { get; set; }
        public virtual CompanyService CompanyService { get; set; }
        public virtual IList<ServiceRecordItem> ServiceRecordItems { get; set; }

    }
}

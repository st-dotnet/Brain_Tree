using System;
using System.Collections.Generic;

namespace VT.Data.Entities
{
    public class ServiceRecordItem
    {
        public int ServiceRecordItemId { get; set; }
        public int ServiceRecordId { get; set; }
        public int? CompanyServiceId { get; set; }
        public int? CustomerServiceId { get; set; }
        public int CustomerId { get; set; }
        public string ServiceName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ServiceRecordItemType Type { get; set; }
        public Double? CostOfService { get; set; }
        public string Description { get; set; }

        public virtual ServiceRecord ServiceRecord { get; set; }
        public virtual CustomerService CustomerService { get; set; }
        public virtual CompanyService CompanyService { get; set; }
        public virtual ICollection<ServiceRecordAttachment> ServiceRecordAttachments { get; set; }
    }
}

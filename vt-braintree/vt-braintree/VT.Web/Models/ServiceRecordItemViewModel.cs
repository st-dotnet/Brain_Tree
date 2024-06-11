using System;
using System.Collections.Generic;

namespace VT.Web.Models
{
    public class ServiceRecordItemViewModel
    {
        public int ServiceItemId { get; set; }
        public int? CustomerServiceId { get; set; }
        public string ServiceName { get; set; }
        public int? CompanyServiceId { get; set; }
        public int? CompanyId { get; set; }
        public string Description { get; set; }
        public decimal? Cost { get; set; }

        public List<ServiceRecordItemAttachmentViewModel> Attachments { get; set; }
    }
    public class ServiceRecordItemAttachmentViewModel
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
    }
}
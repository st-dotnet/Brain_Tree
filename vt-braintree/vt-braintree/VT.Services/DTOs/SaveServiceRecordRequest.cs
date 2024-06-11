using System;
using System.Collections.Generic;
using VT.Data;

namespace VT.Services.DTOs
{
    public class SaveServiceRecordRequest
    {
        public int ServiceRecordId { get; set; }
        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
        public int CompanyWorkerId { get; set; }
        public string Description { get; set; }
        public double TotalAmount { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool BilledToCompany { get; set; }
        public ServiceRecordStatus Status { get; set; }

        public List<SaveServiceRecordItemRequest> SaveServiceRecordItems { get; set; }
    }

    public class SaveServiceRecordItemRequest
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
        public List<ServiceRecordItemAttachmentRequest> Attachments { get; set; }
    }

    public class ServiceRecordItemAttachmentRequest
    {
        public int ServiceRecordItemId { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
    }
}

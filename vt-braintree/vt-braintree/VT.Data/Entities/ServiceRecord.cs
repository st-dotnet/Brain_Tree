using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VT.Data.Entities
{
    public class ServiceRecord
    {
        public ServiceRecord()
        {
            Commissions = new List<Commission>();
        }

        public int ServiceRecordId { get; set; }
        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
        public int CompanyWorkerId { get; set; }
        public string Description { get; set; }
        public double TotalAmount { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool BilledToCompany { get; set; }
        public bool IsInvoiceSent { get; set; }
        public ServiceRecordStatus Status { get; set; }
        public bool IsVoid { get; set; }
        public DateTime? VoidTime { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Company Company { get; set; }
        public virtual CompanyWorker CompanyWorker { get; set; }
        public DateTime? InvoiceDate { get; set; }

        [StringLength(Int32.MaxValue)]
        public string BtTransactionId { get; set; }
        public virtual ICollection<ServiceRecordItem> ServiceRecordItems { get; set; }
        public virtual ICollection<Commission> Commissions { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT.Services.DTOs
{
    public class GetVoidInvoiceDetailResponse
    {
        public List<GetVoidInvoiceDetailItem> Items { get; set; }
        public string CompanyName { get; set; }
    }

    public class GetVoidInvoiceDetailItem
    {
        public DateTime? InvoiceDate { get; set; }
        public int InvoiceNumber { get; set; }
        public string Customer { get; set; }
        public string CustomerEmail { get; set; }
        public double Amount { get; set; }
        public string PaymentType { get; set; }
        public DateTime? VoidedOn { get; set; }
        public string Comments { get; set; }
    }
}

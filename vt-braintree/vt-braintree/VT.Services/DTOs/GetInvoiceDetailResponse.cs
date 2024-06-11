using System;
using System.Collections.Generic;

namespace VT.Services.DTOs
{
    public class GetInvoiceDetailResponse
    {
        public List<GetInvoiceDetailItem> Items { get; set; }
        public string CompanyName { get; set; }
    }

    public class GetInvoiceDetailItem
    {
        public DateTime? InvoiceDate { get; set; }
        public int InvoiceNumber { get; set; }
        public string Customer { get; set; }
        public string CustomerEmail { get; set; }
        public double Amount { get; set; }
        public string PaymentType { get; set; }
        public DateTime ServiceDate { get; set; }
        public string TransactionId { get; set; }
    }
}

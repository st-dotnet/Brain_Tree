using System;
using System.Collections.Generic;

namespace VT.Services.DTOs
{
    public class GetCommissionExpenseResponse 
    {
        public List<GetCommissionExpenseDetailItem> Items { get; set; }
    }

    public class GetCommissionExpenseDetailItem
    {
        public DateTime Date { get; set; }
        public int CommissionId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public string WhenCollected { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace VT.Data.Entities
{
    public class Commission
    {
        public int CommissionId { get; set; }
        public DateTime Date { get; set; }
        public int ServiceRecordId { get; set; }
        public decimal Amount { get; set; }
        public string BtTransactionId { get; set; }
        public CommissionType Type { get; set; }
        public virtual ServiceRecord ServiceRecord { get; set; }
    }
}

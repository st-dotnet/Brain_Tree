using System;
using System.Collections.Generic;

namespace VT.Services.DTOs
{
    public class GetInvoiceDetailRequest  : CommissionFilterRequest
    {
    }

    public class CommissionFilterRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CompanyId { get; set; }
        public List<int> Customers { get; set; }
    }
}

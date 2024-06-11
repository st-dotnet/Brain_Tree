using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT.Services.DTOs
{
    public class GetVoidInvoiceDetailRequest : VoidInvoicesFilterRequest
    {
    }

    public class VoidInvoicesFilterRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CompanyId { get; set; }
        public List<int> Customers { get; set; }
    }
}

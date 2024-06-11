using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT.Services.DTOs
{
    public class GetCommissionExpenseRequest : CommissionFilterRequest
    {
    }

    public class GetCommissionsRequest  
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } 
        public List<int> CompanyId { get; set; }
    }
}

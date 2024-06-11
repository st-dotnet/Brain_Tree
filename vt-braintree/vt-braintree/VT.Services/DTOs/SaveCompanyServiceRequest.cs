using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT.Services.DTOs
{
    public class SaveCompanyServiceRequest
    {
        public int CompanyServiceId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

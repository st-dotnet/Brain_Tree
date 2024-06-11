using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VT.Services.Services;

namespace VT.Services.DTOs
{
    public class SaveCompanyServiceResponse : BaseResponse
    {
        public CompanyService CompanyService { get; set; }
    }
}

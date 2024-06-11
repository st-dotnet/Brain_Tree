using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VT.Data.Entities;

namespace VT.Services.DTOs
{
    public class CustomerSaveResponse :BaseResponse
    {
        public Customer Customer { get; set; }
    }
}

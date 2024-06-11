using System.Collections.Generic;
using System.Web.Mvc;

namespace VT.Web.Models
{
    public class UserCustomerAccessModel
    {
        public int CompanyWorkerUserId { get; set; }
        public List<int> From { get; set; }
        public List<int> To { get; set; }
    }

    public class UserCustomerAccessDetailModel : UserCustomerAccessModel
    {
        public List<SelectListItem> FromList { get; set; }
        public List<SelectListItem> ToList { get; set; } 
    }
}

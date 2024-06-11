using System.Collections.Generic;
using System.Web.Mvc;

namespace VT.Web.Models
{
    public class ImportCustomerViewModel
    {
        public int CompanyId { get; set; }
        public IList<SelectListItem> Organizations { get; set; }
    }
}
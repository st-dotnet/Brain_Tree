using VT.Data;

namespace VT.Web.Models
{
    public class ChangeStatusViewModel
    {
        public int ServiceRecordId { get; set; }
        public ServiceRecordStatus Status { get; set; }
    }
}

   
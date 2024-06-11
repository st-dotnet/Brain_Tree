using System;

namespace VT.Web.Models
{
    public class ServiceItemAttachmentListViewModel
    {
        public int ServiceItemAttachmentId { get; set; }
        public int ServiceItemId { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
    }
}
using System;

namespace VT.Data.Entities
{
    public class ServiceRecordAttachment
    {
        public int ServiceRecordAttachmentId { get; set; }
        public int ServiceRecordItemId { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }

        public virtual ServiceRecordItem ServiceRecordItem { get; set; }
    }
}

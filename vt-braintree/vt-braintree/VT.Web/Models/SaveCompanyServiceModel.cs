using System.Collections.Generic;
using EO.Internal;

namespace VT.Web.Models
{
    public class SaveCompanyServiceModel
    {
        public int CustomerServiceId { get; set; }
        public int CompanyServiceId { get; set; }
        public int? CompanyId { get; set; }
        public string Name { get; set; }
        public decimal? Cost { get; set; }
        public string Description { get; set; }
        public string ImageFileNameBefore { get; set; }
        public string ImageFileNameAfter { get; set; }

        public string AwsAccessKeyId { get; set; }
        public string Policy { get; set; }
        public string Signature { get; set; }
        public string Bucket { get; set; }
        public string Acl { get; set; }

        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public int ImageQuality { get; set; }
        public bool ImageCrop { get; set; }

        public int CustomerId { get; set; }
        public List<string> UploadImages { get; set; }
    }
}
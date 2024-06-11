using System.Collections.Generic;

namespace VT.Web.Models
{
    public class CustomerDetailViewModel
    {
        public int CustomerId { get; set; }

        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }

        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
        public string ContactMobile { get; set; }
        public string ContactMiddleName { get; set; }

        public string AwsAccessKeyId { get; set; }
        public string Policy { get; set; }
        public string Signature { get; set; }
        public string Bucket { get; set; }
        public string Acl { get; set; }


        public List<ServiceRecordItemViewModel> Items { get; set; }
    }
}
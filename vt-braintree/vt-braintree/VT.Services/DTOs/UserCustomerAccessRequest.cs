using System.Collections.Generic;

namespace VT.Services.DTOs
{
    public class UserCustomerAccessRequest
    {
        public int UserId { get; set; }
        public List<int> Customers { get; set; }
    }
}

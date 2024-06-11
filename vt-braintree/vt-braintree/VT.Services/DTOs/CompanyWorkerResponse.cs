using System.Collections.Generic;

namespace VT.Services.DTOs
{
    public class CompanyWorkerResponse : BaseResponse
    {
        public int CompanyWorkerId { get; set; }
        public string Username { get; set; } //Email
        public string AuthKey { get; set; } //Password
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeleted { get; set; } 
        public List<int> To { get; set; } 
    }
}

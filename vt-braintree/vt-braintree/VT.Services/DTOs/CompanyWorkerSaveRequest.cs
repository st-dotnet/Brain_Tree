using System.Collections.Generic;

namespace VT.Services.DTOs
{
    public class CompanyWorkerSaveRequest
    {
        public int CompanyWorkerId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? CompanyId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool IsAdmin { get; set; }  
        public List<int> AccessibleCustomers { get; set; }
    }
}

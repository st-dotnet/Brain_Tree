using System.Collections.Generic;
using System.Web.Mvc;

namespace VT.Web.Models
{
    public class SaveUserModel
    {
        public SaveUserModel()
        {
            FromList = new List<SelectListItem>();
            ToList = new List<SelectListItem>();
        }

        public int CompanyWorkerId { get; set; }
        public string Username { get; set; } //Email
        public string AuthKey { get; set; } //Password
        public string Confirm { get; set; } //Password
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool IsAdmin { get; set; } 
        
        public List<int> From { get; set; }
        public List<int> To { get; set; }
        public List<SelectListItem> FromList { get; set; }
        public List<SelectListItem> ToList { get; set; }
    }
}
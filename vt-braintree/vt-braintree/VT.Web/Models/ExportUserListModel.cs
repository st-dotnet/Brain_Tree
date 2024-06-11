using System.Collections.Generic;

namespace VT.Web.Models
{
    public class ExportUserListModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Css { get; set; }
        public List<ExportUserValidation> Data { get; set; }
    }

    public class ExportUserValidation
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public bool IsAdministrator { get; set; }
    }
}
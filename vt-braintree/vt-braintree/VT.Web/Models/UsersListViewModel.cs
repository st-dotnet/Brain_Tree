namespace VT.Web.Models
{
    public class UsersListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string IsAdmin { get; set; }
    }
}
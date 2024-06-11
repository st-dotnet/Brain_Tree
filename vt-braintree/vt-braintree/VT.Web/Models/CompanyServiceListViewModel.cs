namespace VT.Web.Models
{
    public class CompanyServiceListViewModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? CompanyId { get; set; }
        public bool IsGpsOn { get; set; }
        public int Threshold { get; set; }
    }
}
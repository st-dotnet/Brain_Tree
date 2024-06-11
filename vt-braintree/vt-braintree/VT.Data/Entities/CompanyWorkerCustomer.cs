namespace VT.Data.Entities
{
    public class CompanyWorkerCustomer
    {
        public int CompanyWorkerId { get; set; }
        public int CustomerId { get; set; }
        public int CustomerOrder { get; set; }


        public virtual Customer Customer { get; set; }
        public virtual CompanyWorker CompanyWorker { get; set; }
    }
}

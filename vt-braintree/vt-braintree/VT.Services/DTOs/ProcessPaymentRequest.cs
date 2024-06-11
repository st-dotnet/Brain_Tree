using VT.Data.Entities;

namespace VT.Services.DTOs
{
    public class ProcessPaymentRequest
    {
        public int ServiceRecordId { get; set; }
        public ServiceRecord ServiceRecord { get; set; }
    }
}

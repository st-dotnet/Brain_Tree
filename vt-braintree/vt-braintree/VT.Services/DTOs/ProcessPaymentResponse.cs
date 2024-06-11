using VT.Data;
using VT.Data.Entities;

namespace VT.Services.DTOs
{
    public class ProcessPaymentResponse : BaseResponse
    {
        public ServiceRecordStatus Status { get; set; }
        public ServiceRecord ServiceRecord { get; set; }
        public Commission Commission { get; set; }
    }
}

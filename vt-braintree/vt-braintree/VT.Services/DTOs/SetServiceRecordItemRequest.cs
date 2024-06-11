using System;

namespace VT.Services.DTOs
{
    public class SetServiceRecordItemRequest
    {
        public int ServiceRecordItemId { get; set; }
        public Decimal? CostOfService { get; set; }
    }
}

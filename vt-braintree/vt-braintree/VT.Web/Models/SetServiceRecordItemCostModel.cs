using System;

namespace VT.Web.Models
{
    public class SetServiceRecordItemCostModel
    {
        public int ServiceRecordItemId { get; set; }
        public Decimal? CostOfService { get; set; }
    }
}
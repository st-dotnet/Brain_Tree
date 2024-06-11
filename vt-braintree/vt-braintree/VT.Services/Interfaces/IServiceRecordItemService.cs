using System.Collections.Generic;
using VT.Data.Entities;
using VT.Services.DTOs;

namespace VT.Services.Interfaces
{
    public interface IServiceRecordItemService
    {
        IList<ServiceRecordItem> GetRecordItems(int serviceRecordId);
        IList<ServiceRecordAttachment> GetServiceRecordAttachments(int serviceRecordItemId);
        SetServiceRecordItemResponse SetServiceRecordItemCost(SetServiceRecordItemRequest request);
    }
}

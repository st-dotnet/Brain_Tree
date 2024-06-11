using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using VT.Data.Context;
using VT.Data.Entities;
using VT.Services.DTOs;
using VT.Services.Interfaces;

namespace VT.Services.Services
{
    public class ServiceRecordItemService : IServiceRecordItemService
    {
        #region Field(s)

        private readonly IVerifyTechContext _context;

        #endregion

        #region Constructor

        public ServiceRecordItemService(IVerifyTechContext context)
        {
            _context = context;
        }

        #endregion

        #region Interface implementation

        public IList<ServiceRecordItem> GetRecordItems(int serviceRecordId)
        {
            return _context.ServiceRecordItems.Where(x => x.ServiceRecordId == serviceRecordId).ToList();
        }

        public IList<ServiceRecordAttachment> GetServiceRecordAttachments(int serviceRecordItemId)
        {
            return _context.ServiceRecordAttachments.Where(x => x.ServiceRecordItemId == serviceRecordItemId).ToList();
        }

        public SetServiceRecordItemResponse SetServiceRecordItemCost(SetServiceRecordItemRequest request)
        {
            var response = new SetServiceRecordItemResponse();
            var item =
                _context.ServiceRecordItems.FirstOrDefault(x => x.ServiceRecordItemId == request.ServiceRecordItemId);
            if (item == null)
            {
                response.Message = "Service record item does not exist in the system.";
                return response;
            }

            item.CostOfService = Convert.ToDouble(request.CostOfService);
            _context.SaveChanges();

            var serviceRecord = _context.ServiceRecords.Include(x => x.ServiceRecordItems).FirstOrDefault(x => x.ServiceRecordId == item.ServiceRecordId);

            if (serviceRecord != null)
            {
                var amount = serviceRecord.ServiceRecordItems.Sum(x => x.CostOfService); 
                serviceRecord.TotalAmount = amount != null ? amount.Value : 0;
            }
            _context.SaveChanges();
            response.Success = true;
            return response;
        }

        #endregion                
    }
}

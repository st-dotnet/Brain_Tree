using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using VT.Common.Utils;
using VT.Data;
using VT.Services.DTOs;
using VT.Services.Interfaces;
using VT.Web.Models;

namespace VT.Web.Controllers
{
    public class ServiceRecordsController : BaseController
    {
        #region Fields

        private readonly IServiceRecordService _serviceRecordService;
        private readonly IServiceRecordItemService _serviceRecordItemService;
        private readonly ICompanyService _companyService;

        #endregion
        
        #region Constructor

        public ServiceRecordsController(IServiceRecordService serviceRecordService,
            IServiceRecordItemService serviceRecordItemService,
            ICompanyService companyService) 
        {
            _serviceRecordService = serviceRecordService;
            _serviceRecordItemService = serviceRecordItemService;
            _companyService = companyService;
        }

        #endregion

        #region Action Method(s)

        [Route("~/ServiceRecords")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("~/ServiceRecords/GetServiceRecords")]
        public ActionResult GetServiceRecords([DataSourceRequest] DataSourceRequest request)
        {
            var data = GetAllServiceRecordList().ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/ServiceRecords/GetServiceRecordItems/{id}")]
        public ActionResult GetServiceRecordItems(int id, [DataSourceRequest] DataSourceRequest request)
        {
            var data = GetServiceRecordItemList(id).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/ServiceRecords/Attachments/{id}")]
        public ActionResult Attachments(int id)
        {
            var data = _serviceRecordItemService.GetServiceRecordAttachments(id)
                .Select(x => new ServiceItemAttachmentListViewModel
            {
                ServiceItemAttachmentId = x.ServiceRecordAttachmentId,
                ServiceItemId = x.ServiceRecordItemId,
                Url = x.Url,
                Description = x.Description,
                Date = x.Date,
                Type = x.Type
            });

            return PartialView("Attachments", data);
        }

        [HttpPost]
        [Route("~/ServiceRecords/SetCost")]
        public ActionResult SetCost(SetServiceRecordItemCostModel model)
        {
            //prepare dto
            var request = Mapper.Map<SetServiceRecordItemRequest>(model);
            var response =_serviceRecordItemService.SetServiceRecordItemCost(request);

            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }

        [HttpPost]
        [Route("~/ServiceRecords/VoidServiceActivity")]
        public ActionResult VoidServiceActivity(SetVoidServiceRecordModel model)
        {
            var response = _serviceRecordService.VoidServiceActivity(model.ServiceId);
            
            return Json(new
            {
                success = response.Success,
                message = response.Message
            });

        }
        #endregion

        #region Private Methods

        private IEnumerable<ServiceRecordDetail> GetAllServiceRecordList()
        {
            var list = _serviceRecordService.GetAllServiceRecords(CurrentIdentity.CompanyId);
            var result = list.Select(x => new ServiceRecordDetail
            {
                BilledToCompany = x.BilledToCompany,
                CompanyId = x.CompanyId,
                CompanyName = x.Company != null ? x.Company.Name : "N/A",
                CompanyWorkerEmail = string.Format("{0} {1} ({2})", x.CompanyWorker.FirstName, x.CompanyWorker.LastName, x.CompanyWorker.Email),
                CustomerId = x.CustomerId,
                CustomerName = x.Customer.Name,
                Description = x.Description,
                EndTime = x.EndTime,
                HasNonService = x.ServiceRecordItems.Any(y => y.CostOfService == null || y.CostOfService == 0),
                ServiceRecordId = x.ServiceRecordId,
                StartTime = x.StartTime,
                TotalAmount = x.TotalAmount,
                Status = EnumUtil.GetDescription(x.Status),
                InvoiceDate = x.InvoiceDate,
                TransactionId = x.BtTransactionId
            }).ToList();
            return result;
        }

        
        private IEnumerable<ServiceRecordItemDetail> GetServiceRecordItemList(int id)
        {
            var list = _serviceRecordItemService.GetRecordItems(id);
            var result = list.Select(x => new ServiceRecordItemDetail
            {
                CompanyServiceId = x.CompanyServiceId != null ? x.CompanyServiceId.Value : 0,
                CostOfService = x.CostOfService,
                ServiceName = x.ServiceName,
                Description = x.Description,
                CustomerId = x.CustomerId,
                EndTime = x.EndTime,
                ServiceRecordId = x.ServiceRecordId,
                StartTime = x.StartTime,
                ServiceRecordItemId = x.ServiceRecordItemId,
                Type = EnumUtil.GetDescription(x.Type),
            }).ToList();
            return result;
        }

        #endregion
    }
}
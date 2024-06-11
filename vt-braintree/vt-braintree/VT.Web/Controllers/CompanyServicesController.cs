using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using VT.Common.Utils;
using VT.Data;
using VT.Services.Interfaces;
using VT.Web.Models;
using VT.Services.DTOs;
using System.Globalization;

namespace VT.Web.Controllers
{
    public class CompanyServicesController : BaseController
    {
        #region Fields

        private readonly ICompanyServiceService _companyServiceService;
        private readonly ICompanyService _companyService;
        private readonly IServiceRecordService _serviceRecordService;
        private readonly IServiceRecordItemService _serviceRecordItemService;

        #endregion

        #region Constructor

        public CompanyServicesController(ICompanyServiceService companyServiceService,
            ICompanyService companyService, IServiceRecordService serviceRecordService,
            IServiceRecordItemService serviceRecordItemService) 
        {
            _companyServiceService = companyServiceService;
            _companyService = companyService;
            _serviceRecordService = serviceRecordService;
            _serviceRecordItemService = serviceRecordItemService; 
        }

        #endregion

        #region Action Method(s)

        [HttpGet]
        [Route("~/CompanyServices")]
        public ActionResult Index()
        {
            PopulateViews();
            return View();
        }

        [HttpPost]
        [Route("~/CompanyServices/DeleteCompanyServices")]
        public ActionResult DeleteCompanyServices(DeleteUsersViewModel model)
        {
            var response = _companyServiceService.DeleteCompanyServices(model.Ids);
            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }

        [HttpPost]
        [Route("~/CompanyServices/GetCompanyService/{id}")]
        public ActionResult GetCompanyService(int id)
        {
            var companyService = _companyServiceService.GetCompanyService(id);
            if (companyService == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Company service does not exist in the database."
                });
            }
            var model = new CompanyServiceListViewModel
            {
                Id = companyService.CompanyServiceId,
                Name = companyService.Name,
                Description = companyService.Description,
                CompanyId = companyService.CompanyId
            };
            return Json(new
            {
                success = true,
                message = string.Empty,
                cs = model
            });
        }

        [HttpPost]
        [Route("~/CompanyServices/GetCompanyServiceDetail/{id}")]
        public ActionResult GetCompanyServiceDetail(int id)
        {
            var companyService = _companyServiceService.GetCompanyService(id);
            if (companyService == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Company service does not exist in the database."
                });
            }
            var model = new CompanyServiceListViewModel
            {
                Id = companyService.CompanyServiceId,
                Name = companyService.Name,
                Description = companyService.Description,
                CompanyId = companyService.CompanyId
            };
            return PartialView("CompanyServiceDetail", model);
        }

        [HttpPost]
        [Route("~/CompanyServices/CompanyServiceList")]
        public ActionResult CompanyServiceList([DataSourceRequest] DataSourceRequest request)
        {
            var data = GetCompanyServiceList().ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/CompanyServices/SaveCompanyService")]
        public ActionResult SaveCompanyService(SaveCompanyServiceModel model)
        {
            var response = _companyServiceService.SaveCompanyService(new SaveCompanyServiceRequest
            {
                CompanyServiceId = model.CompanyServiceId,
                CompanyId = model.CompanyId != null ? model.CompanyId.Value : 0, //TODO: CompanyId can be nullable here            
                Name = model.Name,
                Description = model.Description
            });

            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }

        [HttpPost]
        [Route("~/CompanyServices/ServiceRecords/{id}")]
        public ActionResult ServiceRecords(int id, [DataSourceRequest] DataSourceRequest request)
        {
            var data = GetServiceRecordList(id).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/CompanyServices/ServiceRecordItems/{id}")]
        public ActionResult ServiceRecordItems(int id, int companyServiceId, [DataSourceRequest] DataSourceRequest request)
        {
            var data = GetServiceRecordItemList(id, companyServiceId).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private Methods

        private IEnumerable<ServiceRecordDetail> GetServiceRecordList(int id)
        {
            var list = _serviceRecordService.GetServiceRecordsByCompanyService(id);
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
                Status = EnumUtil.GetDescription(x.Status),
                TotalAmount = x.TotalAmount
            }).ToList();
            return result;
        }

        private IEnumerable<ServiceRecordItemDetail> GetServiceRecordItemList(int id, int? companyServiceId)
        {
            var list = _serviceRecordItemService.GetRecordItems(id);
            var result = list.Select(x => new ServiceRecordItemDetail
            {
                CompanyServiceId = x.CompanyServiceId !=null ? x.CompanyServiceId.Value : 0,
                CostOfService = x.CostOfService,
                ServiceName = x.ServiceName,
                CustomerId = x.CustomerId,
                EndTime = x.EndTime,
                Description = x.Description,
                ServiceRecordId = x.ServiceRecordId,
                StartTime = x.StartTime,
                ServiceRecordItemId = x.ServiceRecordItemId,
                Type = EnumUtil.GetDescription(x.Type),
            }).ToList();
            return result;
        }

        private IEnumerable<CompanyServiceListViewModel> GetCompanyServiceList()
        {
            var companies = _companyServiceService.GetAllCompanyServices(CurrentIdentity.CompanyId);
            var list = companies.Select(x => new CompanyServiceListViewModel
            {
                Id = x.CompanyServiceId,
                CompanyName = x.Company.Name,
                Name = x.Name,
                Description = x.Description,
                CompanyId = x.CompanyId
            }).ToList();
            return list;
        }

        private void PopulateViews()
        {
            var companies = _companyService.GetOranizationList();
            var list = new List<SelectListItem> { new SelectListItem { Text = "--Select--", Value = "" } };
            list.AddRange(companies.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CompanyId.ToString(CultureInfo.InvariantCulture)
            }));
            ViewData["Organizations"] = list;
        }

        #endregion

    }
}
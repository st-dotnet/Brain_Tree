using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using VT.Common;
using VT.Common.Utils;
using VT.Data;
using VT.Services.DTOs;
using VT.Services.Interfaces;
using VT.Web.Models;

namespace VT.Web.Controllers
{
    public class BillingController : BaseController
    {
        #region Fields

        private readonly ICompanyService _companyService;
        private readonly IServiceRecordService _serviceRecordService;
        private readonly IServiceRecordItemService _serviceRecordItemService;
        private readonly IPaymentGatewayService _paymentService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Constructor

        public BillingController(ICustomerService customerService,
            ICompanyService companyService,
            IServiceRecordService serviceRecordService,
            IServiceRecordItemService serviceRecordItemService,
            IPaymentGatewayService braintreePaymentService)  
        {
            _customerService = customerService;
            _companyService = companyService;
            _serviceRecordService = serviceRecordService;
            _serviceRecordItemService = serviceRecordItemService;
            _paymentService = braintreePaymentService; 
        }

        #endregion

        #region Action Method(s)
        
        [Route("~/Billing")]
        public ActionResult Index()
        {
            PopulateViews();
            return View();
        }

        [Route("~/Billing/Customer")]
        public ActionResult Customer()
        {
            PopulateViews();
            return View();
        }

        [Route("~/VoidedInvoice")]
        public ActionResult VoidedInvoice()
        {
            PopulateViews();
            return View();
        }

        [HttpPost]
        [Route("~/Billing/GetCustomerServiceRecords")]
        public ActionResult GetCustomerServiceRecords(int id, [DataSourceRequest] DataSourceRequest request)
        {
            var data = GetCustomerServiceRecordList(id).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [Route("~/Billing/GetServiceRecords")]
        public ActionResult GetServiceRecords(int id,[DataSourceRequest] DataSourceRequest request)
        {
            var data = GetAllServiceRecordList(id).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Billing/GetVoidInvoiceServicesList")]
        public ActionResult GetVoidInvoiceServicesList(int id, [DataSourceRequest] DataSourceRequest request)
        {
            var data = GetVoidServiceRecordList(id).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Billing/GetServiceRecordItems/{id}")]
        public ActionResult GetServiceRecordItems(int id, [DataSourceRequest] DataSourceRequest request)
        {
            var data = GetServiceRecordItemList(id).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Billing/ChargeCustomer")] //charge company sub-merchant 
        public ActionResult ChargeCustomer(ChargeCustomerViewModel model)
        {
            //prepare dto 
            var request = Mapper.Map<ChargeCustomerRequest>(model);
            var response = _paymentService.ProcessChargeCompany(request);
            
            return Json(new 
            {
                success = response.Success,
                message = response.Success ? string.Format("Transaction successful. Transaction number: {0}", response.Transaction): response.Message
            });
        }

        [HttpGet]
        [Route("~/Billing/Download/{id}")] //charge company sub-merchant 
        public FileResult Download(int id)
        {
            //prepare dto 
            var array = _serviceRecordService.GetPdf(id);

            return File(array, String.Format("{0}.pdf", id));
        }



        [HttpPost]
        [Route("~/Billing/ChargeCustomerAccount")] //charge customer credit card
        public ActionResult ChargeCustomerAccount(ChargeCustomerAccountViewModel model)
        {
            //prepare dto 
            var request = Mapper.Map<ChargeCustomerCcRequest>(model);

            var response = model.ChargeExternal ? _paymentService.SetPaidExternal(request) : _paymentService.ProcessChargeCustomer(request);

            return Json(new
            {
                success = response.Success,
                message = response.Success ? "Transaction successful." : response.Message
            });
        }

        #endregion

        #region Private Methods
        
        private void PopulateViews()
        {
            if (User.IsInRole(UserRoles.SuperAdmin.ToString()))
            {
                ViewData["Organizations"] = GetOrganizations();
            }
            else
            {
                ViewData["Customers"] = GetCustomers(CurrentIdentity.CompanyId);
            }
            ViewData["Status"] = GetSelectListItems();
        }

        private List<SelectListItem> GetOrganizations()
        {
            var companies = _companyService.GetOranizationList();
            var list = companies.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CompanyId.ToString(CultureInfo.InvariantCulture)
            }).ToList();

            return list;
        }

        private IEnumerable<ServiceRecordDetail> GetVoidServiceRecordList(int companyId)
        {
            var list = _serviceRecordService.GetVoidServiceRecords(companyId);
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
                TransactionId = x.BtTransactionId,
                VoidTime = x.VoidTime
            }).ToList();
            return result;
        }
        private List<SelectListItem> GetCustomers(int? companyId)
        {
            var companies = _customerService.GetAllCustomers(companyId);
            var list = companies.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CustomerId.ToString(CultureInfo.InvariantCulture)
            }).OrderBy(x => x.Text).ToList();

            return list;
        }

        private IEnumerable<ServiceRecordDetail> GetAllServiceRecordList(int? companyId)
        {
            var list = _serviceRecordService.GetAllOutstandingReceivables(companyId);
            var result = list.Select(x => new ServiceRecordDetail
            {
                BilledToCompany = x.BilledToCompany || x.Status == ServiceRecordStatus.PaidByCcOnFile,
                CompanyId = x.CompanyId,
                ServiceFeePercentage = Convert.ToDecimal(x.Company.ServiceFeePercentage),
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
                RecordStatus = x.Status,
                InvoiceDate = x.InvoiceDate,
                TransactionId = x.BtTransactionId
            }).ToList();
            return result;
        }

        private IEnumerable<CustomerServiceRecordDetail> GetCustomerServiceRecordList(int customerId)
        {
            var list = _serviceRecordService.GetOutstandingReceivableByCustomerId(customerId);
            var result = list.Select(x => new CustomerServiceRecordDetail 
            {
                CompanyId = x.CompanyId,
                ServiceFeePercentage = Convert.ToDecimal(x.Company.ServiceFeePercentage),
                CompanyName = x.Company != null ? x.Company.Name : "N/A",
                HasCustomerCc = x.Customer != null && (!string.IsNullOrEmpty(x.Customer.GatewayCustomerId) && x.Customer.IsCcActive),
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
                RecordStatus = x.Status,
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

        private IEnumerable<SelectListItem> GetSelectListItems()
        {
            var enumValues = Enum.GetValues(typeof(ServiceRecordStatus)) as ServiceRecordStatus[];
            if (enumValues == null) return null;
            return enumValues.Select(enumValue => new SelectListItem
            {
                Value = ((int)enumValue).ToString(CultureInfo.InvariantCulture),
                Text = EnumUtil.GetDescription(enumValue)
            }).ToList();
        }

        #endregion
    }
}
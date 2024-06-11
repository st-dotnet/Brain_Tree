using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using VT.Common;
using VT.Services.DTOs;
using VT.Services.Interfaces;
using VT.Web.Models;

namespace VT.Web.Controllers
{
    public class AccountingController : BaseController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ICompanyService _companyService;
        private readonly ICommissionService _commissionService;

        #endregion

        #region Constructor

        public AccountingController(ICustomerService customerService,
            ICompanyService companyService, 
            ICommissionService commissionService)
        {
            _customerService = customerService;
            _companyService = companyService;
            _commissionService = commissionService;
        }

        #endregion

        #region Action Method(s)

        // GET: Accounting
        public ActionResult UnPaidCommissions()
        {
            return View();
        }

        public ActionResult Commissions()
        {
            return View();
        }

        [Route("~/UnPaidServices")]
        public ActionResult UnPaidServices()
        {
            PopulateViewData();
             
            var model = new CommissionInputModel
            {
                CompanyId = CurrentIdentity.CompanyId,
                CompanyName = CurrentIdentity.CompanyName
            };
            return View(model);
        }

        [Route("~/Accounting/GetCustomers/{id}")]
        public JsonResult GetCustomers(int id)
        {
            var customers = GetCustomerList(id);
            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Accounting/GetUnpaidServices")]
        public ActionResult GetUnpaidServices()
        {
            return PartialView("UnpaidServicesGrid");
        }

        [Route("~/Accounting/GetUnpaidServicesList")]
        public ActionResult GetUnpaidServicesList([DataSourceRequest] DataSourceRequest request,
            DateTime? startDate, DateTime? endDate, int? companyId, List<int> customers)
        {
            var invoiceResponse = _commissionService.GetInvoiceDetails(new GetInvoiceDetailRequest
            {
                StartDate = startDate,
                EndDate = endDate,
                CompanyId = companyId,
                Customers = customers
            });

            var result = invoiceResponse.Items.Any() ? 
                Mapper.Map<List<InvoicesViewModel>>(invoiceResponse.Items) : 
                new List<InvoicesViewModel>();

            return Json(result.ToDataSourceResult(request));
        }


         [Route("~/Accounting/GetVoidInvoiceServicesList")]
        public ActionResult GetVoidInvoiceServicesList([DataSourceRequest] DataSourceRequest request,
            DateTime? startDate, DateTime? endDate, int? companyId, List<int> customers)
        {
            var voidInvoiceResponse = _commissionService.GetVoidInvoiceDetails(new GetVoidInvoiceDetailRequest()
            {
                StartDate = startDate,
                EndDate = endDate,
                CompanyId = companyId,
                Customers = customers
            });

            var result = voidInvoiceResponse.Items.Any() ? 
                Mapper.Map<List<VoidInvoicesViewModel>>(voidInvoiceResponse.Items) :
                new List<VoidInvoicesViewModel>();

            return Json(result.ToDataSourceResult(request));
        }
      



        [Route("~/Accounting/GetCommissionExpenseList")]
        public ActionResult GetCommissionExpenseList([DataSourceRequest] DataSourceRequest request, 
            DateTime? startDate, DateTime? endDate, int? companyId, List<int> customers)
        {
            var commissionExpenseResponse = _commissionService.GetCommissionExpenseDetails(new GetCommissionExpenseRequest
            {
                StartDate = startDate,
                EndDate = endDate,
                CompanyId = companyId,
                Customers = customers
            });

            var result = commissionExpenseResponse.Items.Any() ? 
                Mapper.Map<List<CommissionExpenseViewModel>>(commissionExpenseResponse.Items) : 
                new List<CommissionExpenseViewModel>();

            return Json(result.ToDataSourceResult(request));
        }

        [Route("~/Accounting/ExportInvoices")]
        public ActionResult ExportInvoices(DateTime startDate, DateTime endDate, 
            int? companyId, string companyName, string customers)
        {
            var customerList = GetIntIdsList(customers);

            var invoiceResponse = _commissionService.GetInvoiceDetails(new GetInvoiceDetailRequest
            {
                StartDate = startDate,
                EndDate = endDate,
                CompanyId = companyId,
                Customers = customerList
            });

            var result = invoiceResponse.Items.Any() ?
                Mapper.Map<List<InvoicesViewModel>>(invoiceResponse.Items) :
                new List<InvoicesViewModel>();

            var fileName =  string.Format("{0}_Invoices_{1}_to_{2}_on_{3}.csv", companyName,
                    startDate.ToString("MMddyyyy"), endDate.ToString("MMddyyyy"),
                    DateTime.UtcNow.ToString("MMddyyyy"));

            return CreateCsvFile(result, fileName);
        }


        [Route("~/Accounting/ExportVoidInvoices")]
        public ActionResult ExportVoidInvoices(DateTime startDate, DateTime endDate,
            int? companyId, string companyName, string customers)
        {
            var customerList = GetIntIdsList(customers);

            var voidinvoiceResponse = _commissionService.GetVoidInvoiceDetails(new GetVoidInvoiceDetailRequest
            {
                StartDate = startDate,
                EndDate = endDate,
                CompanyId = companyId,
                Customers = customerList
            });

            var result = voidinvoiceResponse.Items.Any() ?
                Mapper.Map<List<VoidInvoicesViewModel>>(voidinvoiceResponse.Items) :
                new List<VoidInvoicesViewModel>();

            var fileName = string.Format("{0}_VoidInvoices_{1}_to_{2}_on_{3}.csv", companyName,
                    startDate.ToString("MMddyyyy"), endDate.ToString("MMddyyyy"),
                    DateTime.UtcNow.ToString("MMddyyyy"));

            return CreateCsvFile(result, fileName);
        }



        [Route("~/Accounting/ExportCommission")]
        public ActionResult ExportCommission(DateTime startDate, DateTime endDate, 
            int? companyId, string companyName, string customers)
        {
            var customerList = GetIntIdsList(customers);

            var commissionExpenseResponse = _commissionService.GetCommissionExpenseDetails(new GetCommissionExpenseRequest
            {
                StartDate = startDate,
                EndDate = endDate,
                CompanyId = companyId,
                Customers = customerList
            });

            var result = commissionExpenseResponse.Items.Any() ?
                Mapper.Map<List<CommissionExpenseViewModel>>(commissionExpenseResponse.Items) :
                new List<CommissionExpenseViewModel>();

            var fileName = string.Format("{0}_Commissions_{1}_to_{2}_on_{3}.csv", companyName,
                    startDate.ToString("MMddyyyy"), endDate.ToString("MMddyyyy"),
                    DateTime.UtcNow.ToString("MMddyyyy"));

            return CreateCsvFile(result, fileName);
        }

        #endregion

        #region Private Method(s)

        private List<int> GetIntIdsList(string ids)
        {
            var customerList = new List<int>();

            if (!string.IsNullOrEmpty(ids))
            {
                var arr = ids.Replace("[", String.Empty).Replace("]", String.Empty).Split(',');
                foreach (var item in arr)
                {
                    int value;
                    if (Int32.TryParse(item, out value))
                        customerList.Add(value);
                }
            }
            return customerList;
        }

        private void PopulateViewData()
        {
            ViewData["Organizations"] = GetOrganizations();
            ViewData["Customers"] = new List<SelectListItem>();
        }

        private List<SelectListItem> GetOrganizations()
        {
            var companies = _companyService.GetOranizationList();
            var list = new List<SelectListItem> { new SelectListItem { Text = "--Select--", Value = "" } };
            list.AddRange(companies.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CompanyId.ToString(CultureInfo.InvariantCulture)
            }));
            return list;
        }

        private List<SelectListItem> GetCustomerList(int? companyId)
        {
            if (companyId == null) return new List<SelectListItem>(); 

            var companies = _customerService.GetAllCustomers(companyId);
            var list = companies.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CustomerId.ToString(CultureInfo.InvariantCulture),
                Selected = true
            }).ToList();

            var organiztions = new List<SelectListItem>
            {
                //new SelectListItem {Selected = true, Text = "All", Value = "0"},
            };

            organiztions.AddRange(list);

            return organiztions;
        }

        #endregion
    }
}
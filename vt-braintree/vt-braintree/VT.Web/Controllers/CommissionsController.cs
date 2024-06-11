using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using EO.Internal;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using VT.Common.Utils;
using VT.Services.DTOs;
using VT.Services.Interfaces;
using VT.Web.Models;

namespace VT.Web.Controllers
{
    public class CommissionsController : BaseController
    {
        #region Fields

        private readonly ICommissionService _commissionService;
        private readonly ICompanyService _companyService;

        #endregion

        #region Constructor

        public CommissionsController(ICommissionService commissionService, ICompanyService companyService)
        {
            _commissionService = commissionService;
            _companyService = companyService;
        }

        #endregion

        #region Action Method(s)

        [Route("~/Commissions")]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        [HttpPost]
        [Route("~/Commissions/GetCommissions")]
        public ActionResult GetUnpaidServices()
        {
            return PartialView("CommissionsGrid");
        }


        [HttpPost]
        [Route("~/Commissions/GetCommissionList")]
        public ActionResult GetCommissionList([DataSourceRequest] DataSourceRequest request,
            DateTime? startDate, DateTime? endDate, List<int> companyId)
        {
            var commissionExpenseResponse = _commissionService.GetCommissionsForSuperAdmin(new GetCommissionsRequest
            {
                StartDate = startDate,
                EndDate = endDate,
                CompanyId = companyId 
            });

            var result = commissionExpenseResponse.Items.Any() ?
                Mapper.Map<List<CommissionExpenseViewModel>>(commissionExpenseResponse.Items) :
                new List<CommissionExpenseViewModel>();

            return Json(result.ToDataSourceResult(request));
        }

        [Route("~/Commissions/ExportCommission")]
        public ActionResult ExportCommission(DateTime? startDate, DateTime? endDate, string companyId)
        {
            var companies = GetIntIdsList(companyId);

            var commissionExpenseResponse = _commissionService.GetCommissionsForSuperAdmin(new GetCommissionsRequest
            {
                StartDate = startDate,
                EndDate = endDate,
                CompanyId = companies, 
            });

            var result = commissionExpenseResponse.Items.Any() ?
                Mapper.Map<List<CommissionExpenseViewModel>>(commissionExpenseResponse.Items) :
                new List<CommissionExpenseViewModel>();

            var fileName = string.Format("CommissionCollected_{0}_to_{1}_on_{2}.csv",
                startDate.Value.ToString("MMddyyyy"), endDate.Value.ToString("MMddyyyy"),
                DateTime.UtcNow.ToString("MMddyyyy"));

            return CreateCsvFile(result, fileName);
        }

        [Route("~/Commissions/GetOrganizations")]
        public JsonResult GetOrganizations()
        {
            var companies = _companyService.GetOranizationList();
            var list = companies.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CompanyId.ToString(CultureInfo.InvariantCulture),
                Selected = true
            }).ToList(); 
            return Json(list, JsonRequestBehavior.AllowGet);
        }

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

        #endregion

        #region Private Method(s)
         
        private void PopulateViewData()
        {
            ViewData["Organizations"] = new List<SelectListItem>(); 
        }
         
         
        #endregion
    }
}
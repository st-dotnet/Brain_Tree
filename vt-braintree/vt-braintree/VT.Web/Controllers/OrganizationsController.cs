using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Helpers;
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
    [Authorize(Roles = "SuperAdmin")]
    public class OrganizationsController : BaseController
    {
        #region Fields

        private readonly ICompanyService _companyService;

        #endregion
        
        #region Constructor

        public OrganizationsController(ICompanyService companyService) 
        {
            _companyService = companyService; 
        }

        #endregion
        
        #region Action Methods

        [Route("~/")]
        public ActionResult Index()
        {
            PopulateViews();
            return View();
        }

        [HttpPost]
        [Route("~/Organizations/SaveOrganization")]
        public ActionResult SaveOrganization(SaveOrganizationViewModel model)
        {
            
            var request = Mapper.Map<CompanySaveRequest>(model);
            var response = _companyService.Save(request);

            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }


        [HttpPost, Route("~/Organizations/CheckOrgName")]
        public ActionResult CheckOrgName(string name)
        {
            var result = _companyService.IsOrgNameExist(name);
            return Content(result.Success ? "true" : "false");
        }

        [HttpPost]
        [Route("~/Organizations/OrganizationList")]
        public ActionResult OrganizationList([DataSourceRequest] DataSourceRequest request)
        {
            var data = GetOrganizationList().ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Organizations/GetOrganizationData/{id?}")]
        public JsonResult GetOrganizationData(int? id)
        {
            var model = id != null ? GetOrganization(id.Value) : new SaveOrganizationViewModel();
            return Json(model);
        }

        [HttpPost]
        [Route("~/Organizations/GetOrganizationDetail/{id}")]
        public ActionResult GetOrganizationDetail(int id)
        {
            var result = _companyService.GetOrganizationDetail(id);
            return PartialView("OrganizationDetail", result);
        }

        [HttpPost]
        [Route("~/Organizations/DeleteOrgs")]
        public ActionResult DeleteOrgs(DeleteOrgViewModel model)
        {
            var response = _companyService.DeleteOrgs(model.Ids);
            return Json(new
            {
                success = response.Success,
                message = response.Message,
            });
        }

        [HttpPost]
        [Route("~/Organizations/GetOrganizations")]
        public ActionResult GetOrganizations()
        {
            var data = _companyService.GetOranizationList();

            var list = new List<SelectListItem>();
            list.AddRange(data.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CompanyId.ToString(CultureInfo.InvariantCulture)
            }));

            return Json(list);
        }

        #endregion


        #region Private Methods

        private void PopulateViews()
        {
            var list = GetPaymentGatewayType();
            ViewData["PaymentGateway"] = list;
        }


        private IEnumerable<OrganizationListViewModel> GetOrganizationList()
        {
            var companies = _companyService.GetAllCompanies();
            var list = companies.Select(x => new OrganizationListViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Customers = x.Customers,
                Services = x.Services,
                Users = x.Users,
                GatewayCustomerId = x.GatewayCustomerId,
                MerchantAccountId = x.MerchantAccountId,
                PaymentGatewayType = x.PaymentGatewayType
            }).ToList();
            return list;
        }

        private SaveOrganizationViewModel GetOrganization(int id)
        {
            var data = _companyService.GetOrganizationDetail(id);
            if (data == null) throw new Exception("This organization does not exist");

            var model = Mapper.Map<SaveOrganizationViewModel>(data);
            return model;
        }


        private List<SelectListItem> GetPaymentGatewayType()
        {
            var enumValues = Enum.GetValues(typeof(PaymentGatewayType)) as PaymentGatewayType[];
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
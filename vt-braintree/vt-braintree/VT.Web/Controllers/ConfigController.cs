using System;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using VT.Common;
using VT.Services.DTOs;
using VT.Services.Interfaces;
using VT.Web.Models;

namespace VT.Web.Controllers
{
    public class ConfigController : BaseController
    {
        #region Fields

        private readonly IPaymentGatewayService _braintreePaymentService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Constructor

        public ConfigController(IPaymentGatewayService braintreePaymentService, ICustomerService customerService)
        {
            _braintreePaymentService = braintreePaymentService;
            _customerService = customerService;
        }

        #endregion

        #region Action Method(s)

        /*----------------------------------CONFIGURE MERCHANT---------------------------------------*/

        //For Super Admin
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        [Route("~/ConfigureMerchant/{id}")]
        public ActionResult ConfigureMerchant(int id) 
        {
            var model = GetMerchantAccountModel(id);
            return View(model);
        }

        //For Company Admin
        [Authorize(Roles = "CompanyAdmin")]
        [HttpGet]
        [Route("~/SetupMerchant")]
        public ActionResult SetupMerchant() 
        {
            if (CurrentIdentity != null && CurrentIdentity.CompanyId != null)
            {
                var model = GetMerchantAccountModel(CurrentIdentity.CompanyId.Value);
                return View("ConfigureMerchant", model);
            }
            return RedirectToAction("Login", "Auth");
        }

        //Common Post Method for Super Admin/Company Admin
        [Authorize(Roles = "SuperAdmin,CompanyAdmin")] 
        [HttpPost]
        [Route("~/ConfigureMerchant")]
        public ActionResult ConfigureMerchant(MerchantAccountViewModel model)
        {
            //prepare dto
            var request = Mapper.Map<OrganizationAccountRequest>(model);

            //get response from braintree service
            var response = _braintreePaymentService.CreateOrganizationMerchantAccount(request);

            return Json(new
            {
                success = response.Success,
                message = response.Success ? "Merchant Account has been successfully added. Reference #: " + response.ReferenceNumber : response.Message
            });
        }


        /* ------------------------------------------------------------------------------------------*/


        /*----------------------------------CONFIGURE CREDIT CARD-------------------------------------*/

        //MERCHANT CREDIT CARD

        //For Super Admin
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        [Route("~/ConfigureCc/{id}")]
        public ActionResult ConfigureCc(int id)
        {
            var model = GetGatewayMerchantViewModel(id);
            return View("ConfigureCc", model);
        }

        //For Company Admin
        [Authorize(Roles = "CompanyAdmin")]
        [HttpGet]
        [Route("~/SetupCc")]
        public ActionResult SetupCc()
        {
            if(CurrentIdentity.CompanyId == null) return RedirectToAction("Login", "Auth");

            var model = GetGatewayMerchantViewModel(CurrentIdentity.CompanyId.Value);
            return View("ConfigureCc", model);
        }

        //CUSTOMER CREDIT CARD
        [Authorize(Roles = "SuperAdmin,CompanyAdmin")]
        [HttpGet]
        [Route("~/ConfigureCustomerCc/{id}")]
        public ActionResult ConfigureCustomerCc(int id)
        {
            var model = GetGatewayCustomerViewModel(id);
            return View("ConfigureCc", model);
        }

        //Common Post Method for Super Admin/Company Admin
        [Authorize(Roles = "SuperAdmin,CompanyAdmin")]
        [HttpPost]
        [Route("~/ConfigureCc")]
        public ActionResult ConfigureCc(GatewayCustomerViewModel model)
        {
            //get nonce from braintree
            //var nonce = Request.Form["payment_method_nonce"];

            //prepare dto
            var request = Mapper.Map<GatewayCustomerRequest>(model);

            //assign nonce
            request.NonceFromTheClient = model.Nonce;


            //get response from braintree service
            var response = model.AccountType == GatewayAccount.Merchant ?
                _braintreePaymentService.SaveMerchantCc(request) : _braintreePaymentService.SaveCustomerCc(request);
            
            return Json(new
            {
                success = response.Success,
                message = response.Success ? "Credit Card has been successfully saved. Reference #: " + response.ReferenceNumber : response.Message
            });
        }

        
        
        /* ------------------------------------------------------------------------------------------*/

        #endregion

        #region Private Methods
        
        private MerchantAccountViewModel GetMerchantAccountModel(int id)
        {
            var response =  _braintreePaymentService.GetMerchantAccountDetail(new MerchantAccountDetailRequest
            {
                CompanyId = id
            });

            var model = Mapper.Map<MerchantAccountViewModel>(response);
            model.RedirectUrl = User.IsInRole(UserRoles.SuperAdmin.ToString())
                ? Url.Action("Index", "Organizations")
                : Url.Action("Index", "Users");
            return model;
        }

        private GatewayCustomerViewModel GetGatewayCustomerViewModel(int id)
        {
            var model = new GatewayCustomerViewModel();

            var response = _braintreePaymentService.GetCustomerCreditCardDetail(new CustomerCreditCardDetailRequest
            {
                CustomerId = id
            });

            if (response != null)
            {
                model.FirstName = response.FirstName;
                model.LastName = response.LastName;
                model.Email = response.Email;
                model.CreditCard = response.CreditCardNumber;
                model.Expiration = response.Expiration;
                model.CcToken = response.CcToken;
            }

            model.CustomerId = id;
            model.ClientToken = GatewayConstant.Gateway.ClientToken.generate();
            model.AccountType = GatewayAccount.Customer;
            model.RedirectUrl = Url.Action("Index", "Customers");
            return model;
        }
        
        private GatewayCustomerViewModel GetGatewayMerchantViewModel(int id)
        {
            var model = new GatewayCustomerViewModel();

            var response = _braintreePaymentService.GetMerchantCreditCardDetail(new MerchantCreditCardDetailRequest
            {
                CompanyId = id
            });

            if (response != null)
            {
                model.FirstName = response.FirstName;
                model.LastName = response.LastName;
                model.Email = response.Email;
                model.CreditCard = response.CreditCardNumber;
                model.Expiration = response.Expiration;
                model.CcToken = response.CcToken;
            }
            model.AccountType = GatewayAccount.Merchant;
            model.CustomerId = id;
            model.ClientToken = GatewayConstant.Gateway.ClientToken.generate();
            model.RedirectUrl = Url.Action("Index", "Organizations");

            return model;
        }

        #endregion
    }
}
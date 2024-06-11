using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using VT.Common;
using VT.Services.DTOs;
using VT.Services.Interfaces;
using VT.Web.Models;

namespace VT.Web.Controllers
{
    [AllowAnonymous]
    public class SetPaymentController : Controller
    {
        #region Fields

        private readonly IPaymentGatewayService _paymentService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Constructor

        public SetPaymentController(IPaymentGatewayService paymentService, 
                         ICustomerService customerService)
        {
            _paymentService = paymentService;
            _customerService = customerService;
        }

        #endregion

        // GET: SetPayment
        [HttpGet]
        [Route("~/SetPaymentMethod/{id}")]
        public ActionResult Index(string id)
        {
            var model = GetGatewayCustomerViewModel(id);
            return View(model);
        }
        
        [HttpPost]
        [Route("~/SetPaymentMethod")]
        public ActionResult SetPaymentMethod(GatewayCustomerViewModel model)
        {
            //prepare dto
            var request = Mapper.Map<GatewayCustomerRequest>(model);

            //assign nonce
            request.NonceFromTheClient = model.Nonce;

            //get response from braintree service
            var response = _paymentService.SaveCustomerCc(request);

            //prepare message view model
            return Json(new
            {
                success = response.Success,
                message = response.Success ? "Customer Account has been successfully added. Reference #: " + response.ReferenceNumber : response.Message
            });
        }
        
        private GatewayCustomerViewModel GetGatewayCustomerViewModel(string id)
        {
            var model = new GatewayCustomerViewModel();

            var customer = _customerService.GetCustomer(id);

            if (customer == null)
            {
                model.IsCustomerNotFound = true;
                return model;
            }

            if (customer.ExpireAt < DateTime.UtcNow)
            {
                model.IsCustomerTokenExpired = true;
                return model;
            }

            var response = _paymentService.GetCustomerCreditCardDetail(new CustomerCreditCardDetailRequest
            {
                CustomerId = customer.CustomerId
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

            model.CustomerId = customer.CustomerId;
            model.ClientToken = GatewayConstant.Gateway.ClientToken.generate();
            model.AccountType = GatewayAccount.Customer;
            model.RedirectUrl = string.Empty;
            return model;
        }
    }
}
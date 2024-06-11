using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Amazon.ElasticTranscoder.Model.Internal.MarshallTransformations;
using AutoMapper;
using VT.Common;
using VT.Common.Utils;
using VT.Data;
using VT.Services.DTOs.SplashPayments;
using VT.Services.Interfaces;
using VT.Web.Models;

namespace VT.Web.Controllers
{
    public class SplashController : BaseController
    {
        #region Field

        private readonly ISplashPaymentService _splashPaymentService;

        #endregion

        #region Constructor

        public SplashController(ISplashPaymentService splashPaymentService)
        {
            _splashPaymentService = splashPaymentService;
        }

        #endregion

        #region Action Method(s)


        [Authorize(Roles = "SuperAdmin,CompanyAdmin")]
        [Route("~/Splash/MerchantSetup/{id?}")]
        public ActionResult MerchantSetup(int? id)
        {
            var companyId = id ?? CurrentIdentity.CompanyId;

            if (companyId != null)
            {
                var model = GetMerchantAccountModel(companyId.Value);
                return model.IsEditMode ? View("EditMerchantSetup", model) : View("MerchantSetup", model);
            }
            return RedirectToAction("Login", "Auth");
        }

        /// <summary>
        /// Save merchant
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("~/Splash/SaveMerchant")]
        public ActionResult SaveMerchant(SplashMerchantModel model)
        {
            var request = new SplashCreateMerchantRequest
            {
                AnnualCCSales = model.AnnualCCSales,
                Established = model.Established,
                MerchantCategoryCode = model.MerchantCategoryCode,
                EntityAddress1 = model.EntityAddress1,
                EntityCity = model.EntityCity,
                EntityCountry = model.EntityCountry,
                EntityEmail = model.EntityEmail,
                EntityEmployerId = model.EntityEmployerId,
                EntityName = model.EntityName,
                EntityPhone = model.EntityPhone,
                EntityState = model.EntityState,
                EntityWebsite = model.EntityWebsite,
                EntityZip = model.EntityZip,
                CardOrAccountNumber = model.CardOrAccountNumber,
                AccountsPaymentMethod = model.AccountsPaymentMethod,
                AccountsRoutingCode = model.AccountsRoutingCode,
                MemberTitle = model.MemberTitle,
                MemberDateOfBirth = model.MemberDateOfBirth,
                MemberDriverLicense = model.MemberDriverLicense,
                MemberDriverLicenseState = model.MemberDriverLicenseState,
                MemberEmail = model.MemberEmail,
                MemberFirstName = model.MemberFirstName,
                MemberLastName = model.MemberLastName,
                MemberOwnerShip = model.MemberOwnerShip,
                MemberSocialSecurityNumber = model.MemberSocialSecurityNumber,
                CompanyId = CurrentIdentity.CompanyId.HasValue ? CurrentIdentity.CompanyId.Value : 0
            };

            var response = _splashPaymentService.CreateMerchant(request);

            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }

        /// <summary>
        /// Save merchant
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("~/Splash/SaveMerchantInfo")]
        public ActionResult SaveMerchantInfo(SplashMerchantEditModel model)
        {
            var request = new UpdateSplashMerchantRequest
            {
                AnnualCCSales = model.AnnualCCSales,
                Established = model.Established,
                MerchantCategoryCode = model.MerchantCategoryCode,
                CompanyId = CurrentIdentity.CompanyId.HasValue ? CurrentIdentity.CompanyId.Value : 0
            };

            var response = _splashPaymentService.UpdateMerchantInfo(request);

            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }

        /// <summary>
        /// Save merchant
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("~/Splash/SaveEntity")]
        public ActionResult SaveEntity(SplashMerchantEntityModel model)
        {
            var request = new UpdateSplashMerchantEntityRequest
            {
                EntityAddress1 = model.EntityAddress1,
                EntityCity = model.EntityCity,
                EntityCountry = model.EntityCountry,
                EntityEmail = model.EntityEmail,
                EntityEmployerId = "123456789",
                EntityName = model.EntityName,
                EntityPhone = model.EntityPhone,
                EntityState = model.EntityState,
                EntityType = "1",
                EntityWebsite = model.EntityWebsite,
                EntityZip = model.EntityZip,
                CompanyId = CurrentIdentity.CompanyId.HasValue ? CurrentIdentity.CompanyId.Value : 0
            };

            var response = _splashPaymentService.UpdateMerchantEntity(request);

            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }

        /// <summary>
        /// update member
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("~/Splash/SaveMember")]
        public ActionResult SaveMember(SplashMerchantMemberModel model)
        {
            var request = new UpdateSplashMerchantMemberRequest
            {
                MemberTitle = model.MemberTitle,
                MemberDateOfBirth = model.MemberDateOfBirth,
                MemberDriverLicense = model.MemberDriverLicense,
                MemberDriverLicenseState = model.MemberDriverLicenseState,
                MemberEmail = model.MemberEmail,
                MemberFirstName = model.MemberFirstName,
                MemberLastName = model.MemberLastName,
                MemberOwnerShip = model.MemberOwnerShip,
                MemberSocialSecurityNumber = model.MemberSocialSecurityNumber,
                CompanyId = CurrentIdentity.CompanyId.HasValue ? CurrentIdentity.CompanyId.Value : 0
            };

            var response = _splashPaymentService.UpdateMerchantMember(request);

            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }

        /// <summary>
        /// update account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("~/Splash/SaveAccount")]
        public ActionResult SaveAccount(SplashMerchantAccountModel model)
        {
            var request = new UpdateSplashMerchantAccountRequest
            {
                AccountsPaymentMethod = model.AccountsPaymentMethod,
                AccountsRoutingCode = model.AccountsRoutingCode,
                CardOrAccountNumber = model.CardOrAccountNumber,
                CompanyId = CurrentIdentity.CompanyId.HasValue ? CurrentIdentity.CompanyId.Value : 0
            };

            var response = _splashPaymentService.UpdateMerchantAccount(request);

            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }


        /*****************************CUSTOMER**********************************/


        //CUSTOMER CREDIT CARD
        [Authorize(Roles = "SuperAdmin,CompanyAdmin")]
        [HttpGet]
        [Route("~/ConfigureSplashCustomerCc/{id}")]
        public ActionResult ConfigureSplashCustomerCc(int id)
        {
            PopulateViews();
            var model = GetCustomerModel(id);
            return model.IsEditMode ? View("EditCustomerSetup", model) : View("SetupCc", model);
        }


        /// <summary>
        /// Save credit card for company or customer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("~/Splash/SaveCustomer")]
        public ActionResult SaveCustomer(SplashCustomerModel model)
        {
            var request = new SplashCustomerCreateRequest
            {
                CustomerId = model.CustomerId,
                CustomerFirstName = model.CustomerFirstName,
                CustomerLastName = model.CustomerLastName,
                CustomerAddress1 = model.CustomerAddress1,
                CustomerPhone = model.CustomerPhone,
                CustomerEmail = model.CustomerEmail,
                PaymentMethod = model.PaymentMethod,
                PaymentNumber = model.PaymentNumber,
                PaymentCvv = model.PaymentCvv,
                PaymentExpiration = model.PaymentExpiration,
                CompanyId = CurrentIdentity.CompanyId.HasValue ? CurrentIdentity.CompanyId.Value : 0
            };

            var response = model.IsCustomerMode ? _splashPaymentService.CreateCcCustomerForCustomer(request) :
                _splashPaymentService.CreateCcCustomerForCompany(request);

            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }

        /// <summary>
        /// Save customer card detail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("~/Splash/SavePaymentDetail")]
        public ActionResult SavePaymentDetail(SplashCustomerModel model)
        {
            var request = new AddCustomerCreditCardRequest
            {
                CustomerId = model.CustomerId,
                PaymentNumber = model.PaymentNumber,
                PaymentMethod = model.PaymentMethod,
                PaymentCvv = model.PaymentCvv,
                PaymentExpiration = model.PaymentExpiration,
                CompanyId = CurrentIdentity.CompanyId.HasValue ? CurrentIdentity.CompanyId.Value : 0
            };

            var response = model.CustomerId > 0 ? _splashPaymentService.AddCustomerCreditCard(request) : _splashPaymentService.AddCompanyCreditCard(request);

            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }

        /// <summary>
        /// Save customer detail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("~/Splash/SaveCustomerDetail")]
        public ActionResult SaveCustomerDetail(SplashCustomerModel model)
        {
            var request = new UpdateSplashCustomerRequest
            {
                CustomerId = model.CustomerId,
                FirstName = model.CustomerFirstName,
                LastName = model.CustomerLastName,
                Email = model.CustomerEmail,
                CompanyId = CurrentIdentity.CompanyId.HasValue ? CurrentIdentity.CompanyId.Value : 0
            };

            var response = model.CustomerId > 0 ? _splashPaymentService.UpdateCustomerCustomer(request) : 
                                _splashPaymentService.UpdateCompanyCustomer(request);

            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }


        /// <summary>
        /// Disable Company Cc
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("~/Splash/DisableCompanyCc")]
        public ActionResult DeletePayment()
        {
             var response = _splashPaymentService.DisableCompanyCreditCard(CurrentIdentity.CompanyId.HasValue ? 
               CurrentIdentity.CompanyId.Value : 0); 

            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }
          
        /// <summary>
        /// Disable Company Cc
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("~/Splash/DisableCustomerCc/{id}")]
        public ActionResult DisableCustomerCc(int id)
        {
            var response = _splashPaymentService.DisableCustomerCreditCard(id);

            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }

        /// <summary>
        /// Setup Merchant
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SuperAdmin,CompanyAdmin")]
        [HttpGet]
        [Route("~/Splash/SetupMerchantCc/{id?}")]
        public ActionResult SetupMerchantCc(int? id)
        {
            PopulateViews();
            var companyId = id ?? CurrentIdentity.CompanyId;
            if (companyId != null )
            {
                var model = GetCompanyModel(companyId.Value);
                return model.IsEditMode ? View("EditCustomerSetup", model) : View("SetupCc", model);
            }
            return RedirectToAction("Login", "Auth");
        }

        #endregion


        #region Private methods

        private void PopulateViews()
        {
            var list = GetPaymentMethod();
            ViewData["PaymentMethod"] = list;
        }


        private List<SelectListItem> GetPaymentMethod()
        {
            var enumValues = Enum.GetValues(typeof(PaymentMethod)) as PaymentMethod[];
            if (enumValues == null) return null;
            return enumValues.Select(enumValue => new SelectListItem
            {
                Value = ((int)enumValue).ToString(CultureInfo.InvariantCulture),
                Text = EnumUtil.GetDescription(enumValue)
            }).ToList();
        }

        private SplashMerchantModel GetMerchantAccountModel(int id)
        {
            var response = _splashPaymentService.GetMerchantDetail(id);

            if (response.Success)
            {
                var model = Mapper.Map<SplashMerchantModel>(response);
                model.RedirectUrl = User.IsInRole(UserRoles.SuperAdmin.ToString())
                    ? Url.Action("Index", "Organizations")
                    : Url.Action("Index", "Users");
                model.IsEditMode = true;
                return model;
            }
            return new SplashMerchantModel();
        }

        private SplashCustomerModel GetCompanyModel(int id)
        {
            var response = _splashPaymentService.GetCompanyCcDetail(id);

            if (response.Success)
            {
                var model = Mapper.Map<SplashCustomerModel>(response);
                model.RedirectUrl = User.IsInRole(UserRoles.SuperAdmin.ToString())
                    ? Url.Action("Index", "Organizations")
                    : Url.Action("Index", "Users");
                model.IsEditMode = true;
                return model;
            }
            return new SplashCustomerModel();
        }

        private SplashCustomerModel GetCustomerModel(int id)
        {
            var response = _splashPaymentService.GetCustomerCcDetail(id);

            var model = Mapper.Map<SplashCustomerModel>(response);
            model.RedirectUrl = User.IsInRole(UserRoles.SuperAdmin.ToString())
                ? Url.Action("Index", "Organizations")
                : Url.Action("Index", "Users");
            model.IsEditMode = !string.IsNullOrEmpty(response.PaymentNumber);
            model.IsCustomerMode = true;
            model.CustomerId = id;
            return model;
        }

        private SplashCustomerModel GetSplashCustomerViewModel(int id)
        {
            var model = new SplashCustomerModel();

            var response = _splashPaymentService.GetCompanyCcDetail(id);

            if (response != null)
            {
                model.CustomerFirstName = response.CustomerFirstName;
                model.CustomerLastName = response.CustomerLastName;
                model.CustomerEmail = response.CustomerEmail;
                model.PaymentNumber = response.PaymentNumber;
                model.PaymentExpiration = response.PaymentExpiration;
            }

            model.CompanyId = id;
            model.RedirectUrl = Url.Action("Index", "Customers");
            return model;
        }

        #endregion  
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using VT.Common;
using VT.Data;
using VT.Services.DTOs;
using VT.Services.Interfaces;
using VT.Web.Components;
using VT.Web.Models;

namespace VT.Web.Controllers
{
    public class CompanyUserController : BaseController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IServiceRecordService _serviceRecordService;
        private readonly ICustomerServiceService _customerServiceService;

        #endregion

        #region Constructor

        public CompanyUserController(ICustomerService customerService,
            ICustomerServiceService customerServiceService,
            IServiceRecordService serviceRecordService
            )
        {
            _customerService = customerService;
            _serviceRecordService = serviceRecordService;
            _customerServiceService = customerServiceService;
        }

        #endregion

        #region Action Method(s)

        [HttpGet]
        [Route("~/CompanyUser/{id?}")]
        public ActionResult Index(int? id)
        {
            PopulateViews(id);
            var model = new SaveCustomerViewModel
            {
                CustomerId = id != null ? id.Value : 0
            };
            return View(model);
        }

        [HttpGet]
        [Route("~/CompanyUser/ServiceRecord/{id}")]
        public ActionResult ServiceRecord(int id)
        {
            PopulateViews(id);
            var model = GetCustomerRecord(id);
            //model.Items = GetServiceItemsFromSession();
            return View(model);
        }

        [HttpPost]
        [Route("~/CompanyUser/SaveServiceRecordItem")]
        public ActionResult SaveServiceRecordItem()
        {
            return null;
        }

        [Route("~/CompanyUser/AddServices/{id}")]
        public ActionResult AddServices(int id)
        {
            var companyServices = GetCustomerServiceList(id);
            ViewData["CustomerServices"] = companyServices;

            var model = new SaveCompanyServiceModel
            {
                CustomerId = id,
                ImageWidth = ApplicationSettings.PhotoWidth,
                ImageHeight = ApplicationSettings.PhotoHeight,
                ImageCrop = ApplicationSettings.PhotoCrop,
                ImageQuality = ApplicationSettings.PhotoQuality
            };
            return View(model);
        }

        [HttpGet]
        [Route("~/CompanyUser/AddServiceRecord/{id}")]
        public ActionResult AddServiceRecord(int id)
        {
            //prepare dto
            var items = GetServiceItemsFromSession();

            if (!items.Any())
            {
                TempData["Message"] = "Please add at least one service record item.";
                return RedirectToAction("ServiceRecord", "CompanyUser", new { id = id });
            }

            var description = string.Empty;
            var serviceRecordItems = new List<SaveServiceRecordItemRequest>();

            foreach (var recordItemViewModel in items)
            {
                description += recordItemViewModel.Description + " ";

                var serviceRecordItem = new SaveServiceRecordItemRequest
                {
                    CustomerServiceId = recordItemViewModel.CustomerServiceId == -1 ? (int?)null : recordItemViewModel.CustomerServiceId,
                    CompanyServiceId = recordItemViewModel.CustomerServiceId == -1 ? (int?)null : recordItemViewModel.CompanyServiceId,
                    CostOfService = recordItemViewModel.Cost != null ? Convert.ToDouble(recordItemViewModel.Cost) : 0,
                    ServiceName = recordItemViewModel.ServiceName,
                    CustomerId = id,
                    Description = recordItemViewModel.Description,
                    StartTime = DateTime.UtcNow.AddMinutes(-60),
                    EndTime = DateTime.UtcNow,
                    Type = recordItemViewModel.ServiceName == "Non-Standard"
                        ? ServiceRecordItemType.NonStandard
                        : ServiceRecordItemType.Standard,
                    Attachments = new List<ServiceRecordItemAttachmentRequest>()
                };

                foreach (var attachmentViewModel in recordItemViewModel.Attachments)
                {
                    serviceRecordItem.Attachments.Add(new ServiceRecordItemAttachmentRequest
                    {
                        Date = attachmentViewModel.Date,
                        Description = "After",
                        Type = attachmentViewModel.Type,
                        Url = attachmentViewModel.Url
                    });
                }

                serviceRecordItems.Add(serviceRecordItem);
            }

            var request = new SaveServiceRecordRequest
            {
                BilledToCompany = false,
                CompanyId = CurrentIdentity.CompanyId.Value,
                Description = description,
                CompanyWorkerId = CurrentIdentity.UserId,
                CustomerId = id,
                EndTime = DateTime.UtcNow,
                Status = ServiceRecordStatus.NotProcessed,
                SaveServiceRecordItems = serviceRecordItems
            };

            var response = _serviceRecordService.SaveServiceRecord(request);

            if (response.Success)
            {
                TempData["Message"] = "Service record saved successfully.";
            }
            else
            {
                TempData["Message"] = response.Message;
            }

            Session["ServiceItems"] = new List<ServiceRecordItemViewModel>();

            return RedirectToAction("Index");
        }

        [Route("~/CompanyUser/GetCustomerDetail/{id}")]
        public ActionResult GetCustomerDetail(int id)
        {
            var model = GetCustomerRecord(id);
            return PartialView("CustomerGetDetailsView", model);
        }

        [HttpPost]
        [Route("~/CompanyUser/GetServiceItemList")]
        public ActionResult GetServiceItemList([DataSourceRequest] DataSourceRequest request)
        {
            var items = GetServiceItemsFromSession();
            return Json(items.ToDataSourceResult(request));
        }

        [HttpPost]
        [Route("~/CompanyUser/GetCustomerServiceDetail/{id}")]
        public ActionResult GetCustomerServiceDetail(int id)
        {
            var customerService = _customerServiceService.GetCustomerService(id);
            return Json(new
            {
                success = customerService != null,
                message = customerService == null ? "Customer service does not exist in the system" : string.Empty,
                name = customerService != null ? customerService.Name : string.Empty,
                cost = customerService != null ? customerService.Cost : (double?)null,
                description = customerService != null ? customerService.Description : string.Empty,
                companyServiceId = customerService != null ? customerService.CompanyServiceId : 0
            });
        }

        [HttpPost]
        [Route("~/CompanyUser/SaveServiceItem")]
        public ActionResult SaveServiceItem(SaveCompanyServiceModel model)
        {
            var items = GetServiceItemsFromSession();

            var attachments = new List<ServiceRecordItemAttachmentViewModel>();

            //before image
            /*if (!string.IsNullOrEmpty(model.ImageFileNameBefore))
            {
                attachments.Add(new ServiceRecordItemAttachmentViewModel
                {
                    Date = DateTime.UtcNow,
                    Description = string.Empty,
                    Type = AttachmentType.Before.ToString(),
                    Url = string.Format(ApplicationSettings.AwsS3Url, model.ImageFileNameBefore)
                });
            }

            //after image
            if (!string.IsNullOrEmpty(model.ImageFileNameAfter))
            {
                attachments.Add(new ServiceRecordItemAttachmentViewModel
                {
                   Date = DateTime.UtcNow,
                   Description = string.Empty,
                   Type = AttachmentType.After.ToString(),
                   Url = string.Format(ApplicationSettings.AwsS3Url, model.ImageFileNameAfter)
                });
            }*/

            if (model.UploadImages != null)
            {
                attachments.AddRange(model.UploadImages.Select(image => new ServiceRecordItemAttachmentViewModel
                {
                    Date = GetImageTime(image),
                    Description = string.Empty,
                    Type = AttachmentType.After.ToString(),
                    Url = string.Format(ApplicationSettings.AwsS3Url, image)
                }));
            }

            items.Add(new ServiceRecordItemViewModel
            {
                ServiceItemId = GetNextServiceItemId(),
                CompanyId = model.CompanyId,
                CustomerServiceId = model.CustomerServiceId,
                CompanyServiceId = model.CompanyServiceId,
                Description = model.Description,
                ServiceName = model.Name,
                Cost = model.Cost,
                Attachments = attachments
            });

            Session["ServiceItems"] = items;

            return Json(new
            {
                success = true,
                message = "Service item added successfully."
            });
        }

        private DateTime GetImageTime(string fileName)
        {
            int index = fileName.IndexOf("TIME-");
            index += 5;
            int lastHypenIndex = fileName.LastIndexOf("_");
            if (index >= 0 && index <= fileName.Length && lastHypenIndex > index && (lastHypenIndex - index) > 1)
            {
                string ticksString = fileName.Substring(index, lastHypenIndex - index);

                long val;
                if (Int64.TryParse(ticksString, out val))
                {
                    DateTime date = new DateTime(val);
                    return date;
                }
            }
            return DateTime.UtcNow;
        }

        [HttpPost]
        [Route("~/CompanyUser/GetKeys")]
        public JsonResult GetKeys()
        {
            var items = GetServiceItemsFromSession();

            if (!items.Any())
            {
                return Json(new List<string>());
            }
            else
            {
                var keys = new List<string>();

                foreach (var item in items)
                    keys.AddRange(item.Attachments.Select(x => x.Url));

                var res = keys.Select(key => key.Replace(ApplicationSettings.AwsS3Url, String.Empty)).ToList();

                return Json(res);
            }
        }

        [HttpPost]
        [Route("~/CompanyUser/DeleteServiceItems")]
        public ActionResult DeleteServiceItems(DeleteServiceItemsViewModel model)
        {
            var items = GetServiceItemsFromSession();

            foreach (var id in model.Ids)
            {
                var item = items.FirstOrDefault(x => x.ServiceItemId == id);
                if (item != null) items.Remove(item);
            }

            return Json(new
            {
                success = true,
                message = "Selected service items have been successfully deleted."
            });
        }

        #endregion

        #region Private Method(s)

        private List<ServiceRecordItemViewModel> GetServiceItemsFromSession()
        {
            var items = new List<ServiceRecordItemViewModel>();

            if (Session["ServiceItems"] != null)
            {
                items = (List<ServiceRecordItemViewModel>)Session["ServiceItems"];
            }
            return items;
        }

        private int GetNextServiceItemId()
        {
            var items = new List<ServiceRecordItemViewModel>();

            if (Session["ServiceItems"] != null)
            {
                items = (List<ServiceRecordItemViewModel>)Session["ServiceItems"];
            }
            var id = items.Any() ? items.Min(x => x.ServiceItemId) : 0;
            return id - 1;
        }

        private CustomerDetailViewModel GetCustomerRecord(int id)
        {
            var response = _customerService.GetCustomerDetail(id);

            if (response == null) return null;

            const string bucket = "verifyteck";
            const string acl = "private";
            const string accessKeyId = "AKIAIM5TSADPNSAXTKBA";
            const string secret = "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";

            var policy = AmazonS3Helper.ConstructPolicy(bucket,
                DateTime.UtcNow.Add(new TimeSpan(0, 10, 0, 0)), acl, accessKeyId);

            var model = new CustomerDetailViewModel
            {
                Name = response.Name,
                Address = response.Address,
                City = response.City,
                State = response.State,
                Country = response.Country,
                PostalCode = response.PostalCode,

                ContactEmail = response.ContactEmail,
                ContactFirstName = response.ContactFirstName,
                ContactLastName = response.ContactLastName,
                ContactMobile = response.ContactMobile,
                ContactTelephone = response.ContactTelephone,

                CustomerId = response.CustomerId,
                IsDeleted = response.IsDeleted,
                AwsAccessKeyId = accessKeyId,
                Policy = policy,
                Signature = AmazonS3Helper.CreateSignature(policy, secret),
                Bucket = bucket,
                Acl = acl
            };

            return model;
        }

        private List<SelectListItem> GetCustomerServiceList(int customerId)
        {
            var services = _customerServiceService.GetCustomerServices(customerId);

            var listServices = new List<SelectListItem> { new SelectListItem { Text = "--Select--", Value = "" } };
            listServices.AddRange(services.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CustomerServiceId.ToString(CultureInfo.InvariantCulture)
            }));
            listServices.Add(new SelectListItem { Text = "Non-Standard", Value = "-1" });
            return listServices;
        }

        private void PopulateViews(int? customerId)
        {
            var list = GetCustomerList();
            ViewData["Customers"] = list;

            if (customerId != null)
            {
                var companyServices = GetCustomerServiceList(customerId.Value);
                ViewData["CustomerServices"] = companyServices;
            }

        }

        private List<SelectListItem> GetCustomerList()
        {
            //var customers = _customerService.GetAllCustomers(CurrentIdentity.CompanyId);

            var customers = _customerService.GetUserCustomers(CurrentIdentity.UserId);

            if (!customers.Any())
            {
                customers = _customerService.GetAllCustomers(CurrentIdentity.CompanyId).OrderBy(x => x.Name).ToList();
            }

            var list = new List<SelectListItem> { new SelectListItem { Text = "--Select--", Value = "" } };
            list.AddRange(customers.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CustomerId.ToString(CultureInfo.InvariantCulture)
            }));
            return list.ToList();
        }

        [HttpPost]
        [Route("~/CompanyUser/GetImageName")]
        public ActionResult GetImageName(ImageNameViewModal model)
        {
            var companyId = CurrentIdentity.CompanyId.Value;
            var userId = CurrentIdentity.UserId;
            var newfileName = string.Format("{0}", Guid.NewGuid()); //Path.GetExtension(model.FileName)
            string fileName = string.Format("ORG-{0}_EMP-{1}_CUST-{2}_TIME-{3}_{4}", companyId, userId, model.CustomerId, DateTime.UtcNow.Ticks, newfileName);

            return Json(new
            {
                fileName
            });
        }

        #endregion

    }
}
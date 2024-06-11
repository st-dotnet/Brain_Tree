using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DataAccess;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using VT.Common;
using VT.Common.Utils;
using VT.Data.Entities;
using VT.Services.DTOs;
using VT.Services.Interfaces;
using VT.Web.Models;

namespace VT.Web.Controllers
{
    public class CustomersController : BaseController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ICompanyService _companyService;
        private readonly ICompanyServiceService _companyServiceService;
        private readonly ICustomerServiceService _customerServiceService;
        private readonly IServiceRecordService _serviceRecordService;
        private readonly IServiceRecordItemService _serviceRecordItemService;
        private readonly IEmailService _emailService;

        #endregion

        #region Constructor

        public CustomersController(ICustomerService customerService,
            ICompanyService companyService,
            ICompanyServiceService companyServiceService,
            ICustomerServiceService customerServiceService,
            IServiceRecordService serviceRecordService,
            IServiceRecordItemService serviceRecordItemService,
            IEmailService emailService)
        {
            _customerService = customerService;
            _companyService = companyService;
            _companyServiceService = companyServiceService;
            _customerServiceService = customerServiceService;
            _serviceRecordService = serviceRecordService;
            _serviceRecordItemService = serviceRecordItemService;
            _emailService = emailService;
        }

        #endregion

        #region Action Method(s)

        [HttpGet]
        [Route("~/Customers")]
        public ActionResult Index()
        {
            PopulateViews();
            return View();
        }

        [HttpPost]
        [Route("~/Customers/DeleteCustomers")]
        public ActionResult DeleteCustomers(DeleteCustomerViewModel model)
        {
            var response = _customerService.DeleteCustomers(model.Ids);
            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }

        [HttpPost]
        [Route("~/Customers/AddCustomer")]
        public ActionResult AddCustomer()
        {
            Session["data"] = null;
            return Json(new
            {
                success = true
            });
        }


        [HttpPost]
        [Route("~/Customers/EditCustomer/{id}")]
        public ActionResult EditCustomer(int id)
        {
            Session["data"] = null;

            var response = _customerService.GetCustomerDetail(id);

            if (response == null) return null;

            var model = new SaveCustomerViewModel
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
                ContactMiddleName = response.ContactMiddleName,
                CustomerId = response.CustomerId,
                CompanyId = response.CompanyId,
                IsActive = response.IsActive,
                HasGatewayCustomer = !string.IsNullOrEmpty(response.GatewayCustomerId)
            };

            return Json(model);
        }

        [HttpPost]
        [Route("~/Customers/GetCustomerServicesGrid")]
        public ActionResult GetCustomerServicesGrid(BaseCustomerServiceViewModel model)
        {
            PopulateCompanyServices(model.CompanyId);
            return PartialView("CustomerServicesGrid", new SaveCustomerViewModel
            {
                CompanyId = model.CompanyId,
                CustomerId = model.CustomerId
            });
        }

        [HttpPost]
        [Route("~/Customers/SaveCustomer")]
        public ActionResult SaveCustomer(SaveCustomerViewModel model)
        {
            var response = _customerService.SaveCustomer(new CustomerSaveRequest
            {
                Address = model.Address,
                City = model.City,
                CompanyId = model.CompanyId,
                ContactEmail = model.ContactEmail,
                CustomerId = model.CustomerId,
                ContactFirstName = model.ContactFirstName,
                ContactLastName = model.ContactLastName,
                ContactMobile = model.ContactMobile,
                ContactTelephone = model.ContactTelephone,
                ContactMiddleName = model.ContactMiddleName,
                Country = model.Country,
                Name = model.Name,
                PostalCode = model.PostalCode,
                State = model.State,
                IsActive = model.IsActive
            });

            var data = GetDataFromSession();

            if (response.Success && data != null)
            {
                foreach (var customerServiceViewModel in data.Where(x => x.HasChanged || x.InMemoryAdded))
                {
                    var result = _customerServiceService.SaveCustomerService(new SaveCustomerServiceRequest
                    {
                        CustomerId = response.Customer.CustomerId,
                        CustomerServiceId = customerServiceViewModel.InMemoryAdded ? 0 : customerServiceViewModel.CustomerServiceId,
                        CompanyServiceId = customerServiceViewModel.CompanyServiceId,
                        Name = customerServiceViewModel.ServiceName,
                        Description = customerServiceViewModel.Description,
                        Price = customerServiceViewModel.Price
                    });
                }

                foreach (var customerServiceViewModel in data.Where(x => x.InMemoryDeleted))
                {
                    _customerServiceService.DeleteCustomerService(customerServiceViewModel.CustomerServiceId);
                }
            }

            Session["data"] = null;

            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }

        [HttpPost]
        [Route("~/Customers/CustomerList")]
        public ActionResult CustomerList([DataSourceRequest] DataSourceRequest request)
        {
            var data = GetCustomerList().ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Customers/GetCustomerDetail/{id}")]
        public ActionResult GetCustomerDetail(int id)
        {
            var response = _customerService.GetCustomerDetail(id);

            if (response == null) return null;

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
                IsDeleted = response.IsDeleted
            };

            return PartialView("CustomerDetailView", model);
        }

        [HttpPost]
        [Route("~/Customers/ServiceRecords/{id}")]
        public ActionResult ServiceRecords(int id, [DataSourceRequest] DataSourceRequest request)
        {
            var data = GetServiceRecordList(id).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Customers/ServiceRecordItems/{id}")]
        public ActionResult ServiceRecordItems(int id, [DataSourceRequest] DataSourceRequest request)
        {
            var data = GetServiceRecordItemList(id).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Customers/GetCustomerServicesDetail")]
        public ActionResult GetCustomerServicesDetail(BaseCustomerServiceViewModel request)
        {
            var model = new CustomerServiceViewModel
            {
                CustomerId = request.CustomerId,
                CompanyId = request.CompanyId,
            };
            PopulateCompanyServices(request.CompanyId);
            return PartialView("SaveCustomerService", model);
        }

        [HttpPost]
        [Route("~/Customers/GetCompanyServiceDetail/{id}")]
        public JsonResult GetCompanyServiceDetail(int id)
        {
            var companyService = _companyServiceService.GetCompanyService(id);
            return Json(new
            {
                success = companyService != null,
                message = companyService != null ? string.Empty : "Company service does not exist",
                result = new
                {
                    Name = companyService != null ? companyService.Name : string.Empty,
                    Description = companyService != null ? companyService.Description : string.Empty
                }
            });
        }

        [HttpPost]
        [Route("~/Customers/SaveCustomerServices")]
        public ActionResult SaveCustomerServices(CustomerServiceViewModel model)
        {
            //validation
            var message = string.Empty;

            if (model.CompanyServiceId < 1)
                message += "Company service is required.";

            if (string.IsNullOrEmpty(model.Description))
                message += "<br/> Description is required.";

            if (model.Price <= 0)
                message += "<br/> Price is required.";

            if (message.StartsWith("<br/>"))
            {
                message = message.Substring(5);
            }

            if (!string.IsNullOrEmpty(message))
            {
                return Json(new
                {
                    success = false,
                    message = message
                });
            }

            //New
            var data = GetDataFromSession();

            var companyService = GetCompanyService(model.CompanyServiceId);

            if (companyService != null)
                model.ServiceName = companyService.Name;

            var customerServiceFromData = data.FirstOrDefault(x => x.CustomerServiceId == model.CustomerServiceId);

            if (customerServiceFromData != null)
            {
                customerServiceFromData.CompanyServiceId = model.CompanyServiceId;
                customerServiceFromData.Description = model.Description;
                customerServiceFromData.Price = model.Price;
                customerServiceFromData.HasChanged = true;
            }
            else
            {
                if (data.Any(x => x.CompanyServiceId == model.CompanyServiceId))
                {
                    return Json(new
                    {
                        success = false,
                        message = "For this company service record already exists."
                    });
                }

                model.CustomerServiceId = GetMaxId();
                model.InMemoryAdded = true;
                data.Add(model);
            }

            return Json(new
            {
                success = true,
                message = "Data saved."
            });
        }

        [Route("~/Customers/GetCustomerService/{id}")]
        public JsonResult GetCustomerService(int id)
        {
            var success = false;
            var data = GetDataFromSession();
            var customerService = data.FirstOrDefault(x => x.CustomerServiceId == id);
            success = customerService != null;
            return Json(new
            {
                success = success,
                customerServiceId = customerService != null ? customerService.CustomerServiceId : 0,
                companyServiceId = customerService != null ? customerService.CompanyServiceId : 0,
                description = customerService != null ? customerService.Description : string.Empty,
                price = customerService != null ? customerService.Price : 0,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Customers/DeleteCustomerService/{id}")]
        public ActionResult DeleteCustomerService(int id)
        {
            var success = false;
            var message = string.Empty;

            var data = GetDataFromSession();
            if (data.Any())
            {
                var customerService = data.FirstOrDefault(x => x.CustomerServiceId == id);
                if (customerService != null)
                {
                    customerService.InMemoryDeleted = true;

                    success = true;
                    message = "Customer service deleted.";
                }
            }
            
            return Json(new
            {
                success = success,
                message = message
            });
        }


        [HttpPost]
        [Route("~/Customers/CustomerServiceList/{id}")]
        public ActionResult CustomerServiceList(int id, [DataSourceRequest] DataSourceRequest request)
        {
            var data = GetCustomerServiceList(id).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Customers/DeleteCustomerServices")]
        public ActionResult DeleteCustomerServices(DeleteCustomerServiceViewModel request)
        {
            var result = _customerServiceService.DeleteCustomerServices(request.Ids);
            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        [HttpPost]
        [Route("~/Customers/FillCompanyServices/{id}")]
        public ActionResult FillCompanyServices(int id, [DataSourceRequest] DataSourceRequest request)
        {
            var data = GetCustomerServiceList(id).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("~/Customers/GetCompanyServices/{id}")]
        public ActionResult GetCompanyServices(int id, int? customerId)
        {
            List<CompanyServiceListViewModel> data;
            if (Session["data"] != null)
            {
                //use data from session
                var customerServices = GetDataFromSession();
                var companyServiceIds = customerServices.Select(x => x.CompanyServiceId);
                var companyServices = _companyServiceService.GetAllCompanyServices(id);
                data = companyServices.Where(x => !companyServiceIds.Contains(x.CompanyServiceId)).ToList()
                    .Select(y => new CompanyServiceListViewModel()
                    {
                        Id = y.CompanyServiceId,
                        Name = y.Name,
                        CompanyId = y.CompanyId,
                        Description = y.Description
                    }).ToList();
            }
            else
            {
                //use data from database
                data = GetCompanyServiceList(id, customerId.Value).ToList();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //DropDownList Bind 
        [HttpGet]
        [Route("~/Customers/GetCompanyServicesNew")]
        public ActionResult GetCompanyServicesNew(int? companyId, int? customerId, int? companyServiceId)
        {
            List<CompanyServiceListViewModel> data;
            if (companyServiceId == 0 || companyServiceId == null)
            {
                if (Session["data"] != null)
                {
                    //use data from session
                    var customerServices = GetDataFromSession();
                    var companyServiceIds = customerServices.Select(x => x.CompanyServiceId);
                    var companyServices = _companyServiceService.GetAllCompanyServices(companyId);
                    data = companyServices.Where(x => !companyServiceIds.Contains(x.CompanyServiceId)).ToList()
                        .Select(y => new CompanyServiceListViewModel()
                        {
                            Id = y.CompanyServiceId,
                            Name = y.Name,
                            CompanyId = y.CompanyId,
                            Description = y.Description
                        }).ToList();
                }
                else
                {
                    //use data from database
                    data = GetCompanyServiceList(companyId.Value, customerId.Value).ToList();
                }
            }
            else
            {
                var companyService = _companyServiceService.GetCompanyService(companyServiceId.Value);
                data = new List<CompanyServiceListViewModel>
                {
                    new CompanyServiceListViewModel
                    {
                        CompanyId = companyService.CompanyId,
                        Description = companyService.Description,
                        Id = companyService.CompanyServiceId,
                        Name = companyService.Name
                    }
                };
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Route("~/Customers/GetImportCustomersModal")]
        public ActionResult GetImportCustomersModal()
        {
            return PartialView("ImportCustomers", new ImportCustomerViewModel
            {
                Organizations = GetOrganizations()
            });
        }

        [HttpGet]
        [Route("~/Customers/GetSelectedCompanyServices/{id}")]
        public ActionResult GetSelectedCompanyServices(int id)
        {
            var companyService = _companyServiceService.GetCompanyService(id);
            var list = new List<CompanyServiceListViewModel>
            {
                new CompanyServiceListViewModel
                {
                    CompanyId = companyService.CompanyId,
                    Description = companyService.Description,
                    Id = companyService.CompanyServiceId,
                    Name = companyService.Name
                }
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Customers/VerifyImport")]
        public ActionResult VerifyImport()
        {
            var response = new ImportCustomerListModel();

            try
            {
                if (Request.Files.Count > 0)
                {
                    var httpPostedFileBase = Request.Files[0];
                    if (httpPostedFileBase != null)
                    {
                        response.Data = GetDataFromFile(httpPostedFileBase);

                        var recordCount = response.Data.Count;
                        var validCount = response.Data.Count(x => x.Status == ExportStatus.Valid.ToString());
                        var inValidCount = response.Data.Count(x => x.Status == ExportStatus.Invalid.ToString());

                        if (validCount == recordCount)
                        {
                            response.Message = "All records are valid.";
                            response.Css = "success";
                        }
                        else if (inValidCount == recordCount)
                        {
                            response.Message = "All records are invalid.";
                            response.Css = "danger";
                        }
                        else
                        {
                            response.Message = string.Format("There are {0} valid records and {1} invalid records.",
                                validCount, inValidCount);
                            response.Css = "info";
                        }


                        response.Success = true;
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "Csv file contains no data or is not valid.";
                    response.Css = "danger";
                }
            }
            catch (Exception exception)
            {
                response.Success = false;
                response.Message = "Csv file contains no data or is not valid.";
                response.Css = "danger";
            }
            return Json(response);
        }

        [HttpPost]
        [Route("~/Customers/SendEmail")]
        public ActionResult SendEmail(SendEmailViewModel model)
        {
            try
            {
                string template = EmailConstants.AddUpdateCcInfo;
                var token = Guid.NewGuid();

                var tokenResponse = _customerService.SetExpireTokenForCustomer(new CustomerSetExpireTokenRequest
                {
                    CustomerId = model.CustomerId,
                    Token = token.ToString()
                });

                if (tokenResponse.Success)
                {
                    var request = new SendCustomerEmailRequest
                    {
                        CustomerId = model.CustomerId,
                        FromEmail = ApplicationSettings.FromEmail,
                        SetCreditCardUrl =
                            string.Format("{0}/SetPaymentMethod/{1}", ApplicationSettings.SecureBaseUrl, token),
                        Template = template
                    };

                    var response = _emailService.SendEmail(request);

                    return Json(new
                    {
                        success = response.Success ? "Email Sent" : response.Message,
                        message = response.Message
                    });
                }
                return Json(new
                {
                    success = false,
                    message = "Some error occured while setting expire token. Please contact admin."
                });
            }
            catch (Exception exception)
            {
                return Json(new
                {
                    success = false,
                    message = exception.Message
                });
            }
        }

        [HttpPost]
        [Route("~/Customers/Import")]
        public ActionResult Import(ImportCustomerViewModel model, HttpPostedFileBase uploadCsv)
        {
            if (uploadCsv != null && uploadCsv.ContentLength > 0)
            {
                var data = GetDataFromFile(uploadCsv);
                data = data.Where(x => x.Status == ExportStatus.Valid.ToString()).ToList();

                var importCustomers = Mapper.Map<List<ImportCustomerRequest>>(data);

                var result = _customerService.BatchSave(new CustomerBatchRequest
                {
                    ImportCustomers = importCustomers,
                    CompanyId = model.CompanyId
                });
                
                return Json(new
                {
                    success = result.Success,
                    message = result.Success ? "Records imported successfully." : result.Message
                });
            }
            return Json(new
            {
                success = false,
                message = "Csv file contains not data or not valid."
            });
        }

        #endregion

        #region Private Methods

        private List<ImportCustomerValidation> GetDataFromFile(HttpPostedFileBase httpPostedFileBase)
        {
            var data = new List<ImportCustomerValidation>();

            var datatable = DataTable.New.ReadLazy(httpPostedFileBase.InputStream);
            var emails = _customerService.GetCustomerContactEmails();
            var names = _customerService.GetCustomerNames();

            foreach (var row in datatable.Rows)
            {
                var dataItem = new ImportCustomerValidation();
                bool flag = true;

                if (string.IsNullOrEmpty(row["Email"]))
                {
                    flag = false;
                    dataItem.Reason = @"Email is empty.";
                }
                else
                {
                    if (!ValidateData.IsEmail(row["Email"]))
                    {
                        flag = false;
                        dataItem.Reason = @"Email is not valid.";
                    }
                }
                if (emails.Contains(row["Email"]))
                {
                    flag = false;
                    dataItem.Reason = @"Contact Email already exists in the app.";
                }

                if (string.IsNullOrEmpty(row["CustomerName"]))
                {
                    flag = false;
                    dataItem.Reason = @"Customer name is empty.";
                }

                if (names.Contains(row["CustomerName"]))
                {
                    flag = false;
                    dataItem.Reason = @"Customer name  already exists in the app.";
                }

                if (string.IsNullOrEmpty(row["FirstName"]))
                {
                    flag = false;
                    dataItem.Reason = @"First name is empty.";
                }
                if (string.IsNullOrEmpty(row["LastName"]))
                {
                    flag = false;
                    dataItem.Reason = @"Last name is empty.";
                }
                if (string.IsNullOrEmpty(row["Address"]))
                {
                    flag = false;
                    dataItem.Reason = @"Address is empty.";
                }
                if (string.IsNullOrEmpty(row["City"]))
                {
                    flag = false;
                    dataItem.Reason = @"City is empty.";
                }
                if (string.IsNullOrEmpty(row["State"]))
                {
                    flag = false;
                    dataItem.Reason = @"State is empty.";
                }
                if (string.IsNullOrEmpty(row["PostalCode"]))
                {
                    flag = false;
                    dataItem.Reason = @"Postal code is empty.";
                }
                else
                {
                    if (!ValidateData.IsZipCode(row["PostalCode"]))
                    {
                        flag = false;
                        dataItem.Reason = @"PostalCode is not valid.";
                    }
                }
                if (string.IsNullOrEmpty(row["Country"]))
                {
                    flag = false;
                    dataItem.Reason = @"Country is empty.";
                }

                if (string.IsNullOrEmpty(row["Mobile"]) && string.IsNullOrEmpty(row["Telephone"]))
                {
                    flag = false;
                    dataItem.Reason = @"Mobile/ or Telephone is empty.";
                }
                else
                {
                    if (!string.IsNullOrEmpty(row["Mobile"]) && !ValidateData.IsPhoneNumber(row["Mobile"]))
                    {
                        flag = false;
                        dataItem.Reason = @"Mobile is not valid.";
                    }

                    if (!string.IsNullOrEmpty(row["Telephone"]) && !ValidateData.IsPhoneNumber(row["Telephone"]))
                    {
                        flag = false;
                        dataItem.Reason = @"Telephone is not valid.";
                    }
                }

                dataItem.Status = flag ? ExportStatus.Valid.ToString() : ExportStatus.Invalid.ToString();

                //contact
                dataItem.FirstName = row["FirstName"];
                dataItem.MiddleName = row["MiddleName"];
                dataItem.LastName = row["LastName"];
                dataItem.Telephone = row["Telephone"];
                dataItem.Mobile = row["Mobile"];
                dataItem.Email = row["Email"];

                //Customer
                dataItem.CustomerName = row["CustomerName"];
                dataItem.Address = row["Address"];
                dataItem.City = row["City"];
                dataItem.State = row["State"];
                dataItem.PostalCode = row["PostalCode"];
                dataItem.Country = row["Country"];

                data.Add(dataItem);
            }
            return data;
        }

        private IEnumerable<CompanyServiceListViewModel> GetCompanyServiceList(int id, int customerId)
        {
            var companies = _companyServiceService.GetFilteredCompanyServices(id, customerId);
            var list = companies.Select(x => new CompanyServiceListViewModel
            {
                Id = x.CompanyServiceId,
                Name = x.Name,
                Description = x.Description,
                CompanyId = x.CompanyId
            }).ToList();
            return list;
        }

        private List<CustomerServiceViewModel> GetDataFromSession()
        {
            var data = new List<CustomerServiceViewModel>();

            if (Session["data"] == null)
            {
                Session["data"] = data;
            }
            else
            {
                data = (List<CustomerServiceViewModel>)Session["data"];
            }
            return data;
        }

        private void SetDataToSession(List<CustomerServiceViewModel> data)
        {
            Session["data"] = data;
        }

        private int GetMaxId()
        {
            var data = new List<CustomerServiceViewModel>();

            if (Session["data"] == null)
                Session["data"] = data;
            else
                data = (List<CustomerServiceViewModel>)Session["data"];

            var max = data.Any() ? data.Max(x => x.CustomerServiceId) : 0;
            return (max == 0) ? 1 : max + 1;
        }

        private IEnumerable<ServiceRecordDetail> GetServiceRecordList(int customerId)
        {
            var list = _serviceRecordService.GetServiceRecordsByCustomer(customerId);
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

        private IEnumerable<CustomerListViewModel> GetCustomerList()
        {
            var customers = _customerService.GetAllCustomers(CurrentIdentity.CompanyId);

            var list = (from c in customers
                        let contact = c.ContactPersons.FirstOrDefault()
                        select new CustomerListViewModel
                        {
                            Id = c.CustomerId,
                            CompanyId = c.CompanyId,
                            CompanyName = c.Company != null ? c.Company.Name : "N/A",
                            Email = contact != null ? contact.Email : "N/A",
                            Contact = contact != null ? string.Format("{0} {1}", contact.FirstName, contact.LastName) : "N/A",
                            Name = c.Name,
                            Telephone = contact != null ? contact.Telephone : "N/A",
                            GatewayCustomerId = c.GatewayCustomerId,
                            IsActive = c.IsCcActive,
                        }).ToList();

            return list;
        }

        private IEnumerable<CustomerServiceViewModel> GetCustomerServiceList(int customerId)
        {
            if (Session["data"] != null)
            {
                var list = GetDataFromSession();
                return list.Where(x => !x.InMemoryDeleted).ToList();
            }

            var customerServices = _customerServiceService.GetCustomerServices(customerId);
            var data = (from c in customerServices
                        select new CustomerServiceViewModel
                        {
                            CustomerServiceId = c.CustomerServiceId,
                            CustomerId = c.CustomerId,
                            ServiceName = c.Name,
                            Description = c.Description,
                            Price = c.Cost,
                            CompanyServiceId = c.CompanyServiceId
                        }).ToList();

            //Set data in-memory for manipulation
            SetDataToSession(data);
            return data;
        }

        private IEnumerable<CompanyService> GetCompanyServices(int companyId)
        {
            return _companyServiceService.GetAllCompanyServices(companyId);
        }

        private CompanyService GetCompanyService(int companyServiceId)
        {
            return _companyServiceService.GetCompanyService(companyServiceId);
        }

        private void PopulateViews()
        {
            var list = GetOrganizations();
            ViewData["Organizations"] = list;
        }

        private void PopulateCompanyServices(int companyId)
        {
            var services = GetCompanyServices(companyId);
            var list = new List<SelectListItem> { new SelectListItem { Text = "--Select--", Value = "" } };
            list.AddRange(services.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CompanyServiceId.ToString(CultureInfo.InvariantCulture)
            }));
            ViewData["CompanyServices"] = list;
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

        #endregion
    }
}
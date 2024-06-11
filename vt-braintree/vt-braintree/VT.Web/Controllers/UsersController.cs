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
using VT.Common.Utils;
using VT.Services.DTOs;
using VT.Services.Interfaces;
using VT.Web.Models;
using DataAccess;

namespace VT.Web.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        #region Fields

        private readonly ICompanyWorkerService _companyWorkerService;
        private readonly ICompanyService _companyService;
        private readonly IServiceRecordService _serviceRecordService;
        private readonly IServiceRecordItemService _serviceRecordItemService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Constructor

        public UsersController(ICompanyWorkerService companyWorkerService,
            ICompanyService companyService,
            IServiceRecordService serviceRecordService,
            IServiceRecordItemService serviceRecordItemService,
            ICustomerService customerService)
        {
            _companyWorkerService = companyWorkerService;
            _companyService = companyService;
            _serviceRecordService = serviceRecordService;
            _serviceRecordItemService = serviceRecordItemService;
            _customerService = customerService;
        }

        #endregion

        #region Action Methods

        //display all list of users
        [Route("~/Users")]
        public ActionResult Index()
        {
            PopulateViews();

            bool canSee = false;
            var message = string.Empty;

            if (CurrentIdentity != null)
            {
                canSee = CurrentIdentity.CompanyId == null;

                if (CurrentIdentity.CompanyId != null)
                {
                    var company = _companyService.GetCompany(CurrentIdentity.CompanyId.Value); 
                    if (company != null)
                    {
                        canSee = !string.IsNullOrEmpty(company.GatewayCustomerId) &&
                                 !string.IsNullOrEmpty(company.MerchantAccountId);

                        if (string.IsNullOrEmpty(company.GatewayCustomerId) &&
                            string.IsNullOrEmpty(company.MerchantAccountId))
                        {
                            message = "Your Merchant Account and Credit Card Account is not setup. Please setup these accounts in settings.";
                        }
                        else if (string.IsNullOrEmpty(company.GatewayCustomerId))
                        {
                            message = "Your Credit Card Account is not setup. Please setup these accounts in settings.";
                        }
                        else if (string.IsNullOrEmpty(company.MerchantAccountId))
                        {
                            message = "Your Merchant Account is not setup. Please setup these accounts in settings.";
                        }
                    }
                }
            }

            ViewBag.CanSee = canSee;
            ViewBag.Message = message;

            ViewData["From"] = new List<SelectListItem>();
            ViewData["To"] = new List<SelectListItem>();

            return View();
        }

        //read method of kendo user grid
        [HttpPost]
        [Route("~/Users/UserList")]
        public ActionResult UserList([DataSourceRequest] DataSourceRequest request)
        {
            var data = GetUserList().ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        //delete user(s) method
        [HttpPost]
        [Route("~/Users/DeleteUsers")]
        public ActionResult DeleteUsers(DeleteUsersViewModel model)
        {
            var response = _companyWorkerService.DeleteUsers(model.Ids);
            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }

        [HttpPost]
        [Route("~/Users/GetUserDetail/{id?}")]
        public JsonResult GetUserDetail(int? id)
        {
            var model = id != null ? GetUser(id.Value) : new SaveUserModel { CompanyId = CurrentIdentity.CompanyId };

            if (id != null)
            {
                // list of all customers of this user's company
                var customers = _customerService.GetAllCustomersForUser(id.Value);  // id is user id 

                // list of accessible customers for this user
                var accessList = _customerService.GetUserCustomerAccess(id.Value);

                model.To = accessList.OrderBy(x => x.CustomerOrder).Select(x => x.CustomerId).ToList();

                // from list dropdown
                model.FromList = customers.Where(x => !model.To.Contains(x.CustomerId)).OrderBy(x => x.Name).Select(x => new SelectListItem
                {
                    Value = x.CustomerId.ToString(),
                    Text = x.Name
                }).ToList();

                // to list dropdown
                model.ToList = accessList.OrderBy(x => x.CustomerOrder).Select(x => new SelectListItem
                {
                    Value = x.CustomerId.ToString(),
                    Text = x.Customer.Name,
                    Selected = true
                }).ToList();
            } 

            return Json(model);
        }

        [HttpPost]
        [Route("~/Users/GetUserAllDetails/{id}")]
        public ActionResult GetUserAllDetails(int id)
        {
            var user = _companyWorkerService.GetCompanyWorker(id);
            var model = Mapper.Map<UserDetailViewModel>(user);
            return PartialView("UserDetail", model);
        }

        [HttpPost]
        [Route("~/Users/SaveUser")]
        public ActionResult SaveUser(SaveUserModel model)
        {
            var response = _companyWorkerService.AddUser(new CompanyWorkerSaveRequest
            {
                CompanyId = model.CompanyId,
                CompanyWorkerId = model.CompanyWorkerId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Username,
                IsAdmin = model.IsAdmin,
                MiddleName = model.MiddleName,
                Password = model.AuthKey,
                AccessibleCustomers = model.To
            });

            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }

        [HttpPost]
        [Route("~/Users/GetPasswordResetModal/{id}")]
        public ActionResult GetPasswordResetModal(int id)
        {
            var model = new PasswordResetModel { UserId = id };
            return PartialView("ResetPassword", model);
        }

        [HttpPost]
        [Route("~/Users/ResetPassword")]
        public ActionResult ResetPassword(PasswordResetModel model)
        {
            //dto
            var request = Mapper.Map<ResetPasswordRequest>(model);
            var response = _companyWorkerService.ResetUserPassword(request);
            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }

        [HttpPost]
        [Route("~/Users/ServiceRecords/{id}")]
        public ActionResult ServiceRecords(int id, [DataSourceRequest] DataSourceRequest request)
        {
            var data = GetServiceRecordList(id).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("~/Users/ServiceRecordItems/{id}")]
        public ActionResult ServiceRecordItems(int id, [DataSourceRequest] DataSourceRequest request)
        {
            var data = GetServiceRecordItemList(id).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, Route("~/Users/CheckEmail")]
        public ActionResult CheckEmail(string email)
        {
            var result = _companyWorkerService.IsEmailAlreadyExist(email);
            return Content(result.Success ? "true" : "false");
        }

        [HttpPost]
        [Route("~/Users/VerifyImport")]
        public ActionResult VerifyImport()
        {
            var response = new ExportUserListModel();

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
        [Route("~/Users/Import")]
        public ActionResult Import(ImportUsersModel model, HttpPostedFileBase uploadCsv)
        {
            if (uploadCsv != null && uploadCsv.ContentLength > 0)
            {
                var data = GetDataFromFile(uploadCsv);

                foreach (var dataItem in data.Where(x => x.Status == ExportStatus.Valid.ToString()))
                {
                    _companyWorkerService.AddUser(new CompanyWorkerSaveRequest
                    {
                        CompanyId = model.CompanyId,
                        Email = dataItem.Email,
                        FirstName = dataItem.FirstName,
                        LastName = dataItem.LastName,
                        MiddleName = dataItem.MiddleName,
                        IsAdmin = dataItem.IsAdministrator,
                        Password = dataItem.Password
                    });
                }

                return Json(new
                {
                    success = true,
                    message = "Records imported successfully."
                });
            }
            return Json(new
            {
                success = false,
                message = "Csv file contains not data or not valid."
            });
        }

        [HttpPost]
        [Route("~/Users/UserCustomerAccess")]
        public ActionResult UserCustomerAccess(UserCustomerAccessModel model)
        {
            //if (model.To == null || !model.To.Any())
            //{
            //    return Json(new
            //    {
            //        success = false,
            //        message = "Please assign at least one customer from left multiselect."
            //    });
            //}

            var response = _customerService.UserCustomerAccess(new UserCustomerAccessRequest
            {
                Customers = model.To,
                UserId = model.CompanyWorkerUserId
            });

            return Json(new
            {
                success = response.Success,
                message = response.Message
            });
        }
 
        [Route("~/Users/GetUserCustomerAccess/{id}")]
        public ActionResult GetUserCustomerAccess(int id)
        {
            var model = new UserCustomerAccessDetailModel();

            // list of all customers of this user's company
            var customers = _customerService.GetAllCustomersForUser(id);  // id is user id 
            
            // list of accessible customers for this user
            var accessList = _customerService.GetUserCustomerAccess(id);

            model.To = accessList.OrderBy(x => x.CustomerOrder).Select(x => x.CustomerId).ToList();

            // from list dropdown
            model.FromList = customers.Where(x => !model.To.Contains(x.CustomerId)).OrderBy(x => x.Name).Select(x => new SelectListItem
            {
                Value = x.CustomerId.ToString(),
                Text = x.Name
            }).ToList();

            // to list dropdown
            model.ToList = accessList.OrderBy(x => x.CustomerOrder).Select(x => new SelectListItem
            {
                Value = x.CustomerId.ToString(),
                Text = x.Customer.Name,
                Selected = true
            }).ToList();
             
             

            return Json(model);
        }


        [Route("~/Users/GetCustomers/{id}")]
        public ActionResult GetCustomers(int id) //company id
        { 
            var customers = _customerService.GetAllCustomers(id);  
             
            // from list dropdown
            var model = customers.OrderBy(x => x.Name).Select(x => new SelectListItem
            {
                Value = x.CustomerId.ToString(),
                Text = x.Name
            }).ToList();

            return Json(model);
        }

        #endregion

        #region Private Methods

        private List<ExportUserValidation> GetDataFromFile(HttpPostedFileBase httpPostedFileBase)
        {
            var data = new List<ExportUserValidation>();

            var datatable = DataTable.New.ReadLazy(httpPostedFileBase.InputStream);
            var emails = _companyWorkerService.GetAllActiveUsersEmail();

            foreach (var row in datatable.Rows)
            {
                var dataItem = new ExportUserValidation();
                bool flag = true;

                if (string.IsNullOrEmpty(row["Email"]))
                {
                    flag = false;
                    dataItem.Reason = @"Email is empty.";
                }
                if (emails.Contains(row["Email"]))
                {
                    flag = false;
                    dataItem.Reason = @"Email already exists in the app.";
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
                if (string.IsNullOrEmpty(row["Password"]))
                {
                    flag = false;
                    dataItem.Reason = @"Password name is empty.";
                }

                var isAdmin = false;

                if (!string.IsNullOrEmpty(row["IsAdministrator"]))
                {
                    var value = row["IsAdministrator"].Trim().ToUpper();

                    if (value == "Y" || value == "YES" || value == "TRUE")
                    {
                        isAdmin = true;
                    }
                    else if (value == "N" || value == "NO" || value == "FALSE")
                    {
                        isAdmin = false;
                    }
                    else
                    {
                        flag = false;
                        dataItem.Reason = @"Is Administrator is not valid.";
                    }
                }

                dataItem.Status = flag ? ExportStatus.Valid.ToString() : ExportStatus.Invalid.ToString();
                dataItem.FirstName = row["FirstName"];
                dataItem.MiddleName = row["MiddleName"];
                dataItem.LastName = row["LastName"];
                dataItem.Password = row["Password"];
                dataItem.Email = row["Email"];
                dataItem.IsAdministrator = isAdmin;
                data.Add(dataItem);
            }

            var duplicateEmails = data.GroupBy(x => x.Email).Where(x => x.Count() > 1).Select(x => x.Key).ToList();

            foreach (var dataItem in data)
            {
                if (duplicateEmails.Contains(dataItem.Email))
                {
                    dataItem.Status = ExportStatus.Invalid.ToString();
                    dataItem.Reason = @"Email exists multiple times in the records.";
                }
            }

            return data;
        }

        private SaveUserModel GetUser(int id)
        {
            var data = _companyWorkerService.GetCompanyWorker(id);
            var model = Mapper.Map<SaveUserModel>(data);
            return model;
        }

        private IEnumerable<UsersListViewModel> GetUserList()
        {
            var users = _companyWorkerService.GetAllUsers(CurrentIdentity.CompanyId);

            var list = users.Select(x => new UsersListViewModel
            {
                Id = x.CompanyWorkerId,
                Name = string.Format("{0} {1}", x.FirstName, x.LastName),
                Email = x.Email,
                CompanyId = x.CompanyId,
                CompanyName = x.Company != null ? x.Company.Name : "(N/A :Super Admin)",
                IsAdmin = x.IsAdmin ? "Yes" : "No"
            }).ToList();
            return list;
        }

        private IEnumerable<ServiceRecordDetail> GetServiceRecordList(int id)
        {
            var list = _serviceRecordService.GetServiceRecordsByCompanyWorker(id);
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
                Status = EnumUtil.GetDescription(x.Status)
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
                CustomerId = x.CustomerId,
                EndTime = x.EndTime,
                ServiceRecordId = x.ServiceRecordId,
                StartTime = x.StartTime,
                Description = x.Description,
                ServiceRecordItemId = x.ServiceRecordItemId,
                Type = EnumUtil.GetDescription(x.Type),
            }).ToList();
            return result;
        }

        private void PopulateViews()
        {
            var list = GetOrganizations();
            ViewData["Organizations"] = list;
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
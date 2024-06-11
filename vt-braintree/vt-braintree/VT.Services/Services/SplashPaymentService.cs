using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using VT.Common;
using VT.Common.Utils;
using VT.Data.Context;
using VT.Data.Entities;
using VT.Services.Components;
using VT.Services.DTOs;
using VT.Services.DTOs.SplashPayments;
using VT.Services.Interfaces;

namespace VT.Services.Services
{
    public class SplashPaymentService : ISplashPaymentService
    {
        #region Field

        private readonly IVerifyTechContext _verifyTechContext; 

        #endregion

        #region Constructor

        public SplashPaymentService(IVerifyTechContext verifyTechContext)
        {
            _verifyTechContext = verifyTechContext;
        }

        #endregion

        #region Interface implementation

        //Create Merchant (MerchnatJson)
        public SplashAccountResponse CreateMerchant(SplashCreateMerchantRequest request)
        {
            var response = new SplashAccountResponse();

            var company = _verifyTechContext.Companies.FirstOrDefault(x => x.CompanyId == request.CompanyId);

            if (company == null)
            {
                response.Message = "Company does not exists";
                return response;
            }

            try
            {
                var createMerchantRequest = new SplashCreateMerchant
                {
                    AnnualCCSales = request.AnnualCCSales,
                    Established = request.Established,
                    MerchantCategoryCode = request.MerchantCategoryCode,
                    MerchantNew = "0",
                    Status = "1",
                    TCVersion = "1",
                    Entity = new SplashCreateEntity
                    {
                        LoginId = ApplicationSettings.MerchantLoginId,
                        Address1 = request.EntityAddress1,
                        City = request.EntityCity,
                        Country = request.EntityCountry,
                        Email = request.EntityEmail,
                        EmployerId = "123456789",
                        Name = request.EntityName,
                        Phone = request.EntityPhone,
                        State = request.EntityState,
                        Type = "1",
                        Website = request.EntityWebsite,
                        Zip = request.EntityZip,
                        Accounts = new List<SplashCreateAccount>()
                        {
                            new SplashCreateAccount
                            {
                                Account = new SplashAccount
                                {
                                    CardOrAccountNumber = request.CardOrAccountNumber,
                                    PaymentMethod = request.AccountsPaymentMethod,
                                    RoutingCode = request.AccountsRoutingCode
                                },
                                Primary = "1"
                            }
                        }
                    },
                    Members = new List<SplashCreateMember>()
                    {
                        new SplashCreateMember
                        {
                            Title = request.MemberTitle,
                            DateOfBirth = request.MemberDateOfBirth,
                            DriverLicense = request.MemberDriverLicense,
                            DriverLicenseState = request.MemberDriverLicenseState,
                            Email = request.MemberEmail,
                            FirstName = request.MemberFirstName,
                            LastName = request.MemberLastName,
                            OwnerShip = request.MemberOwnerShip,
                            Primary = "1",
                        }
                    }
                };

                var gateway = new SplashPaymentComponent();
                var json = gateway.CreateMerchant(createMerchantRequest);
                var splashMerchant = JsonConvert.DeserializeObject<SplashGatewayMerchantResponse>(json);

                if (splashMerchant.response.errors.Count != 0)
                {
                    response.Success = false;
                    var error = new StringBuilder();
                    foreach (var splashError in splashMerchant.response.errors)
                    {
                        error.Append(splashError.msg + "<br/>");
                    }
                    response.Message = error.ToString();
                }
                else
                {
                    response.Success = true;
                    response.Message = "Merchant has been saved successfully.";
                    company.MerchantAccountId = splashMerchant.response.data[0].id;
                    company.MerchantJson = json;
                    _verifyTechContext.SaveChanges();

                    // create fees record in the splash payment gateway
                    // this is for commission

                    var percentageFee = company.ServiceFeePercentage;

                    var splashFeeRquest = new CreateFeeRequest
                    {
                        ReferrerId = ApplicationSettings.SplashMerchantEntityId,
                        ForEntityId = company.MerchantAccountId,
                        StartDate = DateTime.UtcNow.ToString("YYYYMMDD"),
                        Amount = percentageFee.ToString(),
                        Currency = "USD",
                        Um = "1", // required 1 for percentage fee and 2 for fixed fee amount
                        Schedule = "7",
                        FeeName = String.Format("{0}% Fees", percentageFee)
                    };
                    var feeResponse = gateway.CreateFee(splashFeeRquest);
                    response.Message = feeResponse.response.errors.Count != 0 ? "Merchant has been successfully created but there was some error occured while creating fees." : "Merchant creation and percentage fee record has been successfully created.";
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse UpdateMerchantInfo(UpdateSplashMerchantRequest request)
        {
            var response = new BaseResponse();

            var company = _verifyTechContext.Companies.FirstOrDefault(x => x.CompanyId == request.CompanyId);

            if (company == null)
            {
                response.Message = "Company does not exists";
                return response;
            }

            if (!string.IsNullOrEmpty(company.MerchantJson))
            {
                try
                {
                    var merchantResponse =
                        JsonConvert.DeserializeObject<SplashGatewayMerchantResponse>(company.MerchantJson);
                    if (merchantResponse != null)
                    {
                        var merchantId = merchantResponse.response.data[0].id;

                        var gateway = new SplashPaymentComponent();

                        var splashMerchantUpdateResponse = gateway.UpdateMerchant(new SplashMerchantUpdate
                        {
                            AnnualCCSales = request.AnnualCCSales,
                            Established = request.Established,
                            MerchantCategoryCode = request.MerchantCategoryCode
                        }, merchantId);

                        if (splashMerchantUpdateResponse.response.errors.Count != 0)
                        {
                            response.Success = false;
                            var error = new StringBuilder();
                            foreach (var splashError in splashMerchantUpdateResponse.response.errors)
                            {
                                error.Append(splashError.msg + "<br/>");
                            }
                            response.Message = error.ToString();
                        }
                        else
                        {
                            merchantResponse.response.data[0].annualCCSales = request.AnnualCCSales;
                            merchantResponse.response.data[0].established = request.Established;
                            merchantResponse.response.data[0].mcc = request.MerchantCategoryCode;

                            company.MerchantJson = JsonConvert.SerializeObject(merchantResponse);
                            _verifyTechContext.SaveChanges();

                            response.Success = true;
                            response.Message = "Merchant has been updated successfully.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.ToString();
                }
            }
            return response;
        }

        //Update company customer
        public BaseResponse UpdateCompanyCustomer(UpdateSplashCustomerRequest request)
        {
            var response = new BaseResponse();

            var company = _verifyTechContext.Companies.FirstOrDefault(x => x.CompanyId == request.CompanyId);

            if (company == null)
            {
                response.Message = "Company does not exists";
                return response;
            }

            if (!string.IsNullOrEmpty(company.CustomerJson))
            {
                try
                {
                    var customerResponse = JsonConvert.DeserializeObject<SplashCreateCustomerResponseObject>(company.CustomerJson);
                    if (customerResponse != null)
                    {
                        var customerId = customerResponse.response.data[0].customer.id;

                        var gateway = new SplashPaymentComponent();

                        var splashCreateCustomerResponse = gateway.UpdateCustomer(new SplashUpdateCustomerRequest
                        {
                            CompanyId = request.CompanyId,
                            CustomerFirstName = request.FirstName,
                            CustomerLastName = request.LastName,
                            CustomerEmail = request.Email
                        }, customerId);

                        if (splashCreateCustomerResponse.response.errors.Count != 0)
                        {
                            response.Success = false;
                            var error = new StringBuilder();
                            foreach (var splashError in splashCreateCustomerResponse.response.errors)
                            {
                                error.Append(splashError.msg + "<br/>");
                            }
                            response.Message = error.ToString();
                        }
                        else
                        {
                            customerResponse.response.data[0].customer.first = request.FirstName;
                            customerResponse.response.data[0].customer.last = request.LastName;
                            customerResponse.response.data[0].customer.email = request.Email;

                            company.CustomerJson = JsonConvert.SerializeObject(customerResponse);
                            _verifyTechContext.SaveChanges();

                            response.Success = true;
                            response.Message = "Customer has been updated successfully.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.ToString();
                }
            }
            return response;
        }

        //Update customer customer
        public BaseResponse UpdateCustomerCustomer(UpdateSplashCustomerRequest request)
        {
            var response = new BaseResponse();

            var customerDb = _verifyTechContext.Customers.FirstOrDefault(x => x.CustomerId == request.CustomerId);

            if (customerDb == null)
            {
                response.Message = "Customer does not exists";
                return response;
            }

            if (!string.IsNullOrEmpty(customerDb.CustomerJson))
            {
                try
                {
                    var customerResponse = JsonConvert.DeserializeObject<SplashCreateCustomerResponseObject>(customerDb.CustomerJson);
                    if (customerResponse != null)
                    {
                        var customerId = customerResponse.response.data[0].customer.id;

                        var gateway = new SplashPaymentComponent();

                        var splashCreateCustomerResponse = gateway.UpdateCustomer(new SplashUpdateCustomerRequest
                        {
                            CompanyId = request.CompanyId,
                            CustomerFirstName = request.FirstName,
                            CustomerLastName = request.LastName,
                            CustomerEmail = request.Email
                        }, customerId);

                        if (splashCreateCustomerResponse.response.errors.Count != 0)
                        {
                            response.Success = false;
                            var error = new StringBuilder();
                            foreach (var splashError in splashCreateCustomerResponse.response.errors)
                            {
                                error.Append(splashError.msg + "<br/>");
                            }
                            response.Message = error.ToString();
                        }
                        else
                        {
                            customerResponse.response.data[0].customer.first = request.FirstName;
                            customerResponse.response.data[0].customer.last = request.LastName;
                            customerResponse.response.data[0].customer.email = request.Email;

                            customerDb.CustomerJson = JsonConvert.SerializeObject(customerResponse);
                            _verifyTechContext.SaveChanges();

                            response.Success = true;
                            response.Message = "Customer has been updated successfully.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.ToString();
                }
            }
            return response;
        }

        //update entity
        public BaseResponse UpdateMerchantEntity(UpdateSplashMerchantEntityRequest request)
        {
            var response = new BaseResponse();

            var company = _verifyTechContext.Companies.FirstOrDefault(x => x.CompanyId == request.CompanyId);

            if (company == null)
            {
                response.Message = "Company does not exists";
                return response;
            }

            if (!string.IsNullOrEmpty(company.MerchantJson))
            {
                try
                {
                    var merchantResponse = JsonConvert.DeserializeObject<SplashGatewayMerchantResponse>(company.MerchantJson);
                    if (merchantResponse != null)
                    {
                        var entityId = merchantResponse.response.data[0].entity.id;

                        var gateway = new SplashPaymentComponent();

                        var splashMerchantEntityUpdateResponse = gateway.UpdateEntity(new SplashUpdateEntity
                        {
                            LoginId = ApplicationSettings.MerchantLoginId,
                            Address1 = request.EntityAddress1,
                            City = request.EntityCity,
                            Country = request.EntityCountry,
                            Email = request.EntityEmail,
                            EmployerId = request.EntityEmployerId,
                            Name = request.EntityName,
                            Phone = request.EntityPhone,
                            State = request.EntityState,
                            Type = "1",
                            Website = request.EntityWebsite,
                            Zip = request.EntityZip
                        }, entityId);

                        if (splashMerchantEntityUpdateResponse.response.errors.Count != 0)
                        {
                            response.Success = false;
                            var error = new StringBuilder();
                            foreach (var splashError in splashMerchantEntityUpdateResponse.response.errors)
                            {
                                error.Append(splashError.msg + "<br/>");
                            }
                            response.Message = error.ToString();
                        }
                        else
                        {
                            merchantResponse.response.data[0].entity.address1 = request.EntityAddress1;
                            merchantResponse.response.data[0].entity.city = request.EntityCity;
                            merchantResponse.response.data[0].entity.country = request.EntityCountry;
                            merchantResponse.response.data[0].entity.email = request.EntityEmail;
                            merchantResponse.response.data[0].entity.name = request.EntityName;
                            merchantResponse.response.data[0].entity.phone = request.EntityPhone;
                            merchantResponse.response.data[0].entity.state = request.EntityState;
                            merchantResponse.response.data[0].entity.website = request.EntityWebsite;
                            merchantResponse.response.data[0].entity.zip = request.EntityZip;


                            company.MerchantJson = JsonConvert.SerializeObject(merchantResponse);
                            _verifyTechContext.SaveChanges();

                            response.Success = true;
                            response.Message = "Entity has been updated successfully.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.ToString();
                }
            }
            return response;
        }

        //Update Member
        public BaseResponse UpdateMerchantMember(UpdateSplashMerchantMemberRequest request)
        {
            var response = new BaseResponse();

            var company = _verifyTechContext.Companies.FirstOrDefault(x => x.CompanyId == request.CompanyId);

            if (company == null)
            {
                response.Message = "Company does not exists";
                return response;
            }

            if (!string.IsNullOrEmpty(company.MerchantJson))
            {
                try
                {
                    var merchantResponse =
                        JsonConvert.DeserializeObject<SplashGatewayMerchantResponse>(company.MerchantJson);
                    if (merchantResponse != null)
                    {
                        var merchantId = merchantResponse.response.data[0].id;
                        var memberId = merchantResponse.response.data[0].members[0].id;

                        var gateway = new SplashPaymentComponent();

                        var splashMerchantMemberUpdateResponse = gateway.UpdateMember(new SplashUpdateMember
                        {
                            Title = request.MemberTitle,
                            DateOfBirth = request.MemberDateOfBirth,
                            Dl = request.MemberDriverLicense,
                            DlState = request.MemberDriverLicenseState,
                            FirstName = request.MemberFirstName,
                            LastName = request.MemberLastName,
                            Email = request.MemberEmail,
                            Ownership = request.MemberOwnerShip,
                            Ssn = request.MemberSocialSecurityNumber,
                            Primary = "1",
                            MerchantId = merchantId
                        }, memberId);

                        if (splashMerchantMemberUpdateResponse.response.errors.Count != 0)
                        {
                            response.Success = false;
                            var error = new StringBuilder();
                            foreach (var splashError in splashMerchantMemberUpdateResponse.response.errors)
                            {
                                error.Append(splashError.msg + "<br/>");
                            }
                            response.Message = error.ToString();
                        }
                        else
                        {
                            merchantResponse.response.data[0].members[0].title = request.MemberTitle;
                            merchantResponse.response.data[0].members[0].first = request.MemberFirstName;
                            merchantResponse.response.data[0].members[0].last = request.MemberLastName;
                            merchantResponse.response.data[0].members[0].dob = request.MemberDateOfBirth;
                            merchantResponse.response.data[0].members[0].dl = request.MemberDriverLicense;
                            merchantResponse.response.data[0].members[0].dlstate = request.MemberDriverLicenseState;
                            merchantResponse.response.data[0].members[0].ownership = request.MemberOwnerShip;
                            merchantResponse.response.data[0].members[0].ssn = request.MemberSocialSecurityNumber;
                            merchantResponse.response.data[0].members[0].email = request.MemberEmail;


                            company.MerchantJson = JsonConvert.SerializeObject(merchantResponse);
                            _verifyTechContext.SaveChanges();

                            response.Success = true;
                            response.Message = "Member has been updated successfully.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.ToString();
                }
            }
            return response;
        }

        //Update merchant account
        public BaseResponse UpdateMerchantAccount(UpdateSplashMerchantAccountRequest request)
        {
            var response = new BaseResponse();

            var company = _verifyTechContext.Companies.FirstOrDefault(x => x.CompanyId == request.CompanyId);

            if (company == null)
            {
                response.Message = "Company does not exists";
                return response;
            }

            if (!string.IsNullOrEmpty(company.MerchantJson))
            {
                try
                {
                    var merchantResponse = JsonConvert.DeserializeObject<SplashGatewayMerchantResponse>(company.MerchantJson);
                    if (merchantResponse != null)
                    {
                        var accountId = merchantResponse.response.data[0].entity.accounts[0].id;

                        var gateway = new SplashPaymentComponent();

                        var splashMerchantAccountUpdateResponse = gateway.UpdateMerchantAccount(new SplashUpdateAccount
                        {
                            AccountCardOrAccountNumber = request.CardOrAccountNumber,
                            AccountRoutingCode = request.AccountsRoutingCode,
                            AccountPaymentMethod = request.AccountsPaymentMethod,
                            Primary = "1"
                        }, accountId);

                        if (splashMerchantAccountUpdateResponse.response.errors.Count != 0)
                        {
                            response.Success = false;
                            var error = new StringBuilder();
                            foreach (var splashError in splashMerchantAccountUpdateResponse.response.errors)
                            {
                                error.Append(splashError.msg + "<br/>");
                            }
                            response.Message = error.ToString();
                        }
                        else
                        {
                            merchantResponse.response.data[0].entity.accounts[0].account.number =
                                request.CardOrAccountNumber;
                            merchantResponse.response.data[0].entity.accounts[0].account.method =
                                request.AccountsPaymentMethod;
                            merchantResponse.response.data[0].entity.accounts[0].account.routing =
                                request.AccountsRoutingCode;


                            company.MerchantJson = JsonConvert.SerializeObject(merchantResponse);
                            _verifyTechContext.SaveChanges();

                            response.Success = true;
                            response.Message = "Account has been updated successfully.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.ToString();
                }
            }
            return response;
        }

        //Get merchant detail
        public SplashGetMerchantResponse GetMerchantDetail(int companyId)
        {
            var response = new SplashGetMerchantResponse();

            //check company
            var company = _verifyTechContext.Companies.FirstOrDefault(x => x.CompanyId == companyId);

            if (company == null)
            {
                response.Message = "Company does not exists in the system.";
                return response;
            }

            if (!string.IsNullOrEmpty(company.MerchantJson))
            {
                try
                {
                    var merchantResponse =
                        JsonConvert.DeserializeObject<SplashGatewayMerchantResponse>(company.MerchantJson);
                    if (merchantResponse != null)
                    {
                        //Merchant
                        response.AnnualCCSales = merchantResponse.response.data[0].annualCCSales.ToString();
                        response.MerchantCategoryCode = merchantResponse.response.data[0].mcc;
                        response.Established = merchantResponse.response.data[0].established.ToString();

                        //Member
                        response.MemberTitle = merchantResponse.response.data[0].members[0].title;
                        response.MemberDateOfBirth = merchantResponse.response.data[0].members[0].dob;
                        response.MemberDriverLicense = merchantResponse.response.data[0].members[0].dl;
                        response.MemberDriverLicenseState = merchantResponse.response.data[0].members[0].dlstate;
                        response.MemberEmail = merchantResponse.response.data[0].members[0].email;
                        response.MemberFirstName = merchantResponse.response.data[0].members[0].first;
                        response.MemberLastName = merchantResponse.response.data[0].members[0].last;
                        response.MemberOwnerShip = merchantResponse.response.data[0].members[0].ownership.ToString();
                        response.MemberSocialSecurityNumber = merchantResponse.response.data[0].members[0].ssn;

                        //Entity
                        response.EntityName = merchantResponse.response.data[0].entity.name;
                        response.EntityAddress1 = merchantResponse.response.data[0].entity.address1;
                        response.EntityCity = merchantResponse.response.data[0].entity.city;
                        response.EntityState = merchantResponse.response.data[0].entity.state;
                        response.EntityCountry = merchantResponse.response.data[0].entity.country;
                        response.EntityEmail = merchantResponse.response.data[0].entity.email;
                        response.EntityPhone = merchantResponse.response.data[0].entity.phone;
                        response.EntityWebsite = merchantResponse.response.data[0].entity.website;
                        response.EntityZip = merchantResponse.response.data[0].entity.zip;

                        //Account
                        response.AccountsPaymentMethod =
                            merchantResponse.response.data[0].entity.accounts[0].account.method;
                        response.CardOrAccountNumber =
                            merchantResponse.response.data[0].entity.accounts[0].account.number;
                        response.AccountsRoutingCode =
                            merchantResponse.response.data[0].entity.accounts[0].account.routing;

                        response.Success = true;
                    }
                }
                catch (Exception ex)
                {
                    response.Message = string.Format("Company [merchant json] is not valid. Extra error info: {0}",
                        ex.ToString());
                    return response;
                }
            }

            return response;
        }

        public SplashCustomerDetailResponse GetCompanyCcDetail(int companyId)
        {
            var response = new SplashCustomerDetailResponse();

            //check company
            var company = _verifyTechContext.Companies.FirstOrDefault(x => x.CompanyId == companyId);

            if (company == null)
            {
                response.Message = "Company does not exists in the system.";
                return response;
            }

            if (!string.IsNullOrEmpty(company.CustomerJson))
            {
                try
                {
                    var customerResponse =
                        JsonConvert.DeserializeObject<SplashCreateCustomerResponseObject>(company.CustomerJson);
                    if (customerResponse != null)
                    {
                        response.CustomerFirstName = customerResponse.response.data[0].customer.first;
                        response.CustomerLastName = customerResponse.response.data[0].customer.last;
                        response.CustomerEmail = customerResponse.response.data[0].customer.email;

                        var number = customerResponse.response.data[0].payment.number;

                        number = number.Length > 4 ? number.Substring(number.Length - 4) : number;

                        response.PaymentNumber = "XXXX" + number;
                        response.PaymentCvv = ".....";
                        response.PaymentExpiration = customerResponse.response.data[0].expiration;
                        response.PaymentInactive = customerResponse.response.data[0].inactive.ToString();

                        response.Success = true;
                    }
                }
                catch (Exception ex)
                {
                    response.Message = string.Format("Company [customer json] is not valid. Extra error info: {0}",
                        ex.ToString());
                    return response;
                }
            }
            return response;
        }

        public SplashCustomerDetailResponse GetCustomerCcDetail(int customerId)
        {
            var response = new SplashCustomerDetailResponse();

            //check customer
            var customerDb = _verifyTechContext.Customers.Include(x => x.ContactPersons).FirstOrDefault(x => x.CustomerId == customerId);

            if (customerDb == null)
            {
                response.Message = "customer does not exists in the system.";
                return response;
            }

            if (!string.IsNullOrEmpty(customerDb.CustomerJson))
            {
                try
                {
                    var customerResponse =
                        JsonConvert.DeserializeObject<SplashCreateCustomerResponseObject>(customerDb.CustomerJson);
                    if (customerResponse != null)
                    {
                        response.CustomerFirstName = customerResponse.response.data[0].customer.first;
                        response.CustomerLastName = customerResponse.response.data[0].customer.last;
                        response.CustomerEmail = customerResponse.response.data[0].customer.email;

                        var number = customerResponse.response.data[0].payment.number;

                        number = number.Length > 4 ? number.Substring(number.Length - 4) : number;

                        response.PaymentNumber = "XXXX" + number;
                        response.PaymentCvv = ".....";
                        response.PaymentExpiration = customerResponse.response.data[0].expiration;
                        response.PaymentInactive = customerResponse.response.data[0].inactive.ToString();
                    }
                }
                catch (Exception ex)
                {
                    response.Message = string.Format("Company [customer json] is not valid. Extra error info: {0}",
                        ex.ToString());
                    return response;
                }
            }
            else
            {
                if (customerDb.ContactPersons.Any())
                {
                    var contactPerson = customerDb.ContactPersons.FirstOrDefault();
                    if (contactPerson != null)
                    {
                        response.CustomerFirstName = contactPerson.FirstName;
                        response.CustomerLastName = contactPerson.LastName;
                        response.CustomerEmail = contactPerson.Email;
                    }
                } 
            }
            response.Success = true;
            return response;
        }

        //Get merchant
        public SplashGatewayMerchantResponse GetMerchant(int companyId)
        {
            var response = new SplashGatewayMerchantResponse();

            var company = _verifyTechContext.Companies.FirstOrDefault(x => x.CompanyId == companyId);

            if (company == null)
            {
                response.Message = "Company does not exists";
                return response;
            }
            else
            {
                response.Success = true;
                response.Message = "Merchant successfully fetched.";
            }
            return response;
        }

        //Get account
        public SplashGetMerchantResponse GetMember(int companyId)
        {
            var response = new SplashGetMerchantResponse();

            //check company
            var company = _verifyTechContext.Companies.FirstOrDefault(x => x.CompanyId == companyId);

            if (company == null)
            {
                response.Message = "Company does not exists";
                return response;
            }

            var merchantInfo = JsonConvert.DeserializeObject<MerchantInfo>(company.MerchantAccountId);

            if (merchantInfo == null)
            {
                response.Message = "Merchant Account information is not available";
                return response;
            }

            response.Success = true;

            var response1 = new SplashAccountResponse();

            var client = new RestClient
            {
                EndPoint = @"https://test-api.splashpayments.com/accounts/" + merchantInfo.AccountId,
                Method = HttpVerb.GET
            };

            var json = client.MakeRequest();

            return response;
        }

        //Get account
        public SplashGetMerchantResponse GetAccount(int companyId)
        {
            var response = new SplashGetMerchantResponse();

            //check company
            var company = _verifyTechContext.Companies.FirstOrDefault(x => x.CompanyId == companyId);

            if (company == null)
            {
                response.Message = "Company does not exists";
                return response;
            }

            var merchantInfo = JsonConvert.DeserializeObject<MerchantInfo>(company.MerchantAccountId);

            if (merchantInfo == null)
            {
                response.Message = "Merchant Account information is not available";
                return response;
            }

            response.Success = true;

            var response1 = new SplashAccountResponse();

            var client = new RestClient
            {
                EndPoint = @"https://test-api.splashpayments.com/accounts/" + merchantInfo.AccountId,
                Method = HttpVerb.GET
            };

            var json = client.MakeRequest();

            return response;
        }

        //Get entity
        public SplashGetMerchantResponse GetEntity(int companyId)
        {
            var response = new SplashGetMerchantResponse();
            var entity = _verifyTechContext.Companies.FirstOrDefault(x => x.CompanyId == companyId);
            return response;
        }

        public SplashTransactionResult Transaction(SplashTransactionRequest request)
        {
            var response = new SplashTransactionResult(); 
            var amount = (request.Amount*100).ToString(); //usd amount converted to cents

            var customerResponse =
                        JsonConvert.DeserializeObject<SplashCreateCustomerResponseObject>(request.CustomerJson);

            if (customerResponse == null)
            {
                response.Message = "Customer Json is not valid.";
                return response;
            }

            var token = customerResponse.response.data[0].token;

            var gateway = new SplashPaymentComponent();
            var transactionResponse = gateway.MakeTransaction(new SplashTransaction
            {
                MerchantId = request.MerchantId,
                Origin = "2",
                Token = token,
                Type = "1",
                Total = amount 
            });

            if (transactionResponse.response.errors.Count != 0)
            {
                response.Success = false;
                var error = new StringBuilder();
                foreach (var splashError in transactionResponse.response.errors)
                {
                    error.Append(splashError.msg + "<br/>");
                }
                response.Message = error.ToString();
            }
            else
            {
                response.Success = true;
                response.TransactionId = transactionResponse.response.data[0].id;
                response.Message = "Transaction Succeeded.";
            }

            return response;
        }
          
        //Disable comapny customer credit card 
        public BaseResponse DisableCompanyCreditCard(int companyId)
        {
            var response = new BaseResponse();

            var company = _verifyTechContext.Companies.FirstOrDefault(x => x.CompanyId == companyId);

            if (company == null)
            {
                response.Message = "Company does not exists";
                return response;
            }

            var customerJsonObj = JsonConvert.DeserializeObject<SplashCreateCustomerResponseObject>(company.CustomerJson);

            // it's token id and not the actual token
            var id = customerJsonObj.response.data[0].id;

            var gateway = new SplashPaymentComponent();

            var splashPaymentDetailResponse = gateway.DisableCustomerCreditCard(new DisableCreditCard
            {
                Inactive = "1",
            }, id);

            if (splashPaymentDetailResponse.response.errors.Count != 0)
            {
                response.Success = false;
                var error = new StringBuilder();
                foreach (var splashError in splashPaymentDetailResponse.response.errors)
                {
                    error.Append(splashError.msg + "<br/>");
                }
                response.Message = error.ToString();
            }
            else
            {
                response.Success = true;
                response.Message = "Payment information has been successfully deleted.";
                customerJsonObj.response.data[0].inactive = 1;
                company.CustomerJson = JsonConvert.SerializeObject(customerJsonObj);

                _verifyTechContext.SaveChanges();
            }
            return response;
        }

        //Disable customer customer credit card
        public BaseResponse DisableCustomerCreditCard(int customerId)
        {
            var response = new BaseResponse();

            var customerCc = _verifyTechContext.Customers.FirstOrDefault(x => x.CustomerId == customerId);

            if (customerCc == null)
            {
                response.Message = "Customer does not exists";
                return response;
            }

            var customerJsonObj = JsonConvert.DeserializeObject<SplashCreateCustomerResponseObject>(customerCc.CustomerJson);

            // it's token id and not the actual token
            var id = customerJsonObj.response.data[0].id;

            var gateway = new SplashPaymentComponent();

            var splashPaymentDetailResponse = gateway.DisableCustomerCreditCard(new DisableCreditCard
            {
                Inactive = "1",
            }, id);

            if (splashPaymentDetailResponse.response.errors.Count != 0)
            {
                response.Success = false;
                var error = new StringBuilder();
                foreach (var splashError in splashPaymentDetailResponse.response.errors)
                {
                    error.Append(splashError.msg + "<br/>");
                }
                response.Message = error.ToString();
            }
            else
            {
                response.Success = true;
                response.Message = "Payment information has been successfully deleted.";
                customerJsonObj.response.data[0].inactive = 1;
                customerCc.CustomerJson = JsonConvert.SerializeObject(customerJsonObj);

                _verifyTechContext.SaveChanges();
            }
            return response;
        }


        //Add company credit card
        public SplashAccountResponse AddCompanyCreditCard(AddCustomerCreditCardRequest request)
        {
            var response = new SplashAccountResponse();

            var company = _verifyTechContext.Companies.FirstOrDefault(x => x.CompanyId == request.CompanyId);

            if (company == null)
            {
                response.Message = "Company does not exists";
                return response;
            }

            if (!string.IsNullOrEmpty(company.CustomerJson))
            {
                try
                {
                    var customerResponse = JsonConvert.DeserializeObject<SplashCreateCustomerResponseObject>(company.CustomerJson);

                    if (customerResponse != null)
                    {
                        var customerId = customerResponse.response.data[0].customer.id;

                        var gateway = new SplashPaymentComponent();

                        var splashAddPaymentDetailResponse = gateway.AddPaymentCreditCard(new AddCustomerCreditCard
                        {
                            Customer = customerId,
                            Inactive = "0",
                            Payment = new CustomerPaymentRequest
                            {
                                Method = request.PaymentMethod,
                                Number = request.PaymentNumber,
                                Expiration = request.PaymentExpiration,
                                Cvv = request.PaymentCvv
                            }
                        });

                        if (splashAddPaymentDetailResponse.response.errors.Count != 0)
                        {
                            response.Success = false;
                            var error = new StringBuilder();
                            foreach (var splashError in splashAddPaymentDetailResponse.response.errors)
                            {
                                error.Append(splashError.msg + "<br/>");
                            }
                            response.Message = error.ToString();
                        }
                        else
                        {
                            customerResponse.response.data[0].payment.number = request.PaymentNumber;
                            customerResponse.response.data[0].payment.method = Convert.ToInt32(request.PaymentMethod);
                            customerResponse.response.data[0].expiration = request.PaymentExpiration;
                            customerResponse.response.data[0].inactive = 0;
                            company.CustomerJson = JsonConvert.SerializeObject(customerResponse);

                            _verifyTechContext.SaveChanges();

                            response.Success = true;
                            response.Message = "Company card has been successfully added.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.ToString();
                }
            }
            return response;
        }

        //Add customer credit card
        public SplashAccountResponse AddCustomerCreditCard(AddCustomerCreditCardRequest request)
        {
            var response = new SplashAccountResponse();

            var customer = _verifyTechContext.Customers.FirstOrDefault(x => x.CustomerId == request.CustomerId);

            if (customer == null)
            {
                response.Message = "Customer does not exists";
                return response;
            }

            if (!string.IsNullOrEmpty(customer.CustomerJson))
            {
                try
                {
                    var customerResponse = JsonConvert.DeserializeObject<SplashCreateCustomerResponseObject>(customer.CustomerJson);

                    if (customerResponse != null)
                    {
                        var customerId = customerResponse.response.data[0].customer.id;

                        var gateway = new SplashPaymentComponent();

                        var splashAddPaymentDetailResponse = gateway.AddPaymentCreditCard(new AddCustomerCreditCard
                        {
                            Customer = customerId,
                            Inactive = "0",
                            Payment = new CustomerPaymentRequest
                            {
                                Method = request.PaymentMethod,
                                Number = request.PaymentNumber,
                                Expiration = request.PaymentExpiration,
                                Cvv = request.PaymentCvv
                            }
                        });

                        if (splashAddPaymentDetailResponse.response.errors.Count != 0)
                        {
                            response.Success = false;
                            var error = new StringBuilder();
                            foreach (var splashError in splashAddPaymentDetailResponse.response.errors)
                            {
                                error.Append(splashError.msg + "<br/>");
                            }
                            response.Message = error.ToString();
                        }
                        else
                        {
                            customerResponse.response.data[0].payment.number = request.PaymentNumber;
                            customerResponse.response.data[0].payment.method = Convert.ToInt32(request.PaymentMethod);
                            customerResponse.response.data[0].expiration = request.PaymentExpiration;
                            customerResponse.response.data[0].inactive = 0;
                            customer.IsCcActive = true;
                            customer.CustomerJson = JsonConvert.SerializeObject(customerResponse);

                            _verifyTechContext.SaveChanges();

                            response.Success = true;
                            response.Message = "Customer card has been successfully added.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.ToString();
                }
            }
            return response;
        }

        //Update entity
        public SplashAccountResponse UpdateEntity(SplashCreateEntityRequest request, string entityId)
        {
            var response = new SplashAccountResponse();

            try
            {
                var entity = new SplashCreateEntity
                {
                    LoginId = request.LoginId,
                    Address1 = request.Address1,
                    City = request.City,
                    Country = request.Country,
                    Email = request.Email,
                    EmployerId = request.EmployerId,
                    Name = request.Name,
                    Phone = request.Phone,
                    State = request.State,
                    Type = request.Type,
                    Website = request.Website,
                    Zip = request.Zip
                };
                response.Message = "Entity has been updated successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
            }
            return response;
        }

        //Create fee
        public SplashAccountResponse CreateFee(CreateFeeRequest request)
        {
            var response = new SplashAccountResponse();

            try
            {
                var fee = new CreateFee
                {
                    ReferrerId = request.ReferrerId,
                    ForEntityId = request.ForEntityId,
                    StartDate = request.StartDate,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    Um = request.Um,
                    Schedule = request.Schedule,
                    FeeName = request.FeeName
                };
                response.Message = "Fee has been created successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
            }
            return response;
        }

        //Delete fee
        public SplashAccountResponse DeleteFee(string feeId)
        {
            var response = new SplashAccountResponse();

            var client = new RestClient();
            client.EndPoint = @"https://test-api.splashpayments.com/fees/" + feeId;
            client.Method = HttpVerb.DELETE;

            var json = client.MakeRequest();
            return response;
        }

        //Create plan
        public SplashAccountResponse CreatePlan(SplashCreatePlanForMerchantRequest request)
        {
            var response = new SplashAccountResponse();
            try
            {
                var plan = new SplashCreatePlanForMerchant
                {
                    MerchantId = request.MerchantId,
                    Amount = request.Amount,
                    Description = request.Description,
                    Name = request.Name,
                    Schedule = request.Schedule
                };
                response.Message = "Plan has been created successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
            }
            return response;
        }

        //Update merchant
        public SplashAccountResponse UpdateMerchant(SplashCreateMerchantRequest request)
        {
            var response = new SplashAccountResponse();
            try
            {
                // Update merchant
                var requestUpdate = new SplashCreateMerchant
                {
                    Established = request.Established,
                    MerchantNew = request.MerchantNew,
                    AnnualCCSales = request.AnnualCCSales,
                    TCVersion = request.TCVersion,
                    Status = request.Status,
                    Entity = new SplashCreateEntity
                    {
                        LoginId = request.EntityLoginId
                    },
                };
                response.Success = true;
                response.Message = "Merchant has been updated successfully.";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
            }
            return response;
        }

        //Create customer
        public SplashAccountResponse CreateCcCustomerForCompany(SplashCustomerCreateRequest request)
        {
            var response = new SplashAccountResponse();

            var createCustomer = _verifyTechContext.Companies.FirstOrDefault(x => x.CompanyId == request.CompanyId);

            if (createCustomer == null)
            {
                response.Message = "This customer does not exist in the database.";
                return response;
            }
            try
            {
                var requestCreateCustomer = new CreateCustomerWithPaymentToken
                {
                    Customer = new SplashCreateCustomer
                    {
                        LoginId = ApplicationSettings.MerchantLoginId,
                        FirstName = request.CustomerFirstName,
                        LastName = request.CustomerLastName,
                        Email = request.CustomerEmail,
                        Inactive = "0"
                    },
                    Payment = new CustomerPaymentRequest
                    {
                        Method = request.PaymentMethod,
                        Number = request.PaymentNumber,
                        Expiration = request.PaymentExpiration,
                        Cvv = request.PaymentCvv
                    }
                };

                var gateway = new SplashPaymentComponent();
                var splashGatewayCustomerResponse = gateway.CreateCustomer(requestCreateCustomer);

                if (splashGatewayCustomerResponse.response.errors.Count != 0)
                {
                    response.Success = false;
                    var error = new StringBuilder();
                    foreach (var splashError in splashGatewayCustomerResponse.response.errors)
                    {
                        error.Append(splashError.msg + "<br/>");
                    }
                    response.Message = error.ToString();
                }
                else
                {
                    createCustomer.GatewayCustomerId = splashGatewayCustomerResponse.response.data[0].customer.id;
                    createCustomer.CustomerJson = JsonConvert.SerializeObject(splashGatewayCustomerResponse);


                    _verifyTechContext.SaveChanges();
                    response.Success = true;
                    response.Message = "Customer has been successfully saved.";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.ToString();
            }
            return response;
        }

        public SplashAccountResponse CreateCcCustomerForCustomer(SplashCustomerCreateRequest request)
        {
            var response = new SplashAccountResponse();

            var customerDb = _verifyTechContext.Customers.FirstOrDefault(x => x.CustomerId == request.CustomerId);

            if (customerDb == null)
            {
                response.Message = "This customer does not exist in the database.";
                return response;
            }
            try
            {
                var requestCreateCustomer = new CreateCustomerWithPaymentToken
                {
                    Customer = new SplashCreateCustomer
                    {
                        LoginId = ApplicationSettings.MerchantLoginId,
                        FirstName = request.CustomerFirstName,
                        LastName = request.CustomerLastName,
                        Email = request.CustomerEmail,
                        Inactive = "0"
                    },
                    Payment = new CustomerPaymentRequest
                    {
                        Method = request.PaymentMethod,
                        Number = request.PaymentNumber,
                        Expiration = request.PaymentExpiration,
                        Cvv = request.PaymentCvv
                    }
                };

                var gateway = new SplashPaymentComponent();
                var splashGatewayCustomerResponse = gateway.CreateCustomer(requestCreateCustomer);

                if (splashGatewayCustomerResponse.response.errors.Count != 0)
                {
                    response.Success = false;
                    var error = new StringBuilder();
                    foreach (var splashError in splashGatewayCustomerResponse.response.errors)
                    {
                        error.Append(splashError.msg + "<br/>");
                    }
                    response.Message = error.ToString();
                }
                else
                {
                    customerDb.GatewayCustomerId = splashGatewayCustomerResponse.response.data[0].customer.id;
                    customerDb.CustomerJson = JsonConvert.SerializeObject(splashGatewayCustomerResponse);
                    customerDb.IsCcActive = true;
                    _verifyTechContext.SaveChanges();
                    response.Success = true;
                    response.Message = "Customer has been successfully saved.";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.ToString();
            }
            return response;
        }

        //Get customer with customer id
        public SplashAccountResponse GetCustomer(string customerId)
        {
            var response = new SplashAccountResponse();

            var client = new RestClient();
            client.EndPoint = @"https://test-api.splashpayments.com/customers/" + customerId;
            client.Method = HttpVerb.GET;

            var json = client.MakeRequest();

            var customer = JsonConvert.DeserializeObject<GetSplashCustomerResponse>(json);

            if (customer.response.errors.Count != 0)
            {
                response.Success = false;
                response.Message = "Some error occured while getting customer details.";
            }
            else
            {
                response.Success = true;
                response.Message = "Query has been executed successfully.";
            }
            return response;
        }

        #endregion
    }
}
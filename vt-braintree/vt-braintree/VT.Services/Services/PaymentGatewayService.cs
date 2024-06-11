using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Braintree;
using Newtonsoft.Json;
using VT.Common;
using VT.Data;
using VT.Data.Context;
using VT.Data.Entities;
using VT.Services.DTOs;
using VT.Services.DTOs.SplashPayments;
using VT.Services.Interfaces;
using Customer = VT.Data.Entities.Customer;

namespace VT.Services.Services
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        #region Field(s)

        private readonly IVerifyTechContext _context;
        private readonly IEmailService _emailService;
        private readonly IPaymentService _paymentService;
        private readonly ISplashPaymentService _splashPaymentService;

        #endregion

        #region Constructor

        public PaymentGatewayService(IVerifyTechContext context,
            IEmailService emailService,
            IPaymentService paymentService, ISplashPaymentService splashPaymentService)
        {
            _context = context;
            _emailService = emailService;
            _paymentService = paymentService;
            _splashPaymentService = splashPaymentService;
        }

        #endregion

        #region Service Methods

        public GatewayCustomerResponse SaveMerchantCc(GatewayCustomerRequest request)
        {
            var response = new GatewayCustomerResponse();
            var company = _context.Companies.FirstOrDefault(x => x.CompanyId == request.CustomerId); //here customer id is companyId

            if (company == null)
            {
                response.Message = "This organization does not exist in the database.";
                return response;
            }
            else
            {
                request.GatewayCustomerId = company.GatewayCustomerId;
            }

            // Save or Update

            response = string.IsNullOrEmpty(company.GatewayCustomerId) ?
                _paymentService.CreateMerchant(request) : _paymentService.UpdateMerchant(request);

            if (response.Success)
            {
                company.GatewayCustomerId = response.ReferenceNumber;
                _context.SaveChanges();
            }
            return response;
        }

        public GatewayAccountDetailResponse GetMerchantAccountDetail(MerchantAccountDetailRequest request)
        {
            var response = new GatewayAccountDetailResponse();

            var company = _context.Companies.FirstOrDefault(x => x.CompanyId == request.CompanyId);

            if (company == null)
            {
                response.Message = "Company does not exist in the system.";
                return response;
            }

            if (string.IsNullOrEmpty(company.MerchantAccountId))
            {
                response.CompanyId = company.CompanyId;
                return response;
            }

            response = _paymentService.GetMerchant(company.MerchantAccountId);
            response.CompanyId = company.CompanyId;

            return response;
        }

        public GatewayCustomerResponse SaveCustomerCc(GatewayCustomerRequest request)
        {
            var response = new GatewayCustomerResponse();

            var customer = _context.Customers.FirstOrDefault(x => x.CustomerId == request.CustomerId);

            if (customer == null)
            {
                response.Message = "This customer does not exist in the database.";
                return response;
            }
            else
            {
                request.GatewayCustomerId = customer.GatewayCustomerId;
            }
            // Save or Update
            response = string.IsNullOrEmpty(customer.GatewayCustomerId) ? _paymentService.CreateCustomer(request) : _paymentService.UpdateCustomer(request);

            return response;
        }

        // TODO: Let's change the name of this method to SaveGatwayMerchantAccount
        public OrganizationAccountResponse CreateOrganizationMerchantAccount(OrganizationAccountRequest request)
        {
            var response = new OrganizationAccountResponse();
            var org = _context.Companies.FirstOrDefault(x => x.CompanyId == request.CompanyId);

            request.MerchantAccountId = org.MerchantAccountId;

            if (org == null)
            {
                response.Message = "This organization does not exist in the database.";
                return response;
            }

            response = string.IsNullOrEmpty(org.MerchantAccountId) ?
                _paymentService.CreateMerchantAccount(request) :
                _paymentService.CreateMerchantAccount(request);

            if (response.Success)
            {
                org.MerchantAccountId = response.ReferenceNumber;
                _context.SaveChanges();
            }
            else
            {
                response.Message = response.Message;
            }
            return response;
        }

        public ProcessPaymentResponse ProcessPayment(ProcessPaymentRequest request)
        {
            var response = new ProcessPaymentResponse();
            var serviceRecord = request.ServiceRecord;

            if (serviceRecord.Company.ServiceFeePercentage <= 0)
            {
                response.Message = "Service record has been successfully created but Company service fee Amount is not Valid";
                return response;
            }

            //prepare email request
            var emailRequest = new SendEmailRequest
            {
                FromEmail = ApplicationSettings.FromEmail,
                ServiceRecord = serviceRecord,
                Template = EmailConstants.GeneralEmailTemplate,
                PdfTemplate = EmailConstants.GeneralPdfTemplate,
            };

            var merchantAccountId = serviceRecord.Company.MerchantAccountId;
            var gatewayCustomerId = serviceRecord.Customer.GatewayCustomerId;
            var amount = Convert.ToDecimal(serviceRecord.TotalAmount);
            var serviceFeePercentage = Convert.ToDecimal(serviceRecord.Company.ServiceFeePercentage);

            var serviceFeeAmount = Math.Round((serviceFeePercentage * amount) / 100m, 2);

            if (serviceRecord.Status == ServiceRecordStatus.NotProcessed)
            {
                //Determine Status
                var status = DetermineStatus(serviceRecord); //, customer.IsCcActive

                if (status == ServiceRecordStatus.PaidByCcOnFile)
                {
                    string transactionId;
                    var gatewayResponse = ChargeCustomer(merchantAccountId, gatewayCustomerId,
                        amount, serviceFeeAmount, serviceRecord.Company.PaymentGatewayType, 
                        serviceRecord.Customer.CustomerJson, out transactionId);
                    serviceRecord.Status = gatewayResponse.Success ? ServiceRecordStatus.PaidByCcOnFile : ServiceRecordStatus.UnPaidPaymentFailed;
                    serviceRecord.BtTransactionId = transactionId;

                    response.Commission = new Commission
                    {
                        Amount = serviceFeeAmount,
                        BtTransactionId = serviceRecord.BtTransactionId,
                        Date = DateTime.UtcNow,
                        ServiceRecordId = serviceRecord.ServiceRecordId,
                        Type = CommissionType.AutoBilledAtTimeOfService
                    };
                }
                else
                {
                    serviceRecord.Status = status;
                }

                emailRequest.SetPaidExternal = status == ServiceRecordStatus.PaidExternal;
                var emailResponse = _emailService.SendEmail(emailRequest);
                response.Message = emailResponse.Message;
                response.Success = emailResponse.Success;
            }
            response.Status = serviceRecord.Status;
            response.ServiceRecord = serviceRecord;
            return response;
        }

        public ChargeCustomerResponse ProcessChargeCompany(ChargeCustomerRequest request)
        {
            var response = new ChargeCustomerResponse();

            var company = _context.Companies.FirstOrDefault(x => x.CompanyId == request.CompanyId);
            if (company == null)
            {
                response.Message = "Company does not exist in the system";
                return response;
            }

            if (string.IsNullOrEmpty(request.ServiceRecordIds))
            {
                response.Message = "System requires Service Record Ids to proceed";
                return response;
            }

            var serviceIds = new List<int>();

            if (!string.IsNullOrEmpty(request.ServiceRecordIds))
            {
                var arr = request.ServiceRecordIds.Split(',');
                serviceIds.AddRange(from s in arr where !string.IsNullOrEmpty(s) select Convert.ToInt32(s));

                if (!serviceIds.Any())
                {
                    response.Message = "System requires Service Record Ids to proceed";
                    return response;
                }
            }

            if (company.PaymentGatewayType == PaymentGatewayType.Braintree)
            {
                var result = _paymentService.Transaction(new GatewayTransactionRequest
                {
                    MerchantId = ApplicationSettings.MerchantAccountId,
                    Amount = request.Amount,
                    CustomerId = company.GatewayCustomerId,
                    DescriptorName = ApplicationSettings.SuperAdminDescriptorName,
                    DescriptorUrl = ApplicationSettings.SuperAdminDescriptorUrl,
                });

                if (result.Success)
                {
                    response.Success = true;
                    //Send Email
                    AfterProcessChargeCompany(serviceIds, result.TransactionId);
                }
                else
                {
                    response.Message = result.Message;
                }
            }
            else
            {
                var result = _splashPaymentService.Transaction(new SplashTransactionRequest
                {
                    MerchantId = ApplicationSettings.SplashMerchantId,
                    Amount = request.Amount,
                    CustomerJson = company.CustomerJson,
                    Descriptor = ApplicationSettings.SuperAdminDescriptorName
                });

                if (result.Success)
                {
                    response.Success = true;
                    //Send Email
                    AfterProcessChargeCompany(serviceIds, result.TransactionId);
                }
                else
                {
                    response.Message = result.Message;
                }
            }



            return response;
        }

        private void AfterProcessChargeCompany(List<int> serviceIds, string transactionId)
        {
            //update service record ids
            foreach (var sr in serviceIds
                .Select(serviceId => _context.ServiceRecords
                    .FirstOrDefault(x => x.ServiceRecordId == serviceId))
                    .Where(sr => sr != null).ToList())
            {
                sr.BilledToCompany = true;

                var serviceCost = sr.ServiceRecordItems.Sum(x => x.CostOfService) ?? 0;
                var commission = (Convert.ToDecimal(serviceCost * sr.Company.ServiceFeePercentage) / 100m);

                sr.Commissions.Add(new Commission
                {
                    Amount = commission,
                    BtTransactionId = transactionId,
                    Date = DateTime.UtcNow,
                    ServiceRecordId = sr.ServiceRecordId,
                    Type = CommissionType.ManuallyBilledBySuperAdmin
                });
                _context.SaveChanges();
            }
        }

        public ChargeCustomerResponse ProcessChargeCustomer(ChargeCustomerCcRequest request)
        {
            var response = new ChargeCustomerResponse();

            var customer = _context.Customers.FirstOrDefault(x => x.CustomerId == request.CustomerId);
            var company =
               _context.Companies.Include(x => x.ContactPersons).FirstOrDefault(x => x.CompanyId == customer.CompanyId);

            if (customer == null)
            {
                response.Message = "Customer does not exist in the system";
                return response;
            }

            if (company == null)
            {
                response.Message = "Company does not exist in the system";
                return response;
            }

            if (string.IsNullOrEmpty(request.ServiceRecordIds))
            {
                response.Message = "System requires Service Record Ids to proceed";
                return response;
            }

            if (string.IsNullOrEmpty(company.MerchantAccountId))
            {
                response.Message = "Merchant Account is not setup. Please setup merchant account in Settings.";
                return response;
            }

            var serviceIds = new List<int>();

            if (!string.IsNullOrEmpty(request.ServiceRecordIds))
            {
                var arr = request.ServiceRecordIds.Split(',');
                serviceIds.AddRange(from s in arr where !string.IsNullOrEmpty(s) select Convert.ToInt32(s));

                if (!serviceIds.Any())
                {
                    response.Message = "System requires Service Record Ids to proceed";
                    return response;
                }
            }

            if (company.PaymentGatewayType == PaymentGatewayType.Splash)
            {
                if (!string.IsNullOrEmpty(customer.CustomerJson))
                {
                    var customerResponse =
                        JsonConvert.DeserializeObject<SplashCreateCustomerResponseObject>(customer.CustomerJson);
                    if (customerResponse != null)
                    {
                        customer.IsCcActive = customerResponse.response.data[0].inactive == 0;
                    }
                }

            }

            if (customer.IsCcActive)
            {
                decimal temp = 0.0M;

                foreach (var sr in serviceIds.Select(serviceId =>
                    _context.ServiceRecords.FirstOrDefault(x => x.ServiceRecordId == serviceId)).Where(sr => sr != null)
                    )
                {
                    var serviceCost = sr.ServiceRecordItems.Sum(x => x.CostOfService) ?? 0;
                    temp += Math.Round((Convert.ToDecimal(serviceCost * customer.Company.ServiceFeePercentage) / 100m), 2);
                }

                var serviceFeeAmount = temp;

                if (company.PaymentGatewayType == PaymentGatewayType.Braintree)
                {
                    var result = _paymentService.Transaction(new GatewayTransactionRequest
                    {
                        MerchantId = customer.Company.MerchantAccountId,
                        Amount = request.Amount,
                        CustomerId = customer.GatewayCustomerId,
                        DescriptorName = customer.Company.Name,
                        ServiceFeeAmount = serviceFeeAmount
                    });

                    if (result.Success)
                    {
                        response.Success = true;
                        AfterProcessCustomerPayment(serviceIds, result.TransactionId, customer);
                    }
                    else
                    {
                        response.Message = result.Message;
                        return response;
                    }
                }
                else
                {
                    var result = _splashPaymentService.Transaction(new SplashTransactionRequest
                    {
                        MerchantId = customer.Company.MerchantAccountId,
                        Amount = request.Amount,
                        CustomerJson = customer.CustomerJson
                    });

                    if (result.Success)
                    {
                        response.Success = true;
                        AfterProcessCustomerPayment(serviceIds, result.TransactionId, customer);
                    }
                    else
                    {
                        response.Message = result.Message;
                        return response;
                    }
                }
            }
            //prepare email request
            var emailRequest = new SendMultipleServicesEmailRequest
            {
                FromEmail = ApplicationSettings.FromEmail,
                ServiceRecords = serviceIds.ToArray(),
                Template = EmailConstants.GeneralEmailTemplate,
                PdfTemplate = EmailConstants.GeneralPdfTemplate,
                IsBillingFromAdmin = true
            };

            _emailService.SendEmail(emailRequest);
            return response;
        }

        private void AfterProcessCustomerPayment(List<int> serviceIds, string transactionId, Customer customer)
        {

            //update service record ids
            foreach (var sr in serviceIds.Select(serviceId =>
                _context.ServiceRecords.FirstOrDefault(x => x.ServiceRecordId == serviceId))
                .Where(sr => sr != null))
            {
                sr.Status = ServiceRecordStatus.PaidByCcOnFile;
                sr.BtTransactionId = transactionId;

                var serviceCost = sr.ServiceRecordItems.Sum(x => x.CostOfService) ?? 0;
                var commission =
                    Math.Round((Convert.ToDecimal(serviceCost * customer.Company.ServiceFeePercentage) / 100m),
                        2);

                var commissionDb =
                    _context.Commissions.FirstOrDefault(x => x.ServiceRecordId == sr.ServiceRecordId);

                if (commissionDb == null)
                {
                    sr.Commissions.Add(new Commission
                    {
                        Amount = commission,
                        BtTransactionId = sr.BtTransactionId,
                        Date = DateTime.UtcNow,
                        ServiceRecordId = sr.ServiceRecordId,
                        Type = CommissionType.AutoBilledByOrgAdmin
                    });
                }

                sr.InvoiceDate = DateTime.UtcNow;
                _context.SaveChanges();
            }
        }


        // Transaction
        public ChargeCustomerResponse ProcessPayment(GetChargeCustomerRequest request)
        {
            var response = new ChargeCustomerResponse();

            try
            {
                var company = _context.Companies.FirstOrDefault(x => x.CompanyId == request.CustomerId);
                if (company == null)
                {
                    response.Message = "Company does not exist in the system.";
                    return response;
                }
                if (string.IsNullOrEmpty(request.ServiceRecordIds))
                {
                    response.Message = "System requires Service Record Ids to proceed";
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
                response.Success = false;
            }
            return response;

        }

        public ChargeCustomerResponse SetPaidExternal(ChargeCustomerCcRequest request)
        {
            var response = new ChargeCustomerResponse();

            if (string.IsNullOrEmpty(request.ServiceRecordIds))
            {
                response.Message = "System requires Service Record Ids to proceed";
                return response;
            }

            var serviceIds = new List<int>();

            if (!string.IsNullOrEmpty(request.ServiceRecordIds))
            {
                var arr = request.ServiceRecordIds.Split(',');
                serviceIds.AddRange(from s in arr where !string.IsNullOrEmpty(s) select Convert.ToInt32(s));

                if (!serviceIds.Any())
                {
                    response.Message = "System requires Service Record Ids to proceed";
                    return response;
                }
            }

            foreach (var sr in serviceIds.Select(serviceId => _context.ServiceRecords.FirstOrDefault(x => x.ServiceRecordId == serviceId)).Where(sr => sr != null))
            {
                sr.Status = ServiceRecordStatus.PaidExternal;
                sr.InvoiceDate = DateTime.UtcNow;
                _context.SaveChanges();
            }
            response.Success = true;

            //prepare email request
            var emailRequest = new SendMultipleServicesEmailRequest
            {
                FromEmail = ApplicationSettings.FromEmail,
                ServiceRecords = serviceIds.ToArray(),
                Template = EmailConstants.GeneralEmailTemplate,
                PdfTemplate = EmailConstants.GeneralPdfTemplate,
                IsBillingFromAdmin = true,
                SetPaidExternal = true
            };

            _emailService.SendEmail(emailRequest);

            return response;
        }

        //To be removed with GetGatewayCustomerDetail
        public MerchantCreditCardDetailResponse GetMerchantCreditCardDetail(MerchantCreditCardDetailRequest request)
        {
            var response = new MerchantCreditCardDetailResponse();

            var company = _context.Companies.FirstOrDefault(x => x.CompanyId == request.CompanyId);

            if (company == null)
            {
                response.Message = "Company does not exist in the system.";
                return response;
            }

            if (company.GatewayCustomerId == null)
            {
                response.Message = "";
                return response;
            }

            var customer = GatewayConstant.Gateway.Customer.Find(company.GatewayCustomerId);

            if (customer != null)
            {
                response.FirstName = customer.FirstName;
                response.LastName = customer.LastName;
                response.Email = customer.Email;

                if (customer.CreditCards.Any())
                {
                    var firstCc = customer.CreditCards[0];
                    response.CreditCardNumber = firstCc.LastFour;
                    response.Expiration = string.Format("{0}/{1}", firstCc.ExpirationMonth, firstCc.ExpirationYear);
                }
            }
            return response;
        }

        //To be removed with GetGatewayCustomerDetail
        public CustomerCreditCardDetailResponse GetCustomerCreditCardDetail(CustomerCreditCardDetailRequest request)
        {
            var response = new CustomerCreditCardDetailResponse();

            var customer = _context.Customers.FirstOrDefault(x => x.CustomerId == request.CustomerId);

            if (customer == null)
            {
                response.Message = "Customer does not exist in the system.";
                return response;
            }

            if (customer.GatewayCustomerId == null)
            {
                response.Message = "Customer's CC is not setup";
                return response;
            }

            if (!customer.IsCcActive)
            {
                response.Message = "Customer payment method is not active.";
                return response;
            }

            var braintreeCustomer = GatewayConstant.Gateway.Customer.Find(customer.GatewayCustomerId);

            if (braintreeCustomer != null)
            {
                response.FirstName = braintreeCustomer.FirstName;
                response.LastName = braintreeCustomer.LastName;
                response.Email = braintreeCustomer.Email;

                if (braintreeCustomer.CreditCards.Any())
                {
                    var firstCc = braintreeCustomer.CreditCards[0];
                    response.CreditCardNumber = firstCc.LastFour;
                    response.Expiration = string.Format("{0}/{1}", firstCc.ExpirationMonth, firstCc.ExpirationYear);
                }
            }
            return response;
        }

        // Get GatewayCustomer detail
        public GetGatewayCustomerResponse GetGatewayCustomerDetail(GetGatewayCustomerRequest request)
        {
            var response = new GetGatewayCustomerResponse();

            try
            {
                if (request.IsMerchant) // company
                {
                    //get company
                    var company = _context.Companies.FirstOrDefault(x => x.CompanyId == request.CustomerId);

                    if (company == null)
                    {
                        response.Message = "Company does not exist in the system.";
                        return response;
                    }
                    if (company.GatewayCustomerId == null)
                    {
                        response.Message = "";
                        return response;
                    }

                    response = _paymentService.GetCustomer(request.CustomerId.ToString());
                }
                else // customer
                {

                    // get customer           
                    var customer = _context.Customers.FirstOrDefault(x => x.CustomerId == request.CustomerId);

                    if (customer == null)
                    {
                        response.Message = "Customer does not exist in the system.";
                        return response;
                    }
                    if (customer.GatewayCustomerId == null)
                    {
                        response.Message = "Customer credit card is not setup.";
                        return response;
                    }
                    if (!customer.IsCcActive)
                    {
                        response.Message = "Customer payment method is not active.";
                        return response;
                    }

                    response = _paymentService.GetCustomer(request.CustomerId.ToString());
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message.ToString();
            }
            return response;
        }

        #endregion

        #region Private Methods

        private bool CanChargeCustomerCc(Customer customer)
        {
            return !string.IsNullOrEmpty(customer.GatewayCustomerId) && customer.IsCcActive;
        }

        private ServiceRecordStatus DetermineStatus(ServiceRecord serviceRecord)
        {
            var status = ServiceRecordStatus.NotProcessed;
            var canChargeCc = CanChargeCustomerCc(serviceRecord.Customer);

            var hasNonStandard = serviceRecord.ServiceRecordItems.Any(x => x.Type == ServiceRecordItemType.NonStandard);

            if (hasNonStandard && !canChargeCc)
            {
                status = ServiceRecordStatus.UnPaidBoth;
            }

            if (hasNonStandard && canChargeCc)
            {
                status = ServiceRecordStatus.UnPaidNonStandardServiceOnly;
            }

            if (!hasNonStandard && !canChargeCc)
            {
                //status = ServiceRecordStatus.UnPaidBillingIdMissingOnly;
                status = ServiceRecordStatus.PaidExternal;
            }

            if (!hasNonStandard && canChargeCc)
            {
                status = ServiceRecordStatus.PaidByCcOnFile;
            }

            return status;
        }

        private ServiceRecordStatus DetermineStatus_Old(ServiceRecord serviceRecord)
        {
            var status = ServiceRecordStatus.NotProcessed;

            if (!serviceRecord.Customer.IsCcActive)
            {
                return ServiceRecordStatus.UnPaidBillingIdMissingOnly;
            }

            var hasNonStandard = serviceRecord.ServiceRecordItems.Any(x => x.Type == ServiceRecordItemType.NonStandard);

            if (hasNonStandard && string.IsNullOrEmpty(serviceRecord.Customer.GatewayCustomerId))
            {
                status = ServiceRecordStatus.UnPaidBoth;
            }

            if (hasNonStandard && !string.IsNullOrEmpty(serviceRecord.Customer.GatewayCustomerId))
            {
                status = ServiceRecordStatus.UnPaidNonStandardServiceOnly;
            }

            if (!hasNonStandard && string.IsNullOrEmpty(serviceRecord.Customer.GatewayCustomerId))
            {
                status = ServiceRecordStatus.UnPaidBillingIdMissingOnly;
            }

            if (!hasNonStandard && !string.IsNullOrEmpty(serviceRecord.Customer.GatewayCustomerId))
            {
                status = ServiceRecordStatus.PaidByCcOnFile;
            }


            return status;
        }

        private BaseResponse ChargeCustomer(string merchantAccountId, string gatewayCustomerId,
            decimal amount, decimal serviceFeeAmount, PaymentGatewayType gatewayType,
            string customerJson, out string transactionId)
        {
            var response = new BaseResponse();
            transactionId = null;

            if (gatewayType == PaymentGatewayType.Braintree)
            {
                var result = GatewayConstant.Gateway.Transaction.Sale(
                    new TransactionRequest
                    {
                        MerchantAccountId = merchantAccountId,
                        CustomerId = gatewayCustomerId,
                        Amount = amount,
                        ServiceFeeAmount = serviceFeeAmount,
                        Options = new TransactionOptionsRequest
                        {
                            SubmitForSettlement = true
                        }
                    });

                if (result.IsSuccess())
                {
                    response.Success = true;
                    transactionId = result.Target != null ? result.Target.Id : null;
                }
                else
                {
                    var errors = result.Errors.DeepAll();

                    foreach (var validationError in errors)
                    {
                        response.Message += validationError.Message + @"<br/>";
                    }
                }
            }
            else
            {
                var result = _splashPaymentService.Transaction(new SplashTransactionRequest
                     { 
                        MerchantId = merchantAccountId, 
                        Amount = amount, 
                        CustomerJson = customerJson
                });

                if (result.Success)
                {
                    response.Success = true;
                    transactionId = result.TransactionId;
                }
                else
                {
                    response.Message = result.Message;
                }
            }

            return response;
        }

        #endregion
    }
}

using Braintree;
using System;
using System.Linq;
using VT.Common;
using VT.Services.DTOs;
using VT.Services.Interfaces;

namespace VT.Services.Services
{
    public class BraintreePaymentService : IPaymentService
    {
        #region Service Methods

        // Create Customer
        public GatewayCustomerResponse CreateCustomer(GatewayCustomerRequest request)
        {
            var response = new GatewayCustomerResponse();

            try
            {
                // Create customer object
                var customerRequest = new CustomerRequest
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PaymentMethodNonce = request.NonceFromTheClient
                };
                var result = GatewayConstant.Gateway.Customer.Create(customerRequest);
                if (result.IsSuccess())
                {
                    response = ProcessCustomerResponse(result);
                    response.Success = true;
                }
                else
                {
                    response.Message = result.Message + @"<br/>";
                    var errors = result.Errors.DeepAll();
                    foreach (var validationError in errors)
                        response.Message += validationError.Message + @"<br/>";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
                response.Success = false;
            }
            return response;
        }

        // Update Customer
        public GatewayCustomerResponse UpdateCustomer(GatewayCustomerRequest request)
        {
            var response = new GatewayCustomerResponse();

            try
            {
                // create customer object
                var customerRequest = new CustomerRequest
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    CreditCard = new CreditCardRequest
                    {
                        PaymentMethodNonce = request.NonceFromTheClient,
                        Options = new CreditCardOptionsRequest
                        {
                            UpdateExistingToken = request.CcToken
                        }
                    }
                };
                var result = GatewayConstant.Gateway.Customer.Update(request.GatewayCustomerId, customerRequest);

                if (result.IsSuccess())
                {
                    response = ProcessCustomerResponse(result);
                    response.Success = true;
                }
                else
                {
                    response.Message = result.Message + @"<br/>";
                    var errors = result.Errors.DeepAll();
                    foreach (var validationError in errors)
                        response.Message += validationError.Message + @"<br/>";

                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
                response.Success = false;
            }
            return response;
        }

        // Get Customer
        public GetGatewayCustomerResponse GetCustomer(string customerId)
        {
            var response = new GetGatewayCustomerResponse();

            try
            {  //get customer with customerId
                var customer = GatewayConstant.Gateway.Customer.Find(customerId);

                if (customer != null)  // customer exist
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
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
                response.Success = false;
            }
            return response;
        }

        // Create Merchant
        public GatewayCustomerResponse CreateMerchant(GatewayCustomerRequest request)   
        {
            var response = new GatewayCustomerResponse();

            try
            {
                //create customer object
                var customerRequest = new CustomerRequest
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PaymentMethodNonce = request.NonceFromTheClient
                };
                var result = GatewayConstant.Gateway.Customer.Create(customerRequest);

                if (result.IsSuccess())
                {
                    response.Success = true;
                    response = ProcessCustomerResponse(result);
                }
                else
                {
                    response.Message = result.Message + @"<br/>";
                    var errors = result.Errors.DeepAll();
                    foreach (var validationError in errors)
                        response.Message += validationError.Message + @"<br/>";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message.ToString();
            }
            return response;
        }

        // Update Merchant
        public GatewayCustomerResponse UpdateMerchant(GatewayCustomerRequest request)
        {
            var response = new GatewayCustomerResponse();

            try
            {
                // update customer object
                var customerRequest = new CustomerRequest
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    CreditCard = new CreditCardRequest
                    {
                        PaymentMethodNonce = request.NonceFromTheClient,
                        Options = new CreditCardOptionsRequest
                        {
                            UpdateExistingToken = request.CcToken
                        }
                    }
                };
                var result = GatewayConstant.Gateway.Customer.Update(request.GatewayCustomerId, customerRequest);

                if (result.IsSuccess())
                {
                    response = ProcessCustomerResponse(result);
                    response.Success = true;

                }
                else
                {
                    response.Message = result.Message + @"<br/>";
                    var errors = result.Errors.DeepAll();
                    foreach (var validationError in errors)
                        response.Message += validationError.Message + @"<br/>";

                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
                response.Success = false;
            }

            return response;
        }

        // Get Merchant
        public GatewayAccountDetailResponse GetMerchant(string merchantId)
        {
            var response = new GatewayAccountDetailResponse();

            try
            {
                var result = GatewayConstant.Gateway.MerchantAccount.Find(merchantId);

                if (result.Status == MerchantAccountStatus.ACTIVE)
                {
                    response.FirstName = result.IndividualDetails.FirstName;
                    response.LastName = result.IndividualDetails.LastName;
                    response.Email = result.IndividualDetails.Email;
                    response.Phone = result.IndividualDetails.Phone;
                    response.DateOfBirth = result.IndividualDetails.DateOfBirth;
                    response.Ssn = result.IndividualDetails.SsnLastFour;
                    if (result.IndividualDetails.Address != null)
                    {
                        response.IndStreetAddress = result.IndividualDetails.Address.StreetAddress;
                        response.IndLocality = result.IndividualDetails.Address.Locality;
                        response.IndRegion = result.IndividualDetails.Address.Region;
                        response.IndPostalCode = result.IndividualDetails.Address.PostalCode;
                    }
                    response.LegalName = result.BusinessDetails.LegalName;
                    response.DbaName = result.BusinessDetails.DbaName;
                    response.TaxId = result.BusinessDetails.TaxId;
                    if (result.BusinessDetails.Address != null)
                    {
                        response.BusStreetAddress = result.BusinessDetails.Address.StreetAddress;
                        response.BusLocality = result.BusinessDetails.Address.Locality;
                        response.BusRegion = result.BusinessDetails.Address.Region;
                        response.BusPostalCode = result.BusinessDetails.Address.PostalCode;
                    }
                    response.Descriptor = result.FundingDetails.Descriptor;
                    response.FundEmail = result.FundingDetails.Email;
                    response.FundMobilePhone = result.FundingDetails.MobilePhone;
                    response.AccountNumber = "xxxxxx" + result.FundingDetails.AccountNumberLast4;
                    response.RoutingNumber = result.FundingDetails.RoutingNumber;
                }
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message.ToString();
            }
           
            return response;
        }

        // TODO: Let's change the name of this method to SaveGatwayMerchantAccount
        public OrganizationAccountResponse CreateMerchantAccount(OrganizationAccountRequest request)
        {
            var response = new OrganizationAccountResponse();

            try
            {
                // create merchant account object
                var merchantAccountRequest = new MerchantAccountRequest
                {
                    Individual = new IndividualRequest
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email,
                        Phone = request.Phone,
                        DateOfBirth = request.DateOfBirth,
                        Ssn = request.Ssn,
                        Address = new AddressRequest
                        {
                            StreetAddress = request.IndStreetAddress,
                            Locality = request.IndLocality,
                            Region = request.IndRegion,
                            PostalCode = request.IndPostalCode
                        }
                    },
                    Business = new BusinessRequest
                    {
                        LegalName = request.LegalName,
                        DbaName = request.DbaName,
                        TaxId = request.TaxId,
                        Address = new AddressRequest
                        {
                            StreetAddress = request.BusStreetAddress,
                            Locality = request.BusLocality,
                            Region = request.BusRegion,
                            PostalCode = request.BusPostalCode
                        }
                    },
                    Funding = new FundingRequest
                    {
                        Descriptor = ApplicationSettings.Descriptor, //request.Descriptor,
                        Destination = FundingDestination.BANK,
                        Email = request.FundEmail,
                        MobilePhone = request.FundMobilePhone,
                        AccountNumber = request.AccountNumber,
                        RoutingNumber = request.RoutingNumber
                    },
                    TosAccepted = true,
                    MasterMerchantAccountId = ApplicationSettings.MerchantAccountId
                };

                var result = !string.IsNullOrEmpty(request.MerchantAccountId) ? GatewayConstant.Gateway.MerchantAccount.Update(request.MerchantAccountId, merchantAccountRequest) : GatewayConstant.Gateway.MerchantAccount.Create(merchantAccountRequest);

                if (result.IsSuccess())
                {
                    response.Success = true;
                    MerchantAccount merchantAccount = result.Target;
                    response.ReferenceNumber = merchantAccount.Id;
                }
                else
                {
                    response.Message = result.Message + @"<br/>";
                    var errors = result.Errors.DeepAll();
                    foreach (var validationError in errors)
                        response.Message += validationError.Message + @"<br/>";
                }   
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
                response.Success = false;
            }
            return response;
        }
        
        // Transaction
        public GatewayTransactionResponse Transaction(GatewayTransactionRequest request)
        {
            var response = new GatewayTransactionResponse();

            try
            {
                var transactionRequest = new TransactionRequest
                {
                    MerchantAccountId = request.MerchantId,
                    CustomerId = request.CustomerId,
                    Amount = Math.Round(request.Amount, 2),
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                transactionRequest.Descriptor = new Braintree.DescriptorRequest();

                if (string.IsNullOrEmpty(request.DescriptorName))
                    transactionRequest.Descriptor.Name = request.DescriptorName;

                if (string.IsNullOrEmpty(request.DescriptorUrl))
                    transactionRequest.Descriptor.Url = request.DescriptorUrl;

                if (request.ServiceFeeAmount > 0)
                    transactionRequest.ServiceFeeAmount = request.ServiceFeeAmount;

                var result = GatewayConstant.Gateway.Transaction.Sale(transactionRequest);

                if (result.IsSuccess())
                {
                    response.Success = true;
                    response.TransactionId = result.Target != null ? result.Target.Id : String.Empty;
                }
                else
                {
                    response.Message = result.Message + @"<br/>";
                    var errors = result.Errors.DeepAll();
                    foreach (var validationError in errors)
                        response.Message += validationError.Message + @"<br/>";
                }
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
                response.Success = false;

            }
            return response;
        }

        #endregion

        #region Private Methods

        // Process Customer Response
        private GatewayCustomerResponse ProcessCustomerResponse(Result<Customer> result)
        {
            var response = new GatewayCustomerResponse();
            try
            {

                if (result.IsSuccess())
                {
                    Customer customerResponse = result.Target;
                    if (customerResponse != null)
                    {
                        if (customerResponse.CreditCards.Any())
                        {
                            response.ReferenceNumber = customerResponse.Id;
                            response.Success = true;
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "Credit card couldn't add. Please try again.";
                        }
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = result.Message.Replace("\n", "<br/>");
                }
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
                response.Success = false;
            }

            return response;
        }

        #endregion
    }
}

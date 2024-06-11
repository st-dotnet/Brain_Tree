using System;
using Newtonsoft.Json;
using VT.Common;
using VT.Common.Utils;
using VT.Services.DTOs;
using VT.Services.DTOs.SplashPayments;
using VT.Services.Services;

namespace VT.Services.Components
{
    public class SplashPaymentComponent
    {
        // create merchant combo request
        public string CreateMerchant(SplashCreateMerchant request)
        {
            try
            {
                string postData = JsonConvert.SerializeObject(request);

                var client = new RestClient
                {
                    EndPoint = string.Format("{0}{1}", ApplicationSettings.SplashApiUrl, "merchants"),
                    Method = HttpVerb.POST,
                    PostData = postData
                };
                var json = client.MakeRequest();

                return json;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        // create customer
        public SplashCreateCustomerResponseObject CreateCustomer(CreateCustomerWithPaymentToken request)
        {
            var response = new SplashCreateCustomerResponseObject();
            try
            {
                string postData = JsonConvert.SerializeObject(request);

                var client = new RestClient
                {
                    EndPoint = string.Format("{0}{1}", ApplicationSettings.SplashApiUrl, "tokens"),
                    Method = HttpVerb.POST,
                    PostData = postData
                };
                var json = client.MakeRequest();

                response = JsonConvert.DeserializeObject<SplashCreateCustomerResponseObject>(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        // get merchant
        public GetMerchantResponse GetMerchant(string merchantId)
        {
            var response = new GetMerchantResponse();

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}{2}", ApplicationSettings.SplashApiUrl, "merchants", merchantId),
                Method = HttpVerb.GET
            };

            var json = client.MakeRequest();

            var splashMerchantResponse = JsonConvert.DeserializeObject<GetMerchantResponse>(json);

            if (splashMerchantResponse.response.errors.Count != 0)
            {
                response.Success = false;
                response.Message = "Some error occured while getting merchant details.";
            }
            else
            {
                response.Success = true;
                response.Message = "Query has been executed successfully.";
            }
            return response;
        }

        // get member
        public GetMemberResponse GetMember(string memberId)
        {
            var response = new GetMemberResponse();

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}{2}", ApplicationSettings.SplashApiUrl, "members", memberId),
                Method = HttpVerb.GET
            };

            var json = client.MakeRequest();

            var splashMerchantResponse = JsonConvert.DeserializeObject<GetMemberResponse>(json);

            if (splashMerchantResponse.response.errors.Count != 0)
            {
                response.Success = false;
                response.Message = "Some error occured while getting member details.";
            }
            else
            {
                response.Success = true;
                response.Message = "Query has been executed successfully.";
            }
            return response;
        }

        // get entity
        public GetEntityResponse GetEntity(string entityId)
        {
            var response = new GetEntityResponse();

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}{2}", ApplicationSettings.SplashApiUrl, "entities", entityId),
                Method = HttpVerb.GET
            };

            var json = client.MakeRequest();

            var splashMerchantResponse = JsonConvert.DeserializeObject<GetEntityResponse>(json);

            if (splashMerchantResponse.response.errors.Count != 0)
            {
                response.Success = false;
                response.Message = "Some error occured while getting entity details.";
            }
            else
            {
                response.Success = true;
                response.Message = "Query has been executed successfully.";
            }
            return response;
        }

        // get account
        public GetAccountResponse GetAccount(string accountId)
        {
            var response = new GetAccountResponse();

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}{2}", ApplicationSettings.SplashApiUrl, "accounts", accountId),
                Method = HttpVerb.GET
            };

            var json = client.MakeRequest();

            var splashMerchantResponse = JsonConvert.DeserializeObject<GetAccountResponse>(json);

            if (splashMerchantResponse.response.errors.Count != 0)
            {
                response.Success = false;
                response.Message = "Some error occured while getting account details.";
            }
            else
            {
                response.Success = true;
                response.Message = "Query has been executed successfully.";
            }
            return response;
        }

        // get customer
        public GatewayCustomerResponse GetCustomer(string customerId)
        {
            var response = new GatewayCustomerResponse();

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}{2}", ApplicationSettings.SplashApiUrl, "customers", customerId),
                Method = HttpVerb.GET
            };


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

        // make transaction
        public SplashTransactionResponse MakeTransaction(SplashTransaction request)
        { 
            var postData = JsonConvert.SerializeObject(request); 
            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}", ApplicationSettings.SplashApiUrl, "txns"),
                Method = HttpVerb.POST,
                PostData = postData
            };  
            var json = client.MakeRequest(); 
            var transactionResponse = JsonConvert.DeserializeObject<SplashTransactionResponse>(json); 
            return transactionResponse;
        }

        // create fee
        public FeeResponse CreateFee(CreateFeeRequest request)
        { 
            var postData = JsonConvert.SerializeObject(request);

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}", ApplicationSettings.SplashApiUrl, "fees"),
                Method = HttpVerb.POST,
                PostData = postData
            };

            var json = client.MakeRequest();

            var feeResponse = JsonConvert.DeserializeObject<FeeResponse>(json); 
            return feeResponse;
        }

        // delete fee
        public GatewayCustomerResponse DeleteFee(string feeId)
        {
            var response = new GatewayCustomerResponse();

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}{2}", ApplicationSettings.SplashApiUrl, "fees", feeId),
                Method = HttpVerb.DELETE
            };

            var json = client.MakeRequest();
            return response;
        }

        // get token
        public GatewayCustomerResponse GetToken(string token)
        {
            var response = new GatewayCustomerResponse();

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}{2}", ApplicationSettings.SplashApiUrl, "tokens", "token"),
                Method = HttpVerb.GET
            };

            var json = client.MakeRequest();

            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);

            if (tokenResponse.response.errors.Count != 0)
            {
                response.Success = false;
                response.Message = "Some error occured while getting token details.";
            }
            else
            {
                response.Success = true;
                response.Message = "Query has been executed successfully.";
            }
            return response;
        }

        // delete customer
        public GatewayCustomerResponse DeleteCustomer(string customerId)
        {
            var response = new GatewayCustomerResponse();

            //string postData = JsonConvert.SerializeObject(customerId);

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}{2}", ApplicationSettings.SplashApiUrl, "customers", customerId),
                Method = HttpVerb.DELETE
            };

            var json = client.MakeRequest();

            var deleteCustomerResponse = JsonConvert.DeserializeObject<SplashDeleteCustomerResponse>(json);

            if (deleteCustomerResponse.response == null)
            {
                response.Message = "Customer with this id does not exists.";
                response.Success = false;
                return response;
            }
            if (deleteCustomerResponse.response.errors.Count != 0)
            {
                response.Message = "Could not delete the customer";
                response.Success = false;
            }
            else
            {
                response.Message = "Customer has been deleted successfully.";
                response.Success = true;
            }
            return response;
        }

        // delete merchant
        public GatewayCustomerResponse DeleteMerchant(string merchantId)
        {
            var response = new GatewayCustomerResponse();

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}{2}", ApplicationSettings.SplashApiUrl, "merchants", merchantId),
                Method = HttpVerb.DELETE
            };


            var json = client.MakeRequest();

            return response;
        }

        // delete plan
        public GatewayCustomerResponse DeletePlan(string planId)
        {
            var response = new GatewayCustomerResponse();

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}{2}", ApplicationSettings.SplashApiUrl, "plans", planId),
                Method = HttpVerb.DELETE
            };

            var json = client.MakeRequest();

            var deletePlanResponse = JsonConvert.DeserializeObject<SplashPlanResponse>(json);

            if (deletePlanResponse.response.errors.Count != 0)
            {
                response.Message = "Some error occured while deleting the plan.";
                response.Success = false;
            }
            else
            {
                response.Message = "Plan has been deleted successfully.";
                response.Success = true;
            }
            return response;
        }

        //Update customer
        public SplashCreateCustomerResponseObject UpdateCustomer(SplashUpdateCustomerRequest request, string customerId)
        {
            string postData = JsonConvert.SerializeObject(request);

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}/{2}", ApplicationSettings.SplashApiUrl, "customers", customerId),
                Method = HttpVerb.PUT,
                PostData = postData
            };
            var json = client.MakeRequest();

            var response = JsonConvert.DeserializeObject<SplashCreateCustomerResponseObject>(json);
            return response;
        }

        //Disable customer credit card
        public SplashPaymentDetailResponse DisableCustomerCreditCard(DisableCreditCard request, string tokenId)
        {
            string postData = JsonConvert.SerializeObject(request);

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}/{2}", ApplicationSettings.SplashApiUrl, "tokens", tokenId),
                Method = HttpVerb.PUT,
                PostData = postData
            };

            var json = client.MakeRequest();
            var response = JsonConvert.DeserializeObject<SplashPaymentDetailResponse>(json);
            return response;
        }

        //Update customer credit card
        public SplashAddPaymentDetailResponse AddPaymentCreditCard(AddCustomerCreditCard request)
        {
            string postData = JsonConvert.SerializeObject(request);

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}", ApplicationSettings.SplashApiUrl, "tokens"),
                Method = HttpVerb.POST,
                PostData = postData
            };

            var json = client.MakeRequest();
            var response = JsonConvert.DeserializeObject<SplashAddPaymentDetailResponse>(json);
            return response;
        }

        //Update merchant
        public SplashMerchantUpdateObject UpdateMerchant(SplashMerchantUpdate request, string merchantId)
        {
            var response = new SplashMerchantUpdateObject();

            string postData = JsonConvert.SerializeObject(request);

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}/{2}", ApplicationSettings.SplashApiUrl, "merchants", merchantId),
                Method = HttpVerb.PUT,
                PostData = postData
            };

            var json = client.MakeRequest();

            response = JsonConvert.DeserializeObject<SplashMerchantUpdateObject>(json);
            return response;
        }

        // update merchant entity
        public SplashMerchantEntityUpdateObject UpdateEntity(SplashUpdateEntity request, string entityId)
        {
            var response = new SplashMerchantEntityUpdateObject();

            string postData = JsonConvert.SerializeObject(request);

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}/{2}", ApplicationSettings.SplashApiUrl, "entities", entityId),
                Method = HttpVerb.PUT,
                PostData = postData
            };

            var json = client.MakeRequest();

            response = JsonConvert.DeserializeObject<SplashMerchantEntityUpdateObject>(json);

            return response;
        }

        // update merchant member
        public SplashMerchantMemberUpdateObject UpdateMember(SplashUpdateMember request, string memberId)
        {
            var response = new SplashMerchantMemberUpdateObject();

            var postData = JsonConvert.SerializeObject(request);

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}/{2}", ApplicationSettings.SplashApiUrl, "members", memberId),
                Method = HttpVerb.PUT,
                PostData = postData
            };
            var json = client.MakeRequest();

            response = JsonConvert.DeserializeObject<SplashMerchantMemberUpdateObject>(json);

            return response;
        }

        // update merchant account
        public SplashMerchantAccountUpdateObject UpdateMerchantAccount(SplashUpdateAccount request, string accountId)
        {
            var response = new SplashMerchantAccountUpdateObject();

            var postData = JsonConvert.SerializeObject(request);

            var client = new RestClient
            {
                EndPoint = string.Format("{0}{1}/{2}", ApplicationSettings.SplashApiUrl, "accounts", accountId),
                Method = HttpVerb.PUT,
                PostData = postData
            };
            var json = client.MakeRequest();

            response = JsonConvert.DeserializeObject<SplashMerchantAccountUpdateObject>(json);

            return response;
        }

    }
}

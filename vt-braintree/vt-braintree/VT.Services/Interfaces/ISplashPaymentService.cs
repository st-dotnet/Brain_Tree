using VT.Services.DTOs;
using VT.Services.DTOs.SplashPayments;

namespace VT.Services.Interfaces
{
    public interface ISplashPaymentService
    {
        SplashAccountResponse CreateMerchant(SplashCreateMerchantRequest request);

        BaseResponse UpdateMerchantInfo(UpdateSplashMerchantRequest request);
        BaseResponse UpdateMerchantEntity(UpdateSplashMerchantEntityRequest request);
        BaseResponse UpdateMerchantMember(UpdateSplashMerchantMemberRequest request);
        BaseResponse UpdateMerchantAccount(UpdateSplashMerchantAccountRequest request);
         
        SplashAccountResponse UpdateMerchant(SplashCreateMerchantRequest request); 
        SplashGetMerchantResponse GetMerchantDetail(int companyId);
        SplashCustomerDetailResponse GetCompanyCcDetail(int companyId);
        SplashCustomerDetailResponse GetCustomerCcDetail(int customerId);
        SplashGatewayMerchantResponse GetMerchant(int companyId);
        SplashGetMerchantResponse GetMember(int companyId);
        SplashGetMerchantResponse GetAccount(int companyId);
        SplashGetMerchantResponse GetEntity(int companyId);

        SplashAccountResponse CreateCcCustomerForCompany(SplashCustomerCreateRequest request);
        SplashAccountResponse CreateCcCustomerForCustomer(SplashCustomerCreateRequest request);

        BaseResponse UpdateCustomerCustomer(UpdateSplashCustomerRequest request);
        BaseResponse UpdateCompanyCustomer(UpdateSplashCustomerRequest request);
        SplashAccountResponse GetCustomer(string customerId);
         
        SplashTransactionResult Transaction(SplashTransactionRequest request);
          
        BaseResponse DisableCompanyCreditCard(int companyId);
        BaseResponse DisableCustomerCreditCard(int customerId);
        SplashAccountResponse AddCompanyCreditCard(AddCustomerCreditCardRequest request);
        SplashAccountResponse AddCustomerCreditCard(AddCustomerCreditCardRequest request);
        SplashAccountResponse UpdateEntity(SplashCreateEntityRequest request, string entityId);
        SplashAccountResponse CreateFee(CreateFeeRequest request);
        SplashAccountResponse DeleteFee(string feeId); 
        SplashAccountResponse CreatePlan(SplashCreatePlanForMerchantRequest request);
    }
}

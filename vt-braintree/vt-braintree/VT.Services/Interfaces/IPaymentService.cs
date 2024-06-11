using VT.Services.DTOs;

namespace VT.Services.Interfaces
{
    public interface IPaymentService
    {
        GatewayCustomerResponse CreateMerchant(GatewayCustomerRequest request);
        GatewayCustomerResponse UpdateMerchant(GatewayCustomerRequest request);
        GatewayAccountDetailResponse GetMerchant(string merchantId);
        GatewayCustomerResponse CreateCustomer(GatewayCustomerRequest request);
        GatewayCustomerResponse UpdateCustomer(GatewayCustomerRequest request);
        GetGatewayCustomerResponse GetCustomer(string customerId);
        GatewayTransactionResponse Transaction(GatewayTransactionRequest request);
        OrganizationAccountResponse CreateMerchantAccount(OrganizationAccountRequest request);
        //OrganizationAccountResponse UpdateMerchantAccount(OrganizationAccountRequest request);
    }
}

using VT.Services.DTOs;

namespace VT.Services.Interfaces
{
    public interface IPaymentGatewayService
    {
        GatewayCustomerResponse SaveMerchantCc(GatewayCustomerRequest request);
        GatewayCustomerResponse SaveCustomerCc(GatewayCustomerRequest request);

        OrganizationAccountResponse CreateOrganizationMerchantAccount(OrganizationAccountRequest request);
        ProcessPaymentResponse ProcessPayment(ProcessPaymentRequest request);
        ChargeCustomerResponse ProcessChargeCompany(ChargeCustomerRequest request);
        ChargeCustomerResponse ProcessChargeCustomer(ChargeCustomerCcRequest request);
        ChargeCustomerResponse SetPaidExternal(ChargeCustomerCcRequest request);
        GatewayAccountDetailResponse GetMerchantAccountDetail(MerchantAccountDetailRequest request);
        MerchantCreditCardDetailResponse GetMerchantCreditCardDetail(MerchantCreditCardDetailRequest request);
        CustomerCreditCardDetailResponse GetCustomerCreditCardDetail(CustomerCreditCardDetailRequest request);
        GetGatewayCustomerResponse GetGatewayCustomerDetail(GetGatewayCustomerRequest request);
        ChargeCustomerResponse ProcessPayment(GetChargeCustomerRequest request);
    }
}

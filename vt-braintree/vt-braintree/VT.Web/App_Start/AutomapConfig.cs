using AutoMapper;
using VT.Services.DTOs;
using VT.Services.DTOs.SplashPayments;
using VT.Web.Models;

namespace VT.Web
{
    public class AutomapConfig
    {
        public static void Setup()
        {
            Mapper.CreateMap<OrganizationDetailResponse, SaveOrganizationViewModel>();
            Mapper.CreateMap<SaveOrganizationViewModel, CompanySaveRequest>();
            Mapper.CreateMap<PasswordResetModel,ResetPasswordRequest>();
            Mapper.CreateMap<CompanyWorkerResponse, SaveUserModel>();
            Mapper.CreateMap<CompanyWorkerResponse,UserDetailViewModel>();
            Mapper.CreateMap<MerchantAccountViewModel, OrganizationAccountRequest>();
            Mapper.CreateMap<GatewayCustomerViewModel, GatewayCustomerRequest>();
            Mapper.CreateMap<ChargeCustomerViewModel, ChargeCustomerRequest>();
            Mapper.CreateMap<SetServiceRecordItemCostModel, SetServiceRecordItemRequest>();
            Mapper.CreateMap<ChargeCustomerAccountViewModel, ChargeCustomerCcRequest>();
            Mapper.CreateMap<CustomerDetailResponse, CustomerDetailViewModel>();
            Mapper.CreateMap<GatewayAccountDetailResponse, MerchantAccountViewModel>();
            Mapper.CreateMap<ImportCustomerValidation, ImportCustomerRequest>();
            Mapper.CreateMap<SplashCustomerDetailResponse, SplashCustomerModel>();
             

            Mapper.CreateMap<GetInvoiceDetailItem, InvoicesViewModel>();
            Mapper.CreateMap<GetVoidInvoiceDetailItem, VoidInvoicesViewModel>();
            Mapper.CreateMap<GetCommissionExpenseDetailItem, CommissionExpenseViewModel>();
             
            Mapper.CreateMap<SplashGetMerchantResponse, SplashMerchantModel>();
        }
    }
}
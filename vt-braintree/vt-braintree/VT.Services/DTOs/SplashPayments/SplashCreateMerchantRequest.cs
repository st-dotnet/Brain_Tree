namespace VT.Services.DTOs.SplashPayments
{
    public class BaseSplashCreateMerchantRequest
    {
        public int CompanyId { get; set; }
        public string AnnualCCSales { get; set; }
        public string Established { get; set; }
        public string MerchantCategoryCode { get; set; }
        public string MerchantNew { get; set; }
        public string Status { get; set; }
        public string TCVersion { get; set; }
        public string EntityLoginId { get; set; }
        public string EntityAddress1 { get; set; }
        public string EntityCity { get; set; }
        public string EntityCountry { get; set; }
        public string EntityEmail { get; set; }
        public string EntityEmployerId { get; set; }
        public string EntityName { get; set; }
        public string EntityPhone { get; set; }
        public string EntityState { get; set; }
        public string EntityType { get; set; }
        public string EntityWebsite { get; set; }
        public string EntityZip { get; set; }
        public string CardOrAccountNumber { get; set; }
        public string AccountsPaymentMethod { get; set; }
        public string AccountsRoutingCode { get; set; }
        public string AccountsPrimary { get; set; }
        public string MemberTitle { get; set; }
        public string MemberDateOfBirth { get; set; }
        public string MemberDriverLicense { get; set; }
        public string MemberDriverLicenseState { get; set; }
        public string MemberEmail { get; set; }
        public string MemberFirstName { get; set; }
        public string MemberLastName { get; set; }
        public string MemberOwnerShip { get; set; }
        public string MemberPrimary { get; set; }
        public string MemberSocialSecurityNumber { get; set; }
    }

    public class SplashCreateMerchantRequest : BaseSplashCreateMerchantRequest
    {  
    }

    public class SplashGetMerchantResponse : BaseSplashCreateMerchantRequest
    {
        public string Message { get; set; }
        public bool Success { get; set; }
    }

    public class UpdateSplashMerchantRequest
    {
        public int CompanyId { get; set; }

        public string AnnualCCSales { get; set; }
        public string Established { get; set; }
        public string MerchantCategoryCode { get; set; }
    }

    public class UpdateSplashMerchantEntityRequest
    {
        public int CompanyId { get; set; }
        public string EntityLoginId { get; set; }
        public string EntityAddress1 { get; set; }
        public string EntityCity { get; set; }
        public string EntityCountry { get; set; }
        public string EntityEmail { get; set; }
        public string EntityEmployerId { get; set; }
        public string EntityName { get; set; }
        public string EntityPhone { get; set; }
        public string EntityState { get; set; }
        public string EntityType { get; set; }
        public string EntityWebsite { get; set; }
        public string EntityZip { get; set; }
    }

    public class UpdateSplashMerchantMemberRequest
    {
        public int CompanyId { get; set; }
        public string MemberTitle { get; set; }
        public string MemberDateOfBirth { get; set; }
        public string MemberDriverLicense { get; set; }
        public string MemberDriverLicenseState { get; set; }
        public string MemberEmail { get; set; }
        public string MemberFirstName { get; set; }
        public string MemberLastName { get; set; }
        public string MemberOwnerShip { get; set; }
        public string MemberSocialSecurityNumber { get; set; }
    }

    public class UpdateSplashMerchantAccountRequest
    {
        public int CompanyId { get; set; }
        public string CardOrAccountNumber { get; set; }
        public string AccountsPaymentMethod { get; set; }
        public string AccountsRoutingCode { get; set; }
    }
}

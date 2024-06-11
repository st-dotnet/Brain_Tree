namespace VT.Web.Models
{
    public class SplashMerchantModel
    {
        public int CompanyId { get; set; }

        public string AnnualCCSales { get; set; }
        public string Established { get; set; }
        public string MerchantCategoryCode { get; set; }

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

        public string MemberTitle { get; set; }
        public string MemberDateOfBirth { get; set; }
        public string MemberDriverLicense { get; set; }
        public string MemberDriverLicenseState { get; set; }
        public string MemberEmail { get; set; }
        public string MemberFirstName { get; set; }
        public string MemberLastName { get; set; }
        public string MemberOwnerShip { get; set; }
        public string MemberSocialSecurityNumber { get; set; }

        public string RedirectUrl { get; set; }

        public bool IsEditMode { get; set; }
    }

    public class SplashMerchantEditModel
    {
        public int CompanyId { get; set; }

        public string AnnualCCSales { get; set; }
        public string Established { get; set; }
        public string MerchantCategoryCode { get; set; } 
    }

    public class SplashMerchantEntityModel
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

    public class SplashMerchantMemberModel
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

    public class SplashMerchantAccountModel
    {
        public int CompanyId { get; set; }
        public string CardOrAccountNumber { get; set; }
        public string AccountsPaymentMethod { get; set; }
        public string AccountsRoutingCode { get; set; } 
    }
}
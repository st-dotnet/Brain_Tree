using System.ComponentModel;

namespace VT.Data
{
    public enum ServiceRecordItemType
    {
        [Description("STANDARD")]
        Standard = 1,
        [Description("NON STANDARD")]
        NonStandard = 2
    }

    public enum ServiceRecordStatus
    {
        [Description("NOT PROCESSED")]
        NotProcessed = 1,

        [Description("PAID BY CC")]                         //PAID - BY CC ON FILE
        PaidByCcOnFile = 2,

        [Description("INVOICED")]                           //PAID - EXTERNAL
        PaidExternal = 3,

        [Description("MISSING PRICE")]                      //UNPAID - NON STANDARD SERVICE ONLY
        UnPaidNonStandardServiceOnly = 4,

        // We are not using these anymore

        [Description("UNPAID - BILLING ID MISSING ONLY")]
        UnPaidBillingIdMissingOnly = 5,

        [Description("UNPAID - BOTH")]
        UnPaidBoth = 6,

        [Description("PAYMENT FAILED")]                     //UNPAID - PAYMENT FAILED
        UnPaidPaymentFailed = 7
    }

    public enum CommissionType
    {
        [Description("Auto billed at time of service")]
        AutoBilledAtTimeOfService = 1,
        [Description("Auto billed by Organization Admin")]
        AutoBilledByOrgAdmin = 2,
        [Description("Manually Charged by Super Admin")]
        ManuallyBilledBySuperAdmin = 3
    }

    public enum PaymentGatewayType
    {
        [Description("Braintree")]
        Braintree = 1,
        [Description("Splash")]
        Splash = 2
    }

    public enum PaymentMethod
    {
        [Description("American Express")]
        AmericanExpress = 1,
        [Description("Visa")]
        Visa = 2,
        [Description("Master Card ")]
        MasterCard = 3,
        [Description("Diners Club")]
        DinersClub = 4,
        [Description("Discover")]
        Discover = 5
    }
}

using System.ComponentModel;

namespace VT.Common
{
    public enum UserRoles
    {
        [Description("Super Admin")]
        SuperAdmin,

        [Description("Company Admin")]
        CompanyAdmin,

        [Description("Company User")]
        CompanyUser
    }

    public enum ContactTypes
    {
        Office,
        Resident,
    }
    
    public enum ExportStatus
    {
        Valid,
        Invalid,
    }

    public enum GatewayAccount
    {
        Merchant,
        Customer
    }
    public enum AttachmentType
    {
        Before,
        After
    }

    public enum EmailType
    {
        Invoice,
        WorkOrder
    }
}

using System.ComponentModel;

namespace SmartWr.Ipos.Core.Enums
{
    public enum AuditType : int
    {
        [Description("PRODUCT EDIT")]
        EDIT_PRODUCT,
        [Description("CATEGORY EDIT")]
        EDIT_CATEGORY,
        [Description("NEW PRODUCT")]
        NEW_PRODUCT,
        [Description("NEW WASTE")]
        NEW_WASTE,
        [Description("WASTE EDIT")]
        EDIT_WASTE,
        [Description("BANK SETTLEMENT")]
        BANK_SETTLEMENT,
        [Description("NEW CATEGORY")]
        NEW_CATEGORY,
        [Description("NEW ACCOUNT")]
        NEW_ACCOUNT,
        [Description("ACCOUNT EDIT")]
        EDIT_ACCOUNT
    }
}

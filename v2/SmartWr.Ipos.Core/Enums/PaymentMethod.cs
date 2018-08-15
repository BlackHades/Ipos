using System.ComponentModel;
namespace SmartWr.Ipos.Core.Enums
{
    public enum PaymentMethod
    {
        CASH = 1,
        
        [Description("POS CARD")]
        POS_CARD = 2,
        
        CHEQUE = 3,

        [Description("ONLINE TRANSFER")]
        ONLINE_TRANSFER
    }
}
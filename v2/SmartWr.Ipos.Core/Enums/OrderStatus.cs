using System.ComponentModel;

namespace SmartWr.Ipos.Core.Enums
{
    public enum OrderStatus
    {
        OPEN = 0,
        CLOSED = 1,
        [Description("NEW OPEN")]
        NEW_OPEN = 2,
        RECALL = 3,
        POST = 4
    }
}
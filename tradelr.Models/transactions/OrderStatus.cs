using System.ComponentModel;

namespace tradelr.Models.transactions
{
    public enum OrderStatus
    {
        [Description("saved")]
        DRAFT = 0,
        [Description("sent / outstanding")]
        SENT = 1,
        [Description("viewed / outstanding")]
        VIEWED,
        [Description("partial")]
        PARTIAL,
        [Description("paid")]
        PAID,
        [Description("shipped")]
        SHIPPED
    }
}

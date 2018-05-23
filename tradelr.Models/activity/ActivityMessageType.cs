namespace tradelr.Models.activity
{
    public class ActivityMessageType : IActivityMessageType
    {
        public static readonly ActivityMessageType AUTOPOST_GOOGLEBLOG_FAIL =
            new ActivityMessageType(
                "Product Autopost to Blogger failed: {0} (<a href='/dashboard/networks'>check network permissions</a>)",
                ActivityType.PRODUCT, true);
        public static readonly ActivityMessageType AUTOPOST_GBASE_FAIL =
            new ActivityMessageType(
                "Product Autopost to Google Merchant Center failed: {0} (<a href='/dashboard/networks'>check network permissions</a>)",
                ActivityType.PRODUCT, true);
        public static readonly ActivityMessageType AUTOPOST_TUMBLR_FAIL =
            new ActivityMessageType(
                "Product Autopost to Tumblr failed: {0} (<a href='/dashboard/networks'>check network permissions</a>)",
                ActivityType.PRODUCT, true);
        public static readonly ActivityMessageType AUTOPOST_WORDPRESS_FAIL =
            new ActivityMessageType(
                "Product Autopost to Wordpress failed: {0} (<a href='/dashboard/networks'>check network permissions</a>)",
                ActivityType.PRODUCT, true);
        public static readonly ActivityMessageType ORDER_NEW =
            new ActivityMessageType("New purchase order: #{0}", ActivityType.ORDER, false);
        public static readonly ActivityMessageType ORDER_RECEIVED = 
            new ActivityMessageType("New order received: #{0} from {1}", ActivityType.ORDER, false);
        public static readonly ActivityMessageType ORDER_SENT =
            new ActivityMessageType("Order sent: #{0}", ActivityType.ORDER, false);
        public static readonly ActivityMessageType ORDER_VIEWED =
            new ActivityMessageType("Order viewed: #{0} by {1}", ActivityType.ORDER, false);
        public static readonly ActivityMessageType ORDER_UPDATED =
            new ActivityMessageType("Order updated: #{0}", ActivityType.ORDER, true);
        public static readonly ActivityMessageType ORDER_SHIPPED =
            new ActivityMessageType("Order shipped: #{0}", ActivityType.ORDER, true);
        public static readonly ActivityMessageType INVOICE_NEW =
            new ActivityMessageType("New invoice: #{0}", ActivityType.INVOICE, false);
        public static readonly ActivityMessageType INVOICE_RECEIVED =
            new ActivityMessageType("New invoice received: #{0} from {1}", ActivityType.INVOICE, false);
        public static readonly ActivityMessageType INVOICE_SENT =
            new ActivityMessageType("Invoice sent: #{0}", ActivityType.INVOICE, false);
        public static readonly ActivityMessageType INVOICE_VIEWED =
            new ActivityMessageType("Invoice viewed: #{0} by {1}", ActivityType.INVOICE, false);
        public static readonly ActivityMessageType INVOICE_UPDATED =
            new ActivityMessageType("Invoice updated: #{0}", ActivityType.INVOICE, true);
        public static readonly ActivityMessageType INVOICE_PAYMENT_RECEIVED_FULL =
            new ActivityMessageType("Payment(full) received for Invoice #{0}", ActivityType.INVOICE, true);
        public static readonly ActivityMessageType INVOICE_PAYMENT_RECEIVED_PARTIAL =
            new ActivityMessageType("Payment(partial) received for Invoice #{0}", ActivityType.INVOICE, true);
        public static readonly ActivityMessageType INVOICE_PAYMENT_ACCEPTED =
            new ActivityMessageType("Payment for Invoice #{0} accepted", ActivityType.INVOICE, true);
        public static readonly ActivityMessageType INVOICE_PAYMENT_REJECTED =
            new ActivityMessageType("Payment for Invoice #{0} rejected", ActivityType.INVOICE, true);
        public static readonly ActivityMessageType PRODUCT_ALARM =
            new ActivityMessageType("Inventory low({0}): {1}", ActivityType.PRODUCT, true);
        public static readonly ActivityMessageType PRODUCT_NEW =
            new ActivityMessageType("New product: {0}", ActivityType.PRODUCT, false);
        public static readonly ActivityMessageType PRODUCT_UPDATED =
            new ActivityMessageType("Product updated: {0}", ActivityType.PRODUCT, true);
        public static readonly ActivityMessageType PROFILE_UPDATED =
            new ActivityMessageType("Profile updated: {0}", ActivityType.PROFILE, true);
        public static readonly ActivityMessageType INVENTORY_UPDATED =
            new ActivityMessageType("Inventory updated: #{0}", ActivityType.INVENTORY, true);
        public static readonly ActivityMessageType INVENTORY_ALARM =
            new ActivityMessageType("Inventory out of stock: #{0}", ActivityType.INVENTORY, true);
        public static readonly ActivityMessageType NETWORK_NEW =
            new ActivityMessageType("Request to link network accepted by {0}", ActivityType.CONTACT, false);
        public static readonly ActivityMessageType CONTACT_NEW =
            new ActivityMessageType("New contact: {0}", ActivityType.CONTACT, false);
        public static readonly ActivityMessageType CONTACT_UPDATED =
            new ActivityMessageType("Contact updated: {0}", ActivityType.CONTACT, true);

        public ActivityType activityType { get; private set; }
        
        private ActivityMessageType(string msg, ActivityType type, bool multiples)
            :base(msg,multiples)
        {
            activityType = type;
        }

        public override string GetMessageType()
        {
            return activityType.ToString();
        }
    }
}
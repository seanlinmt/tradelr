namespace tradelr.Models.history
{
    public enum ChangeHistoryType
    {
        PRODUCT,
        CONTACT,
        INVOICE,
        ORDERS
    }

    public class ChangeHistory
    {
        public string documentType { get; set;}
        public string documentName { get; set; }
        public string documentLoc { get; set;}        
    }

    public static class ChangeHistoryHelper
    {
        public static string ToDocumentType(this ChangeHistoryType type)
        {
            switch (type)
            {
                case ChangeHistoryType.PRODUCT:
                    return "Product";
                case ChangeHistoryType.CONTACT:
                    return "Contact";
                case ChangeHistoryType.INVOICE:
                    return "Invoice";
                case ChangeHistoryType.ORDERS:
                    return "Purchase Order";
                default:
                    return "";
            }
        }

        public static string ToFieldDisplay(this string fieldname)
        {
            switch (fieldname)
            {
                case "comments":
                    return "Terms";
                case "invoice":
                    return "Invoice";
                case "order":
                case "orders":
                    return "Purchase Order";
                case "shippingCost":
                    return "Shipping Cost";
                default:
                    return fieldname;
            }
        }
    }
}

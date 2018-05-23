using tradelr.DBML;

namespace tradelr.Models.contacts
{
    public class ContactFilterName
    {
        public const string SUPPLIER_FILTERNAME = "Suppliers";
        public const string CUSTOMER_FILTERNAME = "Customers";
        public long id { get; set; }
        public string title { get; set; }
    }

    public static class ContactFilterNameHelper
    {
        public static ContactFilterName ToModel(this contactGroup value)
        {
            return new ContactFilterName()
                       {
                           id = value.id,
                           title = value.title
                       };
        }
    }
}

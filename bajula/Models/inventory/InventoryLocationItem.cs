using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DataAccess;
using tradelr.DBML;

namespace tradelr.Models.inventory
{
    public class InventoryLocationItem
    {
        public long id { get; set; }
        public long variantID { get; set; }
        public long locationID { get; set; }
        public string inventorySKU { get; set; }
        public int? inStock { get; set; }
        public int? onOrder { get; set; }
        public int? alarmLevel { get; set; }
        public int? reserved { get; set; }
        public int? sold { get; set; }
    }

    public static class InventoryLocationItemHelper
    {
        public static InventoryLocationItem ToModel(this inventoryLocationItem value)
        {
            return new InventoryLocationItem()
                       {
                           id = value.id,
                           variantID = value.variantid,
                           locationID = value.locationid,
                           inventorySKU = value.product_variant.sku,
                           inStock = value.available,
                           onOrder = value.onOrder,
                           alarmLevel = value.alarmLevel,
                           reserved = value.reserved,
                           sold = value.sold
                       };
        }
    }
}
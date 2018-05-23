using System.Collections.Generic;
using tradelr.DBML;

namespace tradelr.Models.offline.tables
{
    public class InventoryLocItemColumn : IColumn
    {
        public CFlag cflag { get; set; }
        public long? id { get; set; }
        public long? serverid { get; set; }

        public long locationid { get; set; }
        public int? inventoryLevel { get; set; }
        public long? productid { get; set; } // can be null because product is deleted before ilocitems on client side
        public int? onOrder { get; set; }
        public int? alarmLevel { get; set; }
    }

    public static class InventoryLocItemColhelper
    {
        public static InventoryLocItemColumn ToSyncModel(this inventoryLocationItem v, CFlag flag, long? offlineid = null)
        {
            return new InventoryLocItemColumn
            {
                cflag = flag,
                serverid = v.id,
                locationid = v.locationid,
                inventoryLevel = v.available,
                productid = v.variantid,
                onOrder = v.onOrder,
                alarmLevel = v.alarmLevel,
                id = offlineid
            };
        }

        public static List<InventoryLocItemColumn> ToSyncModel(this IEnumerable<inventoryLocationItem> values, CFlag flag, long? offlineid = null)
        {
            var result = new List<InventoryLocItemColumn>();
            foreach (var v in values)
            {
                result.Add(v.ToSyncModel(flag, offlineid));
            }
            return result;
        }
    }
}
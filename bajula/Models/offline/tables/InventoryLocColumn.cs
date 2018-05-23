using System.Collections.Generic;
using tradelr.DBML;

namespace tradelr.Models.offline.tables
{
    public class InventoryLocColumn : IColumn
    {
        public CFlag cflag { get; set; }
        public long? id { get; set; }
        public long? serverid { get; set; }
        public string name { get; set; }
    }

    public static class InventoryLocColhelper
    {
        public static InventoryLocColumn ToSyncModel(this inventoryLocation v, CFlag flag, long? offlineid = null)
        {
            return new InventoryLocColumn
            {
                cflag = flag,
                serverid = v.id,
                name = v.name,
                id = offlineid
            };
        }

        public static List<InventoryLocColumn> ToSyncModel(this IEnumerable<inventoryLocation> values, CFlag flag, long? offlineid = null)
        {
            var result = new List<InventoryLocColumn>();
            foreach (var v in values)
            {
                result.Add(v.ToSyncModel(flag, offlineid));
            }
            return result;
        }
    }
}
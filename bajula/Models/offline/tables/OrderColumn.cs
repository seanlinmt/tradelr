using System.Collections.Generic;
using tradelr.DBML;

namespace tradelr.Models.offline.tables
{
    public class OrderColumn : IColumn
    {
        public CFlag cflag { get; set; }
        public long? id { get; set; }
        public long? serverid { get; set; }
    }

    public static class OrderColhelper
    {
        public static OrderColumn ToSyncModel(this order v, CFlag flag, long? offlineid = null)
        {
            return new OrderColumn();
        }

        public static List<OrderColumn> ToSyncModel(this IEnumerable<order> values, CFlag flag, long? offlineid = null)
        {
            var result = new List<OrderColumn>();
            foreach (var v in values)
            {
                result.Add(v.ToSyncModel(flag, offlineid));
            }
            return result;
        }
    }
}
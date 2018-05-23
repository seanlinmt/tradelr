using System.Collections.Generic;
using tradelr.DBML;

namespace tradelr.Models.offline.tables
{
    public class StockUnitColumn : IColumn
    {
        public CFlag cflag { get; set; }
        public long? id { get; set; }
        public long? serverid { get; set; }

        public string name { get; set; }
    }

    public static class StockUnitColhelper
    {
        public static StockUnitColumn ToSyncModel(this stockUnit v, CFlag flag, long? offlineid = null)
        {
            return new StockUnitColumn()
                       {
                           cflag = flag,
                           serverid = v.id,
                           name = v.MASTERstockUnit.name,
                           id = offlineid
                       };
        }

        public static List<StockUnitColumn> ToSyncModel(this IEnumerable<stockUnit> values, CFlag flag, long? offlineid = null)
        {
            var result = new List<StockUnitColumn>();
            foreach (var v in values)
            {
                result.Add(v.ToSyncModel(flag, offlineid));
            }
            return result;
        }
    }
}
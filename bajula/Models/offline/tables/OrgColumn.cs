using System.Collections.Generic;
using tradelr.DBML;

namespace tradelr.Models.offline.tables
{
    public class OrgColumn : IColumn
    {
        public CFlag cflag { get; set; }
        public long? id { get; set; }
        public long? serverid { get; set; }
        
    }

    public static class OrgColhelper
    {
        public static OrgColumn ToSyncModel(this organisation v, CFlag flag, long? offlineid = null)
        {
            return new OrgColumn();
        }

        public static List<OrgColumn> ToSyncModel(this IEnumerable<organisation> values, CFlag flag, long? offlineid = null)
        {
            var result = new List<OrgColumn>();
            foreach (var v in values)
            {
                result.Add(v.ToSyncModel(flag, offlineid));
            }
            return result;
        }
    }
}
using System.Collections.Generic;
using tradelr.DBML;

namespace tradelr.Models.offline.tables
{
    public class UserColumn : IColumn
    {
        public CFlag cflag { get; set; }
        public long? id { get; set; }
        public long? serverid { get; set; }
        
    }

    public static class UserColhelper
    {
        public static UserColumn ToSyncModel(this user v, CFlag flag, long? offlineid = null)
        {
            return new UserColumn();
        }

        public static List<UserColumn> ToSyncModel(this IEnumerable<user> values, CFlag flag, long? offlineid = null)
        {
            var result = new List<UserColumn>();
            foreach (var v in values)
            {
                result.Add(v.ToSyncModel(flag, offlineid));
            }
            return result;
        }
    }
}
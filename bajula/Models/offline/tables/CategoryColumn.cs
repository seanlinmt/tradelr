using System;
using System.Collections.Generic;
using tradelr.DBML;

namespace tradelr.Models.offline.tables
{
    public class CategoryColumn : IColumn
    {
        public string name { get; set; }
        public long? parentid { get; set; }
        public CFlag cflag { get; set; }
        public long? id { get; set; }
        public long? serverid { get; set; }
    }

    public static class CategoryColhelper
    {
        public static CategoryColumn ToSyncModel(this productCategory v, CFlag flag, long? offlineid = null)
        {
            return new CategoryColumn
                                 {
                                     cflag = flag,
                                     id = offlineid,
                                     name = v.MASTERproductCategory.name,
                                     parentid = v.parentID,
                                     serverid = v.id
                                 };
        }

        public static List<CategoryColumn> ToSyncModel(this IEnumerable<productCategory> values, CFlag flag, long? offlineid = null)
        {
            var result = new List<CategoryColumn>();
            foreach (var v in values)
            {
                result.Add(v.ToSyncModel(flag, offlineid));
            }
            return result;
        }
    }
}
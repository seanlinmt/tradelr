using System;
using System.Collections.Generic;
using tradelr.Common.Models.currency;
using tradelr.DBML;

namespace tradelr.Models.offline.tables
{
    public class SettingsColumn : IColumn
    {
        public string currency { get; set; }
        public int flags { get; set; }

        public CFlag cflag { get; set; }
        public long? id { get; set; }
        public long? serverid { get; set; }
    }

    public static class SettingsColhelper
    {
        public static List<SettingsColumn> ToSyncModel(this MASTERsubdomain v, CFlag flag)
        {
            return new List<SettingsColumn>
                       {
                           new SettingsColumn
                               {
                                   cflag = flag,
                                   id = v.id,
                                   currency = v.organisation.MASTERsubdomain.currency.ToCurrencySymbol(),
                                   flags = v.flags,
                                   serverid = v.id  // required so we don't keep creating more rows in offlne db
                               }
                       };
        }
    }
}
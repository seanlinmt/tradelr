using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.offline.tables
{
    public interface IColumn
    {
        CFlag cflag { get; set; }
        long? id { get; set; }
        long? serverid { get; set; }
    }
}
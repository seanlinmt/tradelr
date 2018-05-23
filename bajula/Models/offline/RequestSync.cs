using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.offline
{
    /// <summary>
    /// this is on a per table basis
    /// </summary>
    public class RequestSync
    {
        public TableName type { get; set; }
        public string data { get; set; }
    }
}
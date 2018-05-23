using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Libraries
{
    public class FilterBoxListInfo
    {
        public string id { get; set; }
        public string title { get; set; }
        public bool isSub { get; set; }
        public string details { get; set; }

        // additional fields
        public bool allowDelete { get; set; }
    }
}

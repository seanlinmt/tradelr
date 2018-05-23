using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.yahoo
{
    public class Field
    {
        public string uri { get; set; }
        public string created { get; set; }
        public string updated { get; set; }
        public string id { get; set; }
        public FieldType type { get; set; }
        public dynamic value { get; set; }
    }
}
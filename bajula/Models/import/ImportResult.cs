using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.import
{
    public class ImportResult
    {
        public string id { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
    }
}
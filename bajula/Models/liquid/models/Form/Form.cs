using System.Collections.Generic;
using DotLiquid;

namespace tradelr.Models.liquid.models.Form
{
    public class Form : Drop
    {
        public bool posted_successfully { get; set; }
        public FormErrors errors { get; set; }
        public string email { get; set; }
        public string body { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
    }
}
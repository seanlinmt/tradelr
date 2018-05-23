using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ColourLovers
{
    public class Pattern
    {
        public long id { get; set; }
        public string title { get; set; }
        public long rank { get; set; }
        public string imageUrl { get; set; }
        public Colours colors { get; set; }
    }
}

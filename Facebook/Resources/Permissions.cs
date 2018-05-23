using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clearpixels.Facebook.Resources
{
    public class Permissions
    {
        public byte installed { get; set; }
        public byte email { get; set; }
        public byte bookmarked { get; set; }
        public byte read_stream { get; set; }
        public byte publish_stream { get; set; }
        public byte manage_pages { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Libraries.Loader
{
    public class LoadedContent
    {
        public string content { get; set; }
        public List<string> filenames { get; set; }

        public LoadedContent()
        {
            filenames = new List<string>();
        }
    }
}

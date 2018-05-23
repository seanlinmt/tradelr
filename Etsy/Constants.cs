using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Etsy
{
    public static class Constants
    {
#if DEBUG
        // auth information
        public const string ApplicationKey = "zfxhdk3xrvj5pr7s3q6rh724";
        public const string ApplicationSecret = "AV3JvXZevJQb";
#else
        public const string ApplicationKey = "zfxhdk3xrvj5pr7s3q6rh724";
        public const string ApplicationSecret = "AV3JvXZevJQb";
#endif
        public const string BaseUrl = "http://openapi.etsy.com/v2/sandbox/";
        // 100 entries
        public const int LIMIT_ENTRIES = 100;
        
    }
}

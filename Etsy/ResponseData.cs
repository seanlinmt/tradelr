using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Etsy
{
    public class ResponseData<T>
    {
        public int count { get; set; }
        public List<T> results { get; set; }

        [DataMember(Name = "params")]
        public Dictionary<string, string> parameters { get; set; }

        public string type { get; set; }
    }
}

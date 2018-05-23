using System.Collections.Generic;

namespace clearpixels.Facebook
{
    public class ResponseCollection<T>
    {
        public List<T> data { get; set; }
        public Paging paging { get; set; }
        public int count { get; set; }
    }
}

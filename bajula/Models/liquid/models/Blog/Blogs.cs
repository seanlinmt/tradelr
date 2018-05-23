using System.Collections.Generic;
using System.Linq;
using DotLiquid;
using tradelr.DBML;

namespace tradelr.Models.liquid.models.Blog
{
    public class Blogs : Drop
    {
        public IEnumerable<Blog> all { get; set; }

        public override object BeforeMethod(string method)
        {
            method = method.Trim();
            return all.Where(x => string.Compare(x.handle, method, true) == 0).SingleOrDefault(); 
        }

        public Blogs(MASTERsubdomain sd)
        {
            all = sd.blogs.Select(x => new Blog(x));
        }
    }
}
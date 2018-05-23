using System.Collections.Generic;

namespace tradelr.OpenSocial.Models
{
    public class Gallery
    {
        public IEnumerable<Product> products { get; set; }
        public string viewAllUrl { get; set; }
    }
}
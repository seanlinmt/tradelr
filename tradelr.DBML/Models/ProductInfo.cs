using System.Collections.Generic;
using System.Linq;

namespace tradelr.DBML.Models
{
    public class ProductInfo
    {
        public product p { get; set; }
        public List<string> photo_urls { get; private set; }

        public ProductInfo()
        {
            photo_urls = new List<string>();
        }

        public void AddPhotoUrl(string url)
        {
            photo_urls.Add(url);
        }
    }
}
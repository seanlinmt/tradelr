using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.Constants;

namespace tradelr.Models.products
{
    public class ProductDigital
    {
        public string id { get; set; }
        public string url { get; set; }
        public string name { get; set; }

        public int downloadCount { get; set; }
        public string limit { get; set; }

        public DateTime? expiry { get; set; }
        public string linkid { get; set; }
    }

    public static class ProductDigitalHelper
    {
        public static ProductDigital ToModel(this products_digital row)
        {
            if (row == null)
            {
                return new ProductDigital();
            }

            return new ProductDigital()
                       {
                           expiry = row.expiryDate,
                           id = row.id.ToString(),
                           url = row.filepath,
                           name = row.filename,
                           limit = row.limit.ToString(),
                           linkid = row.linkid
                       };
        }
    }
}
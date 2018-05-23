using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DataAccess;
using tradelr.DBML;

namespace tradelr.Models.products
{
    public class ProductGroup
    {
        public product product { get; set; }
        public int count { get; set; }
    }
}
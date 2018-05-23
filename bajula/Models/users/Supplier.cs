using System;
using System.Collections.Generic;
using System.Web.Mvc;
using tradelr.DataAccess;
using tradelr.Models.jqgrid;

namespace tradelr.Models.users
{
    public class Supplier
    {
        public long id { get; set; } // supplierid
        public string name { get; set; }
        public SelectList supplierList { get; set; }
    }


    public static class SupplierHelper
    {
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Common.Models.currency;
using tradelr.DBML;
using tradelr.DBML.Models;
using tradelr.Models.products;
using tradelr.Models.transactions;

namespace tradelr.Areas.dashboard.Models.product
{
    public class ContactTransaction
    {
        public string productName { get; set; }
        public string productediturl { get; set; }
        public string unitPrice { get; set; }
    }
}
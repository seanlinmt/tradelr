using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Areas.dashboard.Models.product
{
    public class ContactTransactionsViewModel
    {
        public IEnumerable<ContactTransaction> products_sold { get; set; }
        public IEnumerable<ContactTransaction> products_bought { get; set; } 
    }
}
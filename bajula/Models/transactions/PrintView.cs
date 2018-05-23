using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using tradelr.DataAccess;
using tradelr.Models.comments;

namespace tradelr.Models.transactions
{
    public class PrintView : BaseViewModel
    {
        public PrintView(BaseViewModel viewmodel) : base(viewmodel)
        {
        }

        public long transactionID { get; set; }
        public OrderView primaryView { get; set; }
        public OrderView secondaryView { get; set; }
    }
}

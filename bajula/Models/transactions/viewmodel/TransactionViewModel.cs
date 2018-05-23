using System.Collections.Generic;
using tradelr.Libraries;

namespace tradelr.Models.transactions.viewmodel
{
    public class TransactionViewModel : BaseViewModel
    {
        public TransactionViewModel(BaseViewModel viewmodel) : base(viewmodel)
        {
        }

        public IEnumerable<FilterBoxListInfo> timeline { get; set; }
        public IEnumerable<FilterBoxListInfo> statuses { get; set; } 
    }
}
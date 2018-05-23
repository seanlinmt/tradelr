using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using tradelr.DBML;
using tradelr.Models.address;
using tradelr.Models.history;

namespace tradelr.Models.transactions.viewmodel
{
    public class OrderViewModel : BaseViewModel
    {
        public Order o { get; private set; }
        public ContactAddressesViewModel Addresses { get; set; }

        // change history
        public IEnumerable<ChangeHistoryItem> ChangeItems { get; set; }

        public SelectList ContactList { get; set; }
        public SelectList ContactTypes { get; set; } 
        
        public bool LimitHit { get; set; }

        // currency
        public IEnumerable<SelectListItem> currencyList { get; set; } 
        public string CurrencyInfo { get; set; }

        // location
        public IEnumerable<SelectListItem> locationList { get; set; } 

        // still needed for new empty orders
        public OrderViewModel(BaseViewModel viewmodel)
            : base(viewmodel)
        {
            if (o == null)
            {
                o = new Order();
            }
            ChangeItems = Enumerable.Empty<ChangeHistoryItem>();
            Addresses = new ContactAddressesViewModel();
        }
        public OrderViewModel(BaseViewModel viewmodel, order dborder, long sessionid, TransactionType type)
            : this(viewmodel)
        {
            o = dborder.ToModel(type, sessionid);
        }
    }
}
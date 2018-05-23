using System.Collections.Generic;
using System.Web.Mvc;
using tradelr.Areas.dashboard.Models.product;
using tradelr.Models;
using tradelr.Models.address;
using tradelr.Models.comments;
using tradelr.Models.contacts;

namespace tradelr.Areas.dashboard.Models.contact
{
    public class ContactViewModel : BaseViewModel
    {
        public Contact contact { get; set; }
        public bool editMode { get; set; }
        public ContactAddressesViewModel addresses { get; set; }
        public ContactTransactionsViewModel productTransactions { get; set; }

        public SelectList organisationList { get; set; }

        public List<OrderComment> comments { get; set; }

        public ContactViewModel(BaseViewModel viewmodel)
            : base(viewmodel)
        {
            comments = new List<OrderComment>();
            addresses = new ContactAddressesViewModel();
        }

    }
}
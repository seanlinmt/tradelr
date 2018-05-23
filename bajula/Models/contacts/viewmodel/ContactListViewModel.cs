using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using tradelr.DBML;
using tradelr.Libraries;
using tradelr.Libraries.Extensions;

namespace tradelr.Models.contacts.viewmodel
{
    public class ContactListViewModel : BaseViewModel
    {
        public ContactListViewModel(BaseViewModel viewmodel) : base(viewmodel)
        {
        }

        public IEnumerable<FilterBoxListInfo> contactGroups { get; set; }
        public IEnumerable<FilterBoxListInfo> contactTypes { get; set; }

        public void PopulateContactGroups(ITradelrRepository repository, long subdomainid)
        {
            contactGroups =
                repository.GetContactGroups(subdomainid).Select(
                    x => new SelectListItem {Text = x.title, Value = x.id.ToString()}).
                    ToFilterList();
        }
    }
}
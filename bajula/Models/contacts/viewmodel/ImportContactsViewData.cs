using System.Collections.Generic;

namespace tradelr.Models.contacts.viewmodel
{
    public class ImportContactsViewData : BaseViewModel
    {
        public ImportContactsViewData(BaseViewModel viewmodel) : base(viewmodel)
        {
        }

        public string hostName { get; set; }
        public long subdomainid { get; set; }
        public long appid { get; set; }

        // facebook
        public string fbuid { get; set; }
        public string invitedFbuidList { get; set; }

        // callback data
        public IEnumerable<ContactBasic> contacts { get; set; }
    }
}
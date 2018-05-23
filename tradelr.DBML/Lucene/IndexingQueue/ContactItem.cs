using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tradelr.DBML.Helper;
using tradelr.Library;

namespace tradelr.DBML.Lucene.IndexingQueue
{
    [Serializable]
    public class ContactItem : BaseQueueItem, IQueueItem
    {
        public string email { get; set; }
        public string name { get; set; }
        public string fullname { get; set; }
        public string orgname { get; set; }
        public string notes { get; set; }


        public ContactItem()
        {
            
        }

        public ContactItem(string id) : base(id, LuceneIndexType.CONTACTS)
        {
            
        }

        public ContactItem(user u) : this(u.id.ToString())
        {
            email = Utility.EmptyIfNull(u.email).ToLower();
            name = u.ToFullName().ToLower();
            fullname = u.ToFullName().ToLower();
            orgname = u.organisation.HasValue ? Utility.EmptyIfNull(u.organisation1.name).ToLower() : "";
            notes = Utility.EmptyIfNull(u.notes);
        }
    }
}

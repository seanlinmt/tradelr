using System;
using System.Linq;
using tradelr.Library.Caching;
using tradelr.Models.activity;
using tradelr.Models.users;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void AddWordpress(wordpressSite wordpress)
        {
            db.wordpressSites.InsertOnSubmit(wordpress);
        }

        public void DeleteWordpress(wordpressSite wordpress)
        {
            db.wordpressSites.DeleteOnSubmit(wordpress);
            db.SubmitChanges();
        }

        public void AddWordpressPost(wordpressPost wordpressPost)
        {
            db.wordpressPosts.InsertOnSubmit(wordpressPost);
        }
    }
}
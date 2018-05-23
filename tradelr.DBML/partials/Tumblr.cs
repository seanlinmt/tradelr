using System;
using System.Linq;
using tradelr.Library.Caching;
using tradelr.Models.activity;
using tradelr.Models.users;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void AddTumblr(tumblrSite tumblr)
        {
            db.tumblrSites.InsertOnSubmit(tumblr);
        }

        public void DeleteTumblr(tumblrSite tumblr)
        {
            db.tumblrSites.DeleteOnSubmit(tumblr);
            db.SubmitChanges();
        }

        public void AddTumblrPost(tumblrPost post)
        {
            db.tumblrPosts.InsertOnSubmit(post);
        }
    }
}
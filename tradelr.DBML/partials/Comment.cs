using System.Linq;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void AddComment(product_comment comment)
        {
            db.product_comments.InsertOnSubmit(comment);
            db.SubmitChanges();
        }

        public product_comment GetComment(long parentid, long subdomainid)
        {
            return db.product_comments.Where(x => x.id == parentid && x.subdomainid == subdomainid).SingleOrDefault();
        }
    }
}
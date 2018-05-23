using System.Linq;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public review GetReview(long id)
        {
            return db.reviews.Where(x => x.id == id).SingleOrDefault();
        }

        public void AddReview(review fb)
        {
            db.reviews.InsertOnSubmit(fb);
            db.SubmitChanges();
        }
    }
}
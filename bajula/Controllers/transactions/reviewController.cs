using System.Web.Mvc;

using tradelr.Libraries;
using tradelr.Libraries.ActionFilters;
using tradelr.Library.Caching;
using tradelr.Library.JSON;
using tradelr.Models.users;

namespace tradelr.Controllers.transactions
{
    //[ElmahHandleError]
    public class reviewController : baseController
    {
        [HttpPost]
        [RoleFilter(role = UserRole.USER)]
        public ActionResult Create(long? reviewid, string comment,  byte overall, byte willshopagain, byte delivery, byte support)
        {
            if (!reviewid.HasValue)
            {
                return SendJsonErrorResponse("Could not locate transaction");
            }

            var review = repository.GetReview(reviewid.Value);
            if (review == null)
            {
                return SendJsonErrorResponse("Could not locate transaction " + reviewid.Value);
            }

            review.rating_overall = overall;
            review.rating_willshopagain = willshopagain;
            review.rating_delivery = delivery;
            review.rating_support = support;
            review.comment = comment;
            review.pending = false;

            db.SubmitChanges();
            CacheHelper.Instance.invalidate_dependency(DependencyType.organisation, subdomainid.Value.ToString());

            return Json("".ToJsonOKMessage());
        }
    }
}

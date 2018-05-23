using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tradelr.Areas.dashboard.Models.store.blog;
using tradelr.Controllers;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Helpers;
using tradelr.Library;
using tradelr.Library.JSON;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    [PermissionFilter(permission = UserPermission.NETWORK_STORE)]
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class commentsController : baseController
    {
        [HttpPost]
        public ActionResult Approve(long id)
        {
            var comment =
                MASTERdomain.blogs
                .SelectMany(x => x.articles)
                .SelectMany(y => y.article_comments)
                .Where(y => y.id == id)
                .SingleOrDefault();

            if (comment == null)
            {
                return Json("Comment not found".ToJsonFail());
            }

            comment.isApproved = true;
            comment.isSpam = false;
            repository.Save();
            var row = this.RenderViewToString("articleCommentTableRow", comment.ToModel());
            var msg = "Comment approved".ToJsonOKMessage();
            msg.data = row;
            return Json(msg);
        }

        [HttpPost]
        public ActionResult NotSpam(long id)
        {
            var comment =
                MASTERdomain.blogs
                .SelectMany(x => x.articles)
                .SelectMany(y => y.article_comments)
                .Where(y => y.id == id)
                .SingleOrDefault();

            if (comment == null)
            {
                return Json("Comment not found".ToJsonFail());
            }

            comment.isSpam = false;
            repository.Save();
            var row = this.RenderViewToString("articleCommentTableRow", comment.ToModel());
            var msg = "Comment marked as not spam".ToJsonOKMessage();
            msg.data = row;
            return Json(msg);
        }

        [HttpPost]
        public ActionResult Delete(long id)
        {
            var comment =
                MASTERdomain.blogs
                .SelectMany(x => x.articles)
                .SelectMany(y => y.article_comments)
                .Where(y => y.id == id)
                .SingleOrDefault();

            if (comment == null)
            {
                return Json("Comment not found".ToJsonFail());
            }

            db.article_comments.DeleteOnSubmit(comment);
            repository.Save();
            var row = this.RenderViewToString("articleCommentTableRow", comment.ToModel());
            var msg = "Comment deleted".ToJsonOKMessage();
            msg.data = row;
            return Json(msg);
        }

        [HttpPost]
        public ActionResult Spam(long id)
        {
            var comment =
                MASTERdomain.blogs
                .SelectMany(x => x.articles)
                .SelectMany(y => y.article_comments)
                .Where(y => y.id == id)
                .SingleOrDefault();

            if (comment == null)
            {
                return Json("Comment not found".ToJsonFail());
            }

            comment.isApproved = true;
            comment.isSpam = true;
            repository.Save();
            var row = this.RenderViewToString("articleCommentTableRow", comment.ToModel());
            var msg = "Comment marked as spam".ToJsonOKMessage();
            msg.data = row;
            return Json(msg);
        }
    }
}

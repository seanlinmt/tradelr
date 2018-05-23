using System;
using System.Collections.Generic;
using tradelr.Common;
using tradelr.Common.Constants;
using tradelr.Common.Library.Imaging;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.Constants;
using clearpixels.Logging;
using tradelr.Models.products;
using tradelr.Models.transactions;
using tradelr.Models.users;

namespace tradelr.Models.comments
{
    public class OrderComment
    {
        public const string ORDER_SHIP_DETAILED =
            "Order has been shipped.\r\n Shipping Service: {0}\r\n Tracking Number: {1}\r\n Tracking Information: {2}";
        public const string ORDER_SHIP_STANDARD =
            "Order has been shipped.\r\n Shipping Service: {0}\r\n Tracking Number: {1}";

        public const string ORDER_SHIP = "Order has been shipped.";

        public const string SHIPPING_WAIT_FOR_COST = "Waiting for shipping cost to be updated";

        public long id { get; set; }
        public long? parentid { get; set; }
        public string profileLink { get; set; }
        public string creatorLink { get; set; }
        public string creatorName { get; set; }
        public string comment { get; set; }
        public string dateCreated { get; set; }
        public int leftmargin { get; set; }
        
        // the following is required for contact views
        public DateTime created { get; set; }
        public string contextLink { get; set; }
        public string contextName { get; set; }
        public CommentType type { get; set; }

        public bool hideReply { get; set; }
    }

    public static class CommentHelper
    {
        public static IEnumerable<OrderComment> ToContextualModel(this IEnumerable<product_comment> values)
        {
            foreach (var val in values)
            {
                yield return new OrderComment()
                                 {
                                     id = val.id,
                                     comment = val.comment.ToHtmlBreak(),
                                     dateCreated = val.created.ToLocalTime().ToString(GeneralConstants.DATEFORMAT_FULL),
                                     type = CommentType.PRODUCT,
                                     created = val.created,
                                     contextName = val.product.title,
                                     contextLink = val.product.ToProductUrl()
                                 };
            }
        }

        public static IEnumerable<OrderComment> ToContextualModel(this IEnumerable<comment> rows, long subdomainid)
        {
            foreach (var row in rows)
            {
                if (row.transaction.order.user1.organisation1.subdomain == subdomainid ||
                    (row.transaction.order.receiverUserid.HasValue && row.transaction.order.user.organisation1.subdomain == subdomainid))
                {
                    yield return new OrderComment()
                    {
                        id = row.id,
                        comment = row.comments.ToHtmlBreak(),
                        dateCreated = row.created.ToLocalTime().ToString(GeneralConstants.DATEFORMAT_FULL),
                        type = CommentType.TRANSACTION,
                        created = row.created,
                        contextName = row.transaction.ToTransactionName(),
                        contextLink = row.transaction.ToTransactionLink()
                    };
                }
            }
        }

        public static OrderComment ToModel(this comment val, bool hideReply)
        {
            return new OrderComment()
                       {
                           id = val.id,
                           comment = val.comments.ToHtmlBreak(),
                           creatorName = val.creator.HasValue? val.user.ToEmailName(true): "tradelr",
                           creatorLink = val.creator.HasValue? string.Format("/contacts/{0}", val.creator.Value):"",
                           dateCreated = val.created.ToLocalTime().ToString(GeneralConstants.DATEFORMAT_FULL),
                           profileLink = val.creator.HasValue? val.user.GetProfilePhoto(Imgsize.THUMB): "<img src=\"/Content/img/icon_50.png\" alt=\"\" />",
                           hideReply = hideReply
                       };
        }

        public static OrderComment ToProductModel(this product_comment val)
        {
            return new OrderComment()
            {
                id = val.id,
                parentid = val.replyto,
                comment = val.comment.ToHtmlBreak(),
                creatorName = val.user.ToEmailName(true),
                dateCreated = val.created.ToLocalTime().ToString(GeneralConstants.DATEFORMAT_FULL),
                profileLink = val.user.GetProfilePhoto()
            };
        }

        public static IEnumerable<OrderComment> ToModel(this IEnumerable<comment> values, bool hideReply)
        {
            foreach (var val in values)
            {
                yield return val.ToModel(hideReply);
            }
        }
    }
}

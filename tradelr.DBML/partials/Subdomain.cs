using System;
using System.Linq;
using tradelr.DBML.Lucene;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public MASTERsubdomain GetSubDomain(long subdomainid)
        {
            return db.MASTERsubdomains.Single(x => x.id == subdomainid);
        }

        public void DeleteSubdomain(long id)
        {
            var sd = GetSubDomain(id);
            string subdomainname = sd.name;

            db.facebook_tokens.DeleteAllOnSubmit(sd.facebook_tokens);
            db.SubmitChanges();
            db.facebookPages.DeleteAllOnSubmit(sd.facebookPages);
            db.SubmitChanges();
            db.friends.DeleteAllOnSubmit(sd.friends);
            db.SubmitChanges();
            db.googleBlogs.DeleteAllOnSubmit(sd.googleBlogs);
            db.SubmitChanges();
            db.images.DeleteAllOnSubmit(sd.images);
            db.SubmitChanges();

            var inventorylocitems = sd.inventoryLocations.SelectMany(x => x.inventoryLocationItems).AsQueryable();
            db.inventoryHistories.DeleteAllOnSubmit(inventorylocitems.SelectMany(x => x.inventoryHistories));
            db.inventoryLocationItems.DeleteAllOnSubmit(inventorylocitems);
            db.SubmitChanges();
            db.inventoryLocations.DeleteAllOnSubmit(sd.inventoryLocations);
            db.SubmitChanges();
            db.oauth_tokens.DeleteAllOnSubmit(sd.oauth_tokens);
            db.SubmitChanges();
            db.opensocialPages.DeleteAllOnSubmit(sd.opensocialPages);
            db.SubmitChanges();

            // payment methods
            db.paymentMethods.DeleteAllOnSubmit(sd.paymentMethods);
            
            // product collections
            db.productCollectionMembers.DeleteAllOnSubmit(sd.product_collections.SelectMany(x => x.productCollectionMembers));
            db.product_collections.DeleteAllOnSubmit(sd.product_collections);
            db.SubmitChanges();

            // order stuff
            var orders = sd.organisations.SelectMany(x => x.users.SelectMany(y => y.orders1)).AsQueryable();
            var transactions = orders.Select(x => x.transactions);

            db.payments.DeleteAllOnSubmit(orders.SelectMany(x => x.payments));

            // comments stuff
            db.comments.DeleteAllOnSubmit(transactions.SelectMany(x => x.comments));

            db.transactions.DeleteAllOnSubmit(transactions);

            db.orderItems.DeleteAllOnSubmit(orders.SelectMany(x => x.orderItems));

            db.cartitems.DeleteAllOnSubmit(sd.carts.SelectMany(x => x.cartitems));

            db.carts.DeleteAllOnSubmit(sd.carts);

            // ebay orders
            var ebayorders = sd.ebay_orders;
            if (ebayorders.Any())
            {
                foreach (var ebayorder in ebayorders)
                {
                    if (ebayorder != null && 
                        ebayorder.ebay_orderitems != null)
                    {
                        db.ebay_orderitems.DeleteAllOnSubmit(ebayorder.ebay_orderitems);
                    }
                }
                db.ebay_orders.DeleteAllOnSubmit(ebayorders);
            }


            db.orders.DeleteAllOnSubmit(orders);

            db.SubmitChanges();
            
            db.product_comments.DeleteAllOnSubmit(sd.product_comments);
            db.product_variants.DeleteAllOnSubmit(sd.products.SelectMany(x => x.product_variants));

            // remove tags
            db.tags.DeleteAllOnSubmit(sd.tags);

            db.SubmitChanges();

            foreach (var p in sd.products)
            {
                p.thumb = null;
                if (p.wordpressPosts != null)
                {
                    db.wordpressPosts.DeleteOnSubmit(p.wordpressPosts);
                }
            }

            db.SubmitChanges();

            db.product_images.DeleteAllOnSubmit(sd.product_images);

            
            db.SubmitChanges();


            if (sd.wordpressSites != null)
            {
                db.wordpressSites.DeleteOnSubmit(sd.wordpressSites);
            }

            

            db.SubmitChanges();

            db.products.DeleteAllOnSubmit(sd.products); // product delete

            db.SubmitChanges();
            
            db.productCategories.DeleteAllOnSubmit(sd.productCategories);
            
            db.reviews.DeleteAllOnSubmit(sd.reviews);
            db.shippingRules.DeleteAllOnSubmit(sd.shippingProfiles.SelectMany(x => x.shippingRules));
            db.shippingProfiles.DeleteAllOnSubmit(sd.shippingProfiles);
            db.tags.DeleteAllOnSubmit(sd.tags);
            db.twitterSearches.DeleteAllOnSubmit(sd.twitterSearches);
            db.videos.DeleteAllOnSubmit(sd.videos);
            db.linkRequests.DeleteAllOnSubmit(sd.organisations.SelectMany(x => x.users.SelectMany(y => y.linkRequests)));
            db.messages.DeleteAllOnSubmit(sd.organisations.SelectMany(x => x.users.SelectMany(y => y.messages)));
            

            var contactgroups = sd.contactGroups;

            db.contactGroupMembers.DeleteAllOnSubmit(contactgroups.SelectMany(x => x.contactGroupMembers));
            db.contactGroupPricings.DeleteAllOnSubmit(contactgroups.SelectMany(x => x.contactGroupPricings));
            db.contactGroups.DeleteAllOnSubmit(contactgroups);
            
            // user stuff
            var users = sd.organisations.SelectMany(x => x.users).AsQueryable();

            // now go through all orders and set orders where the receiver equals to users on this domain
            var userids = users.Select(x => x.id).ToArray();
            foreach (var o in db.orders)
            {
                if (!o.receiverUserid.HasValue)
                {
                    continue;
                }
                if (userids.Contains(o.receiverUserid.Value))
                {
                    o.receiverUserid = null;
                }
            }

            // liquid stuff
            db.pages.DeleteAllOnSubmit(users.SelectMany(x => x.pages));
            db.links.DeleteAllOnSubmit(sd.linklists.SelectMany(x => x.links));
            db.linklists.DeleteAllOnSubmit(sd.linklists);

            var articles = sd.blogs.SelectMany(x => x.articles).AsQueryable();
            db.article_comments.DeleteAllOnSubmit(articles.SelectMany(x => x.article_comments));
            db.article_tags.DeleteAllOnSubmit(articles.SelectMany(x => x.article_tags));
            db.articles.DeleteAllOnSubmit(articles);
            db.blogs.DeleteAllOnSubmit(sd.blogs);
            
            // friends
            db.friends.DeleteAllOnSubmit(sd.friends1);

            // delete change history
            var changehistoryitems = users.SelectMany(x => x.changeHistoryItems);
            db.changeHistoryItems.DeleteAllOnSubmit(changehistoryitems);

            db.activities.DeleteAllOnSubmit(users.SelectMany(x => x.activities));
            db.users.DeleteAllOnSubmit(users);
            db.organisations.DeleteAllOnSubmit(sd.organisations);
            sd.creator = null;
            db.SubmitChanges();

            // coupons
            db.coupons.DeleteAllOnSubmit(sd.coupons);

            // delete stock unit
            db.stockUnits.DeleteAllOnSubmit(sd.stockUnits);

            sd.facebookCoupon = null;
            db.SubmitChanges();

            db.MASTERsubdomains.DeleteOnSubmit(sd);
            db.SubmitChanges();

            // delete lucene directory
#if LUCENE
            LuceneUtil.Instance.DeleteLuceneDirectory(subdomainname);
#endif
            // TODO: delete theme directories
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using tradelr.DBML.Helper;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.ActionResults;
using tradelr.Library;
using tradelr.Models.facebook;
using tradelr.Models.liquid.models.Blog;
using Blog = tradelr.Models.liquid.models.Blog.Blog;

namespace tradelr.Controllers.liquid
{
    public class blogsController : baseController
    {
        public ActionResult Index(string id)
        {
            var blog = MASTERdomain.blogs.Where(x => string.Equals(x.permalink, id, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
            if (blog == null)
            {
                return Content(CreatePageMissingTemplate().Render());
            }

            var template = CreateLiquidTemplate("blog", blog.title);
            // opengraph
            var opengraph = MASTERdomain.organisation.ToOpenGraph(null,null);
            template.AddHeaderContent(this.RenderViewToString("~/Views/store/liquid/defaultHeader.ascx", opengraph));
            
            template.InitContentTemplate("templates/blog.liquid");
            template.AddParameters("blog", new Blog(blog));

            return Content(template.Render());
        }

        public ActionResult Atom(string blogid)
        {
            var feed = new SyndicationFeed();
            var blog = MASTERdomain.blogs.Where(x => string.Equals(x.permalink, blogid, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
            if (blog != null)
            {
                var creator = new SyndicationPerson();
                creator.Name = blog.MASTERsubdomain.organisation.name; 
                feed.Authors.Add(creator);
                feed.Id = Request.Url.ToString();
                feed.Title = new TextSyndicationContent(string.Format("{0} - {1}", blog.MASTERsubdomain.organisation.name, blog.title));
                feed.LastUpdatedTime = blog.updated;
                var items = new List<SyndicationItem>();
                foreach (var article in blog.articles)
                {
                    if (!article.published.HasValue)
                    {
                        continue;
                    }
                    var item = new SyndicationItem
                                   {
                                       Title = new TextSyndicationContent(article.title),
                                       Id = string.Format("articles-{0}", article.id),
                                       PublishDate = article.published.Value,
                                       LastUpdatedTime = article.published.Value
                                   };

                    var author = new SyndicationPerson();
                    author.Name = article.user.ToFullName();
                    item.Authors.Add(author);
                    item.Summary = new TextSyndicationContent(article.content, TextSyndicationContentKind.Html);
                    var link =
                        SyndicationLink.CreateAlternateLink(
                            new Uri(accountHostname.ToDomainUrl(article.ToLiquidUrl())), "text/html");
                    item.Links.Add(link);
                    items.Add(item);
                }
                feed.Items = items;

            }

            return new AtomActionResult(feed);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using tradelr.DBML.Helper;
using tradelr.DBML.Lucene;
using tradelr.Libraries.ActionFilters;
using clearpixels.Logging;
using tradelr.Models.liquid.models;
using tradelr.Models.liquid.models.Product;
using Version = Lucene.Net.Util.Version;

namespace tradelr.Controllers.liquid
{
    public class searchController : baseController
    {
        public ActionResult Index(string q, int? page)
        {
            var template = CreateLiquidTemplate("search", "Search");
            template.InitContentTemplate("templates/search.liquid");
            var searchresult = new Search();
            template.AddParameters("search", searchresult);
            if (!string.IsNullOrEmpty(q))
            {
                searchresult.performed = true;
                var ids = new List<string>();
                try
                {
                    var analyzer = new StandardAnalyzer(Version.LUCENE_29);
                    using (var searcher = new IndexSearcher(LuceneUtil.GetDirectoryInfo(LuceneIndexType.PRODUCTS, accountSubdomainName), true))
                    {
                        var queryparser = new MultiFieldQueryParser(Version.LUCENE_29, new[] { "sku", "title", "category", "details" }, analyzer);
                        var query = queryparser.Parse(q);
                        var hits = searcher.Search(query);

                        for (int i = 0; i < hits.Length(); i++)
                        {
                            Document doc = hits.Doc(i);
                            ids.Add(doc.Get("id"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Syslog.Write(ex);
                }

                var products = repository.GetProducts(subdomainid.Value).Where(x => ids.Contains(x.id.ToString())).IsActive();

                searchresult.terms = q;
                searchresult.results = products.ToLiquidModel(sessionid, "").ToList();
            }

            // return first page if we don't have page value but just return products
            if (page.HasValue)
            {
                template.AddRegisters("current_page", page.Value);
            }

            return Content(template.Render());
        }

    }
}

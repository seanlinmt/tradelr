using System.Collections.Generic;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace tradelr.DBML.Lucene
{
    public class LuceneSearch
    {
        public IEnumerable<LuceneHit> ProductSearch(string searchterm, string subdomain_name)
        {
            var ids = new HashSet<LuceneHit>();
            using (var searcher = new IndexSearcher(LuceneUtil.GetDirectoryInfo(LuceneIndexType.PRODUCTS, subdomain_name), true))
            {
                var term1 = new Term("sku", searchterm);
                var term2 = new Term("title", searchterm);
                var term3 = new Term("details", searchterm);
                var query = new FuzzyQuery(term1, 0.7f);
                var hits1 = searcher.Search(query);
                query = new FuzzyQuery(term2, 0.7f);
                var hits2 = searcher.Search(query);
                query = new FuzzyQuery(term3, 0.7f);
                var hits3 = searcher.Search(query);

                for (int i = 0; i < hits1.Length(); i++)
                {
                    Document doc = hits1.Doc(i);
                    ids.Add(new LuceneHit() { id = doc.Get("id"), score = hits1.Score(i) });
                }

                for (int i = 0; i < hits2.Length(); i++)
                {
                    Document doc = hits2.Doc(i);
                    ids.Add(new LuceneHit() { id = doc.Get("id"), score = hits2.Score(i) });
                }

                for (int i = 0; i < hits3.Length(); i++)
                {
                    Document doc = hits3.Doc(i);
                    ids.Add(new LuceneHit() { id = doc.Get("id"), score = hits3.Score(i) });
                }
            }

            return ids;
        }

        public IEnumerable<LuceneHit> ContactSearch(string searchterm, string subdomain_name)
        {
            var ids = new HashSet<LuceneHit>();

            using(var searcher = new IndexSearcher(LuceneUtil.GetDirectoryInfo(LuceneIndexType.CONTACTS, subdomain_name), true))
            {
                var term1 = new Term("email", searchterm);
                var term2 = new Term("name", searchterm);
                var term3 = new Term("orgname", searchterm);
                var term4 = new Term("fullname", searchterm);
                var query = new FuzzyQuery(term1, 0.7f);
                var hits1 = searcher.Search(query);
                query = new FuzzyQuery(term2, 0.7f);
                var hits2 = searcher.Search(query);
                query = new FuzzyQuery(term3, 0.7f);
                var hits3 = searcher.Search(query);
                query = new FuzzyQuery(term4, 0.7f);
                var hits4 = searcher.Search(query);
                
                for (int i = 0; i < hits1.Length(); i++)
                {
                    Document doc = hits1.Doc(i);
                    ids.Add(new LuceneHit() { id = doc.Get("id"), score = hits1.Score(i) });
                }

                for (int i = 0; i < hits2.Length(); i++)
                {
                    Document doc = hits2.Doc(i);
                    ids.Add(new LuceneHit() { id = doc.Get("id"), score = hits2.Score(i) });
                }

                for (int i = 0; i < hits3.Length(); i++)
                {
                    Document doc = hits3.Doc(i);
                    ids.Add(new LuceneHit() { id = doc.Get("id"), score = hits3.Score(i) });
                }
                for (int i = 0; i < hits4.Length(); i++)
                {
                    Document doc = hits4.Doc(i);
                    ids.Add(new LuceneHit() { id = doc.Get("id"), score = hits4.Score(i) });
                }
            }

            return ids;
        }

        public IEnumerable<LuceneHit> TransactionSearch(string searchterm, string subdomain_name)
        {
            var ids = new HashSet<LuceneHit>();
            using (
                var searcher =
                    new IndexSearcher(LuceneUtil.GetDirectoryInfo(LuceneIndexType.TRANSACTION, subdomain_name), true))
            {
                var term1 = new Term("receiver", searchterm);
                var term2 = new Term("sku", searchterm);
                var term3 = new Term("description", searchterm);
                var term4 = new Term("receiverfullname", searchterm);
                var query = new FuzzyQuery(term1, 0.7f);
                var hits1 = searcher.Search(query);
                query = new FuzzyQuery(term2, 0.7f);
                var hits2 = searcher.Search(query);
                query = new FuzzyQuery(term3, 0.7f);
                var hits3 = searcher.Search(query);
                query = new FuzzyQuery(term4, 0.7f);
                var hits4 = searcher.Search(query);

                
                for (int i = 0; i < hits1.Length(); i++)
                {
                    Document doc = hits1.Doc(i);
                    ids.Add(new LuceneHit() { id = doc.Get("id"), score = hits1.Score(i) });
                }

                for (int i = 0; i < hits2.Length(); i++)
                {
                    Document doc = hits2.Doc(i);
                    ids.Add(new LuceneHit() { id = doc.Get("id"), score = hits2.Score(i) });
                }

                for (int i = 0; i < hits3.Length(); i++)
                {
                    Document doc = hits3.Doc(i);
                    ids.Add(new LuceneHit() { id = doc.Get("id"), score = hits3.Score(i) });
                }

                for (int i = 0; i < hits4.Length(); i++)
                {
                    Document doc = hits4.Doc(i);
                    ids.Add(new LuceneHit() { id = doc.Get("id"), score = hits4.Score(i) });
                }
            }

            return ids;
        }
    }
}
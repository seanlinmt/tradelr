using System;
using System.IO;
using System.Linq;
using clearpixels.Logging;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using tradelr.DBML.Lucene.IndexingQueue;
using tradelr.Library;
using tradelr.Library.Constants;
using Version = Lucene.Net.Util.Version;
#if LUCENE
#endif

namespace tradelr.DBML.Lucene
{
#if LUCENE
    public class LuceneUtil
    {
        private StandardAnalyzer analyzer;
        private IndexWriter writer;
        private FSDirectory fsdir;
        private bool writerInUse;
        private LuceneUtil()
        {
            analyzer = new StandardAnalyzer(Version.LUCENE_29);
            writerInUse = false;
            writer = null;
        }

        public readonly static LuceneUtil Instance = new LuceneUtil();

        public void ReIndex()
        {
            writerInUse = true;
            using (var repository = new TradelrRepository())
            {
                // get subdomains
                var subdomains = repository.GetSubDomains();
                foreach (var subdomain in subdomains)
                {
                    // index contacts
                    writer = CreateWriter(LuceneIndexType.CONTACTS, subdomain.name);
                    if (writer != null)
                    {
                        var contacts = repository.GetAllContacts(subdomain.id);
                        foreach (var contact in contacts)
                        {
                            try
                            {
                                var action = new ContactItem(contact);
                                var doc = IndexContact(action);
                                writer.AddDocument(doc);
                            }
                            catch (Exception ex)
                            {
                                Syslog.Write(ex);
                            }

                        }
                        writer.Optimize();
                        CloseWriter();
                    }

                    // index products
                    writer = CreateWriter(LuceneIndexType.PRODUCTS, subdomain.name);
                    if (writer != null)
                    {
                        var products = repository.GetProducts(subdomain.id);
                        foreach (var product in products)
                        {
                            try
                            {
                                var action = new ProductItem(product);
                                var doc = IndexProduct(action);
                                writer.AddDocument(doc);
                            }
                            catch (Exception ex)
                            {
                                Syslog.Write(ex);
                            }

                        }

                        writer.Optimize();
                        CloseWriter();
                    }


                    // orders
                    writer = CreateWriter(LuceneIndexType.TRANSACTION, subdomain.name);
                    if (writer != null)
                    {
                        MASTERsubdomain subdomain1 = subdomain;
                        var orders = repository.GetOrders().Where(x => x.user1.organisation1.subdomain == subdomain1.id);
                        foreach (var order in orders)
                        {
                            try
                            {
                                var acttion = new TransactionItem(order);
                                var doc = IndexTransaction(acttion);
                                writer.AddDocument(doc);
                            }
                            catch (Exception ex)
                            {
                                Syslog.Write("Lucene index error: order ID =" + order.id);
                                Syslog.Write(ex);
                            }
                        }

                        writer.Optimize();
                        CloseWriter();
                    }
                }
            }
            writerInUse = false;
        }


        public bool ModifyIndex(LuceneAction action)
        {
            if (writerInUse)
            {
                return false;
            }

            writer = CreateWriter(action.type, action.subdomainName);
            if (writer == null)
            {
                Syslog.Write("LUCENE: Failed to obtained Lucene IndexWriter, action:{0}, subdomain:{1}", action.type, action.subdomainName);
                return false;
            }
            if (action.itemKey == 0)
            {
                Syslog.Write("LUCENE: indexKey is 0, action:{0}, subdomain:{1}", action.type, action.subdomainName);
                CloseWriter();
                return false;
            }
            var termID = new Term("id", action.itemKey.ToString());

            if (action.deleteOnly)
            {
                writer.DeleteDocuments(termID);
            }
            else
            {
                writer.DeleteDocuments(termID);
                Document doc;
                switch (action.type)
                {
                    case LuceneIndexType.CONTACTS:
                        doc = IndexContact((ContactItem)action.data);
                        break;
                    case LuceneIndexType.TRANSACTION:
                        doc = IndexTransaction((TransactionItem)action.data);
                        break;
                    case LuceneIndexType.PRODUCTS:
                        doc = IndexProduct((ProductItem)action.data);
                        break;
                    default:
                        Syslog.Write("LUCENE: Unknown action:{0}, subdomain:{1}", action.type, action.subdomainName);
                        CloseWriter();
                        return false;
                }

                writer.AddDocument(doc);
            }

            writer.Optimize();
            CloseWriter();

            return true;
        }

        private void CloseWriter()
        {
            writer.Close();
            fsdir.Close();
            writerInUse = false;
        }

        private IndexWriter CreateWriter(LuceneIndexType type, string subdomain)
        {
            try
            {
                fsdir = GetDirectoryInfo(type, subdomain);
                writer = new IndexWriter(fsdir, analyzer, true, new IndexWriter.MaxFieldLength(100000));
                return writer;
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
            }
            return null;
        }

        public static FSDirectory GetDirectoryInfo(LuceneIndexType type, string subdomain)
        {
            DirectoryInfo dir;
            // add contacts index
            var path = Path.Combine(string.Concat(GeneralConstants.APP_ROOT_DIR, GeneralConstants.LUCENE_INDEX_LOCATION, "\\", subdomain),
                                    type.ToString());
            
            if (!System.IO.Directory.Exists(path))
            {
                dir = System.IO.Directory.CreateDirectory(path);
            }
            else
            {
                dir = new DirectoryInfo(path);
            }
            return FSDirectory.Open(dir);
        }

        public void DeleteLuceneDirectory(string subdomain)
        {
            // add contacts index
            var path = Path.Combine(string.Concat(GeneralConstants.APP_ROOT_DIR, GeneralConstants.LUCENE_INDEX_LOCATION, "\\", subdomain));

            if (System.IO.Directory.Exists(path))
            {
                System.IO.Directory.Delete(path, true);
            }
        }

        private Document IndexTransaction(TransactionItem o)
        {
            var doc = new Document();
            doc.Add(new Field("id", o.id, Field.Store.YES, Field.Index.NO));

            var receiverField = new Field("receiver", o.receiver, Field.Store.YES, Field.Index.ANALYZED);
            var receiverFullNameField = new Field("receiverfullname", o.receiverfullname, Field.Store.YES, Field.Index.NOT_ANALYZED);
            var skuField = new Field("sku", o.sku, Field.Store.COMPRESS, Field.Index.ANALYZED);
            var descrField = new Field("description", o.description, Field.Store.COMPRESS, Field.Index.ANALYZED);

            doc.Add(receiverField);
            doc.Add(receiverFullNameField);
            doc.Add(skuField);
            doc.Add(descrField);

            return doc;
        }

        private Document IndexProduct(ProductItem p)
        {
            var doc = new Document();

            doc.Add(new Field("id", p.id, Field.Store.YES, Field.Index.NO));

            var skuField = new Field("sku", p.sku, Field.Store.YES, Field.Index.ANALYZED);
            skuField.SetBoost(5f);

            var titleField = new Field("title", p.title, Field.Store.YES, Field.Index.ANALYZED);
            titleField.SetBoost(4f);
            var categoryField = new Field("category", p.category,Field.Store.YES, Field.Index.ANALYZED);
            categoryField.SetBoost(2f);
            var detailsField = new Field("details", p.details, Field.Store.COMPRESS, Field.Index.ANALYZED);
            categoryField.SetBoost(3f);
            doc.Add(skuField);
            doc.Add(titleField);
            doc.Add(categoryField);
            doc.Add(detailsField);
            return doc;
        }

        private Document IndexContact(ContactItem u)
        {
            var doc = new Document();

            doc.Add(new Field("id", u.id, Field.Store.YES, Field.Index.NO));

            var emailfield = new Field("email", Utility.EmptyIfNull(u.email).ToLower(), Field.Store.YES, Field.Index.ANALYZED);
            var namefield = new Field("name", u.name, Field.Store.YES, Field.Index.ANALYZED);
            var fullnamefield = new Field("fullname", u.fullname, Field.Store.YES, Field.Index.NOT_ANALYZED);
            var orgfield = new Field("orgname", u.orgname, Field.Store.YES, Field.Index.ANALYZED);
            var notesfield = new Field("notes", Utility.EmptyIfNull(u.notes), Field.Store.COMPRESS, Field.Index.ANALYZED);
            doc.Add(emailfield);
            doc.Add(namefield);
            doc.Add(fullnamefield);
            doc.Add(orgfield);
            doc.Add(notesfield);

            return doc;
        }
    }
#endif
}


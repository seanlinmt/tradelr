using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Google.GData.Client;
using Google.GData.ContentForShopping;
using Google.GData.ContentForShopping.Elements;
using tradelr.Common.Models.currency;
using tradelr.Common.Models.photos;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.Constants;
using clearpixels.Logging;
using tradelr.Models.activity;
using tradelr.Models.google;
using tradelr.Models.products;

namespace tradelr.Models.export.gbase
{
    public class GoogleBaseExporter : ExportItem
    {
        private const int COUNTRY_US = 185;
        private const int COUNTRY_UK = 184;
        private const int COUNTRY_GERMANY = 65;
        
        public string CountryCode { get; set; }
        public GoogleItemType ItemType { get; set; }
        public string ProductType { get; set; }
        public string CurrencyCode { get; set; }
        
        // org info
        public string OrgName { get; set; }
        private decimal exchangeRate { get; set; }

        private readonly ContentForShoppingService service;
        private ProductFeed feed;
        private string accountid;

        public ProductEntry entry { get; private set; }
        public List<ProductEntry> entries { get; set; }
        public string AtomID { get; set;}

        private GoogleBaseExporter(string hostname) : base(hostname)
        {
            entry = new ProductEntry();
            entries = new List<ProductEntry>();
        }

        public GoogleBaseExporter(long subdomainid, string hostname, long? sessionid = null)
            : this(hostname)
        {
            ownerid = sessionid;
            service = new ContentForShoppingService("tradelr");
            var authFactory = new GAuthSubRequestFactory("gbase", "tradelr");

            using (var repository = new TradelrRepository())
            {
                var sd = repository.GetSubDomain(subdomainid);

                if (sd.gbaseid.HasValue && 
                    !string.IsNullOrEmpty(sd.googleBase.sessiontoken))
                {
                    service.RequestFactory = authFactory;
                    service.SetAuthenticationToken(sd.googleBase.sessiontoken);
                    accountid = sd.googleBase.accountid.ToString();
                }
                else
                {
                    // use tradelr as default
                    service.setUserCredentials("tradelr.com@gmail.com", "tuaki1976");
                    accountid = "8812401";
                }

                // get feed
                if (sd.gbaseid.HasValue)
                {
                    var query = new ProductQuery("schema", accountid);
                    feed = service.Query(query);

                    InitLocalisation(sd.gbaseid.HasValue ? sd.googleBase.country : COUNTRY_US, sd.currency.ToCurrency());
                }
                
            }
        }

        private void InitLocalisation(int targetCountry, Currency currency)
        {
            exchangeRate = 1;
            switch (targetCountry)
            {
                case COUNTRY_GERMANY:
                    CurrencyCode = "EUR";
                    CountryCode = "DE";
                    if (currency.code != "EUR")
                    {
                        exchangeRate = CurrencyConverter.Instance.GetRate(currency.code, "EUR");
                    }
                    break;
                case COUNTRY_UK:
                    CurrencyCode = "GBP";
                    CountryCode = "GB";
                    if (currency.code != "GDP")
                    {
                        exchangeRate = CurrencyConverter.Instance.GetRate(currency.code, "GBP");
                    }
                    break;
                default:
                case COUNTRY_US:
                    CurrencyCode = "USD";
                    CountryCode = "US";
                    if (currency.code != "USD")
                    {
                        exchangeRate = CurrencyConverter.Instance.GetRate(currency.code, "USD");
                    }
                    break;
            }
        }

        public void InitValues(product p)
        {
            // init common values
            base.InitValues(p, exchangeRate);

            // init gbase specific values
            ItemType = GoogleItemType.PRODUCTS;
            ProductType = p.productCategory.ToGoogleProductType();
        }

        public void AddProductImages(IEnumerable<Photo> productPhotos)
        {
            if (productPhotos == null)
            {
                return;
            }

            // TODO: need to handle default image

            // need to remove all for updates, nothing initially when adding so not a problem

            entry.AdditionalImageLinks.Clear();
            foreach (var photo in productPhotos)
            {
                entry.AdditionalImageLinks.Add(new AdditionalImageLink(hostName.ToDomainUrl(photo.url)));
            }
        }

        public string AddToGoogleBase()
        {
            UpdateEntry();
            // try insert
            string gbaseId = "";
            bool hasError = false;
            try
            {
                entry = service.Insert(feed, entry);
                gbaseId = entry.Id.AbsoluteUri;
            }
            catch (WebException ex)
            {
                var resp = ex.Response;
                hasError = true;
                if (resp != null)
                {
                    using (var sr = new StreamReader(resp.GetResponseStream()))
                    {
                        var err = sr.ReadToEnd();
                        Syslog.Write(string.Concat("ADD: ", err, " ", hostName, " ", ProductId));
                    }
                }
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                Syslog.Write(string.Concat("ADD: ", ex.Message, " ", hostName, " ", ProductId));
                hasError = true;
            }

            if (hasError && ownerid.HasValue)
            {
                using (var repository = new TradelrRepository())
                {
                    repository.AddActivity(ownerid.Value,
                                   new ActivityMessage(ProductId, ownerid,
                                               ActivityMessageType.AUTOPOST_GBASE_FAIL,
                                               string.Format("<a href='{0}'>{1}</a>", producturl, productname)),
                                               subdomainid);
                    repository.Save(); 
                }
                
            }

            return gbaseId;
        }

        public bool GetFromGoogleBase(string gbaseid)
        {
            try
            {
                foreach (ProductEntry p in feed.Entries)
                {
                    if (p.Id.AbsoluteUri == gbaseid)
                    {
                        entry = p;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return false;
            }
            return true;
        }

        public bool GetAndUpdateFromGoogleBase(product p)
        {
            GetFromGoogleBase(p.gbase_product.externalid);
            
            if (entry == null)
            {
                return false;
            }
            InitValues(p);
            UpdateEntry(false);
            return true;
        }

        public bool GetAndUpdateStatus(string gbaseid, bool isDraft)
        {
            try
            {
                GetFromGoogleBase(gbaseid);
                if (entry == null)
                {
                    return false;
                }
                entry.IsDraft = isDraft;
                entry = service.Update(entry);
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return false;
            }
            return true;
        }

        private void UpdateEntry(bool isCreate = true)
        {
            entry.Title.Text = Title;
            entry.Content.Content = Description;
#if DEBUG
            entry.IsDraft = true;
#else
            entry.AlternateUri =
                new AtomUri(
                    hostName.ToDomainUrl(
                        string.Concat(GeneralConstants.URL_SINGLE_PRODUCT_SHOW, ProductId, "/", Title.ToSafeUrl()), true));
#endif
            // attributes
            entry.TargetCountry = CountryCode;
            // we need this or else duplicate attribute will exist (strange that target_country cannot be found or removed)
            if (isCreate)
            {
                entry.ProductId = ProductId.ToString();
                entry.Condition = "New";
                entry.ProductType = ProductType;
                entry.TargetCountry = CountryCode;
            }
            
            entry.ProductType = ItemType.ToDescriptionString();
            if (SellingPrice.HasValue)
            {
                entry.Price = new Price(CurrencyCode, SellingPrice.Value.ToString());
            }
        }

        public bool UpdateToGoogleBase()
        {
            bool hasError = false;
            try
            {
                var atomid = service.Update(entry).Id.AbsoluteUri;
                if (AtomID != atomid)
                {
                    Syslog.Write(string.Format("Inserted GBASE not equal, productid {0} :{1} {2}", ProductId, AtomID, atomid));
                }
            }
            catch(WebException ex)
            {
                var resp = ex.Response;
                hasError = true;
                if (resp != null)
                {
                    using (var sr = new StreamReader(resp.GetResponseStream()))
                    {
                        var err = sr.ReadToEnd();
                        Syslog.Write(string.Format("Update err: {0}, hostname: {1}", err, hostName));
                    }
                }
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                hasError = true;
            }

            if (hasError && ownerid.HasValue)
            {
                using (var repository = new TradelrRepository())
                {
                    repository.AddActivity(ownerid.Value,
                                   new ActivityMessage(ProductId, ownerid,
                                               ActivityMessageType.AUTOPOST_GBASE_FAIL,
                                               string.Format("<a href='{0}'>{1}</a>", producturl, productname)),
                                               subdomainid);
                    repository.Save();
                }
                
                return false;
            }
            return true;
        }

        public void DeleteFromGoogleBase(string gbaseid)
        {
            try
            {
                var targetUri = new Uri(gbaseid);
                service.Delete(targetUri);
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
            }
        }

        public void UpdateProductGbaseID(string gbaseid)
        {
            using (var repository = new TradelrRepository())
            {
                var p = repository.GetProduct(ProductId);
                if (p != null)
                {
                    p.gbaseID = gbaseid;
                    repository.Save();
                }
            }
        }

        // try to retrieve feed, account does not exist if exception is thrown
        public bool VerifyAccount()
        {
            if (feed != null)
            {
                return true;
            }
            return false;
        }

        public void GetAllProducts()
        {
            foreach (ProductEntry item in feed.Entries)
            {
                entries.Add(item);
            }
        }
    }

    public class GoogleBaseWorker
    {
        private readonly GoogleBaseExporter item;

        public GoogleBaseWorker(GoogleBaseExporter item)
        {
            this.item = item;
        }

        public void Post()
        {
            var id = item.AddToGoogleBase();
            if (string.IsNullOrEmpty(id))
            {
                return;
            }
            item.UpdateProductGbaseID(id);
        }

        public void Update()
        {
            item.UpdateToGoogleBase();
        }
    }
}
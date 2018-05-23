using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Google.GData.ContentForShopping;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.currency;
using tradelr.Common.Models.photos;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.DBML.Models;
using tradelr.Models.export.gbase;
using tradelr.Models.google.gbase;
using tradelr.Models.inventory;
using tradelr.Models.photos;
using tradelr.Models.products;

namespace tradelr.Models.networks
{
    public class NetworksGbase : Networks
    {
        
        public override sealed long subdomainid { get; set; }
        public override sealed long sessionid { get; set; }

        private long locationid { get; set; }
        private Currency currency { get; set; }
        private decimal? exchangerate { get; set; }
        private string hostName { get; set; }

        public NetworksGbase(long subdomainid, long sessionid, string hostname)
        {
            this.subdomainid = subdomainid;
            this.sessionid = sessionid;
            this.hostName = hostname;
        }

        public override void StartSynchronisation(bool? upload)
        {
            // pull items
            var gbase = new GoogleBaseExporter(subdomainid, hostName, sessionid);
            gbase.GetAllProducts();
            using (var repository = new TradelrRepository())
            {
                // create network location
                var inventoryLocation = new inventoryLocation
                {
                    name = LOCATIONNAME_GBASE,
                    subdomain = subdomainid,
                    lastUpdate = DateTime.UtcNow
                };
                locationid = repository.AddInventoryLocation(inventoryLocation, subdomainid);

                // init some settings
                locationid = repository.GetInventoryLocation(LOCATIONNAME_GBASE, subdomainid).id;
                currency = repository.GetSubDomain(subdomainid).currency.ToCurrency();

                var products = repository.GetProducts(subdomainid);
                foreach (var entry in gbase.entries)
                {
                    ProductEntry entry1 = entry;
                    var oldTypeEntry = products.SingleOrDefault(x => x.gbaseID == entry1.Id.AbsoluteUri);
                    if(oldTypeEntry != null)
                    {
                        // old type entry exists

                        // remove id
                        oldTypeEntry.gbaseID = null;

                        // create gbase entry, don't need to create a variant since one would already exist
                        oldTypeEntry.gbase_product = CreateGbaseInformationEntry(entry);
                    }
                    else
                    {
                        var newtypeEntry =
                            products.Where(x => x.gbase_product.externalid == entry1.Id.AbsoluteUri).Select(
                                x => x.gbase_product).SingleOrDefault();
                        if (newtypeEntry != null)
                        {
                            // new type entry exists
                            // update status
                            newtypeEntry.expirydate = entry.ExpirationDate;
                            if (entry.IsDraft)
                            {
                                newtypeEntry.flags |= (int)InventoryItemFlag.DRAFT;
                            }
                            else
                            {
                                newtypeEntry.flags &= ~(int) InventoryItemFlag.DRAFT;
                            }
                        }
                        else
                        {
                            // entry does not exist
                            // create and add entry to tradelr
                            var p = CreateProductFromEntry(entry);

                            // create variant
                            var variants = new List<product_variant>();
                            
                            var variant = new product_variant()
                                              {
                                                  sku =
                                                      string.Concat("GBASE", entry.Id.AbsoluteUri.Substring(
                                                          entry.Id.AbsoluteUri.LastIndexOf('/') + 1))
                                              };
                            variants.Add(variant);

                            // add gbase info
                            p.gbase_product = CreateGbaseInformationEntry(entry);
                            p.product_variants.AddRange(variants);
                            var pinfo = new ProductInfo()
                                            {
                                                p = p
                                            };
                            repository.AddProduct(pinfo, subdomainid);

                            // images
                            foreach (var link in entry.AdditionalImageLinks)
                            {
                                var image = link.Value.ReadAndSaveProductImageFromUrl(subdomainid, sessionid, p.id);
                                p.thumb = image.id;
                            }
                        }
                    }
                }

                // upload to google base for each entry that does not have a special entry
                if (upload.HasValue && upload.Value)
                {
                    var newproducts =
                        repository.GetProducts(subdomainid).Where(
                            x => !x.gbase.HasValue && (x.flags & (int) ProductFlag.ARCHIVED) == 0);
                    foreach (var p in newproducts)
                    {
                        var gb = new GoogleBaseExporter(subdomainid, hostName, sessionid);
                        gb.InitValues(p);
#if !DEBUG
                        IEnumerable<Photo> productPhotos = repository.GetImages(PhotoType.PRODUCT, p.id).ToModel(Imgsize.LARGE);
                        gb.AddProductImages(productPhotos);
#endif
                        var worker = new GoogleBaseWorker(gb);
                        new Thread(worker.Post).Start();
                    }
                }

                // save
                repository.Save();
            }
            
        }

        public static string URLFromEntry(ProductEntry entry)
        {
            return entry.Links.Where(x => x.Type == "text/html").Select(x => x.AbsoluteUri).SingleOrDefault();
        }

        private gbase_product CreateGbaseInformationEntry(ProductEntry entry)
        {
            var flag = InventoryItemFlag.GBASE;
            if (entry.IsDraft)
            {
                flag |= InventoryItemFlag.DRAFT;
            }
            return new gbase_product()
                       {
                           flags = (int) flag,
                           expirydate = entry.ExpirationDate,
                           externalid = entry.Id.AbsoluteUri,
                           externallink = URLFromEntry(entry)
                       };
        }

        private product CreateProductFromEntry(ProductEntry entry)
        {
            // do this check assuming products all must have the same target country
            if (exchangerate == null)
            {
                exchangerate = 1;
                var targetcountry = entry.TargetCountry;

                switch (targetcountry)
                {
                    case "AU": // australia
                        if (currency.code != "AUD")
                        {
                            exchangerate = CurrencyConverter.Instance.GetRate("AUD", currency.code);
                        }
                        break;
                    case "CN": // china
                        if (currency.code != "CNY")
                        {
                            exchangerate = CurrencyConverter.Instance.GetRate("CNY", currency.code);
                        }
                        break;
                    case "FR": // france
                        if (currency.code != "EUR")
                        {
                            exchangerate = CurrencyConverter.Instance.GetRate("EUR", currency.code);
                        }
                        break;
                    case "DE": // germany
                        if (currency.code != "EUR")
                        {
                            exchangerate = CurrencyConverter.Instance.GetRate("EUR", currency.code);
                        }
                        break;
                    case "IT": // italy
                        if (currency.code != "EUR")
                        {
                            exchangerate = CurrencyConverter.Instance.GetRate("EUR", currency.code);
                        }
                        break;
                    case "JP": // japan
                        if (currency.code != "JPY")
                        {
                            exchangerate = CurrencyConverter.Instance.GetRate("JPY", currency.code);
                        }
                        break;
                    case "NL": // netherlands
                        if (currency.code != "EUR")
                        {
                            exchangerate = CurrencyConverter.Instance.GetRate("EUR", currency.code);
                        }
                        break;
                    case "ES": // spain
                        if (currency.code != "EUR")
                        {
                            exchangerate = CurrencyConverter.Instance.GetRate("EUR", currency.code);
                        }
                        break;
                    case "GB": // united kingdom
                        if (currency.code != "GDP")
                        {
                            exchangerate = CurrencyConverter.Instance.GetRate("GBP", currency.code);
                        }
                        break;
                    case "US": // united states
                        if (currency.code != "USD")
                        {
                            exchangerate = CurrencyConverter.Instance.GetRate("USD", currency.code);
                        }
                        break;
                    default:
                        throw new NotImplementedException(string.Format("Unknown country {0}", targetcountry));
                        break;
                }
            }

            return new product()
                              {
                                  flags = (int)ProductFlag.NONE,
                                  title = entry.Title.Text,
                                  details = entry.Content.Content,
                                  sellingPrice = Convert.ToDecimal(entry.Price.Value) * exchangerate,
                                  subdomainid = subdomainid
                              };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ebay.Resources;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using tradelr.DBML;
using tradelr.DBML.Models;
using tradelr.Library.Constants;
using clearpixels.Logging;
using tradelr.Models.facebook.import;

namespace tradelr.Models.products
{
    public class ProductImport
    {
        private const int MaxPhotoImport = 20;

        private decimal? GetCellValueAsDecimal(Cell cell)
        {
            decimal? value = null;
            if (cell != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(cell.StringCellValue))
                    {
                        value = decimal.Parse(cell.StringCellValue, NumberStyles.AllowCurrencySymbol |
                                                              NumberStyles.AllowDecimalPoint |
                                                              NumberStyles.AllowThousands);
                    }
                }
                catch
                {
                    // if error then cell is double
                    value = Convert.ToDecimal(cell.NumericCellValue);
                }
            }
            
            return value;
        }

        private int? GetCellValueAsInt(Cell cell)
        {
            int? value = null;
            if (cell != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(cell.StringCellValue))
                    {
                        value = int.Parse(cell.StringCellValue);
                    }
                }
                catch
                {
                    // if error then cell is double
                    value = Convert.ToInt32(cell.NumericCellValue);
                }
            }
            
            return value;
        }

        private string GetCellValueAsString(Cell cell)
        {
            string value = "";
            if (cell != null)
            {
                try
                {
                    value = cell.StringCellValue;
                }
                catch
                {
                    // if error then cell is numeric
                    value = cell.NumericCellValue.ToString();
                }
            }

            return value;
        }

        public List<ProductInfo> Import(Stream inputStream, long owner, long subdomain)
        {
            var templateWorkbook = new HSSFWorkbook(inputStream, true);
            var sheet = templateWorkbook.GetSheet("Products");
            int count = 0;
            var productsList = new List<ProductInfo>();
            using (var repository = new TradelrRepository())
            {
                while (true)
                {
                    var row = sheet.GetRow(count++);
                    if (row == null)
                    {
                        break;
                    }
                    var sku = GetCellValueAsString(row.GetCell(0, MissingCellPolicy.RETURN_NULL_AND_BLANK));
                    if (sku.StartsWith(";"))
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(sku))
                    {
                        break;
                    }

                    var title = GetCellValueAsString(row.GetCell(1, MissingCellPolicy.RETURN_NULL_AND_BLANK));
                    var description = GetCellValueAsString(row.GetCell(2, MissingCellPolicy.RETURN_NULL_AND_BLANK));
                    var maincategory = GetCellValueAsString(row.GetCell(3, MissingCellPolicy.RETURN_NULL_AND_BLANK));
                    var subcategory = GetCellValueAsString(row.GetCell(4, MissingCellPolicy.RETURN_NULL_AND_BLANK));
                    var stockunit = GetCellValueAsString(row.GetCell(5, MissingCellPolicy.RETURN_NULL_AND_BLANK));
                    var costprice = GetCellValueAsDecimal(row.GetCell(6, MissingCellPolicy.RETURN_NULL_AND_BLANK));
                    var sellingprice = GetCellValueAsDecimal(row.GetCell(7, MissingCellPolicy.RETURN_NULL_AND_BLANK));
                    var instock = GetCellValueAsInt(row.GetCell(8, MissingCellPolicy.RETURN_NULL_AND_BLANK));
                    var photos = GetCellValueAsString(row.GetCell(9, MissingCellPolicy.RETURN_NULL_AND_BLANK));

                    var product = new product
                                          {
                                              subdomainid = subdomain,
                                              details = description,
                                              title = title,
                                              costPrice = costprice,
                                              sellingPrice = sellingprice
                                          };

                    if (!string.IsNullOrEmpty(stockunit))
                    {
                        var masterunit = repository.AddMasterStockUnit(stockunit);
                        var su = new stockUnit { unitID = masterunit.id, subdomainid = subdomain };
                        product.stockUnitId = repository.AddStockUnit(su);
                    }

                    product.otherNotes = "";
                    var inventoryloc =
                        repository.GetInventoryLocation(GeneralConstants.INVENTORY_LOCATION_DEFAULT,
                                                        subdomain);

                    // create inventoryLocItem
                    var inventoryLocItem = new inventoryLocationItem
                                               {
                                                   locationid = inventoryloc.id,
                                                   lastUpdate = DateTime.UtcNow
                                               };
                    var invWorker = new InventoryWorker(inventoryLocItem, subdomain, true, false);
                    invWorker.SetValues("product created", instock, null, null, null);

                    MASTERproductCategory mastercat;
                    long? catid = null;
                    if (!string.IsNullOrEmpty(maincategory))
                    {
                        mastercat = repository.AddMasterProductCategory(maincategory);
                        var cat = new productCategory()
                        {
                            masterID = mastercat.id,
                            subdomain = subdomain
                        };
                        catid = repository.AddProductCategory(cat, subdomain);
                    }

                    // add sub category
                    if (!string.IsNullOrEmpty(subcategory) && !string.IsNullOrEmpty(maincategory))
                    {
                        mastercat = repository.AddMasterProductCategory(subcategory);
                        var subcat = new productCategory()
                        {
                            masterID = mastercat.id,
                            subdomain = subdomain,
                            parentID = catid
                        };
                        catid = repository.AddProductCategory(subcat, subdomain);
                    }
                    product.category = catid;
                    product.created = DateTime.UtcNow;
                    product.updated = product.created;
                    if (!productsList.Where(x => x.p.product_variants.Count(y => y.sku == sku) != 0).Any())
                    {
                        var pi = new ProductInfo()
                        {
                            p = product
                        };

                        if (!string.IsNullOrEmpty(photos))
                        {
                            var photourls = photos.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var photourl in photourls)
                            {
                                pi.AddPhotoUrl(photourl);
                            }
                        }

                        var variant = new product_variant { sku = sku };
                        variant.inventoryLocationItems.Add(inventoryLocItem);
                        pi.p.product_variants.Add(variant);
                        productsList.Add(pi);
                    }
                }
            }
            return productsList;
        }

        public ProductInfo ImportFacebook(FBImportProduct info, string access_token, long subdomainid)
        {
            // check for duplicate sku
            var p = new product
            {
                subdomainid = subdomainid,
                details = info.description,
                title = info.title,
            };
            p.facebook_imports.Add(new facebook_import() { facebookID = info.id });

            if (!string.IsNullOrEmpty(info.sellingprice))
            {
                decimal price;
                if (decimal.TryParse(info.sellingprice, out price))
                {
                    p.sellingPrice = price;
                }
            }
            p.updated = DateTime.UtcNow;
            p.created = DateTime.UtcNow;

            // handle photos
            var photourls = info.photoids.Select(x => x.ToFacebookPhotoUrl(access_token)).Take(MaxPhotoImport);

            // create inventoryLocItem
            long locationid;
            using (var repository = new TradelrRepository())
            {
                locationid = repository.GetInventoryLocation(GeneralConstants.INVENTORY_LOCATION_DEFAULT,
                                                             subdomainid).id;
            }
            var inventoryLocItem = new inventoryLocationItem
            {
                locationid = locationid,
                lastUpdate = DateTime.UtcNow
            };

            // create variant
            var variant = new product_variant
            {
                sku = string.IsNullOrEmpty(info.sku) ? info.id.ToString(): info.sku
            };
            variant.inventoryLocationItems.Add(inventoryLocItem);

            // add photourls
            // create our add object
            var pinfo = new ProductInfo { p = p };
            foreach (var photourl in photourls)
            {
                pinfo.photo_urls.Add(photourl);
            }

            pinfo.p.product_variants.Add(variant);

            return pinfo;
        }

        public ProductInfo ImportEbay(Listing listing, long subdomainid)
        {
            var p = new product
                        {
                            subdomainid = subdomainid,
                            details = listing.description,
                            title = listing.title,
                            ebay_product = new ebay_product()
                                               {
                                                   ebayid = listing.id,
                                                   listingType = listing.listingType.ToString(),
                                                   siteid = listing.siteid.ToString(),
                                                   categoryid = listing.categoryid1,
                                                   categoryid_secondary = listing.categoryid2,
                                                   condition = listing.condition,
                                                   returnPolicy = listing.returnPolicy,
                                                   returnWithin = listing.returnWithin,
                                                   refundPolicy = listing.refundPolicy,
                                                   duration = listing.duration,
                                                   startTime = listing.startTime,
                                                   endTime = listing.endTime,
                                                   quantity = listing.quantity,
                                                   dispatchTime = listing.dispatchTime,
                                                   isActive = listing.isActive,
                                                   startPrice = Convert.ToDecimal(listing.startPrice),
                                                   buynowPrice = listing.buynowPrice.HasValue? Convert.ToDecimal(listing.buynowPrice.Value):(decimal?) null,
                                                   reservePrice = listing.reservePrice.HasValue? Convert.ToDecimal(listing.reservePrice.Value):(decimal?)null
                                               },
                            created = DateTime.UtcNow,
                            updated = DateTime.UtcNow,
                            sellingPrice =  Convert.ToDecimal(listing.productPrice)
                        };

            // and create inventory location if not exist
            long locationid;
            using (var repository = new TradelrRepository())
            {
                locationid = repository.GetInventoryLocation(GeneralConstants.INVENTORY_LOCATION_DEFAULT, subdomainid).id;
            }

            var inventoryLocItem = new inventoryLocationItem
            {
                locationid = locationid,
                lastUpdate = DateTime.UtcNow
            };

            // create our add object
            var pinfo = new ProductInfo { p = p };
            foreach (var photoUrl in listing.photoUrls)
            {
                pinfo.AddPhotoUrl(photoUrl);
            }
            // create variant
            foreach (var ebayvariant in listing.variants)
            {
                var variant = new product_variant
                {
                    sku = string.IsNullOrEmpty(ebayvariant.sku) ? listing.id : ebayvariant.sku
                };
                foreach (var name in ebayvariant.properties.AllKeys)
                {
                    switch (name)
                    {
                        case "colour":
                        case "color":
                            variant.color = ebayvariant.properties[name];
                            break;
                        case "size":
                            variant.size = ebayvariant.properties[name];
                            break;
                        default:
                            Syslog.Write("Unknown eBay variant property {0}:{1}", name, ebayvariant.properties[name]);
                            break;
                    }
                }
                variant.inventoryLocationItems.Add(inventoryLocItem);
                pinfo.p.product_variants.Add(variant);
            }

            return pinfo;
        }
    }
}
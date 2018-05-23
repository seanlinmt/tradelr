using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using clearpixels.OAuth;
using Ebay.Enums;
using FacebookToolkit.Utility;
using api.trademe.co.nz.v1;
using eBay.Service.Core.Soap;
using tradelr.Areas.dashboard.Models.product;
using tradelr.Areas.dashboard.Models.shipping;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.currency;
using tradelr.Common.Models.photos;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.DBML.Lucene;
using tradelr.DBML.Models;
using tradelr.Library;
using tradelr.Library.Caching;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using tradelr.Library.files;
using clearpixels.Logging;
using tradelr.Models.activity;
using tradelr.Models.counter;
using tradelr.Models.export.ebay;
using tradelr.Models.export.gbase;
using tradelr.Models.export.trademe;
using tradelr.Models.export.tumblr;
using tradelr.Models.export.wordpress;
using tradelr.Models.facebook;
using tradelr.Models.google.blog;
using tradelr.Models.inventory;
using tradelr.Models.networks;
using tradelr.Models.products.viewmodel;
using tradelr.Models.shipping;
using tradelr.Models.shipwire;
using tradelr.Models.users;
using tradelr.Libraries;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Helpers;
using tradelr.Models.products;
using FacebookStreamPost = tradelr.Models.facebook.FacebookStreamPost;
using Product = tradelr.Models.products.Product;
using TransactionType = tradelr.Models.transactions.TransactionType;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [TradelrHttps]
    public class productController : baseController
    {
        private ProductViewModel GetProductViewModel(user usr, bool? trackInventory, long? shippingProfileID)
        {
            var viewmodel = new ProductViewModel(baseviewmodel);
            
            var settings = (UserSettings)usr.settings;

            // product autoposting
            viewmodel.isPostToBlogger = settings.HasFlag(UserSettings.POST_TO_BLOGGER);
            viewmodel.isPostToFacebook = settings.HasFlag(UserSettings.POST_TO_FACEBOOK);
            viewmodel.isPostToGoogle = settings.HasFlag(UserSettings.POST_TO_GOOGLE);
            viewmodel.isPostToTumblr = settings.HasFlag(UserSettings.POST_TO_TUMBLR);
            viewmodel.isPostToWordpress = settings.HasFlag(UserSettings.POST_TO_WORDPRESS);
            viewmodel.isPostToEbay = settings.HasFlag(UserSettings.POST_TO_EBAY);
            viewmodel.isPostToTrademe = settings.HasFlag(UserSettings.POST_TO_TRADEME);

            // ebay
            var ebay_token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.EBAY, true);
            if (ebay_token != null)
            {
                viewmodel.isEbaySynced = true;
            }

            if (!string.IsNullOrEmpty(usr.organisation1.MASTERsubdomain.GetPaypalID()))
            {
                viewmodel.hasPaypalAccount = true;
            }

            // trademe
            var trademe_token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.TRADEME, true);
            if (trademe_token != null)
            {
                viewmodel.isTrademeSynced = true;
            }

            /*
            if (usr.organisation1.country.HasValue)
            {
                viewdata.showGoogleProductSearch = GoogleBaseItem.SupportedCountries.Contains(usr.organisation1.country.Value);
            }
            else
            {
                viewdata.showGoogleProductSearch = false;
            }
            */

            // post to facebook stream / create facebook album
            viewmodel.facebookStreams.Add(new CheckBoxListInfo("","personal account", settings.HasFlag(UserSettings.POST_TO_FACEBOOK_OWN)));
            viewmodel.facebookAlbums.Add(new CheckBoxListInfo("", "personal account", settings.HasFlag(UserSettings.CREATE_ALBUM_FACEBOOK_OWN)));

            var fb_tokens = MASTERdomain.facebook_tokens;
            foreach (var fbToken in fb_tokens)
            {
                var flags = (FacebookTokenSettings)fbToken.flags;
                var cb_tostream = new CheckBoxListInfo(fbToken.id.ToString(), string.Format("{0} <span class='font_grey ml10'>{1}</span>", fbToken.name, fbToken.category), flags.HasFlag(FacebookTokenSettings.POST_STREAM));
                viewmodel.facebookStreams.Add(cb_tostream);

                var cb_createalbum = new CheckBoxListInfo(fbToken.id.ToString(), string.Format("{0} <span class='font_grey ml10'>{1}</span>", fbToken.name, fbToken.category), flags.HasFlag(FacebookTokenSettings.CREATE_ALBUM));
                viewmodel.facebookAlbums.Add(cb_createalbum);
            }
            
            // dimensions
            viewmodel.isMetric = settings.HasFlag(UserSettings.METRIC_VIEW);
            viewmodel.distanceUnit = Dimension.GetDistanceUnit(settings.HasFlag(UserSettings.METRIC_VIEW));
            viewmodel.weightUnit = Dimension.GetWeightUnit(settings.HasFlag(UserSettings.METRIC_VIEW));

            // shipping profiles
            viewmodel.shippingProfiles = MASTERdomain.ToShippingProfiles(shippingProfileID);

            // shipwire
            viewmodel.showShipwire = !string.IsNullOrEmpty(usr.organisation1.MASTERsubdomain.shipwireEmail);

            // inventory tracking
            viewmodel.trackInventoryList = new[]
                                               {
                                                   new SelectListItem()
                                                       {
                                                           Text = "The number of products in stock is tracked",
                                                           Value = "True",
                                                           Selected =
                                                               trackInventory.HasValue &&
                                                               trackInventory.Value
                                                       },
                                                   new SelectListItem()
                                                       {
                                                           Text =
                                                               "The number of products in stock is NOT tracked",
                                                           Value = "False",
                                                           Selected =
                                                               trackInventory.HasValue &&
                                                               !trackInventory.Value
                                                       }
                                               };
            
            // category
            viewmodel.mainCategoryList = repository.GetProductCategories(null, subdomainid.Value)
                .Select(
                    x => new ProductCategory() { title = x.MASTERproductCategory.name, id = x.id })
                .OrderBy(x => x.title)
                .Select(x => new SelectListItem
                {
                    Value = x.id.ToString(),
                    Text = x.title
                }).ToSelectList();

            // stock unit
            viewmodel.stockUnitList = repository.GetAllStockUnits(subdomainid.Value)
                .OrderBy(x => x.MASTERstockUnit.name)
                .Select(x => new SelectListItem
                {
                    Text = x.MASTERstockUnit.name,
                    Value = x.id.ToString()
                }).ToSelectList();

            // collections
            viewmodel.collections = MASTERdomain.product_collections
                                                .AsQueryable()
                                                .IsVisible()
                                                .Select(x => new SelectListItem()
                                                {
                                                    Text = x.name,
                                                    Value = x.id.ToString()
                                                });

            return viewmodel;
        }

        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_ADD)]
        public ActionResult Add()
        {
            if (MASTERdomain.trialExpired)
            {
                return RedirectToAction("TrialExpired", "Error", new { Area = "" });
            }

            var usr = repository.GetUserById(sessionid.Value, subdomainid.Value);

            var viewdata = GetProductViewModel(usr, null, null);

            viewdata.product = new Product()
            {
                isFBConnected = !string.IsNullOrEmpty(usr.FBID),
                currency = usr.organisation1.MASTERsubdomain.currency.ToCurrency(),
                tax = usr.organisation1.MASTERsubdomain.default_taxrate.HasValue? usr.organisation1.MASTERsubdomain.default_taxrate.Value.ToString():""
            };

            // add empty variant
            viewdata.product.variants.Add(new Variant());

            // handle inventory locations
            var inventorylocs = repository.GetInventoryLocationsExceptSyncNetworks(subdomainid.Value);
            foreach (var location in inventorylocs)
            {
                var loc = new InventoryLocation
                              {
                                  id = location.id, 
                                  title = location.name,
                                  createmode = true
                              };
                foreach (var variant in viewdata.product.variants)
                {
                    loc.locationItems.Add(new InventoryLocationItem
                                              {
                                                  inventorySKU = variant.sku
                                              });
                }
                viewdata.product.inventoryLocations.Add(loc);
            }

            // check that limits are not hit
            if (accountLimits.skus.HasValue)
            {
                if (stats.products_mine == accountLimits.skus)
                {
                    viewdata.product.limitHit = true;
                }
                else if (stats.products_mine > accountLimits.skus)
                {
                    Syslog.Write(new Exception("Product limit exceeded"));
                }
            }
            return View(viewdata);
        }

        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_MODIFY)]
        private ActionResult Archive(string id, string ids)
        {
            var productids = new List<long>();
            var success = new List<long>();
            if (string.IsNullOrEmpty(id))
            {
                var idsarray = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                productids.AddRange(idsarray.Select(long.Parse));
            }
            else
            {
                productids.Add(long.Parse(id));
            }

            foreach (var productid in productids)
            {
                // check that this product is current owners
                var p = repository.GetProduct(productid, subdomainid.Value);
                if (p == null)
                {
                    continue;
                }

                // delete from gbase
                if (p.gbase.HasValue)
                {
                    var gbase = new GoogleBaseExporter(subdomainid.Value, accountHostname, sessionid.Value);
                    gbase.DeleteFromGoogleBase(p.gbase_product.externalid);
                }

                var gbp = p.gbase_product;

                // delete blogger entries
                repository.DeleteGoogleBlogsProductPosts(productid);

                repository.DeleteInventoryLocationItems(productid);

                // set archive flag
                p.flags |= (int)ProductFlag.ARCHIVED;

                // update counters
                repository.UpdateCounters(subdomainid.Value, -1, CounterType.PRODUCTS_MINE);

                if (gbp != null)
                {
                    var gbase = new GoogleBaseExporter(subdomainid.Value, accountHostname, sessionid.Value);
                    gbase.DeleteFromGoogleBase(gbp.externalid);
                    repository.DeleteGoogleBaseProduct(gbp);
                }

                repository.Save();

                CacheHelper.Instance.invalidate_dependency(DependencyType.products_single, p.id.ToString());
                success.Add(productid);
            }

            try
            {
                repository.Save();
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
            }

            CacheHelper.Instance.invalidate_dependency(DependencyType.products_subdomain, subdomainid.ToString());

            return Json(success.ToJsonOKData());
        }

        private int? GetEbayCategoryID(int? ebay_cat_0, int? ebay_cat_1, int? ebay_cat_2, int? ebay_cat_3, 
            int? ebay_cat_4, int? ebay_cat_5)
        {
            if (ebay_cat_5.HasValue)
            {
                return ebay_cat_5.Value;
            }

            if (ebay_cat_4.HasValue)
            {
                return ebay_cat_4.Value;
            }

            if (ebay_cat_3.HasValue)
            {
                return ebay_cat_3.Value;
            }

            if (ebay_cat_2.HasValue)
            {
                return ebay_cat_2.Value;
            }

            if (ebay_cat_1.HasValue)
            {
                return ebay_cat_1.Value;
            }

            if (ebay_cat_0.HasValue)
            {
                return ebay_cat_0.Value;
            }

            return null;
        }

        private string GetTrademeCategoryID(string cat0, string cat1, string cat2, string cat3)
        {
            if (!string.IsNullOrEmpty(cat3))
            {
                return cat3;
            }

            if (!string.IsNullOrEmpty(cat2))
            {
                return cat2;
            }

            if (!string.IsNullOrEmpty(cat1))
            {
                return cat1;
            }

            if (!string.IsNullOrEmpty(cat0))
            {
                return cat0;
            }

            return null;
        }
        
        [HttpPost]
        [RoleFilter(role = UserRole.USER)]
        [ValidateInput(false)]
        public ActionResult Create(long? collection, IEnumerable<string> sku, string title, string details, long? maincategory, long? subcategory, 
            string stockUnit, string sellingPrice, string costPrice, string specialPrice,
            string notes, string photoIDs,
            // inventory
            long[] location,  string[] inStock, string[] reorderLevel, bool trackInventory,
            // autopost
            bool toEbay, bool toFB, bool toGoogle, bool toBlogger, bool toWordpress, bool toTumblr,
            bool toTrademe,
            // ebay
            int? ebay_cat_0, int? ebay_cat_1, int? ebay_cat_2, int? ebay_cat_3, int? ebay_cat_4, int? ebay_cat_5,
            SiteCodeType? ebay_site, int? ebay_condition, int? ebay_quantity, string ebay_refund_policy, long? ebayshippingprofile,
            string ebay_return_within, ReturnsAccepted? ebay_return_policy, string ebay_duration, bool? ebay_includeAddress,
            int? ebay_handling_time, bool? ebay_autorelist, ListingType? ebay_listingtype, decimal? ebay_startprice, decimal? ebay_buynowprice,
            decimal? ebay_reserveprice,
            // trademe
            string trademe_cat_0, string trademe_cat_1, string trademe_cat_2, string trademe_cat_3,
            bool? trademe_autorelist, bool? trademe_isnew, bool? trademe_authenticatedonly, bool? trademe_safetrader, decimal? trademe_startprice,
            decimal? trademe_reserveprice, decimal? trademe_buynowprice, bool? trademe_bold, bool? trademe_gallery, bool? trademe_featured,
            bool? trademe_homepage, ListingDuration? trademe_duration, int? trademe_quantity, PickupOption? trademe_pickup, string trademe_shippingcosts,
            decimal[] trademe_scost, string[] trademe_sdesc,
            // shipping
            long shippingprofile, decimal?[] shipping_cost, int?[] shipping_destination,
            ShippingProfileType? shippingType,
            // extra
            bool metric, decimal? weight, decimal? height, decimal? length, decimal? width, byte? shipwire_packing,  string[] color, string[] size,
            ShipwireCategory? shipwire_category, string shipwire_fragile, string shipwire_dangerous,
            string shipwire_perishable, string taxrate, string tags, IEnumerable<string> fbalbums, IEnumerable<string> fbstreams,
            // digital products
            long? digital_id, int? digital_limit, string digital_expiry
            )
        {
            Debug.Assert(shippingprofile != 0);

            var owner = sessionid.Value;

            if (accountLimits.skus.HasValue)
            {
                if (stats.products_mine >= accountLimits.skus.Value)
                {
                    return SendJsonErrorResponse("Product limit exceeded. Please upgrade your <a href=\"/dashboard/account/plan\">plan</a>.");
                } 
            }
            var variants = new List<product_variant>();
            foreach (var s in sku)
            {
                // create variants
                var variant = new product_variant {sku = s};
                variants.Add(variant);
            }
            
            // do verifications
            // check decimal places
            if (!string.IsNullOrEmpty(taxrate))
            {
                var decimalIndex = taxrate.IndexOf(".");
                if (decimalIndex != -1)
                {
                    if (taxrate.Length - decimalIndex - 1 > 2)
                    {
                        return
                        SendJsonErrorResponse("You're only allow 2 decimal places for your tax rate. For example, 12.50");
                    }
                }
            }

            var p = new product();
            try
            {
                p.subdomainid = subdomainid.Value;
                p.title = title;
                p.trackInventory = trackInventory;
                p.details = details;
                p.flags = (int)ProductFlag.NONE;
                
                p.otherNotes = notes;
                p.shippingProfileID = shippingprofile;
                if (!string.IsNullOrEmpty(tags))
                {
                    tags = tags.Trim();
                    var taglist = tags.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var entry in taglist)
                    {
                        var t = new tag();
                        t.name = entry.Trim();
                        t.handle = t.name.ToPerma();
                        t.subdomainid = subdomainid.Value;
                        p.tags1.Add(t);
                    }
                }

                if (!string.IsNullOrEmpty(stockUnit))
                {
                    p.stockUnitId = long.Parse(stockUnit);
                }

                if (!string.IsNullOrEmpty(sellingPrice))
                {
                    p.sellingPrice = decimal.Parse(sellingPrice,
                                                NumberStyles.AllowCurrencySymbol |
                                                NumberStyles.AllowDecimalPoint |
                                                NumberStyles.AllowThousands);
                }

                if (!string.IsNullOrEmpty(costPrice))
                {
                    p.costPrice = decimal.Parse(costPrice,
                                                NumberStyles.AllowCurrencySymbol |
                                                NumberStyles.AllowDecimalPoint |
                                                NumberStyles.AllowThousands);
                }

                if (!string.IsNullOrEmpty(specialPrice))
                {
                    p.specialPrice = decimal.Parse(specialPrice,
                                                NumberStyles.AllowCurrencySymbol |
                                                NumberStyles.AllowDecimalPoint |
                                                NumberStyles.AllowThousands);
                }

                p.otherNotes = notes;

                if (subcategory.HasValue && subcategory.Value > 0)
                {
                    p.category = subcategory;
                }
                else if (maincategory.HasValue && maincategory.Value > 0)
                {
                    p.category = maincategory;
                }

                // dimensions
                var dimension = new Dimension(weight, height, length, width, metric);
                p.dimensions = dimension.Serialize();

                // handle shipwire
                if (shipwire_packing.HasValue)
                {
                    var shipwire = new tradelr.Models.shipwire.Shipwire(shipwire_packing.Value,
                                                            shipwire_category.Value,
                                                            !string.IsNullOrEmpty(shipwire_fragile),
                                                            !string.IsNullOrEmpty(shipwire_dangerous),
                                                            !string.IsNullOrEmpty(shipwire_perishable));
                    p.shipwireDetails = shipwire.Serialize();
                }

                // handle variant stuff
                for (int i = 0; i < location.Length; i++)
                {
                    for (int j = 0; j < variants.Count; j++)
                    {
                        var variantcolor = color[j];
                        var variantsize = size[j];

                        var variant = variants[j];
                        variant.size = variantsize;
                        variant.color = variantcolor;
                        var locid = location[i];
                        var level = inStock[i * variants.Count + j];
                        var alarm = reorderLevel[i * variants.Count + j];

                        var inventoryloc = repository.GetInventoryLocation(locid, subdomainid.Value);
                        if (inventoryloc == null)
                        {
                            throw new ArgumentException("Unknown inventory location");
                        }

                        var inventoryItem = new inventoryLocationItem
                        {
                            locationid = locid
                        };
                        variant.inventoryLocationItems.Add(inventoryItem);

                        if (!string.IsNullOrEmpty(alarm))
                        {
                            int alarmLevel;
                            if (int.TryParse(alarm, out alarmLevel))
                            {
                                inventoryItem.alarmLevel = alarmLevel;
                            }
                        }

                        if (!string.IsNullOrEmpty(level))
                        {
                            int inventoryLevel;
                            if (int.TryParse(level, out inventoryLevel))
                            {
                                var invWorker = new InventoryWorker(inventoryItem, subdomainid.Value, trackInventory, digital_id.HasValue);
                                invWorker.SetValues("product created", inventoryLevel, null, null, null);
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(costPrice))
                {
                    p.costPrice = decimal.Parse(costPrice,
                                            NumberStyles.AllowCurrencySymbol |
                                            NumberStyles.AllowDecimalPoint |
                                            NumberStyles.AllowThousands);
                }

                if (!string.IsNullOrEmpty(taxrate))
                {
                    p.tax = decimal.Parse(taxrate);
                }
                else
                {
                    p.tax = null;
                }

                // add collection
                if (collection.HasValue)
                {
                    p.productCollectionMembers.Add(new productCollectionMember()
                    {
                        collectionid = collection.Value
                    });
                }
                

                // split photos
                var photos = photoIDs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                
                // verify external posts first
                #region verify ebay post
                EbayExporter ebayExporter = null;
                if (toEbay)
                {
                    // default to 1
                    if (!ebay_quantity.HasValue)
                    {
                        ebay_quantity = 1;
                    }

                    var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.EBAY, true);
                    if (token != null)
                    {
                        ebayExporter = new EbayExporter(ebay_site.Value, accountHostname, token.token_key, MASTERdomain);
                        var ebaycategory = GetEbayCategoryID(ebay_cat_0, ebay_cat_1, ebay_cat_2, ebay_cat_3, ebay_cat_4,
                                                             ebay_cat_5);
                        
                        // verify category
                        // detect non leaf category by checking if duration value exists
                        if (!ebaycategory.HasValue || string.IsNullOrEmpty(ebay_duration))
                        {
                            return Json("eBay category not selected".ToJsonFail());
                        }

                        // verify auction fields
                        if (ebay_listingtype.Value == ListingType.Chinese)
                        {
                            if (!ebay_startprice.HasValue)
                            {
                                return Json("You must specify a start price for auctions".ToJsonFail());
                            }

                            //  buynow price must be 10% higher
                            if (ebay_buynowprice.HasValue && ((double)ebay_buynowprice.Value / (double)ebay_startprice.Value) < 1.1)
                            {
                                return Json("Buy Now Price must be at least 10% higher than starting price".ToJsonFail());
                            }

                            // reserve price must be larger than starting price
                            if (ebay_reserveprice.HasValue && ebay_reserveprice.Value < ebay_startprice.Value)
                            {
                                return Json("Reserve Price cannot be less than the Start Price".ToJsonFail());
                            }
                        }

                        // verify shipping profile
                        var ebayprofile = MASTERdomain.ebay_shippingprofiles.SingleOrDefault(x => x.id == ebayshippingprofile);
                        if (ebayprofile == null)
                        {
                            return SendJsonErrorResponse("Could not find eBay shipping profile");
                        }

                        var respEbayshippingcheck = ebayprofile.HaveValidDomesticRules();
                        if (!respEbayshippingcheck.success)
                        {
                            return SendJsonErrorResponse(respEbayshippingcheck.message);
                        }

                        ebayExporter.BuildItem(p,
                            ebaycategory.Value,
                            ebay_quantity.Value,
                            ebay_condition,
                            ebay_return_policy.Value,
                            ebay_duration,
                            ebay_refund_policy,
                            ebay_return_within,
                            ebay_includeAddress.Value,
                            ebayshippingprofile.Value,
                            ebay_handling_time.Value,
                            ebay_autorelist.Value,
                            ebay_listingtype.Value,
                            ebay_startprice,
                            ebay_buynowprice,
                            ebay_reserveprice);

                        // verify
                        if (!ebayExporter.VerifyItem())
                        {
                            return SendJsonErrorResponse(ebayExporter.ErrorMessage);
                        }
                    }
                }
                #endregion

                #region verify trademe
                TrademeExporter trademeExporter = null;
                if (toTrademe)
                {
                    var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.TRADEME, true);
                    if (token != null)
                    {
                        trademeExporter = new TrademeExporter(accountHostname, token.token_key, token.token_secret, MASTERdomain);

                        var trademecategory = GetTrademeCategoryID(trademe_cat_0, trademe_cat_1, trademe_cat_2, trademe_cat_3);

                        // verify category
                        if (string.IsNullOrEmpty(trademecategory))
                        {
                            return Json("TradeMe category not selected".ToJsonFail());
                        }

                        // validate and set prices
                        var respString = trademeExporter.ValidateAndSetPrices(trademe_startprice, trademe_reserveprice, trademe_buynowprice);

                        if (!string.IsNullOrEmpty(respString))
                        {
                            return Json(respString.ToJsonFail());
                        }

                        // validate and set shipping
                        respString = trademeExporter.ValidateAndSetShipping(trademe_shippingcosts, trademe_scost, trademe_sdesc);
                        if (!string.IsNullOrEmpty(respString))
                        {
                            return Json(respString.ToJsonFail());
                        }

                        trademeExporter.BuildItem(p, 
                            trademecategory,
                            trademe_duration.Value,
                            trademe_pickup.Value,
                            trademe_isnew.Value,
                            trademe_authenticatedonly.Value,
                            trademe_quantity.Value,
                            photos,
                            trademe_safetrader.Value,
                            trademe_autorelist.Value,
                            trademe_bold.Value,
                            trademe_featured.Value,
                            trademe_homepage.Value,
                            trademe_gallery.Value);

                        if (!trademeExporter.VerifyItem())
                        {
                            return SendJsonErrorResponse(trademeExporter.ErrorMessage);
                        }
                    }
                }

                #endregion

                // insert into database
                p.product_variants.AddRange(variants);
                var productinfo = new ProductInfo()
                                      {
                                          p = p
                                      };
                repository.AddProduct(productinfo, subdomainid.Value);  //////////////// SUBMITTED

                // update digital products
                if (digital_id.HasValue)
                {
                    var digitalproduct = db.products_digitals.SingleOrDefault(x => x.id == digital_id.Value);
                    if (digitalproduct != null)
                    {
                        digitalproduct.limit = digital_limit;
                        if (!string.IsNullOrEmpty(digital_expiry))
                        {
                            digitalproduct.expiryDate = DateTime.Parse(digital_expiry);
                        }
                        digitalproduct.productid = p.id;
                    }
                }

                // update shipping
                if (shippingType.HasValue && shippingType.Value == ShippingProfileType.FLATRATE)
                {
                    ShippingProfile.UpdateFlatrateShipping(shippingprofile, shipping_cost, shipping_destination, subdomainid.Value);
                }

                /////// NEW THREAD: add activity log, check for abuse
                new Thread(() =>
                               {
                                   var abuse = new AbuseWorker(accountSubdomainName, p.id);
                                   abuse.CheckForRestrictedKeywords(p.details);
                                   using (var repo = new TradelrRepository())
                                   {
                                       repo.AddActivity(owner,
                                       new ActivityMessage(p.id, null,
                                                   ActivityMessageType.PRODUCT_NEW,
                                                   new HtmlLink(p.title, p.id).ToProductString()), subdomainid.Value);
                                   }
                               }).Start();

                // update total of out of stock items (scenario where no number in stock specified)
                repository.UpdateProductsOutOfStock(subdomainid.Value);

                // update images with product id
                repository.UpdateProductImages(subdomainid.Value, p.id, photos); //////////// SUBMIT

                if (photos.Length != 0)
                {
                    repository.UpdateProductMainThumbnail(p.id, subdomainid.Value, photos[0]);
                    if (!p.thumb.HasValue)
                    {
                        p.thumb = long.Parse(photos[0]);
                        repository.Save();
                    }
                }

                // get product photos for posting, we do't resize these
                IEnumerable<Photo> productPhotos = null;
                if (toBlogger || toGoogle || toFB || toWordpress || toTumblr)
                {
                    productPhotos = p.product_images.ToModel(Imgsize.LARGE);
                }

                #region post to blogger

                if (toBlogger)
                {
                    var blogger = new GoogleBlogExporter(p.MASTERsubdomain.bloggerSessionKey, accountHostname, repository, sessionid.Value);

                    blogger.FillBlogEntry(p);
                    blogger.AddPhotos(productPhotos);
                    foreach (var blog in p.MASTERsubdomain.googleBlogs)
                    {
                        var dest = new Uri(blog.blogUri);
                        
                        var posturl = blogger.PostBlogEntry(dest);
                        if (!string.IsNullOrEmpty(posturl))
                        {
                            var postEntry = new googleBlogsProductPost();
                            postEntry.productid = p.id;
                            postEntry.postUrl = posturl;
                            db.googleBlogsProductPosts.InsertOnSubmit(postEntry);
                        }
                        
                    }
                    repository.Save();
                } 
                #endregion

                #region post to eBay
                if (ebayExporter != null)
                {
                    new Thread(() => ebayExporter.AddEbayItem()).Start();
                }
                #endregion

                #region post to trademe
                if (trademeExporter != null)
                {
                    new Thread(() => trademeExporter.AddItem()).Start();
                }
                #endregion

                /*
                #region post to etsy
                if (toEtsy)
                {
                    var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.ETSY, true);
                    if (token != null)
                    {
                        var etsy = new EtsyWorker(token.token_key, token.token_secret, subdomainid.Value);
                        etsy.productPhotos = productPhotos;
                        new Thread(()=>etsy.PostNewProductToEtsy(p)).Start();
                    }
                }
                #endregion
                */
                #region post to facebook
                if (toFB)
                {
                    // post to stream
                    if (fbstreams != null)
                    {
                        foreach (var fbstreamid in fbstreams)
                        {
                            if (string.IsNullOrEmpty(fbstreamid))
                            {
                                var token = repository.GetOAuthToken(subdomainid.Value, sessionid.Value.ToString(), OAuthTokenType.FACEBOOK);
                                if (token != null)
                                {
                                    var facebook = new FacebookStreamPost(token.token_key, p, accountHostname);
                                    new Thread(facebook.PostToStream).Start();
                                }
                            }
                            else
                            {
                                var fbtoken =
                                    MASTERdomain.facebook_tokens.SingleOrDefault(x => x.id.ToString() == fbstreamid);
                                if (fbtoken != null)
                                {
                                    var facebook = new FacebookStreamPost(fbtoken.accesstoken, p, accountHostname);
                                    new Thread(facebook.PostToStream).Start();
                                }
                            }
                        }
                    }

                    // create album
                    if (fbalbums != null && productPhotos.Count() != 0)
                    {
                        foreach (var fbalbumid in fbalbums)
                        {
                            if (string.IsNullOrEmpty(fbalbumid))
                            {
                                var token = repository.GetOAuthToken(subdomainid.Value, sessionid.Value.ToString(), OAuthTokenType.FACEBOOK);
                                if (token != null)
                                {
                                    var facebook = new FacebookStreamPost(token.token_key, p, accountHostname);
                                    new Thread(() => facebook.CreateProductAlbum("me", productPhotos.Select(x => x.bigUrl)))
                                        .Start();
                                }
                            }
                            else
                            {
                                var fbtoken =
                                    MASTERdomain.facebook_tokens.SingleOrDefault(x => x.id.ToString() == fbalbumid);
                                if (fbtoken != null)
                                {
                                    var facebook = new FacebookStreamPost(fbtoken.accesstoken, p, accountHostname);
                                    new Thread(
                                        () =>
                                        facebook.CreateProductAlbum(fbtoken.pageid, productPhotos.Select(x => x.bigUrl))).
                                        Start();
                                }
                            }
                        }
                    }
                } 
                #endregion

                #region post to google product search
                if (toGoogle)
                {
                    var gbase = new GoogleBaseExporter(subdomainid.Value, accountHostname, sessionid.Value);
                    gbase.InitValues(p);
#if !DEBUG
                    // add images
                    gbase.AddProductImages(productPhotos);
#endif

                    var worker = new GoogleBaseWorker(gbase);
                    new Thread(worker.Post).Start();
                } 
                #endregion

                #region post to tumblr
                if (toTumblr)
                {
                    var tumblr = new TumblrExporter(p.MASTERsubdomain.tumblrSites.email,
                                                          p.MASTERsubdomain.tumblrSites.password,
                                                          accountHostname,
                                                          sessionid.Value,
                                                          subdomainid.Value);

                    tumblr.FillBlogEntry(p);
                    tumblr.AddPhotos(productPhotos);
                    var worker = new TumblrExporterWorker(tumblr);
                    new Thread(worker.Post).Start();
                }
                #endregion

                #region post to wordpress
                if (toWordpress)
                {
                    var wordpress = new WordpressExporter(accountHostname,sessionid.Value);
                    if (wordpress.AddCredentials(p.MASTERsubdomain.wordpressSites, subdomainid.Value))
                    {
                        wordpress.FillBlogEntry(p);
                        wordpress.AddPhotos(productPhotos);
                        var worker = new WordpressExporterWorker(wordpress);
                        new Thread(worker.Post).Start();
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            return Json(p.id.ToJsonOKData());
        }

        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_MODIFY)]
        public ActionResult Delete(string id, string ids)
        {
            var productids = new List<long>();
            var success = new List<long>();
            if (string.IsNullOrEmpty(id))
            {
                var idsarray = ids.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                productids.AddRange(idsarray.Select(long.Parse));
            }
            else
            {
                productids.Add(long.Parse(id));
            }

            foreach (var productid in productids)
            {
                // check that this product is current owners
                var p = repository.GetProduct(productid, subdomainid.Value);
                if (p == null)
                {
                    continue;
                }

                // delete data using this product
#if LUCENE
                var indexer = new LuceneWorker(db, MASTERdomain.ToIdName());
                indexer.DeleteFromIndex(LuceneIndexType.PRODUCTS, productid);
#endif
                // delete blogger entries
                repository.DeleteGoogleBlogsProductPosts(productid);

                // delete inventory locations
                repository.DeleteInventoryLocationItems(productid);

                // delete from carts that have not been checked out
                var variantids = p.product_variants.Select(x => x.id).ToArray();

                var cartitems = db.cartitems.Where(x => variantids.Contains(x.variantid) && !x.cart.orderid.HasValue);
                db.cartitems.DeleteAllOnSubmit(cartitems);
                repository.Save();

                // get reference
                var gbaseAtomId = p.gbase_product != null ? p.gbase_product.externalid : "";
                var wordpressPostid = p.wordpressPosts != null ? p.wordpressPosts.postid: (int?)null;
                var tumblrPostid = p.tumblrPosts != null? p.tumblrPosts.postid:"";

                // if product is in use then we just archive it
                if (p.IsInUse())
                {
                    p.flags |= (int)ProductFlag.ARCHIVED;
                }
                else
                {
                    repository.DeleteProduct(productid, subdomainid.Value); // actual product deleted
                }

                // then delete gbase entry
                if (!string.IsNullOrEmpty(gbaseAtomId))
                {
                    var gbase = new GoogleBaseExporter(subdomainid.Value, accountHostname, sessionid.Value);

                    // delete from gbase
                    new Thread(() => gbase.DeleteFromGoogleBase(gbaseAtomId)).Start();
                }

                if (wordpressPostid.HasValue)
                {
                    var wpi = new WordpressExporter(accountHostname,sessionid.Value);
                    if (wpi.AddCredentials(MASTERdomain.wordpressSites, subdomainid.Value))
                    {
                        var wordpressWorker = new WordpressExporterWorker(wpi);
                        new Thread(() => wordpressWorker.Delete(wordpressPostid.Value)).Start();
                    }
                }

                if (!string.IsNullOrEmpty(tumblrPostid))
                {
                    var tum = new TumblrExporter(MASTERdomain.wordpressSites.email,
                                                 MASTERdomain.wordpressSites.password,
                                                 accountHostname,
                                                 sessionid.Value,
                                                 subdomainid.Value,
                                                 tumblrPostid);
                    var tumblrWorker = new TumblrExporterWorker(tum);
                    new Thread(tumblrWorker.Delete).Start();
                }

                success.Add(productid);
            }

            repository.Save();

            if (success.Count == 0)
            {
                return Json("Product is in use".ToJsonFail());
            }
            
            return Json(success.ToJsonOKData());
        }

        [HttpPost]
        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_MODIFY)]
        public ActionResult DigitalDelete(long id)
        {
            var digital = db.products_digitals.SingleOrDefault(x => x.id == id);
            if (digital == null)
            {
                return Json("File not found".ToJsonFail());
            }

            // delete digital file
            FileHandler.Delete(digital.filepath);

            db.products_digitals.DeleteOnSubmit(digital);
            repository.Save();

            return Json("File deleted successfully".ToJsonOKMessage());
        }

        [HttpPost]
        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_MODIFY)]
        public ActionResult DigitalUpload(string qqfile, long? productid)
        {
            Stream inputStream;
            if (Request.Files.Count != 0)
            {
                inputStream = Request.Files[0].InputStream;
            }
            else
            {
                inputStream = Request.InputStream;
            }
            inputStream.Position = 0;

            // make it safe
            var file_name = Path.GetFileNameWithoutExtension(qqfile).ToSafeUrl() + Path.GetExtension(qqfile);

            var handler = new FileHandler(file_name, UploadFileType.DIGITAL, MASTERdomain.uniqueid);

            var url = handler.Save(inputStream);

            var digital = new products_digital
                              {
                                  filepath = url,
                                  filename = qqfile,
                                  productid = productid
                              };

            var attempts = 0;
            while (attempts++ < 10)
            {
                var uniqueid = Crypto.Utility.GetRandomString();
                if (!db.products_digitals.Any(x => x.linkid == uniqueid))
                {
                    digital.linkid = uniqueid;
                    break;
                }
            }

            if (string.IsNullOrEmpty(digital.linkid))
            {
                throw new Exception("Unable to generate unique ID");
            }

            db.products_digitals.InsertOnSubmit(digital);

            repository.Save();

            return Json(new { id = digital.id, url = url, name = qqfile, uniqueid = digital.linkid }.ToJsonOKData());
        }


        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_MODIFY)]
        [NoCache]
        public ActionResult Edit(long? id)
        {
            var usr = repository.GetUserById(sessionid.Value, subdomainid.Value);

            // for offline access
            if (!id.HasValue)
            {
                return View("add", GetProductViewModel(usr, null, null));
            }
            var p = repository.GetProduct(id.Value, subdomainid.Value);
            var product = p.ToModel(subdomainid.Value, id.Value, sessionid);
            if (product == null)
            {
                return View("add", GetProductViewModel(usr, null, null));
            }

            var viewmodel = GetProductViewModel(usr, product.trackInventory, product.shippingProfileID);
            viewmodel.editMode = true;
            viewmodel.product = product;

            // init dimensions
            product.dimension = p.dimensions.ToDimension();
            if (!((UserSettings)usr.settings).HasFlag(UserSettings.METRIC_VIEW))
            {
                product.dimension.ToImperial();
            }

            // get photos
            product.productPhotos = p.product_images.ToModel(Imgsize.MEDIUM);
            
            if (product.parentCategory.HasValue)
            {
                viewmodel.mainCategoryList = repository.GetProductCategories(null, subdomainid.Value)
                            .Select(x => new ProductCategory() { title = x.MASTERproductCategory.name, id = x.id })
                            .OrderBy(x => x.title)
                            .Select(x => new SelectListItem
                            {
                                Value = x.id.ToString(),
                                Text = x.title
                            }).ToSelectList(product.parentCategory, "None", "");

                viewmodel.subCategoryList = repository.GetProductCategories(product.parentCategory, subdomainid.Value)
                            .Select(x => new ProductCategory() { title = x.MASTERproductCategory.name, id = x.id })
                            .OrderBy(x => x.title)
                            .Select(x => new SelectListItem
                            {
                                Value = x.id.ToString(),
                                Text = x.title
                            }).ToSelectList(product.category, "None", "");
            }
            else
            {
                viewmodel.mainCategoryList = repository.GetProductCategories(null, subdomainid.Value)
                            .Select(x => new ProductCategory() { title = x.MASTERproductCategory.name, id = x.id })
                            .OrderBy(x => x.title)
                            .Select(x => new SelectListItem
                                 {
                                     Value = x.id.ToString(),
                                     Text = x.title
                                 }).ToSelectList(product.category, "None", "");
                if (product.category.HasValue)
                {
                    viewmodel.subCategoryList = repository.GetProductCategories(product.category, subdomainid.Value)
                            .Select(x => new ProductCategory() { title = x.MASTERproductCategory.name, id = x.id })
                            .OrderBy(x => x.title)
                            .Select(x => new SelectListItem
                            {
                                Value = x.id.ToString(),
                                Text = x.title
                            }).ToSelectList("", "None", "");
                }
            }
            
            viewmodel.stockUnitList = repository.GetAllStockUnits(subdomainid.Value)
                .OrderBy(x => x.MASTERstockUnit.name)
                .Select(x => new SelectListItem
                                 {
                                     Text = x.MASTERstockUnit.name,
                                     Value = x.id.ToString()
                                 }).ToSelectList(product.stockUnitId, "None", "");

            var inventorylocs = repository.GetInventoryLocationsExceptSyncNetworks(subdomainid.Value);

            // inventory locations are cached so we need to clear it
            viewmodel.product.inventoryLocations.Clear();
            foreach (var location in inventorylocs)
            {
                var loc = new InventoryLocation
                {
                    id = location.id,
                    title = location.name
                };
                foreach (var variant in viewmodel.product.variants)
                {
                    var inventoryitem = repository.GetInventoryLocationItem(variant.id.Value, location.id, subdomainid.Value);
                    if (inventoryitem == null)
                    {
                        loc.locationItems.Add(new InventoryLocationItem());
                    }
                    else
                    {
                        loc.locationItems.Add(inventoryitem.ToModel());
                    }
                    
                }
                viewmodel.product.inventoryLocations.Add(loc);
            }
            return View("Add", viewmodel);
        }

        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult Private(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return Json("No products selected".ToJsonFail());
            }

            var productids = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var products = repository.GetProducts(subdomainid.Value, productids, null, null);
            foreach (var product in products)
            {
                product.flags ^= (int)ProductFlag.INACTIVE;
                CacheHelper.Instance.invalidate_dependency(DependencyType.products_single, product.id.ToString());
            }

            try
            {
                repository.Save();
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            CacheHelper.Instance.invalidate_dependency(DependencyType.products_subdomain, subdomainid.ToString());
            
            return Json("".ToJsonOKData());
        }


        /// <summary>
        /// Gets list of products
        /// </summary>
        /// <param name="id">filterby owner id</param>
        /// <param name="cat">filterby product category</param>
        /// <param name="rows">entries per page</param>
        /// <param name="page">page index</param>
        /// <param name="sidx">column to use as search index</param>
        /// <param name="sord">sort order</param>
        /// <param name="alarm">to list out of stock items first</param>
        /// <param name="flag">product type</param>
        /// <param name="location"></param>
        /// <param name="collection"></param>
        /// <returns>returns jqgrid formated JSON data</returns>
        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_VIEW)]
        public ActionResult List(string cat, int rows, int page, string term,
                                 string sidx, string sord, string alarm, long? location, ProductFlag? flag, long? collection)
        {
            if (!string.IsNullOrEmpty(cat))
            {
                cat = cat.Trim();
            }

            // to select all categories
            if (cat == "All")
            {
                cat = "";
            }

            if (!flag.HasValue)
            {
                flag = ProductFlag.NONE;
            }

            IEnumerable<product> results = repository.GetProducts(subdomainid.Value, cat, sidx, sord, alarm, flag.Value, collection);
#if LUCENE
            if (!string.IsNullOrEmpty(term))
            {
                var search = new LuceneSearch();
                var ids = search.ProductSearch(term.ToLowerInvariant(), accountSubdomainName);
                results = results.Where(x => ids.Select(y => y.id).Contains(x.id.ToString())).AsEnumerable();
                results = results.Join(ids, x => x.id.ToString(), y => y.id, (x, y) => new { x, y.score })
                    .OrderByDescending(x => x.score).Select(x => x.x);
            }
#endif

            inventoryLocation iloc = null;
            if (!string.IsNullOrEmpty(alarm))
            {
                if (!location.HasValue)
                {
                    results =
                        results.OrderBy(
                            x => x.product_variants.SelectMany(y => y.inventoryLocationItems).Sum(y => y.available));
                }
                else
                {
                    results =
                        results.OrderBy(
                            x =>
                            x.product_variants.SelectMany(y => y.inventoryLocationItems).Where(
                                y => y.locationid == location).Sum(y => y.available));
                }
            }
            else
            {
                if (location.HasValue)
                {
                    // iloc can be null if sync network deleted but user has not refresh main page
                    iloc = repository.GetInventoryLocation(location.Value, subdomainid.Value);
                    results = results.Where(x => x.product_variants.SelectMany(y => y.inventoryLocationItems).Where(
                        y => y.locationid == location).Sum(y => y.available) != 0 ||
                        // show synced locations even though no product
                                                 (iloc != null && 
                                                 ((iloc.name == Networks.LOCATIONNAME_GBASE && x.gbase.HasValue) 
                                                 ||
                                                 (iloc.name == Networks.LOCATIONNAME_EBAY && x.ebayID.HasValue))));
                }
            }

            var records = results.Count();
            var total = (records/rows);
            if (records%rows != 0)
            {
                total++;
            }

            // return in the format required for jqgrid
            results = results.Skip(rows*(page - 1)).Take(rows);

            var products = results.ToProductJqGrid(iloc);
            products.page = page;
            products.records = records;
            products.total = total;

            return Json(products);
        }

        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_MODIFY)]
        public ActionResult PaymentTerms(string paymentTerms)
        {
            if (Request.HttpMethod == "POST")
            {
                MASTERdomain.paymentTerms = paymentTerms;
                repository.Save();
                return Json("".ToJsonOKMessage());
            }

            return View("paymentTerms", (object)MASTERdomain.paymentTerms);
        }

        [RoleFilter(role = UserRole.USER)]
        [HttpGet]
        [PermissionFilter(permission = UserPermission.INVENTORY_MODIFY)]
        public ActionResult Tax()
        {
            return View(MASTERdomain.default_taxrate);
        }

        [RoleFilter(role = UserRole.USER)]
        [HttpPost]
        [PermissionFilter(permission = UserPermission.INVENTORY_MODIFY)]
        public ActionResult Tax(decimal? taxrate)
        {
            MASTERdomain.default_taxrate = taxrate;
            repository.Save();
            return Json("".ToJsonOKMessage());
        }

        [HttpPost]
        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_VIEW)]
        public ActionResult Transactions(long id)
        {
            var p = repository.GetProduct(id, subdomainid.Value);

            var viewmodel = new ProductTransactionsViewModel();
            viewmodel.productid = p.id;
            
            return View(viewmodel);
        }

        [HttpPost]
        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_VIEW)]
        public ActionResult TransactionsContent(long id, string term)
        {
            
            IEnumerable<LuceneHit> ids = null;
#if LUCENE
            if (!string.IsNullOrEmpty(term))
            {
                var search = new LuceneSearch();
                ids = search.ContactSearch(term.ToLowerInvariant(), accountSubdomainName);
            }
#endif
            var viewmodel = new List<ProductVariantTransaction>();
            var variants = repository.GetProduct(id, subdomainid.Value).product_variants;

            foreach (var variant in variants)
            {
                var variantid = variant.id;
                var invoiceitems = repository.GetOrders()
                    .Where(x => x.type == TransactionType.INVOICE.ToString())
                    .SelectMany(x => x.orderItems).Where(x => x.variantid == variantid);

                if (ids != null)
                {
                    invoiceitems = invoiceitems.Where(x => ids.Select(y => y.id).Contains(x.order.receiverUserid.ToString()));
                }

                var sold = invoiceitems
                    .GroupBy(x => new {x.unitPrice, x.order.user, x.order.currency})
                    .Select(y => y.Key)
                    .ToArray()
                    .Select(z => new ProductTransaction()
                                     {
                                         contactName = z.user.ToFullName(),
                                         contactUrl =
                                             string.Format("{0}{1}", GeneralConstants.URL_SINGLE_CONTACT, z.user.id),
                                         unitPrice =
                                             z.unitPrice.HasValue
                                                 ? z.unitPrice.Value.ToString("n" + z.currency.ToCurrency().decimalCount)
                                                 : ""
                                     });

                var orderitems = repository.GetOrders()
                    .Where(x => x.type == TransactionType.ORDER.ToString())
                    .SelectMany(x => x.orderItems).Where(x => x.variantid == variantid);

                if (ids != null)
                {
                    orderitems = orderitems.Where(x => ids.Select(y => y.id).Contains(x.order.receiverUserid.ToString()));
                }

                var bought = orderitems
                    .GroupBy(x => new {x.unitPrice, x.order.user, x.order.currency})
                    .Select(y => y.Key)
                    .ToArray()
                    .Select(z => new ProductTransaction()
                                     {
                                         contactName = z.user.ToFullName(),
                                         contactUrl =
                                             string.Format("{0}{1}", GeneralConstants.URL_SINGLE_CONTACT, z.user.id),
                                         unitPrice =
                                             z.unitPrice.HasValue
                                                 ? z.unitPrice.Value.ToString("n" + z.currency.ToCurrency().decimalCount)
                                                 : ""
                                     });

                var transaction = new ProductVariantTransaction();
                transaction.variant_name = variant.ToProductFullTitle();
                transaction.sku = variant.sku;
                transaction.products_sold = sold;
                transaction.products_bought = bought;
                viewmodel.Add(transaction);
            }

            return View(viewmodel);
        }

        [HttpPost]
        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_MODIFY)]
        [ValidateInput(false)]
        public ActionResult Update(string maincategory, string subcategory, string details, long id, string[] sku, 
            string title, string stockUnit, string sellingPrice, string costPrice, string specialPrice,
            string notes, string photoIDs, string[] color, string[] size, string[] sku_old, string[] variantid,
            string defaultPhotoID, bool? metric, decimal? weight,
            // inventory
            string[] inStock, string[] reorderLevel, long[] location, bool trackInventory,
            // autopost
            bool toWordpress, bool toTumblr, bool toEbay, bool toGoogle, bool toBlogger, 
            bool toTrademe,
            // ebay - nullable because if ebay not sync, they will be missing
            int? ebay_cat_0, int? ebay_cat_1, int? ebay_cat_2, int? ebay_cat_3, int? ebay_cat_4, int? ebay_cat_5, int? ebay_handling_time,
            SiteCodeType? ebay_site, int? ebay_condition, int? ebay_quantity, string ebay_refund_policy, long? ebayshippingprofile,
            string ebay_return_within, ReturnsAccepted? ebay_return_policy, string ebay_duration, bool? ebay_includeAddress, bool? ebay_relist,
            bool? ebay_autorelist, ListingType? ebay_listingtype, decimal? ebay_startprice, decimal? ebay_buynowprice, decimal? ebay_reserveprice,
            // trademe
            string trademe_cat_0, string trademe_cat_1, string trademe_cat_2, string trademe_cat_3,
            bool? trademe_autorelist, bool? trademe_isnew, bool? trademe_authenticatedonly, bool? trademe_safetrader, decimal? trademe_startprice,
            decimal? trademe_reserveprice, decimal? trademe_buynowprice, bool? trademe_bold, bool? trademe_gallery, bool? trademe_featured,
            bool? trademe_homepage, ListingDuration? trademe_duration, int? trademe_quantity, PickupOption? trademe_pickup, string trademe_shippingcosts,
            decimal[] trademe_scost, string[] trademe_sdesc, bool? trademe_relist,
            // extras
            decimal? height, decimal? length, decimal? width, byte? shipwire_packing, ShipwireCategory? shipwire_category,
            string shipwire_fragile, string shipwire_dangerous, string shipwire_perishable, 
            string taxrate,
            // shipping profile
            long shippingprofile, string tags, decimal?[] shipping_cost, int?[] shipping_destination,
            ShippingProfileType? shippingType, 
            // digital products
            long? digital_id, int? digital_limit, string digital_expiry)
        {
            Debug.Assert(shippingprofile != 0);

            long owner = sessionid.Value;
            try
            {
                var existing = repository.GetProduct(id, subdomainid.Value);
                if (existing == null)
                {
                    return SendJsonErrorResponse("Bad Request");
                }

                existing.details = details;
                existing.trackInventory = trackInventory;

                // handle category
                if (subcategory != null || maincategory != null)
                {
                    if (!string.IsNullOrEmpty(subcategory))
                    {
                        existing.category = long.Parse(subcategory);
                    }
                    else if (!string.IsNullOrEmpty(maincategory))
                    {
                        existing.category = long.Parse(maincategory);
                    }
                    else
                    {
                        existing.category = null;
                    }
                }

                if (title != null)
                {
                    existing.title = title;
                }

                // dimensions
                var dimension = new Dimension(weight, height, length, width, metric ?? true);
                existing.dimensions = dimension.Serialize();

                // handle digital goods
                var digitalproduct = existing.products_digitals;
                if (!digital_id.HasValue)
                {
                    // nothing to do here. delete handle by digital product delete button
                }
                else
                {
                    if (digitalproduct == null)
                    {
                        digitalproduct = new products_digital();
                        existing.products_digitals = digitalproduct;
                    }
                    digitalproduct.limit = digital_limit;
                    if (!string.IsNullOrEmpty(digital_expiry))
                    {
                        digitalproduct.expiryDate = DateTime.Parse(digital_expiry);
                    }
                }

                // handle shipwire
                if (shipwire_packing.HasValue)
                {
                    var shipwire = new tradelr.Models.shipwire.Shipwire(shipwire_packing.Value,
                                                            shipwire_category.Value,
                                                            !string.IsNullOrEmpty(shipwire_fragile),
                                                            !string.IsNullOrEmpty(shipwire_dangerous),
                                                            !string.IsNullOrEmpty(shipwire_perishable));
                    existing.shipwireDetails = shipwire.Serialize();
                }

                // don't think there's a way to delete selected default photo
                if (!string.IsNullOrEmpty(defaultPhotoID))
                {
                    existing.thumb = long.Parse(defaultPhotoID);
                }
                else if (defaultPhotoID == "")
                {
                    existing.thumb = null;
                }

                // handle variant stuff
                if (location != null)
                {
                    for (int i = 0; i < location.Length; i++)
                    {
                        for (int j = 0; j < sku.Length; j++)
                        {
                            var vid = variantid[j];
                            var variantsku = sku[j];
                            var oldsku = sku_old[j];
                            string variantcolor = "";
                            try
                            {
                                variantcolor = color[j];
                            }
                            catch (Exception ex)
                            {
                                Syslog.Write(ex);
                            }
                            string variantsize = "";
                            try
                            {
                                variantsize = size[j];
                            }
                            catch (Exception ex)
                            {
                                Syslog.Write(ex);
                            }

                            product_variant variant = existing.product_variants.SingleOrDefault(x => x.id.ToString() == vid);
                            if (variantsku == oldsku)
                            {
                                // do nothing
                            }
                            else if (string.IsNullOrEmpty(vid)) // new entry
                            {
                                // covers situation where user adds new variant -> save -> save again
                                variant = existing.product_variants.Where(x => x.sku == variantsku).SingleOrDefault();
                                if (variant == null) 
                                {
                                    variant = new product_variant { sku = variantsku, productid = existing.id };
                                    existing.product_variants.Add(variant);
                                    repository.Save();
                                }
                            }
                            else if (variantsku != oldsku)
                            {
                                // entry rename
                                variant.sku = variantsku;
                            }
                            if (variant == null)
                            {
                                // handles empty rows
                                continue;
                            }

                            variant.size = variantsize;
                            variant.color = variantcolor;

                            var locid = location[i];
                            var level = inStock[i * sku.Length + j];
                            var alarm = reorderLevel[i * sku.Length + j];

                            var inventoryloc = repository.GetInventoryLocation(locid, subdomainid.Value);
                            if (inventoryloc == null)
                            {
                                throw new ArgumentException("Unknown inventory location");
                            }

                            var inventoryItem =
                                inventoryloc.inventoryLocationItems.SingleOrDefault(x => x.variantid == variant.id && x.locationid == locid);
                            if (inventoryItem == null)
                            {
                                inventoryItem = new inventoryLocationItem
                                {
                                    locationid = locid
                                };
                                variant.inventoryLocationItems.Add(inventoryItem);
                            }

                            if (!string.IsNullOrEmpty(level))
                            {
                                int inventoryLevel;
                                if (int.TryParse(level, out inventoryLevel))
                                {
                                    var invWorker = new InventoryWorker(inventoryItem, subdomainid.Value, trackInventory, digital_id.HasValue);
                                    invWorker.SetValues("variant added", inventoryLevel, null, null, null);
                                }
                            }

                            if (!string.IsNullOrEmpty(alarm))
                            {
                                int alarmLevel;
                                if (int.TryParse(alarm, out alarmLevel))
                                {
                                    inventoryItem.alarmLevel = alarmLevel;
                                }
                            }

                            // after submit because we want changes to be saved first
                            if (inventoryItem.available <= inventoryItem.alarmLevel)
                            {
                                repository.AddActivity(sessionid.Value,
                                                       new ActivityMessage(existing.id, sessionid.Value,
                                                                   ActivityMessageType.PRODUCT_ALARM,
                                                                   inventoryItem.inventoryLocation.name,
                                                                   new HtmlLink(existing.title, existing.id).
                                                                       ToProductString()), subdomainid.Value);
                            }
                        }
                    }
                }

                if (stockUnit != null)
                {
                    if (stockUnit != "")
                    {
                        existing.stockUnitId = long.Parse(stockUnit);
                    }
                    else
                    {
                        existing.stockUnitId = null;
                    }
                }
                if (sellingPrice != null)
                {
                    if (sellingPrice != "")
                    {
                        existing.sellingPrice = Decimal.Parse(sellingPrice,
                                                    NumberStyles.AllowCurrencySymbol |
                                                    NumberStyles.AllowDecimalPoint |
                                                    NumberStyles.AllowThousands);
                    }
                    else
                    {
                        existing.sellingPrice = null;
                    }
                }
                
                if (costPrice != null)
                {
                    if (costPrice != "")
                    {
                        existing.costPrice = Decimal.Parse(costPrice,
                                                    NumberStyles.AllowCurrencySymbol |
                                                    NumberStyles.AllowDecimalPoint |
                                                    NumberStyles.AllowThousands);
                    }
                    else
                    {
                        existing.costPrice = null;
                    }
                }

                if (specialPrice != null)
                {
                    if (specialPrice != "")
                    {
                        existing.specialPrice = Decimal.Parse(specialPrice,
                                                    NumberStyles.AllowCurrencySymbol |
                                                    NumberStyles.AllowDecimalPoint |
                                                    NumberStyles.AllowThousands);
                    }
                    else
                    {
                        existing.specialPrice = null;
                    }
                }

                if (!string.IsNullOrEmpty(taxrate))
                {
                    var decimalIndex = taxrate.IndexOf(".");
                    if (decimalIndex != -1)
                    {
                        if (taxrate.Length - decimalIndex - 1 > 2)
                        {
                            return
                            SendJsonErrorResponse("You're only allow 2 decimal places for your tax rate. For example, 12.50");
                        }
                    }
                    existing.tax = decimal.Parse(taxrate);
                }
                else
                {
                    existing.tax = null;
                }

                if (notes != null)
                {
                    existing.otherNotes = notes;
                }

                // handle shipping
                existing.shippingProfileID = shippingprofile;
                if (shippingType.HasValue && shippingType.Value == ShippingProfileType.FLATRATE)
                {
                    ShippingProfile.UpdateFlatrateShipping(shippingprofile, shipping_cost, shipping_destination, subdomainid.Value);
                }

                // split photos
                var photos = photoIDs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                
                // handle tags
                db.tags.DeleteAllOnSubmit(existing.tags1);
                if (!string.IsNullOrEmpty(tags))
                {
                    tags = tags.Trim();
                    var taglist = tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var entry in taglist)
                    {
                        var t = new tag();
                        t.name = entry.Trim();
                        t.handle = t.name.ToPerma();
                        t.subdomainid = subdomainid.Value;
                        existing.tags1.Add(t);
                    }
                }
                existing.updated = DateTime.UtcNow;

                // verify posts
                EbayExporter ebayExporter = null;
                if (toEbay)
                {
                    // default to 1
                    if (!ebay_quantity.HasValue)
                    {
                        ebay_quantity = 1;
                    }

                    var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.EBAY, true);
                    if (token != null)
                    {
                        ebayExporter = new EbayExporter(ebay_site.Value, accountHostname, token.token_key, MASTERdomain);
                        var ebaycategory = GetEbayCategoryID(ebay_cat_0, ebay_cat_1, ebay_cat_2, ebay_cat_3, ebay_cat_4,
                                                             ebay_cat_5);
                        // using duration to detect leaf category
                        if (!ebaycategory.HasValue || string.IsNullOrEmpty(ebay_duration))
                        {
                            return Json("eBay category not selected".ToJsonFail());
                        }

                        // verify auction fields
                        if (ebay_listingtype.Value == ListingType.Chinese)
                        {
                            if (!ebay_startprice.HasValue)
                            {
                                return Json("You must specify a start price for auctions".ToJsonFail());
                            }

                            //  buynow price must be 10% higher
                            if (ebay_buynowprice.HasValue && ((double)ebay_buynowprice.Value / (double)ebay_startprice.Value) < 1.1)
                            {
                                return Json("Buy Now Price must be at least 10% higher than starting price".ToJsonFail());
                            }

                            // reserve price must be larger than starting price
                            if (ebay_reserveprice.HasValue && ebay_reserveprice.Value <= ebay_startprice.Value)
                            {
                                return Json("Reserve Price must be higher than the Start Price".ToJsonFail());
                            }
                        }

                        // verify shipping profile
                        var ebayprofile = MASTERdomain.ebay_shippingprofiles.SingleOrDefault(x => x.id == ebayshippingprofile);
                        if (ebayprofile == null)
                        {
                            return SendJsonErrorResponse("Could not find eBay shipping profile");
                        }

                        var respEbayshippingcheck = ebayprofile.HaveValidDomesticRules();
                        if (!respEbayshippingcheck.success)
                        {
                            return SendJsonErrorResponse(respEbayshippingcheck.message);
                        }

                        ebayExporter.BuildItem(existing,
                            ebaycategory.Value,
                            ebay_quantity.Value,
                            ebay_condition,
                            ebay_return_policy.Value,
                            ebay_duration,
                            ebay_refund_policy,
                            ebay_return_within,
                            ebay_includeAddress.Value,
                            ebayshippingprofile.Value,
                            ebay_handling_time.Value,
                            ebay_autorelist.Value,
                            ebay_listingtype.Value,
                            ebay_startprice,
                            ebay_buynowprice,
                            ebay_reserveprice);

                        // can only verify for new products or is a relist
                        if (!existing.ebayID.HasValue || 
                            (ebay_relist.HasValue && ebay_relist.Value))
                        {
                            if (!ebayExporter.VerifyItem())
                            {
                                return SendJsonErrorResponse(ebayExporter.ErrorMessage);
                            }
                        }
                    }
                }

                TrademeExporter trademeExporter = null;
                if (toTrademe)
                {
                    var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.TRADEME, true);
                    if (token != null)
                    {
                        trademeExporter = new TrademeExporter(accountHostname, token.token_key, token.token_secret, MASTERdomain);

                        var trademecategory = GetTrademeCategoryID(trademe_cat_0, trademe_cat_1, trademe_cat_2, trademe_cat_3);

                        // verify category
                        if (string.IsNullOrEmpty(trademecategory))
                        {
                            return Json("TradeMe category not selected".ToJsonFail());
                        }

                        // validate and set prices
                        var respString = trademeExporter.ValidateAndSetPrices(trademe_startprice, trademe_reserveprice, trademe_buynowprice);

                        if (!string.IsNullOrEmpty(respString))
                        {
                            return Json(respString.ToJsonFail());
                        }

                        // validate and set shipping
                        respString = trademeExporter.ValidateAndSetShipping(trademe_shippingcosts, trademe_scost, trademe_sdesc);
                        if (!string.IsNullOrEmpty(respString))
                        {
                            return Json(respString.ToJsonFail());
                        }

                        trademeExporter.BuildItem(existing,
                            trademecategory,
                            trademe_duration.Value,
                            trademe_pickup.Value,
                            trademe_isnew.Value,
                            trademe_authenticatedonly.Value,
                            trademe_quantity.Value,
                            photos,
                            trademe_safetrader.Value,
                            trademe_autorelist.Value,
                            trademe_bold.Value,
                            trademe_featured.Value,
                            trademe_homepage.Value,
                            trademe_gallery.Value);

                        if (!trademeExporter.VerifyItem())
                        {
                            return SendJsonErrorResponse(trademeExporter.ErrorMessage);
                        }
                    }
                }

                repository.Save("product.update"); /////////////// SUBMIT
                CacheHelper.Instance.invalidate_dependency(DependencyType.products_subdomain, subdomainid.Value.ToString());
                CacheHelper.Instance.invalidate_dependency(DependencyType.products_single, id.ToString());
#if LUCENE
                // index product
                var indexer = new LuceneWorker(db, MASTERdomain.ToIdName());
                indexer.AddToIndex(LuceneIndexType.PRODUCTS, existing);
#endif
                // get and save differences
                // remove support for product change history
                /*
                var changed = existing.ToTrackChangesModel();
                var comparer = new CompareObject();
                var diff = comparer.Compare(original, changed);
                if (diff.Count != 0)
                {
                    repository.AddChangeHistory(sessionid.Value, existing.id, ChangeHistoryType.PRODUCT, diff);
                }
                */
                repository.UpdateProductsOutOfStock(existing.subdomainid);

                // update images with product id
                if (photos.Length != 0)
                {
                    repository.UpdateProductImages(subdomainid.Value, id, photos);
                    if (photos.Length != 0 && !existing.thumb.HasValue)
                    {
                        existing.thumb = long.Parse(photos[0]);
                    }
                }

                repository.Save();

                // NEW THREAD: add activity log, check abuse
                new Thread(() =>
                {
                    var abuse = new AbuseWorker(accountSubdomainName, existing.id);
                    abuse.CheckForRestrictedKeywords(existing.details);
                    using (var repo = new TradelrRepository())
                    {
                        repo.AddActivity(owner,
                        new ActivityMessage(id, null,
                            ActivityMessageType.PRODUCT_UPDATED,
                             new HtmlLink(existing.title, existing.id).ToProductString()), subdomainid.Value);
                    }
                }).Start();

                IEnumerable<Photo> productPhotos = null;
                if (toBlogger || toGoogle || toWordpress || toTumblr)
                {
                    productPhotos = existing.product_images.ToModel(Imgsize.LARGE);
                }

                #region post to eBay
                if (ebayExporter != null)
                {
                    if (!existing.ebayID.HasValue || 
                        (ebay_relist.HasValue && ebay_relist.Value))
                    {
                        new Thread(() => ebayExporter.AddEbayItem()).Start();
                    }
                    else
                    {
                        if (existing.ebay_product.isActive)
                        {
                            ebayExporter.UpdateEbayItem(existing.ebay_product.ebayid);
                        }
                    }
                }
                #endregion

                #region post to trademe
                if (trademeExporter != null)
                {
                    if (!existing.trademeID.HasValue ||
                        (trademe_relist.HasValue && trademe_relist.Value))
                    {
                        new Thread(() => trademeExporter.AddItem()).Start();
                    }
                    else
                    {
                        if (existing.trademe_product.isActive)
                        {
                            trademeExporter.UpdateItem(existing.trademe_product.listingid);
                        }
                    }
                }
                #endregion
                /*
                #region post to etsy

                if (toEtsy)
                {
                    var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.ETSY, true);
                    if (token != null)
                    {
                        EtsyWorker etsy = new EtsyWorker(token.token_key, token.token_secret, subdomainid.Value);
                        if (!existing.etsy.HasValue)
                        {
                            etsy.productPhotos = productPhotos;
                            new Thread(() => etsy.PostNewProductToEtsy(existing)).Start();
                        }
                        else
                        {
                            new Thread(() => etsy.UpdateEtsyListing(existing)).Start();
                        }
                    }
                    
                }
                #endregion
                */
                
                #region to google product search

                var newProductEntry = false;
                if (toGoogle)
                {
                    var gbase = new GoogleBaseExporter(subdomainid.Value, accountHostname, sessionid.Value);
                    if (!existing.gbase.HasValue)
                    {
                        gbase.InitValues(existing);
                        newProductEntry = true;
                    }
                    else
                    {
                        if (!gbase.GetAndUpdateFromGoogleBase(existing))
                        {
                            gbase = new GoogleBaseExporter(subdomainid.Value, accountHostname, sessionid.Value);
                            gbase.InitValues(existing);
                            newProductEntry = true;
                        }
                    }
#if !DEBUG
                    // add images
                    gbase.AddProductImages(productPhotos);
#endif
                    var worker = new GoogleBaseWorker(gbase);
                    if (newProductEntry)
                    {
                        new Thread(worker.Post).Start();
                    }
                    else
                    {
                        new Thread(worker.Update).Start();
                    }
                }
                #endregion

                #region to blooger
                if (toBlogger)
                {
                    var blogger = new GoogleBlogExporter(existing.MASTERsubdomain.bloggerSessionKey, accountHostname, repository, sessionid.Value);

                    bool haveExistingPosts = false;
                    foreach (var entry in existing.googleBlogsProductPosts)
                    {
                        haveExistingPosts = true;
                        try
                        {
                            blogger.GetBlogEntry(existing, entry.postUrl);
                            blogger.AddPhotos(productPhotos);
                            blogger.UpdateBlogEntry();
                        }
                        catch (Exception ex)
                        {
                            Syslog.Write(ex);
                        }
                    }
                    
                    // user did not previously enable post to blogger
                    if (!haveExistingPosts)
                    {
                        blogger.FillBlogEntry(existing);
                        blogger.AddPhotos(productPhotos);
                        foreach (var blog in existing.MASTERsubdomain.googleBlogs)
                        {
                            var dest = new Uri(blog.blogUri);
                            var posturl = blogger.PostBlogEntry(dest);
                            
                            if (!string.IsNullOrEmpty(posturl))
                            {
                                var postEntry = new googleBlogsProductPost();
                                postEntry.productid = existing.id;
                                postEntry.postUrl = posturl;
                                db.googleBlogsProductPosts.InsertOnSubmit(postEntry);
                            }
                        }
                    }

                    repository.Save();
                }
                #endregion

                #region post to tumblr
                if (toTumblr)
                {
                    var tumblr = new TumblrExporter(existing.MASTERsubdomain.tumblrSites.email,
                                                          existing.MASTERsubdomain.tumblrSites.password,
                                                          accountHostname,
                                                          sessionid.Value,
                                                          subdomainid.Value,
                                                          existing.tumblrPosts != null ? existing.tumblrPosts.postid:"");
                    tumblr.FillBlogEntry(existing);
                    tumblr.AddPhotos(productPhotos);
                    var worker = new TumblrExporterWorker(tumblr);
                    new Thread(worker.Post).Start();
                }
                #endregion

                #region post to wordpress
                if (toWordpress)
                {
                    var wordpress = new WordpressExporter(accountHostname, sessionid.Value);
                    if (wordpress.AddCredentials(existing.MASTERsubdomain.wordpressSites, subdomainid.Value))
                    {
                        bool haveExistingPosts = false;
                        if (existing.wordpressPosts != null)
                        {
                            haveExistingPosts = true;
                            wordpress.GetBlogEntry(existing, existing.wordpressPosts.postid);
                            wordpress.AddPhotos(productPhotos);
                            var worker = new WordpressExporterWorker(wordpress);
                            new Thread(worker.Update).Start();
                        }

                        // user did not previously enable post to wordpress
                        if (!haveExistingPosts)
                        {
                            wordpress.FillBlogEntry(existing);
                            wordpress.AddPhotos(productPhotos);
                            var worker = new WordpressExporterWorker(wordpress);
                            new Thread(worker.Post).Start();
                        }
                    }
                }
                #endregion
            }
            catch(FacebookException ex)
            {
                Syslog.Write(ex);
                return Json("Product not posted to Facebook. Permission denied.".ToJsonFail());
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            return Json(id.ToJsonOKData());
        }


        [HttpGet]
        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_VIEW)]
        public ActionResult Variants(string term, long? sinceid)
        {
            var options = new VariantsContentOptions();
            options.sinceid = sinceid;
            options.term = term;
            return View(options);
        }

        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_VIEW)]
        public ActionResult VariantsContent(string term, long? sinceid)
        {
            IEnumerable<product> products = repository.GetProducts(subdomainid.Value);
            if (!string.IsNullOrEmpty(term))
            {
                var search = new LuceneSearch();
                var product_subdomain = accountSubdomainName;
                var ids = search.ProductSearch(term, product_subdomain);
                products = products.Where(x => ids.Select(y => y.id).Contains(x.id.ToString())).AsEnumerable();
                products = products.Join(ids, x => x.id.ToString(), y => y.id, (x, y) => new { x, y.score })
                        .OrderByDescending(x => x.score).Select(x => x.x);
            }

            if (sinceid.HasValue)
            {
                products = products.Where(x => x.id > sinceid);
            }
            products = products.OrderBy(x => x.id).Take(20);
            var viewmodel = products.ToBaseModel();
            return View(viewmodel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">variant ID</param>
        /// <returns></returns>
        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_MODIFY)]
        public ActionResult VariantsDelete(long id)
        {
            var variant = repository.GetProductVariant(id, subdomainid.Value);
            if (variant == null)
            {
                return Json("Unable to find variant to delete".ToJsonFail());
            }

            // check that not in use
            if (repository.IsProductVariantInUse(id, subdomainid.Value))
            {
                return Json("Unable to delete variant. Variant is in use".ToJsonFail());
            }

            try
            {
                repository.DeleteProductVariant(variant);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            return Json("".ToJsonOKMessage());
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TradeMe;
using TradeMe.models;
using TradeMe.services;
using api.trademe.co.nz.v1;
using tradelr.Common.Library.Imaging;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.Constants;
using clearpixels.Logging;
using PaymentMethod = tradelr.Library.payment.PaymentMethod;

namespace tradelr.Models.export.trademe
{
    public class TrademeExporter : ExportItem
    {
        private ListingRequest item;
        private SellingService service;
        private MASTERsubdomain sd;
        private trademe_product trademeproduct;

        private string key;
        private string secret;

        public TrademeExporter(string hostname, string key, string secret, MASTERsubdomain sd)
            : this(hostname)
        {
            service = new SellingService(key,secret);
            this.sd = sd;
            this.key = key;
            this.secret = secret;
        }

        private TrademeExporter(string hostname) : base(hostname)
        {
            trademeproduct = new trademe_product();
           
        }

        public void AddItem()
        {
            var response = service.CreateListing(new CreateListingRequest(item));

            if (response.CreateListingResult.Success)
            {
                // insert into database
                using (var repository = new TradelrRepository())
                {
                    var product = repository.GetProduct(ProductId);
                    product.trademe_product = trademeproduct;

                    product.trademe_product.listingid = response.CreateListingResult.ListingId;
                    product.trademe_product.startTime = DateTime.UtcNow;
                    product.trademe_product.endTime = DateTime.UtcNow.AddDays(trademeproduct.duration);
                    product.trademe_product.isActive = true;

                    repository.Save("Trademe.AddItem");
                }
            }
            else
            {
                Syslog.Write(response.CreateListingResult.Description);
            }

            
        }

        public void BuildItem(product p, string category, ListingDuration duration, PickupOption pickup, bool isBrandNew, 
            bool authenticatedMembersOnly, int quantity, string[] photoids, bool safetrader, bool autorelist,
            // promotion
            bool isBold, bool isFeatured, bool isHomepageFeatured, bool hasGallery)
        {
            InitValues(p, 1, sd);

            trademeproduct.categoryid = category;
            trademeproduct.duration = (byte)duration.ToInt();
            trademeproduct.pickup = (byte) pickup.ToInt();
            trademeproduct.isnew = isBrandNew;
            trademeproduct.authenticatedOnly = authenticatedMembersOnly;
            trademeproduct.isclassified = false;
            trademeproduct.quantity = quantity;
            trademeproduct.isBold = isBold;
            trademeproduct.isFeatured = isFeatured;
            trademeproduct.isHomepageFeatured = isHomepageFeatured;
            trademeproduct.hasGallery = hasGallery;
            trademeproduct.flatShipping = true;
            trademeproduct.autorelist = autorelist;
            trademeproduct.safetrader = safetrader;

            item.Category = category;
            item.Title = Title;

            // convert paragraph and divs into new lines
            //item.Description.Add(Description);

            
            item.Duration = duration;
            item.Pickup = pickup;
            item.IsBrandNew = isBrandNew;
            item.AuthenticatedMembersOnly = authenticatedMembersOnly;
            item.IsClassified = false;

            // extras
            item.IsBold = isBold;
            item.IsFeatured = isFeatured;
            item.IsHomepageFeatured = isHomepageFeatured;
            item.HasGallery = hasGallery;

            // of course, always
            item.SendPaymentInstructions = true;

            item.HasAgreedWithLegalNotice = true;
            
            //item.PhotoIds.AddRange(UploadPhotos(photoids));

            item.Quantity = quantity;

            HandlePaymentMethods(safetrader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="photoids">tradelr photo id</param>
        /// <returns>trademe photo id</returns>
        private IEnumerable<int> UploadPhotos(IEnumerable<string> photoids)
        {
            var ids = photoids.Select(long.Parse);
            const int timeout = 10000; // 10 seconds
            var cts = new CancellationTokenSource();
            var tasks = new List<Task>();

            var results = new List<int>();

            var photoservice = new PhotoService(key, secret);

            using (var t = new Timer(_ => cts.Cancel(), null, timeout, -1))
            {
                foreach (var photoid in ids)
                {
                    var task = Task.Factory.StartNew(() =>
                    {
                        using (var repository = new TradelrRepository())
                        {
                            var image = repository.GetProductImage(photoid);
                            if (image != null)
                            {
                                if (image.trademephotoid.HasValue)
                                {
                                    results.Add(image.trademephotoid.Value);
                                }
                                else
                                {
                                    // we need to upload photo
                                    var photoreq = new PhotoUploadRequest
                                                    {
                                                        FileName = Path.GetFileName(image.url),
                                                        FileType = Path.GetExtension(image.url).Substring(1),
                                                        IsWaterMarked = true,
                                                        IsUsernameAdded = true,
                                                        PhotoData = Img.ConvertToBase64String(GeneralConstants.APP_ROOT_DIR + image.url)
                                                    };

                                    photoreq.GenerateSignature();

                                    var resp = photoservice.AddPhoto(new AddPhotoRequest(photoreq));
                                    if (resp.AddPhotoResult.Status == PhotoStatus.Success)
                                    {
                                        results.Add(resp.AddPhotoResult.PhotoId);
                                        image.trademephotoid = resp.AddPhotoResult.PhotoId;
                                        repository.Save();
                                    }
                                }
                            }
                        }

                    }, cts.Token);
                    tasks.Add(task);
                }
            }
            Task.WaitAll(tasks.ToArray());

            return results.ToArray();
        }

        public void UpdateItem(int listingid)
        {
            throw new NotImplementedException();
            /*
            var response = service.EditListing(item);

            if (response.Success)
            {
                // update database
                using (var repository = new TradelrRepository())
                {
                    var product = repository.GetProduct(ProductId);
                    product.trademe_product = trademeproduct;
                    product.trademe_product.listingid = listingid;

                    repository.Save("Trademe.UpdateItem");
                }
            }
            else
            {
                Syslog.Write(response.Description);
            }
             * */
        }

        private void HandlePaymentMethods(bool safeTrader)
        {
            bool bankDeposit = false;
            bool creditCard = false;
            bool cash = false;
            var others = new List<string>();

            using (var db = new tradelrDataContext())
            {
                var methods = db.paymentMethods.Where(x => x.subdomainid == subdomainid);
                foreach (var entry in methods)
                {
                    switch (entry.method.ToEnum<PaymentMethod>())
                    {
                        case PaymentMethod.CashInPerson:
                            cash = true;
                            break;
                        case PaymentMethod.COD:
                            cash = true;
                            break;
                        case PaymentMethod.LoanCheck:
                            break;
                        case PaymentMethod.MoneyOrder:
                            break;
                        case PaymentMethod.Moneybookers:
                            break;
                        case PaymentMethod.BankTransfer:
                            bankDeposit = true;
                            break;
                        case PaymentMethod.Paypal:
                            others.Add("Paypal");
                            break;
                        case PaymentMethod.PayOnPickup:
                            break;
                        case PaymentMethod.PersonalCheck:
                            others.Add("Check");
                            break;
                        case PaymentMethod.Other:
                            if (entry.name.ToLower().IndexOf("credit card") != -1)
                            {
                                others.Add(entry.name);
                            }
                            else
                            {
                                creditCard = true;
                            }
                            
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            var results = new List<api.trademe.co.nz.v1.PaymentMethod>();

            if (bankDeposit)
            {
                results.Add(api.trademe.co.nz.v1.PaymentMethod.BankDeposit);
            }

            if (cash)
            {
                results.Add(api.trademe.co.nz.v1.PaymentMethod.Cash);
            }

            if (creditCard)
            {
                results.Add(api.trademe.co.nz.v1.PaymentMethod.CreditCard);
            }

            if (others.Count() != 0)
            {
                results.Add(api.trademe.co.nz.v1.PaymentMethod.Other);
                item.OtherPaymentMethod = string.Join(", ", others);
            }

            if (safeTrader)
            {
                results.Add(api.trademe.co.nz.v1.PaymentMethod.SafeTrader);
            }
            item.PaymentMethods = results.ToArray();
        }

        public string ValidateAndSetPrices(decimal? startPrice, decimal? reservePrice, decimal? buynowPrice)
        {
            // verify start price
            if (!startPrice.HasValue)
            {
                return "TradeMe Start Price not specified";
            }

            if (!reservePrice.HasValue)
            {
                return "TradeMe Reserve Price not specified";
            }

            if ((double)startPrice.Value < 0.5)
            {
                return "TradeMe Start Price must be at least 50c";
            }

            if (reservePrice.Value < startPrice.Value)
            {
                return "TradeMe Reserve Price cannot be less than the Start Price";
            }

            if (buynowPrice.HasValue && buynowPrice.Value < startPrice.Value)
            {
                return "TradeMe BuyNow Price cannot be less than the Start Price";
            }

            item.StartPrice = startPrice.Value;
            item.ReservePrice = reservePrice.Value;
            if (buynowPrice.HasValue)
            {
                item.BuyNowPrice = buynowPrice.Value;
            }

            // set db values
            trademeproduct.startPrice = startPrice.Value;
            trademeproduct.reservePrice = reservePrice.Value;
            trademeproduct.buynowPrice = buynowPrice;


            return "";
        }

        public string ValidateAndSetShipping(string free, decimal[] trademe_scost, string[] trademe_sdesc)
        {
            var results = new List<ShippingOption>();
            if (free == GeneralConstants.FREE || 
                trademe_sdesc == null || 
                trademe_sdesc.Length == 0)
            {
                var option = new ShippingOption {Type = ShippingType.Free};
                results.Add(option);
            }
            else
            {
                // validate
                if (trademe_scost == null || trademe_sdesc.Length != trademe_scost.Length)
                {
                    return "TradeMe shipping cost is invalid";
                }

                for (int i = 0; i < trademe_scost.Length; i++)
                {
                    var option = new ShippingOption();
                    option.Price = trademe_scost[i];
                    option.Type = ShippingType.Custom;
                    option.Method = trademe_sdesc[i];
                    results.Add(option);
                }
            }

            item.ShippingOptions = results.ToArray();

            return "";
        }

        public bool VerifyItem()
        {
            // we use the method that gets the fees without listing the item
            var response = service.Fees(new FeesRequest(item));

            if (response.FeesResult.Success)
            {
                trademeproduct.listingfees = response.FeesResult.TotalCost;
                return true;
            }
            
            ErrorMessage = response.FeesResult.Description;
            return false;
        }
    }
}
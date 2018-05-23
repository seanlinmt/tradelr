using System;
using System.Collections.Generic;
using System.Linq;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.photos;
using tradelr.DBML;
using tradelr.Models.export.gbase;
using tradelr.Models.google.gbase;
using tradelr.Models.inventory;
using tradelr.Models.networks;
using tradelr.Models.photos;
using tradelr.Models.subdomain;

namespace tradelr.Libraries.scheduler
{
    public partial class ScheduledTask
    {
        public static void PollGoogleBase()
        {
            var myLock = new object();
            lock (myLock)
            {
                var date = DateTime.UtcNow;
                using (var repository = new TradelrRepository())
                {
                    // check for expired items
                    foreach (var sd in repository.GetSubDomains())
                    {
                        var products =
                            repository.GetProducts(sd.id).Where(x => x.gbase.HasValue);

                        foreach (var p in products)
                        {
                            var gb = new GoogleBaseExporter(sd.id, sd.ToHostName());
                            if (date > p.gbase_product.expirydate)
                            {
                                
                                gb.InitValues(p);
#if !DEBUG
                                IEnumerable<Photo> productPhotos = repository.GetImages(PhotoType.PRODUCT, p.id).ToModel(Imgsize.LARGE);
                                gb.AddProductImages(productPhotos);
#endif
                                gb.AddToGoogleBase();

                                // delete old entry
                                gb.DeleteFromGoogleBase(p.gbase_product.externalid);

                                // update gbase entry
                                p.gbase_product.externalid = gb.entry.Id.AbsoluteUri;
                                p.gbase_product.expirydate = gb.entry.ExpirationDate;
                                p.gbase_product.externallink = NetworksGbase.URLFromEntry(gb.entry);
                            }
                            else
                            {
                                // get status
                                if (gb.GetFromGoogleBase(p.gbase_product.externalid))
                                {
                                    p.gbase_product.expirydate = gb.entry.ExpirationDate;
                                    
                                    if (gb.entry.IsDraft)
                                    {
                                        p.gbase_product.flags |= (int)InventoryItemFlag.DRAFT;
                                    }
                                    else
                                    {
                                        p.gbase_product.flags &= ~(int) InventoryItemFlag.DRAFT;
                                    }
                                }
                            }
                        }
                    }

                    repository.Save("PollGoogleBase");
                }
            }
        }
    }
}
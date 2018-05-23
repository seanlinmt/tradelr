using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Shipwire;
using tradelr.DataAccess;
using tradelr.DBML;
using tradelr.DBML.Models;
using clearpixels.Logging;
using tradelr.Models.products;

namespace tradelr.Libraries.scheduler
{
    public class InventoryUpdate
    {
        private ShipwireService service { get; set; }
        private string warehouse { get; set; } // fullname of warehouse
        private long subdomainid { get; set; }

        public InventoryUpdate(ShipwireService service, long subdomainid, string warehouse = "")
        {
            this.service = service;
            this.warehouse = warehouse;
            this.subdomainid = subdomainid;
        }

        public void Update(IEnumerable<string> warehouselocations)
        {
            foreach (var warehouselocation in warehouselocations)
            {
                warehouse = warehouselocation;
                Update();
            }
        }

        private void Update()
        {
            if (string.IsNullOrEmpty(warehouse))
            {
                Syslog.Write(string.Format("Shipwire warehouse not specified: {0}", subdomainid));
                return;
            }
            service.CreateInventoryUpdate(warehouse);
            var resp = service.SubmitInventoryUpdate();

            if (resp != null)
            {
                // check for Error
                if (resp.Status == ShipwireService.StatusError && resp.ErrorMessage.ToLower().Contains("password"))
                {
                    // clear invalid credentials
                    using (var repository = new TradelrRepository())
                    {
                        var sd = repository.GetSubDomain(subdomainid);
                        if (sd != null)
                        {
                            sd.shipwireEmail = "";
                            sd.shipwirePassword = "";
                            repository.Save();
                        }
                    }
                }

                // we want to create a location only if there are products
                if (resp.Products.Count != 0)
                {
                    Debug.WriteLine(string.Format("{0}: {1} products", warehouse, resp.Products.Count));
                    using (var repository = new TradelrRepository())
                    {
                        // if items not zero then we create inventory location if it does not already exist
                        var inventoryloc = new inventoryLocation
                                               {
                                                   subdomain = subdomainid,
                                                   name = warehouse,
                                                   lastUpdate = DateTime.UtcNow
                                               };
                        var locid = repository.AddInventoryLocation(inventoryloc, subdomainid);
                        // we go through each product
                        foreach (var product in resp.Products)
                        {
                            var variant = repository.GetProductVariant(product.code, subdomainid, ProductFlag.ARCHIVED);
                            // if product exist and not archived
                            if (variant != null)
                            {
                                Debug.WriteLine("Existing product: " + product.code);
                                // then we just update the inventorylocitem
                                var ilocitem =
                                    repository.GetInventoryLocationItems(locid, subdomainid).SingleOrDefault(x => x.variantid == variant.id);
                                if (ilocitem == null)
                                {
                                    // can be null from do these updates from Shipwire as we're doing on a per location basis
                                    // anyway create an entry
                                    ilocitem = new inventoryLocationItem
                                                   {
                                                       variantid = variant.id,
                                                       locationid = locid,
                                                       lastUpdate = DateTime.UtcNow,
                                                       available = product.quantity
                                                   };
                                    repository.AddInventoryLocationItem(ilocitem, subdomainid);
                                    
                                }
                                else
                                {
                                    // just update as this is a sync
                                    ilocitem.available = product.quantity;
                                    ilocitem.lastUpdate = DateTime.UtcNow;
                                    repository.Save("InventoryUpdate Update");
                                }
                            }
                            else
                            {
                                Debug.WriteLine("New product: " + product.code);
                                
                                // create new product
                                var productInfo = new ProductInfo();

                                // do product
                                var p = new product
                                        {
                                            subdomainid = subdomainid,
                                            title = product.code,
                                            details = "",
                                            created = DateTime.UtcNow
                                        };
                                productInfo.p = p;
                                variant = new product_variant();
                                variant.sku = product.code;

                                // do inventory location item
                                var ilocitem = new inventoryLocationItem
                                                   {
                                                       variantid = variant.id,
                                                       locationid = locid,
                                                       lastUpdate = DateTime.UtcNow
                                                   };
                                var invWorker = new InventoryWorker(ilocitem, subdomainid, true, false); // assume not digital
                                invWorker.SetValues("Shipwire Update", product.quantity, null, null, null);
                                variant.inventoryLocationItems.Add(ilocitem);
                                productInfo.p.product_variants.Add(variant);

                                // finally add to db
                                repository.AddProduct(productInfo, subdomainid);
                            }
                        }
                    }
                }
                else
                {
                    //Syslog.Write(ErrorLevel.WARNING,string.Format("{0}: No products", warehouse));
                }
            }
            else
            {
                Syslog.Write("No response from warehouse " + warehouse);
            }
            
        }
    }
}
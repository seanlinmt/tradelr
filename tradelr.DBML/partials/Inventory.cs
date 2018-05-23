using System;
using System.Linq;
using clearpixels.Logging;
using tradelr.DBML.Models;
using tradelr.Library.Constants;
using tradelr.Models.networks;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void DeleteInventoryLocation(string locationname, long subdomainid)
        {
            var location = GetInventoryLocation(locationname, subdomainid);
            if (location != null)
            {
                DeleteInventoryLocation(location.id, subdomainid);
            }
        }

        public void DeleteInventoryLocation(long id, long subdomainid)
        {
            var loc = GetInventoryLocation(id, subdomainid);
            if (loc == null)
            {
                throw new Exception("Cannot find location");
            }
            // move all entries to main location
            // external variant types does not affect this because it does not have entries
            var mainloc = GetInventoryLocation(GeneralConstants.INVENTORY_LOCATION_DEFAULT, subdomainid);
            foreach (var entry in loc.inventoryLocationItems)
            {
                var variantid = entry.variantid;
                var inventoryItem = mainloc.inventoryLocationItems.SingleOrDefault(x => x.variantid == variantid);
                var newEntry = false;
                if (inventoryItem == null)
                {
                    inventoryItem = new inventoryLocationItem
                                        {
                                            variantid = variantid,
                                            locationid = mainloc.id,
                                            alarmLevel = entry.alarmLevel,
                                            lastUpdate = DateTime.UtcNow
                    };
                    newEntry = true;
                }
                var invWorker = new InventoryWorker(inventoryItem, subdomainid, entry.product_variant.product.trackInventory, entry.product_variant.IsDigital());
                invWorker.SetValues(string.Format("From deleted inventory location {0}", entry.inventoryLocation.name),
                                    entry.available, entry.onOrder, entry.reserved, entry.sold);
                
                if (newEntry)
                {
                    db.inventoryLocationItems.InsertOnSubmit(inventoryItem);
                }
            }

            // delete items
            db.inventoryLocationItems.DeleteAllOnSubmit(loc.inventoryLocationItems);

            // delete history
            var histories = loc.inventoryLocationItems.SelectMany(x => x.inventoryHistories);
            db.inventoryHistories.DeleteAllOnSubmit(histories);

            // TODO: this is unused at the moment
            // update orders that are using this inventory location
            var affectedOrders = db.orders.Where(x => x.inventoryLocation.HasValue &&
                                                      x.inventoryLocation.Value == id);
            foreach (var order in affectedOrders)
            {
                order.inventoryLocation = null;
                order.inventoryLocation_del = loc.name;
            }

            // delete inventory location
            db.inventoryLocationItems.DeleteAllOnSubmit(loc.inventoryLocationItems);
            db.inventoryLocations.DeleteOnSubmit(loc);

            Save();
        }

        public void DeleteInventoryLocationItem(long id, long subdomainid)
        {
            var item = db.inventoryLocationItems.SingleOrDefault(x => x.id == id && x.inventoryLocation.subdomain == subdomainid);
            if (item == null)
            {
                return;
            }
            db.inventoryLocationItems.DeleteOnSubmit(item);

            var histories = item.inventoryHistories;
            db.inventoryHistories.DeleteAllOnSubmit(histories);

            Save();
        }

        public void DeleteInventoryLocationItems(long productid)
        {
            var p = db.products.SingleOrDefault(x => x.id == productid);
            if (p != null)
            {
                var items = p.product_variants.SelectMany(x => x.inventoryLocationItems);
                db.inventoryLocationItems.DeleteAllOnSubmit(items);

                var histories = items.SelectMany(x => x.inventoryHistories);
                db.inventoryHistories.DeleteAllOnSubmit(histories);

                Save();
            }
            else
            {
                Syslog.Write(string.Format("Unable to delete inventoryitems for product {0}", productid));
            }
        }

        public IQueryable<inventoryLocation> GetLocations(long subdomainid)
        {
            return db.inventoryLocations.Where(x => x.subdomain == subdomainid);
        }

        public inventoryLocation GetInventoryLocation(string locationName, long subdomainid)
        {
            if (string.IsNullOrEmpty(locationName))
            {
                locationName = GeneralConstants.INVENTORY_LOCATION_DEFAULT;
            }
            return
                db.inventoryLocations.SingleOrDefault(x => x.subdomain == subdomainid && x.name == locationName);
        }

        public inventoryLocationItem GetInventoryLocationItem(long id, long subdomainid)
        {
            return
                db.inventoryLocationItems.SingleOrDefault(x => x.id == id && x.inventoryLocation.subdomain == subdomainid);
        }

        public inventoryLocationItem GetInventoryLocationItem(long variantid, long inventoryLocationId, long subdomainid)
        {
            return
                db.inventoryLocationItems.SingleOrDefault(x => x.variantid == variantid && x.locationid == inventoryLocationId &&
                                                               x.inventoryLocation.subdomain == subdomainid);
        }

        public IQueryable<inventoryLocationItem> GetInventoryLocationItems(long locationid, long subdomainid)
        {
            return
                db.inventoryLocationItems.Where(
                    x => x.locationid == locationid && x.inventoryLocation.subdomain == subdomainid);
        }

        public inventoryLocation GetInventoryLocation(long locationID, long subdomainid)
        {
            return db.inventoryLocations.SingleOrDefault(x => x.subdomain == subdomainid && x.id == locationID);
        }

        public IQueryable<inventoryLocation> GetInventoryLocationsExceptSyncNetworks(long subdomainid)
        {
            // exclude network sync locations
            return db.inventoryLocations.Where(x => x.subdomain == subdomainid && !Networks.SYNC_NETWORKS.Contains(x.name));
        }
    }
}
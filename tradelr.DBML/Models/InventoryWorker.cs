using System;
using clearpixels.Logging;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Models.activity;
using tradelr.Models.transactions;

namespace tradelr.DBML.Models
{
    public class InventoryWorker
    {
        private readonly inventoryLocationItem item;
        private long subdomainid { get; set; }
        private bool trackInventory { get; set; }
        private bool isDigital { get; set; }

        public InventoryWorker(inventoryLocationItem item, long subdomainid, bool trackInventory, bool isDigital)
        {
            this.item = item;
            this.subdomainid = subdomainid;
            this.trackInventory = trackInventory;
            this.isDigital = isDigital;
        }

        public void SetValues(string description, int? available, int? onOrder, int? reserved, int? sold)
        {
            if (available.HasValue && trackInventory && !isDigital)
            {
                if (item.available.HasValue)
                {
                    item.available += available.Value;
                }
                else
                {
                    item.available = available.Value;
                }
                if (item.available < 0)
                {
                    Syslog.Write("-ve avalaible stock for ilocitem {0}",item.id);
                }
                CheckInventoryLevel();
            }

            if (onOrder.HasValue)
            {
                if (item.onOrder.HasValue)
                {
                    item.onOrder += onOrder.Value;
                }
                else
                {
                    item.onOrder = onOrder.Value;
                }
                if (item.onOrder < 0)
                {
                    Syslog.Write("-ve onorder stock for ilocitem {0}", item.id);
                }
            }

            if (reserved.HasValue)
            {
                if (item.reserved.HasValue)
                {
                    item.reserved += reserved.Value;
                }
                else
                {
                    item.reserved = reserved.Value;
                }
                if (item.reserved < 0)
                {
                    Syslog.Write("-ve reserved stock for ilocitem {0}", item.id);
                }
            }

            if (sold.HasValue)
            {
                if (item.sold.HasValue)
                {
                    item.sold += sold.Value;
                }
                else
                {
                    item.sold = sold.Value;
                }
                if (item.sold < 0)
                {
                    Syslog.Write("-ve sold stock for ilocitem {0}", item.id);
                }
            }

            var history = new inventoryHistory
            {
                created = DateTime.UtcNow,
                available = available,
                description = description,
                onOrder = onOrder,
                sold = sold,
                reserved = reserved
            };
            item.inventoryHistories.Add(history);
        }

        private void CheckInventoryLevel()
        {
            // log activity if is stock below or equals alarm level
            if (item.alarmLevel.HasValue && 
                item.available.HasValue && 
                item.product_variant != null &&
                item.inventoryLocation != null &&
                item.available <= item.alarmLevel)
            {
                using (var repository = new TradelrRepository())
                {
                    repository.AddActivity(subdomainid,
                            new ActivityMessage(item.product_variant.productid, subdomainid,
                                                ActivityMessageType.PRODUCT_ALARM, item.inventoryLocation.name,
                                                new HtmlLink(item.product_variant.product.title,
                                                             item.product_variant.productid).
                                                    ToProductString()), subdomainid);
                }
            }
        }
    }

    public static class InventoryWorkerHelper
    {
        public static string ToHtmlLink(this order o, bool openInNewPage = true)
        {
            var type = o.type.ToEnum<TransactionType>();

            if (o.id == 0)
            {
                return string.Format("<a href='#' onclick='return false;'>{0} #{1}</a>", type.ToDescriptionString(), o.orderNumber.ToString("D8"));
            }

            return string.Format("<a href='{0}{1}' target='{2}'>{3} #{4}</a>",
                                 type == TransactionType.INVOICE ? GeneralConstants.URL_SINGLE_INVOICE : GeneralConstants.URL_SINGLE_ORDER,
                                 o.id,
                                 openInNewPage ? "_blank" : "_self",
                                 type.ToDescriptionString(),
                                 o.orderNumber.ToString("D8"));
        }
    }
}
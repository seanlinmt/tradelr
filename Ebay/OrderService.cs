using System;
using eBay.Service.Call;
using eBay.Service.Core.Soap;

namespace Ebay
{
    public class OrderService : EbayService
    {
        public OrderService(string token)
            : base(token)
        {
            
        }

        // https://developer.ebay.com/DevZone/XML/docs/Reference/ebay/CompleteSale.html
        public void CompleteSale(string orderid, string orderlineitemid, bool paid, bool shipped, FeedbackInfoType feedback, ShipmentType shipment)
        {
            var call = new CompleteSaleCall(api);
            call.DetailLevelList.Add(DetailLevelCodeType.ReturnAll);
            call.CompleteSale("", "", feedback, shipped,paid, ListingTypeCodeType.CustomCode, shipment, orderid, orderlineitemid);
        }

        public OrderTypeCollection GetOrder(string[] orderids)
        {
            var scollection = new StringCollection();
            scollection.AddRange(orderids);

            var call = new GetOrderTransactionsCall(api);

            call.DetailLevelList.Add(DetailLevelCodeType.ReturnAll);

            return call.GetOrderTransactions(scollection);

        }

        // http://developer.ebay.com/devzone/xml/docs/Reference/eBay/GetOrders.html
        public OrderTypeCollection GetOrders(OrderStatusCodeType type, DateTime from, DateTime to)
        {
            var call = new GetOrdersCall(api);

            call.DetailLevelList.Add(DetailLevelCodeType.ReturnAll);

            var timeFilter = new TimeFilter {TimeFrom = from, TimeTo = to};

            var result = new OrderTypeCollection();

            bool hasMoreEntries = true;

            int count = 1;

            while (hasMoreEntries)
            {
                call.Pagination = new PaginationType
                                      {
                                          PageNumber = count, 
                                          EntriesPerPage = 200
                                      };

                OrderTypeCollection orders = call.GetOrders(timeFilter, TradingRoleCodeType.Seller, type);
                result.AddRange(orders);

                hasMoreEntries = call.HasMoreOrders;

                if (count++ > 10)
                {
                    break;
                }
            }

            responseXML = call.SoapResponse;

            return result;
        }
    }
}

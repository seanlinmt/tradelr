using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using Shipwire.inventory;
using Shipwire.order;
using Shipwire.rate;
using Shipwire.tracking;
using clearpixels.Logging;
using Exception = System.Exception;
using Order = Shipwire.order.Order;

namespace Shipwire
{
    public class ShipwireService
    {
        private const string RATEREQUEST = "RateRequestXML";
        private const string SUBMITORDER = "OrderListXML";
        private const string TRACKINGUPDATE = "TrackingUpdateXML";
        private const string INVENTORYUPDATE = "InventoryUpdateXML";
        private string email { get; set; }
        private string password { get; set; }
        private string xml { get; set; }

        private OrderList orderlist { get; set; }
        private RateRequest raterequest { get; set; }
        private TrackingUpdate trackingupdate { get; set; }
        private InventoryUpdate inventoryupdate { get; set; }

        public const string StatusError = "Error";


        public ShipwireService(string email, string password)
        {
            this.email = email;
            this.password = password;
        }

        /// <summary>
        /// returns xml string of the last response
        /// </summary>
        /// <returns></returns>
        public string GetXmlResponse()
        {
            return xml;
        }

        private string PostRequest(string type, string reqxml)
        {
            string xmlString = "";
            HttpWebRequest req;
            switch (type)
            {
                case RATEREQUEST:
                    req = (HttpWebRequest)WebRequest.Create("https://api.shipwire.com/exec/RateServices.php");
                    break;
                case SUBMITORDER:
                    req = (HttpWebRequest)WebRequest.Create("https://api.shipwire.com/exec/FulfillmentServices.php");
                    break;
                case TRACKINGUPDATE:
                    req = (HttpWebRequest)WebRequest.Create("https://api.shipwire.com/exec/TrackingServices.php");
                    break;
                case INVENTORYUPDATE:
                    req = (HttpWebRequest)WebRequest.Create("https://api.shipwire.com/exec/InventoryServices.php");
                    break;
                default:
                    throw new Exception();
            }
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.KeepAlive = false;
            ServicePointManager.Expect100Continue = false;

            using (var swriter = new StreamWriter(req.GetRequestStream()))
            {
                swriter.Write("{0}={1}",type, reqxml);
            }
            try
            {
                var resp = req.GetResponse();
                using (var sr = new StreamReader(resp.GetResponseStream()))
                {
                    xmlString = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
            }
            return xmlString;
        }

        // orders
        public void CreateOrder(string orderid, string shipping, IEnumerable<OrderItem> items, AddressInfo address)
        {
            orderlist = new OrderList(email, password);
            
            var order = new Order(orderid, address, shipping);
            foreach (var item in items)
            {
                order.AddItem(item);
            }
            
            orderlist.AddOrder(order);
        }

        public SubmitOrderResponse SubmitOrder()
        {
            var serializer = new XmlSerializer(typeof(OrderList));
            var ms = new MemoryStream();
            serializer.Serialize(ms, orderlist);

            var reqxml = Encoding.Default.GetString(ms.ToArray());
            xml = PostRequest(SUBMITORDER, reqxml);
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }
            SubmitOrderResponse response;
            using (var reader = new StringReader(xml))
            {
                serializer = new XmlSerializer(typeof (SubmitOrderResponse));
                response = serializer.Deserialize(reader) as SubmitOrderResponse;
            }
            return response;
        }

        // rate request
        public void CreateRateRequest(Order order)
        {
            raterequest = new RateRequest(email, password);

            raterequest.AddOrder(order);
        }

        public RateResponse SubmitRateRequest()
        {
            var serializer = new XmlSerializer(typeof(RateRequest));
            var ms = new MemoryStream();
            serializer.Serialize(ms, raterequest);

            var reqxml = Encoding.Default.GetString(ms.ToArray());
            xml = PostRequest(RATEREQUEST, reqxml);
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }
            RateResponse response;
            using (var reader = new StringReader(xml))
            {
                serializer = new XmlSerializer(typeof(RateResponse));
                response = serializer.Deserialize(reader) as RateResponse;
            }

            return response;
        }

        // tracking update
        public void CreateTrackingUpdate(string transactionid)
        {
            trackingupdate = new TrackingUpdate(email, password)
                                 {
                                     ShipwireId = transactionid
                                 };
        }

        public TrackingUpdateResponse SubmitTrackingUpdate()
        {
            var serializer = new XmlSerializer(typeof(TrackingUpdate));
            var ms = new MemoryStream();
            serializer.Serialize(ms, trackingupdate);

            var reqxml = Encoding.Default.GetString(ms.ToArray());
            xml = PostRequest(TRACKINGUPDATE, reqxml);
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }
            TrackingUpdateResponse response;
            using (var reader = new StringReader(xml))
            {
                serializer = new XmlSerializer(typeof(TrackingUpdateResponse));
                response = serializer.Deserialize(reader) as TrackingUpdateResponse;
            }

            return response;
        }

        public void CreateInventoryUpdate(string location)
        {
            switch (location)
            {
                case WarehouseLocation.CHI:
                    inventoryupdate = new InventoryUpdate(email,password, "CHI");
                    break;
                case WarehouseLocation.LAX:
                    inventoryupdate = new InventoryUpdate(email, password, "LAX");
                    break;
                case WarehouseLocation.REN:
                    inventoryupdate = new InventoryUpdate(email, password, "REN");
                    break;
                case WarehouseLocation.TOR:
                    inventoryupdate = new InventoryUpdate(email, password, "TOR");
                    break;
                case WarehouseLocation.UK:
                    inventoryupdate = new InventoryUpdate(email, password, "UK");
                    break;
                case WarehouseLocation.VAN:
                    inventoryupdate = new InventoryUpdate(email, password, "VAN");
                    break;
                default:
                    Syslog.Write("Unknown warehouse location: " + location);
                    throw new Exception();
            }
        }
//#if !DEBUG
        public InventoryUpdateResponse SubmitInventoryUpdate()
        {
            var serializer = new XmlSerializer(typeof(InventoryUpdate));
            var ms = new MemoryStream();
            serializer.Serialize(ms, inventoryupdate);

            var reqxml = Encoding.Default.GetString(ms.ToArray());
            xml = PostRequest(INVENTORYUPDATE, reqxml);
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }
            InventoryUpdateResponse response;
            using (var reader = new StringReader(xml))
            {
                serializer = new XmlSerializer(typeof(InventoryUpdateResponse));
                response = serializer.Deserialize(reader) as InventoryUpdateResponse;
            }

            return response;
        }
     /*   
#else
        /// <summary>
        /// test method
        /// </summary>
        /// <returns></returns>
        public InventoryUpdateResponse SubmitInventoryUpdate()
        {
            var serializer = new XmlSerializer(typeof(InventoryUpdate));
            switch (inventoryupdate.Warehouse)
            {
                case "CHI":
                    xml = "<InventoryUpdateResponse><Status>Test</Status>" +
                            "<Product code=\"CHI-024\" quantity=\"10\"/>" +
                            "<Product code=\"CHI-500\" quantity=\"20\"/>" +
                            "<TotalProducts>2</TotalProducts><ProductCode/>" + 
                        "</InventoryUpdateResponse>";
                    break;
                case "REN":
                    xml = "<InventoryUpdateResponse><Status>Test</Status>" +
                            "<Product code=\"REN-024\" quantity=\"0\"/>" +
                            "<TotalProducts>1</TotalProducts><ProductCode/>" + 
                        "</InventoryUpdateResponse>";
                    break;
                default:
                    xml = "";
                    break;
            }

            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }
            InventoryUpdateResponse response;
            using (var reader = new StringReader(xml))
            {
                serializer = new XmlSerializer(typeof(InventoryUpdateResponse));
                response = serializer.Deserialize(reader) as InventoryUpdateResponse;
            }

            return response;
        }
#endif
        */
        public bool VerifyCredentials()
        {
            CreateInventoryUpdate(WarehouseLocation.CHI);
            var resp = SubmitInventoryUpdate();
            if (resp.Status == StatusError && resp.ErrorMessage.ToLower().Contains("password"))
            {
                return false;
            }

            return true;
        }
    }
}

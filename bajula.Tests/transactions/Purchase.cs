using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using tradelr.DataAccess;
using tradelr.DBML;
using tradelr.Models.transactions;
using tradelr.Tests.Helpers;
using System.Runtime.Serialization.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tradelr.Tests.transactions
{
    [TestClass]
    public class Purchase
    {
        tradelrDataContext db = new tradelrDataContext();
        TradelrRepository repository = new TradelrRepository();
    
        [TestMethod]
        public void DeserializePurchaseJSONData()
        {
            const string jsonString = "{\"organisation\":\"263\",\"fakeOrderDate\":\"Tue, 20 Oct 09\",\"orderDate\":\"/Date(1255968000000)/\",\"location\":\"2\",\"orderStatus\":\"on\",\"statusDetails\":12,\"currency\":\"320\",\"comments\":\"\",\"orderID\":\"\",\"orderItems\":[{\"rn\":\"1\",\"SKU\":\"33-35-22\",\"SupplierSKU\":\"\",\"description\":\"Chef Anton's Cajun Seasoning\",\"unitPrice\":\"22.00\",\"quantity\":\"4\",\"subtotal\":\"88.00\"},{\"rn\":\"2\",\"SKU\":\"33-35-22\",\"SupplierSKU\":\"\",\"description\":\"Chef Anton's Cajun Seasoning\",\"unitPrice\":\"22.00\",\"quantity\":\"4\",\"subtotal\":\"88.00\"}]}";
            const string jsonString2 = "{\"organisation\":\"302\",\"sales_fakeExpectedDate\":\"\",\"expectedDate\":\"null\",\"orderStatus\":\"Open\",\"sales_fakeOrderDate\":\"Sun, 1 Nov 09\",\"orderDate\":\"/Date(1257004800000)/\",\"comments\":\"\"}";
            
            var ms = new MemoryStream(Encoding.Default.GetBytes(jsonString2));
            var serializer = new DataContractJsonSerializer(typeof(Order));
            //var obj = serializer.ReadObject(ms);
            
        }
    }
}

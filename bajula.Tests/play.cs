using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tradelr.Models.yahoo;

namespace tradelr.Tests
{
    /// <summary>
    /// Summary description for play
    /// </summary>
    [TestClass]
    public class play
    {
        [TestMethod]
        public void SerialiseEnumFlags()
        {
            DateTime? obj = null;
            var ms = new MemoryStream();
            var serializer = new DataContractJsonSerializer(typeof(DateTime));
            serializer.WriteObject(ms, obj);
            string jsonString = Encoding.Default.GetString(ms.ToArray());
        }

        [TestMethod]
        public void playWithTimezones()
        {
            var t = TimeZoneInfo.GetSystemTimeZones();
        }

        [TestMethod]
        public void playWithGuid()
        {
            var guid = Guid.NewGuid().ToString("N");
        }


        [TestMethod]
        public void playWithBlogContent()
        {
            string content = "<div xmlns='http://www.w3.org/1999/xhtml'>" +
               "<div>blabalbalba dfdsfsdfds\n\nupdated2 <img src=\"dfdf/dfd/.jpg\" /> ssssss </div>" +
                "</div>";
            var regex = new Regex("^<div.+?>(.+?)</div>$");
            var match = regex.Match(content);
        }

        [TestMethod]
        public void extractBlogIDs()
        {
            string id = "tag:blogger.com,1999:blog-542864143414144849.post-7976515450217044783";
            var regex = new Regex("blog-(.+?)\\.post-(.+?)$");
            var match = regex.Match(id);
        }

        [TestMethod]
        public void deserializeYahooContacts()
        {
            var jsonstring =
                "{\"contacts\":{\"start\":0,\"count\":2,\"total\":2,\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contacts\",\"contact\":[{\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contact/559\",\"created\":\"2007-06-21T08:33:01Z\",\"updated\":\"2007-06-21T08:33:01Z\",\"isConnection\":false,\"id\":559,\"fields\":[{\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contact/559/nickname/562\",\"created\":\"2007-06-21T08:33:01Z\",\"updated\":\"2007-06-21T08:33:01Z\",\"id\":562,\"type\":\"nickname\",\"value\":\"sean\",\"editedBy\":\"OWNER\",\"flags\":[]},{\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contact/559/email/563\",\"created\":\"2007-06-21T08:33:01Z\",\"updated\":\"2007-06-21T08:33:01Z\",\"id\":563,\"type\":\"email\",\"value\":\"seanlinmt@gmail.com\",\"editedBy\":\"OWNER\",\"flags\":[]},{\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contact/559/name/561\",\"created\":\"2007-06-21T08:33:01Z\",\"updated\":\"2007-06-21T08:33:01Z\",\"id\":561,\"type\":\"name\",\"value\":{\"givenName\":\"sean\",\"middleName\":\"lin\",\"familyName\":\"\",\"prefix\":\"\",\"suffix\":\"\",\"givenNameSound\":\"\",\"familyNameSound\":\"\"},\"editedBy\":\"OWNER\",\"flags\":[]}],\"categories\":[]},{\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contact/560\",\"created\":\"2010-06-26T06:51:15Z\",\"updated\":\"2010-06-26T06:51:15Z\",\"isConnection\":false,\"id\":560,\"fields\":[{\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contact/560/nickname/565\",\"created\":\"2010-06-26T06:51:15Z\",\"updated\":\"2010-06-26T06:51:15Z\",\"id\":565,\"type\":\"nickname\",\"value\":\"clearpixels\",\"editedBy\":\"OWNER\",\"flags\":[]},{\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contact/560/email/566\",\"created\":\"2010-06-26T06:51:15Z\",\"updated\":\"2010-06-26T06:51:15Z\",\"id\":566,\"type\":\"email\",\"value\":\"seanlinmt@clearpixels.co.nz\",\"editedBy\":\"OWNER\",\"flags\":[]},{\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contact/560/name/564\",\"created\":\"2010-06-26T06:51:15Z\",\"updated\":\"2010-06-26T06:51:15Z\",\"id\":564,\"type\":\"name\",\"value\":{\"givenName\":\"Sean\",\"middleName\":\"\",\"familyName\":\"Lin\",\"prefix\":\"\",\"suffix\":\"\",\"givenNameSound\":\"\",\"familyNameSound\":\"\"},\"editedBy\":\"OWNER\",\"flags\":[]}],\"categories\":[]}]}}";

            var serializer = new JavaScriptSerializer();
            var contacts = serializer.Deserialize<Contacts>(jsonstring);
        }
    }
}

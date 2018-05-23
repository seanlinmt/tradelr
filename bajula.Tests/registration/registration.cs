using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using tradelr.Controllers.admin;
using tradelr.Controllers.register;
using tradelr.DataAccess;
using tradelr.DBML;
using tradelr.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tradelr.Tests.registration
{
    /// <summary>
    /// Summary description for registration
    /// </summary>
    [TestClass]
    public class registration
    {
        private ITradelrRepository repository;
        public registration()
        {
            repository = new TradelrRepository();
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void RegisterNewUserWithEmailAndPassword()
        {
            /*
            var th = new TestHelpers();
            th.FakeHttpContext(new[] { "auth" });
            HttpContextBase httpContext = th.httpContext.Object;
            var controller = new registerController();
            var context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
            controller.ControllerContext = context;
            var result = controller.Create("test@test.com", "12345", "12345", "");
            Assert.AreEqual(result.GetType(), typeof(RedirectToRouteResult));
            var routevalues = ((RedirectToRouteResult) result).RouteValues;
            Assert.AreEqual(routevalues["action"], "setup");

            var sessionid = httpContext.Session["id"];

            // update registration information
            th.SetHttpMethod("POST");
            controller.Setup("Singapore Standard Time", "385", "ejunction solutions", "106",
                             "123 Petaling St\\nSelangor", "Kuala Lumpur", "", "93000", "082-251955", "on");

            // delete the user
            //repository.DeleteUser("test@test.com");

            var admincontroller = new adminController();
            var admincontext = new ControllerContext(new RequestContext(httpContext, new RouteData()), admincontroller);
            admincontroller.ControllerContext = admincontext;
             * */
        }
    }
}

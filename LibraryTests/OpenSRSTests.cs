using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSRS;
using OpenSRS.Services;
using System.Xml;
using System.Xml.Serialization;

namespace LibraryTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class OpenSRSTests
    {
#if DEBUG
        private string[] test_domains { get; set; }

        public OpenSRSTests()
        {
            test_domains = new[]
                               {
                                   "wssdxedcrfv.biz", 
                                   "wsxesdfdcrfv.co", 
                                   "wsxsdfedcrfv.com", 
                                   "wsxesdfdcrfv.info", 
                                   "wsxsdfedcrfv.me", 
                                   "wsxevsfdcrfv.mobi", 
                                   "wseeexedcrfv.net", 
                                   "wsxefefvdcrfv.org"
                               };
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
        public void TestDomainRegistrations()
        {
            var owner = new Contact("John", "Doe", "Not Clear Limited",
                                              "2 John St", "Auckland", "",
                                              "1051", "NZ", "+6.0165760616", "",
                                              "tradelr.com@gmail.com");

            var domain = new Domain();
            foreach (var entry in test_domains)
            {
                var resp = domain.RegisterDomain(entry, true, 1, owner);

                Assert.IsNotNull(resp);
                Assert.IsTrue(resp.code == "200");
            }
            
        }
#endif
    }
}

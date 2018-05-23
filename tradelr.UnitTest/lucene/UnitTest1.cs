using Microsoft.VisualStudio.TestTools.UnitTesting;
using tradelr.DBML;
using tradelr.DBML.Lucene.IndexingQueue;

namespace tradelr.UnitTest.lucene
{
    [TestClass]
    public class UnitTest1
    {
        private user userWithNoName;
        private organisation userOrg;

        public UnitTest1()
        {
            userWithNoName = new user();
            userOrg = new organisation(){name = "Test Org Name"};
            userOrg.users.Add(userWithNoName);
        }

        [TestMethod]
        public void TestCreateContactItem()
        {
            var item = new ContactItem(userWithNoName);

            Assert.IsNotNull(item);
        }
    }
}

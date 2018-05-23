using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using tradelr.Controllers;
using tradelr.Controllers.inventory;
using tradelr.DataAccess;
using tradelr.DBML;
using tradelr.Models.products;
using tradelr.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tradelr.Tests.Products
{
    /// <summary>
    /// Summary description for products
    /// </summary>
    [TestClass]
    public class Products
    {
        private ITradelrRepository repository;
        public Products()
        {
            repository = new TradelrRepository();
        }
        /// <summary>
        /// delete users, organisations, customers, productCategories, products, product suppliers, stockunit, suppliers
        /// </summary>
        [TestMethod]
        public void ReadDataFromExcel()
        {
            TestUtils.deleteTestMember();
            var id = TestUtils.addTestMember();
            var th = new TestHelpers();
            th.FakeHttpContext(id);
            HttpContextBase httpContext = th.httpContext.Object;
            var controller = new importController();
            var context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
            controller.ControllerContext = context;
            //var result = controller.Excel("testData.xls");
        }

        [TestMethod]
        public void AddProduct()
        {
            var owner = TestUtils.addTestMember();
            var th = new TestHelpers();
            th.FakeHttpContext();
            HttpContextBase httpContext = th.httpContext.Object;
            var controller = new productsController();
            var context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
            controller.ControllerContext = context;
            
            // product category
            var mastercatid = repository.AddMasterProductCategory("Ingredients").id;
            var productcategory = new productCategory();
            productcategory.masterID = mastercatid;
            //productcategory.subdomain = s;
            var pcid = repository.AddProductCategory(productcategory,0);

            // stock unit
            var su = new stockUnit();
            su.unitID = repository.AddMasterStockUnit("bottle").id;
            su.owner = owner;
            var suid = repository.AddStockUnit(su);
            controller.Session["id"] = owner;
            // supplier
            var supid = repository.GetProductSupplier("Example Supplier Co.").id;
            var result = controller.Create("TEST001","Tomato Sauce", "red tomato sauce\nvery yummy", pcid,
                              suid, "1", "2", "","","", new[] { "2" }, new[] { "" }, new[] { "" }, "", "","","","","");

            Product product = (Product)((JsonResult)(result)).Data;
            // delete product supplier
            //repository.DeleteProductSuppliers(owner);

            // delete product
            repository.DeleteProduct(product.id.Value, 0);
            
            // delete stock unit
            repository.DeleteStockUnits(owner);

            // delete test user
            TestUtils.deleteTestMember();
        }
    }
}

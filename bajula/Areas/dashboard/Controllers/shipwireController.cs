using System;
using System.IO;
using System.Security;
using System.Web.Mvc;
using NPOI.HSSF.UserModel;
using Shipwire;
using tradelr.Common.Models.currency;
using tradelr.Controllers;
using tradelr.Crypto;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using tradelr.Models.products;
using tradelr.Models.shipwire;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.CREATOR)]
    [TradelrHttps]
    public class shipwireController : baseController
    {
        [HttpPost]
        public ActionResult Connected()
        {
            if (!string.IsNullOrEmpty(MASTERdomain.shipwireEmail))
            {
                return Json(true.ToJsonOKData());
            }
            return Json(false.ToJsonOKData());
        }

        [HttpGet]
        public ActionResult Credentials()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Credentials(string email, string password)
        {
            // test password
            var shipwire = new ShipwireService(email, password);
            if(!shipwire.VerifyCredentials())
            {
                return Json("Invalid credentials".ToJsonFail());
            }

            var cryptor = new AESCrypt();

            MASTERdomain.shipwireEmail = email;
            MASTERdomain.shipwirePassword = cryptor.Encrypt(password, subdomainid.Value.ToString());

            repository.Save();
            return Json("Credentials saved".ToJsonOKMessage());
        }

        [HttpPost]
        public ActionResult Clear()
        {
            MASTERdomain.shipwireEmail = "";
            MASTERdomain.shipwirePassword = "";

            repository.Save();
            return Json(true.ToJsonOKData());
        }

        [HttpPost]
        public ActionResult Export()
        {
            try
            {
                var owner = sessionid.Value;
                var folder = string.Concat(GeneralConstants.FILE_UPLOAD_PATH, owner, "/");
                if (!Directory.Exists(GeneralConstants.APP_ROOT_DIR + folder))
                {
                    Directory.CreateDirectory(GeneralConstants.APP_ROOT_DIR + folder);
                }
                FileStream fs =
                    new FileStream(
                        GeneralConstants.APP_ROOT_DIR + "/Content/templates/shipwire.xls",
                        FileMode.Open, FileAccess.Read);

                HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
                var sheet = templateWorkbook.GetSheet("Template");

                // get currency
                var currencycode = MASTERdomain.currency.ToCurrencyCode();

                // rows start from 0
                int rowcount = 16;

                // get products
                var variants = repository.GetProductVariants(subdomainid.Value);
                foreach (var variant in variants)
                {
                    var sku = variant.sku;
                    if (sku.Length > 12 || sku.HasNonword())
                    {
                        continue;
                    }

                    var title = variant.product.title.Substring(0, Math.Min(50, variant.product.title.Length));

                    var row = sheet.CreateRow(rowcount);
                    row.CreateCell(0).SetCellValue(sku);
                    row.CreateCell(1).SetCellValue(SecurityElement.Escape(title));
                    
                    if (!string.IsNullOrEmpty(variant.product.dimensions))
                    {
                        var dimension = variant.product.dimensions.ToDimension();
                        row.CreateCell(3).SetCellValue(dimension.length.ConvertDistance(false).ToString("N1"));
                        row.CreateCell(4).SetCellValue(dimension.width.ConvertDistance(false).ToString("N1"));
                        row.CreateCell(5).SetCellValue(dimension.height.ConvertDistance(false).ToString("N1"));
                        row.CreateCell(6).SetCellValue(dimension.weight.ConvertWeight(false).ToString("N1"));
                    }

                    string costPrice = "";
                    string sellingPrice = "";
                    if (currencycode != "USD")
                    {
                        if (variant.product.costPrice != null)
                            costPrice = CurrencyConverter.Instance.Convert(currencycode, "USD", variant.product.costPrice.Value).ToString("n2");
                        if (variant.product.sellingPrice != null)
                            sellingPrice =
                                CurrencyConverter.Instance.Convert(currencycode, "USD", variant.product.sellingPrice.Value).ToString
                                    ("n2");
                    }
                    else
                    {
                        if (variant.product.costPrice != null) costPrice = variant.product.costPrice.Value.ToString("n2");
                        if (variant.product.sellingPrice != null) sellingPrice = variant.product.sellingPrice.Value.ToString("n2");
                    }

                    row.CreateCell(7).SetCellValue(costPrice);
                    row.CreateCell(8).SetCellValue(sellingPrice);


                    if (!string.IsNullOrEmpty(variant.product.shipwireDetails))
                    {
                        var shipwire = variant.product.shipwireDetails.ToShipwire();
                        row.CreateCell(2).SetCellValue(shipwire.pnp);
                        row.CreateCell(9).SetCellValue(shipwire.category.ToInt());
                        row.CreateCell(10).SetCellValue(shipwire.fragile ? 1 : 0);
                        row.CreateCell(11).SetCellValue(shipwire.dangerous ? 1 : 0);
                        row.CreateCell(12).SetCellValue(shipwire.perishable ? 1 : 0);
                    }
                    rowcount++;
                }

                var ms = new MemoryStream();
                templateWorkbook.Write(ms);

                // return created file path);
                return File(ms.ToArray(), "application/vnd.ms-excel", "shipwire_tradelr.xls");
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
        }
    }
}

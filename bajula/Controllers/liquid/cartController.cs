using System;
using System.Linq;
using System.Web.Mvc;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using tradelr.Models.liquid.models.Cart;
using tradelr.Models.products;

namespace tradelr.Controllers.liquid
{
    public class CartController : baseController
    {
        // id = variant id
        public ActionResult Add(long id, int quantity = 1, bool isJson = false)
        {
            cart cart = null;
            var cartid = GetCartIdFromCookie();

            // if not then we create cookie
            if (!string.IsNullOrEmpty(cartid))
            {
                cart = MASTERdomain.carts.SingleOrDefault(x => x.id.ToString() == cartid);
            }

            if (cart == null)
            {
                cart = new cart
                           {
                               subdomainid = subdomainid.Value
                           };
                MASTERdomain.carts.Add(cart);
            }

            // check that we have enough products
            var variant = repository.GetProductVariants(subdomainid.Value).SingleOrDefault(x => x.id == id);
            if (variant == null)
            {
                return Content(CreatePageMissingTemplate().Render());
            }

            if (!variant.HasStock(quantity))
            {
                var instock = variant.ToQuantity();
                if (isJson)
                {
                    return
                        Json(string.Format("We have only {0} of {1} left", instock, variant.ToProductFullTitle()).ToJsonFail());
                }
                quantity = instock;
            }

            // add item to cart
            var item = cart.cartitems.SingleOrDefault(x => x.variantid == id);
            if (item != null)
            {
                item.quantity = quantity;
            }
            else
            {
                // only add if item has a price
                if (variant.product.sellingPrice.HasValue)
                {
                    item = new cartitem();
                    item.variantid = id;
                    item.quantity = quantity;
                    cart.cartitems.Add(item);
                }
            }

            // save to db
            repository.Save();

            // store cookie
            Response.Cookies["cart"].Value = cart.id.ToString();
            Response.Cookies["cart"].Expires = DateTime.UtcNow.AddDays(GeneralConstants.COOKIE_LIFETIME);

            if (isJson)
            {
                var liquidmodel = item.ToLiquidModel(sessionid);
                return Json(liquidmodel);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Change(long id, int quantity, bool isJson = false)
        {
            var cartid = GetCartIdFromCookie();
            if (string.IsNullOrEmpty(cartid))
            {
                return Content(CreatePageMissingTemplate().Render());
            }

            var cart = MASTERdomain.carts.Where(x => x.id == new Guid(cartid)).SingleOrDefault();

            if (cart != null)
            {
                var item = cart.cartitems.Where(x => x.id == id).SingleOrDefault();
                if (item != null)
                {
                    if (quantity == 0)
                    {
                        db.cartitems.DeleteOnSubmit(item);
                    }
                    else
                    {
                        // check that we have enough products
                        var variant = repository.GetProductVariants(subdomainid.Value).Where(x => x.id == item.variantid).SingleOrDefault();
                        if (variant.product.trackInventory)
                        {
                            var instock = variant.ToQuantity();
                            if (!variant.HasStock(quantity))
                            {
                                if (isJson)
                                {
                                    return
                                        Json(string.Format("We have only {0} of {1} left", instock, variant.ToProductFullTitle()).ToJsonFail());
                                }
                                quantity = instock;
                            }
                        }

                        item.quantity = quantity;
                    }
                }
            }

            repository.Save();

            if (isJson)
            {
                var liquidmodel = cart.ToLiquidModel(sessionid);
                return Json(liquidmodel);
            }

            return RedirectToAction("Index");
        }


        public ActionResult GetJson()
        {
            var cartid = GetCartIdFromCookie();
            var liquidmodel = new Cart();
            if (!string.IsNullOrEmpty(cartid))
            {
                var cart = MASTERdomain.carts.Where(x => x.id.ToString() == cartid).SingleOrDefault();
                if (cart != null)
                {
                    liquidmodel = cart.ToLiquidModel(sessionid);
                }
            }

            return Json(liquidmodel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(int[] updates, string note, string coupon_code)
        {
            var cartid = GetCartIdFromCookie();
            if (!string.IsNullOrEmpty(cartid))
            {
                var cartToUpdate = MASTERdomain.carts.Where(x => x.id.ToString() == cartid).SingleOrDefault();

                if (cartToUpdate != null &&
                    updates != null)
                {
                    for (int i = 0; i < updates.Length; i++)
                    {
                        var quantity = updates[i];
                        var item = cartToUpdate.cartitems[i];
                        if (quantity == 0)
                        {
                            db.cartitems.DeleteOnSubmit(item);
                        }
                        else
                        {
                            var variant = repository.GetProductVariants(subdomainid.Value).Where(x => x.id == item.variantid).SingleOrDefault();
                            if (!variant.HasStock(quantity))
                            {
                                quantity = variant.ToQuantity();
                            }
                            item.quantity = quantity;
                        }
                    }

                    // update note
                    cartToUpdate.note = note;

                    // update discount code
                    cartToUpdate.coupon = coupon_code;

                    repository.Save();
                }
            }

            if (Request.Params.AllKeys.Where(x => x.IndexOf("checkout") != -1).Count() != 0)
            {
                return RedirectToAction("Index", "Checkout");
            }

            // now we render the page
            var template = CreateLiquidTemplate("cart", "Shopping Cart");
            template.InitContentTemplate("templates/cart.liquid");

            return Content(template.Render());
        }

    }
}

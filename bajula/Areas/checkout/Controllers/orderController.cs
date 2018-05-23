using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using clearpixels.Logging;
using tradelr.Areas.checkout.Models;
using tradelr.Areas.checkout.Models.emails;
using tradelr.Areas.dashboard.Models.shipping;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.currency;
using tradelr.Common.Models.photos;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Email;
using tradelr.Email.Models;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using tradelr.Models.activity;
using tradelr.Models.address;
using tradelr.Models.comments;
using tradelr.Models.counter;
using tradelr.Models.liquid.models.Product;
using tradelr.Models.payment;
using tradelr.Models.products;
using tradelr.Models.store;
using tradelr.Models.subdomain;
using tradelr.Models.transactions;
using tradelr.Models.users;

namespace tradelr.Areas.checkout.Controllers
{
    [TradelrHttps]
    public class orderController : baseCartController
    {
        public ActionResult address_step()
        {
            var viewmodel = new CheckoutAddressViewModel();
            if (sessionid.HasValue)
            {
                var usr = repository.GetUserById(sessionid.Value);
                if (usr != null)
                {
                    viewmodel.buyer_name = usr.firstName;
                    viewmodel.cartid = cart.id.ToString();
                    viewmodel.billing = usr.organisation1.address2 == null ? new Address(): usr.organisation1.address2.ToModel();
                    viewmodel.shipping = usr.organisation1.address1 == null ? new Address() : usr.organisation1.address1.ToModel();
                }
            }

            return View(viewmodel);
        }

        public ActionResult close()
        {
            return View("close");
        }

        [HttpPost]
        public ActionResult update_addresses(string billing_first_name, string billing_last_name, string billing_company, string billing_address, string billing_city, long? billing_citySelected, 
            string billing_postcode, string billing_phone,
            string shipping_first_name, string shipping_last_name, string shipping_company, string shipping_address, string shipping_city, long? shipping_citySelected, string shipping_postcode, string shipping_phone, 
            int[] country, string[] states_canadian, string[] states_other, string[] states_us,
            bool ship_same_billing)
        {
            var handler = new AddressHandler(cart.user.organisation1, repository);
            handler.SetShippingAndBillingAddresses(billing_first_name,
                                                   billing_last_name,
                                                   billing_company,
                                                   billing_address,
                                                   billing_city,
                                                   billing_citySelected,
                                                   billing_postcode,
                                                   billing_phone,
                                                   country[0],
                                                   states_canadian[0],
                                                   states_other[0],
                                                   states_us[0],
                                                   shipping_first_name,
                                                   shipping_last_name,
                                                   shipping_company,
                                                   shipping_address,
                                                   shipping_city,
                                                   shipping_citySelected,
                                                   shipping_postcode,
                                                   shipping_phone,
                                                   country[1],
                                                   states_canadian[1],
                                                   states_other[1],
                                                   states_us[1],
                                                   ship_same_billing);

            repository.Save();

            return RedirectToAction("final_step");
        }

        // json is true when checkout is done from an iframe, eg. facebook page
        public ActionResult create(CheckoutStatus status, string shippingmethod, string paymentmethod, bool isJson = false)
        {
            Debug.Assert(!cart.orderid.HasValue);

            var shop_owner = cart.MASTERsubdomain.organisation.users.First();
            var currency = cart.MASTERsubdomain.currency.ToCurrency();
            var buyer = cart.user;

            var transaction = new Transaction(cart.MASTERsubdomain, buyer, TransactionType.INVOICE, repository, sessionid.Value);
            transaction.CreateTransaction(
                repository.GetNewOrderNumber(subdomainid.Value, TransactionType.INVOICE),
                DateTime.UtcNow,
                cart.MASTERsubdomain.paymentTerms,
                currency.id);

            // mark as sent
            transaction.UpdateOrderStatus(OrderStatus.SENT);

            var shoppingcart = new ShoppingCart(currency.code)
                                   {
                                       shippingMethod = shippingmethod
                                   };

            foreach (var item in cart.cartitems)
            {
                var checkOutItem = item.product_variant.ToCheckoutItem(item.quantity, sessionid);
                var orderItem = new orderItem
                {
                    description = item.product_variant.ToProductFullTitle(),
                    variantid = item.product_variant.id,
                    unitPrice = item.product_variant.product.ToUserPrice(cart.userid.Value),
                    tax = item.product_variant.product.tax,
                    quantity = item.quantity
                };
                transaction.AddOrderItem(orderItem, item.product_variant.product.products_digitals);
                // update inventory
                transaction.UpdateInventoryItem(orderItem, item.quantity);

                shoppingcart.items.Add(checkOutItem);
            }

            if (!cart.isDigitalOrder())
            {
                shoppingcart.CalculateShippingCost(cart.cartitems.Select(x => x.product_variant).AsQueryable(), cart.MASTERsubdomain, buyer);

                if (cart.cartitems.Select(x => x.product_variant.product.shippingProfile).UseShipwire())
                {
                    transaction.UpdateShippingMethod(shoppingcart.shipwireShippingName, shoppingcart.shippingMethod);
                }
                else
                {
                    transaction.UpdateShippingMethod(shoppingcart.shippingMethod);
                }
            }
            
            transaction.UpdateTotal(cart.coupon);
            transaction.SaveNewTransaction(); ////////////////////// SAVE INVOICE

            repository.AddActivity(buyer.id,
                new ActivityMessage(transaction.GetID(), shop_owner.id,
                    ActivityMessageType.INVOICE_NEW,
                     new HtmlLink(transaction.GetOrderNumber(), transaction.GetID()).ToTransactionString(TransactionType.INVOICE)), subdomainid.Value);

            // add checkout note as a comment
            if (!string.IsNullOrEmpty(cart.note))
            {
                transaction.AddComment(cart.note, cart.userid.Value);
            }

            // add comment if shipping method not specified
            if (!transaction.HasShippingMethod() && !cart.isDigitalOrder())
            {
                transaction.AddComment(OrderComment.SHIPPING_WAIT_FOR_COST);
            }

            // set cart as processed
            cart.orderid = transaction.GetID();

            // save payment method
            if (!string.IsNullOrEmpty(paymentmethod))
            {
                switch (paymentmethod)
                {
                    case "paypal":
                        cart.paymentMethod = PaymentMethodType.Paypal.ToString();
                        break;
                    default:
                        cart.paymentMethod = PaymentMethodType.Custom.ToString();
                        cart.paymentCustomId = long.Parse(paymentmethod);
                        break;
                }
            }

            repository.Save();

            // send emails
            // send mail to buyer
            var buyerEmailContent = new OrderReceipt()
            {
                viewloc =
                    cart.MASTERsubdomain.ToHostName().ToDomainUrl(transaction.GetOrderLink()),
                shopname = cart.MASTERsubdomain.storeName,
                date = transaction.GetOrderDate().ToShortDateString(),
                shippingAddress = transaction.GetShippingAddress().ToHtmlString(),
                billingAddress = transaction.GetBillingAddress().ToHtmlString(),
                subtotal = string.Format("{0}{1}", currency.symbol, transaction.GetSubTotal().ToString("n" + currency.decimalCount)),
                shippingcost = string.Format("{0}{1}", currency.symbol, transaction.GetShippingCost().ToString("n" + currency.decimalCount)),
                discount = string.Format("{0}{1}", currency.symbol, transaction.GetDiscount().ToString("n" + currency.decimalCount)),
                totalcost = string.Format("{0}{1}{2}", currency.code, currency.symbol, transaction.GetTotal().ToString("n" + currency.decimalCount)),
                orderitems = transaction
                    .GetOrderItems()
                    .Select(x => string.Format("{0} x {1}{2} {3}",
                                                                x.quantity,
                                                                currency.symbol,
                                                                x.unitPrice.Value.ToString("n" + currency.decimalCount),
                                                                x.description))
            };

            // send mail to seller
            var sellerEmailContent = new NewOrderEmailContent
            {
                viewloc =
                    cart.MASTERsubdomain.ToHostName().ToDomainUrl(transaction.GetOrderLink()),
                sender = buyer.organisation1.name
            };

            string buyer_subject;
            string seller_subject;
            switch (status)
            {
                case CheckoutStatus.SHIPPING_FAIL:
                    buyer_subject = string.Format("[{0}]Invoice #{1}", cart.MASTERsubdomain.name,
                                           transaction.GetOrderNumber());
                    seller_subject = string.Format("[{0}]New Invoice #{1} : ACTION REQUIRED", cart.MASTERsubdomain.name,
                                           transaction.GetOrderNumber());
                    buyerEmailContent.message =
                        "Thank you for placing an order with us. Unfortunately, we are not able to provide a shipping cost at this moment. We will contact you once we have the shipping costs. You can check the status of your order by following the link below:";
                    sellerEmailContent.message = "A customer has placed an order on your online store. However, the shipping cost could not be calculated. You will need to manually update the invoice with the shipping cost. To update the invoice, follow the link below:";
                    break;
                case CheckoutStatus.SHIPPING_NONE:
                case CheckoutStatus.SHIPPING_OK:
                    buyer_subject = string.Format("[{0}]Invoice #{1} confirmed", cart.MASTERsubdomain.name,
                                           transaction.GetOrderNumber());
                    seller_subject = string.Format("[{0}]New Invoice #{1}", cart.MASTERsubdomain.name,
                                                   transaction.GetOrderNumber());

                    if (cart.isDigitalOrder())
                    {
                        buyerEmailContent.message = "Download links will be provided once payment is confirmed";
                    }
                    sellerEmailContent.message = "A customer has placed an order on your online store. To view the invoice, follow the link below:";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("status");
            }

            this.SendEmail(EmailViewType.INVOICEORDER_NEW, sellerEmailContent, seller_subject,
                            shop_owner.GetEmailAddress(), shop_owner.ToFullName(), buyer);

            this.SendEmail(EmailViewType.ORDER_RECEIPT, buyerEmailContent, buyer_subject,
                            buyer.GetEmailAddress(), buyer.ToFullName(), shop_owner);

            // handle payment
            string redirecturl = "";
            if (!string.IsNullOrEmpty(paymentmethod))
            {
                switch (paymentmethod)
                {
                    case "paypal":
                        string returnUrl;
                        if (isJson)
                        {
                            returnUrl = string.Format("{0}/checkout/order/{1}/close", GeneralConstants.HTTP_SECURE, cart.id);
                        }
                        else
                        {
                            returnUrl = string.Format("{0}/checkout/order/{1}", GeneralConstants.HTTP_SECURE, cart.id);
                        }
                        var pworker = new PaypalWorker(cart.id.ToString(),
                                                       transaction,
                                                       repository,
                                                       cart.MASTERsubdomain.GetPaypalID(),
                                                       transaction.GetCurrency().id,
                                                       returnUrl);
                        try
                        {
                            redirecturl = pworker.GetPaymentUrl();
                        }
                        catch (Exception ex)
                        {
                            Syslog.Write(ex);
                            return RedirectToAction("Index", "Error");
                        }
                        break;
                    default:
                        break;
                }
            }

            if (!string.IsNullOrEmpty(redirecturl))
            {
                return Redirect(redirecturl);
            }

            if (isJson)
            {
                return View("close");
            }
                        
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult create_account(string email_new, string password_new, string password2_new, string first_name, string last_name)
        {
            // check that passwords are equal
            if (password_new != password2_new)
            {
                return Json("passwords are not the same".ToJsonFail());
            }

            // check if user is already register
            var usr = repository.GetUsersByEmail(email_new, subdomainid.Value).SingleOrDefault();
            if (usr != null)
            {
                return Json("email is currently in use".ToJsonFail());
            }

            // create user
            usr = new user
            {
                role = (int)UserRole.USER,
                firstName = first_name,
                lastName = last_name,
                email = email_new,
                passwordHash = Crypto.Utility.ComputePasswordHash(email_new + password_new),
                viewid = Crypto.Utility.GetRandomString(),
                permissions = (int)UserPermission.USER
            };

            usr.organisation1 = new organisation
            {
                name = usr.ToFullName(),
                address = "",
                subdomain = subdomainid.Value
            };

            // update total contacts count
            repository.UpdateCounters(subdomainid.Value, 1, CounterType.CONTACTS_PRIVATE);

            // attach usr to current cart
            cart.userid = repository.AddUser(usr);

            repository.Save();

            UpdateUserSession(usr.id);

            var data = new Dictionary<string, object>();
            data.Add("host", usr.organisation1.MASTERsubdomain.ToHostName().ToDomainUrl(true));
            data.Add("orgname", usr.organisation1.MASTERsubdomain.organisation.name);
            data.Add("firstname", usr.firstName);

            this.SendEmail(EmailViewType.STORE_NEWUSER, data, "[" + usr.organisation1.MASTERsubdomain.name + "] New Account", usr.GetEmailAddress(), usr.ToFullName(), usr);

            if (cart.isDigitalOrder())
            {
                return RedirectToAction("final_step");
            }

            return RedirectToAction("address_step");
        }

        public ActionResult Index()
        {
            // check that cart has been processed
            if (!cart.orderid.HasValue)
            {
                return RedirectToAction("Index", "Cart", new {Area = "checkout"});
            }

            var viewmodel = new OrderCompletedViewModel(baseviewmodel)
                                {
                                    store_url = cart.MASTERsubdomain.ToHostName().ToDomainUrl(),
                                    order_number = cart.order.orderNumber.ToString("D8"),
                                    order_url = cart.order.ToOrderLink()
                                };

            viewmodel.paymentType = cart.paymentMethod.ToEnum<PaymentMethodType>();
            switch (viewmodel.paymentType)
            {
                case PaymentMethodType.Custom:
                    if (cart.paymentCustomId.HasValue)
                    {
                        viewmodel.paymentmethod.instructions = cart.paymentMethod1.instructions;
                        viewmodel.paymentmethod.name = cart.paymentMethod1.name;
                    }
                    break;
                case PaymentMethodType.Paypal:
                    viewmodel.paymentmethod.name = "Paypal";
                    break;
                default:
                    break;
            }

            foreach (var entry in cart.cartitems)
            {
                var cartitem = new CartItem()
                                   {
                                       name = entry.product_variant.ToProductFullTitle(),
                                       quantity = entry.quantity,
                                       thumbnail_url = entry.product_variant.product.thumb.HasValue
                                                           ? entry.product_variant.product.product_image.ToModel(
                                                               Imgsize.THUMB).url
                                                           : GeneralConstants.PHOTO_NO_THUMBNAIL
                                   };
                viewmodel.cart_items.Add(cartitem);
            }

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult login_account(string email_existing, string password_existing)
        {
            var usr = repository.GetUserByEmailAndPassword(email_existing + password_existing, subdomainid.Value);
            if (usr != null)
            {
                if (cart.userid.HasValue && cart.userid.Value != usr.id)
                {
                    return Json("This is not your cart".ToJsonFailData());
                }

                if (!cart.userid.HasValue)
                {
                    cart.userid = usr.id;
                    repository.Save();
                }
                UpdateUserSession(usr.id);
            }
            else
            {
                return Json("The email or password that you entered is incorrect".ToJsonFail());
            }

            if (cart.isDigitalOrder())
            {
                return RedirectToAction("final_step");
            }

            return RedirectToAction("address_step");
        }

        public ActionResult final_step()
        {
            var viewmodel = new CreateOrderViewModel();
            viewmodel.cartid = cart.id.ToString();

            // is all orderitems digital products?
            viewmodel.isDigitalOrder = cart.isDigitalOrder();

            // shipping
            viewmodel.shipping.shippingAddress = cart.user.organisation1.address1.ToHtmlString();

            var cartItems = new List<CheckoutItem>();
            foreach (var item in cart.cartitems)
            {
                var checkOutItem = item.product_variant.ToCheckoutItem(item.quantity, sessionid);

                cartItems.Add(checkOutItem);
            }

            var shippingProfiles = cart.cartitems
                                        .Select(x => x.product_variant.product.shippingProfile)
                                        .ToArray();
            viewmodel.shipping.shippingMethods = cartItems.ToShippingMethods(cart.MASTERsubdomain, cart.user.organisation1,
                                                                    shippingProfiles)
                                                .Select(x => new SelectListItem()
                                                {
                                                    Text = string.Format("{0} - {1}{2}", x.name, x.currency, x.cost),
                                                    Value = x.id
                                                });

            // payment
            viewmodel.payment.paymentMethods.Initialise(cart.MASTERsubdomain, false);

            // shipping costs known ?
            viewmodel.hasShippingMethods = viewmodel.shipping.shippingMethods != null &&
                                           viewmodel.shipping.shippingMethods.Count() != 0;



            return View(viewmodel);
        }

        
    }
}

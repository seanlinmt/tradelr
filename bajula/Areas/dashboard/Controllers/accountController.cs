using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using OpenSRS;
using OpenSRS.Services;
using PhoneNumbers;
using clearpixels.Logging;
using tradelr.Areas.dashboard.Models.account;
using tradelr.Common.Models.currency;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.DBML.Lucene;
using tradelr.Email;
using tradelr.Email.Models;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Helpers;
using tradelr.Library;
using tradelr.Library.Caching;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using tradelr.Library.geo;

using tradelr.Models.account;
using tradelr.Models.account.plans;
using tradelr.Models.address;
using tradelr.Models.contacts;
using tradelr.Models.payment;
using tradelr.Models.subdomain;
using tradelr.Models.transactions;
using tradelr.Models.users;
using Contact = OpenSRS.Contact;

// handles all stuff to do with account settings
namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [TradelrHttps]
    public class accountController : baseController
    {
        [RoleFilter(role = UserRole.CREATOR)]
        [HttpPost]
        public ActionResult DomainLookup(string domain_name)
        {
            var message = new Domain();
            var code = message.IsDomainTaken(domain_name);

            return Json(code.ToJsonOKData());
        }

        [RoleFilter(role = UserRole.CREATOR)]
        [HttpPost]
        public ActionResult DomainOrder(bool id_theft, string plans, string domain_name, string domain_ext)
        {
            // check that domain extension is supported
            if (!SupportedTLD.Extensions.Contains(domain_ext))
            {
                throw new NotImplementedException();
            }

            // get buyer
            var buyer = repository.GetUserById(sessionid.Value, subdomainid.Value);

            // check that user has all relevant fields filled up
            if (string.IsNullOrEmpty(buyer.firstName) ||
                string.IsNullOrEmpty(buyer.lastName) ||
                string.IsNullOrEmpty(buyer.organisation1.name) ||
                string.IsNullOrEmpty(buyer.organisation1.address) ||
                !buyer.organisation1.city.HasValue ||
                string.IsNullOrEmpty(buyer.organisation1.state) ||
                string.IsNullOrEmpty(buyer.organisation1.postcode) ||
                !buyer.organisation1.country.HasValue ||
                string.IsNullOrEmpty(buyer.organisation1.phone) ||
                string.IsNullOrEmpty(buyer.email))
            {
                return Json("Please ensure that your personal & organisation details are complete".ToJsonFail());
            }

            // get clear pixels domain
            var clearpixeldomain = repository.GetSubDomains().Single(x => x.name == "clearpixels");

            var transaction = new Transaction(clearpixeldomain, buyer, TransactionType.INVOICE, repository,
                                              sessionid.Value);

            var ordernumber = repository.GetNewOrderNumber(clearpixeldomain.id, TransactionType.INVOICE);
            transaction.CreateTransaction(ordernumber, DateTime.UtcNow,"", CurrencyHelper.GetCurrencies().Single(x => x.code == "USD").id);
            transaction.UpdateOrderStatus(OrderStatus.SENT);

            // define id_theft order item
            var idtheft_variant = repository.GetProductVariant("id_theft", clearpixeldomain.id, null);
            var idtheft_orderitem = new orderItem()
                                        {
                                            description = idtheft_variant.product.title,
                                            variantid = idtheft_variant.id,
                                            unitPrice = idtheft_variant.product.sellingPrice,
                                            tax = idtheft_variant.product.tax
                                        };
            int numberOfYears;
            product_variant variant;
            switch (plans)
            {
                case "one_year":
                    variant = repository.GetProductVariant("one_year", clearpixeldomain.id, null);
                    if (variant == null)
                    {
                        throw new NotImplementedException();
                    }
                    numberOfYears = 1;
                    break;
                case "two_year":
                    variant = repository.GetProductVariant("two_year", clearpixeldomain.id, null);
                    if (variant == null)
                    {
                        throw new NotImplementedException();
                    }
                    numberOfYears = 2;
                    break;
                case "five_year":
                    variant = repository.GetProductVariant("five_year", clearpixeldomain.id, null);
                    if (variant == null)
                    {
                        throw new NotImplementedException();
                    }
                    numberOfYears = 5;
                    break;
                case "ten_year":
                    variant = repository.GetProductVariant("ten_year", clearpixeldomain.id, null);
                    if (variant == null)
                    {
                        throw new NotImplementedException();
                    }
                    numberOfYears = 10;
                    break;
                default:
                    throw new NotImplementedException();
            }

            var orderitem = new orderItem()
            {
                description = string.Format("{0}{1} {2}", domain_name, domain_ext, variant.product.title),
                variantid = variant.id,
                unitPrice = variant.product.sellingPrice,
                tax = variant.product.tax,
                quantity = numberOfYears
            };

            transaction.AddOrderItem(orderitem, null);

            if (id_theft)
            {
                idtheft_orderitem.quantity = numberOfYears;
                transaction.AddOrderItem(idtheft_orderitem, null);
            }

            // update order total
            transaction.UpdateTotal();
            transaction.SaveNewTransaction();

            // send email to seller
            Syslog.Write(string.Format("{0}{1} bought by subdomainid {2}", domain_name, domain_ext, subdomainid.Value));

            // handle payment
            var uniqueid = string.Format("{0}{1}", domain_name, domain_ext);
            var pworker = new PaypalWorker(uniqueid,
                                                       transaction,
                                                       repository,
                                                       clearpixeldomain.GetPaypalID(),
                                                       transaction.GetCurrency().id,
                                                       accountHostname.ToDomainUrl("/dashboard/account/DomainProcess?d=" + uniqueid));

            
            var redirecturl = pworker.GetPaymentUrl();

            return Json(redirecturl.ToJsonOKData());
        }

        [HttpGet]
        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult DomainProcess(string d)
        {
            var viewmodel = new DomainNameRegistrationViewModel(baseviewmodel);
            viewmodel.domain_name = d;
            return View(viewmodel);
        }

        [HttpPost]
        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult DomainProcessPoll(string domain_name)
        {
            // check if payment has gone through
            var done = false;

            var payment = repository.GetPaymentByReference(domain_name);
            if (payment == null)
            {
                return Json(accountHostname.ToDomainUrl("/dashboard").ToJsonOKData());
            }

            if (payment.status == PaymentStatus.Accepted.ToString())
            {
                done = true;
            }

            // payment has gone thru
            if (done)
            {
                bool privacy;
                int numberOfYears;

                // get order details
                var orderitems = payment.order.orderItems;
                if (orderitems.Count() == 2)
                {
                    privacy = true;
                    numberOfYears = orderitems.Single(x => x.product_variant.sku != "id_theft").quantity;
                }
                else
                {
                    privacy = false;
                    numberOfYears = orderitems.Single().quantity;
                }

                var owner = MASTERdomain.organisation.users.First();
                var country = owner.organisation1.country.ToCountry();

                // get formatted phone numbers
                var phoneutil = PhoneNumberUtil.GetInstance();
                var parsedNumber = phoneutil.Parse(owner.organisation1.phone, country.code);

                PhoneNumber parsedFax = null;
                if (!string.IsNullOrEmpty(owner.organisation1.fax))
                {
                    parsedFax = phoneutil.Parse(owner.organisation1.fax, country.code);
                }

                var contact = new Contact(owner.firstName,
                                          owner.lastName,
                                          owner.organisation1.name,
                                          owner.organisation1.address,
                                          owner.organisation1.MASTERcity.name,
                                          owner.organisation1.state,
                                          owner.organisation1.postcode,
                                          country.code,
                                          string.Format("+{0}.{1}", parsedNumber.CountryCode, parsedNumber.NationalNumber),
                                          parsedFax == null?"":string.Format("+{0}.{1}", parsedFax.CountryCode, parsedFax.NationalNumber),
                                          owner.email);

                // now we start moving the domain
                var opensrs = new Domain();
                var respcode = opensrs.RegisterDomain(domain_name, privacy, numberOfYears, contact);
                if (respcode == null)
                {
                    Syslog.Write("Unable to register domain for " + subdomainid.Value);
                    return Json("/error".ToJsonOKData());
                }

                Syslog.Write(string.Format("{0}:{1}", subdomainid.Value, respcode.text));

                if (respcode.code == "200")
                {
                    // update user details and redirect to login page
                    MASTERdomain.customDomain = domain_name;
                    repository.Save();
                    return Json(domain_name.ToDomainUrl("/login").ToJsonOKData());
                }
            }
            return Json("".ToJsonFailData());
        }

        [RoleFilter(role = UserRole.USER)]
        [HttpGet]
        public ActionResult Index()
        {
            var usr = repository.GetUserById(sessionid.Value, subdomainid.Value);
            var viewmodel = new AccountViewModel(baseviewmodel)
                                {
                                    customDomain = MASTERdomain.customDomain,
                                    contact = usr.ToModel(sessionid, subdomainid.Value),
                                    affiliateID = MASTERdomain.affiliateID,
                                    orgid = MASTERdomain.organisation.id
                                };

            if (MASTERdomain.affiliateReferrer.HasValue)
            {
                viewmodel.affiliateTo = string.Format("You were referred by <a target='_blank' href='http://{0}'>{1}</a>", 
                    MASTERdomain.MASTERsubdomain1.ToHostName(),
                    MASTERdomain.organisation.name);
            }

            // fill out addresses
            if (usr.organisation1.shippingAddressID.HasValue)
            {
                viewmodel.addresses.shipping = usr.organisation1.address1.ToModel();

            }
            if (usr.organisation1.billingAddressID.HasValue)
            {
                viewmodel.addresses.billing = usr.organisation1.address2.ToModel();
            }
            viewmodel.addresses.hideSameShippingCheckBox = true;

            if(permission.HasFlag(UserPermission.NETWORK_SETTINGS))
            {
                viewmodel.timezoneList = TimeZoneInfo.GetSystemTimeZones()
                    .AsQueryable()
                    .Select(x => new SelectListItem {Text = x.DisplayName, Value = x.Id})
                    .ToSelectList(viewmodel.contact.timezone);

                viewmodel.currencyList = CurrencyHelper.GetCurrencies()
                    .OrderBy(x => x.code)
                    .Select(x => new SelectListItem { Text = x.code + " " + x.name, Value = x.id.ToString() })
                    .ToSelectList(viewmodel.contact.currency);
                viewmodel.paypalID = usr.organisation1.MASTERsubdomain.GetPaypalID();

                viewmodel.offlineEnabled = viewmodel.contact.domainFlags.HasFlag(SubdomainFlags.OFFLINE_ENABLED);
            }

            ///// for creators only
            if (role.HasFlag(UserRole.CREATOR))
            {
                viewmodel.TLDExtensionList = SupportedTLD.Extensions.Select(x => new SelectListItem()
                                                                                     {
                                                                                         Text = x,
                                                                                         Value = x,
                                                                                         Selected = x == ".com"
                                                                                     });

                if (!MASTERdomain.trialExpired && MASTERdomain.trialExpiry.HasValue)
                {
                    viewmodel.isInTrialPeriod = true;
                    viewmodel.trialExpiryDate = MASTERdomain.trialExpiry.Value.ToString(GeneralConstants.DATEFORMAT_JAVASCRIPT);
                }
            }

            return View(viewmodel);
        }

        [HttpGet]
        public ActionResult Password()
        {
            return View();
        }

        [RoleFilter(role = UserRole.USER)]
        [HttpPost]
        public ActionResult Password(string newPass, string newPassConfirm)
        {
            if (sessionid == null)
            {
                return SendJsonSessionExpired();
            }

            var usr = repository.GetUserById(sessionid.Value);
            if (usr == null)
            {
                return SendJsonSessionExpired();
            }
            // check both passwords are similar
            if (newPass != newPassConfirm)
            {
                return Json("Passwords are not the same".ToJsonFail());
            }

            usr.passwordHash = Crypto.Utility.ComputePasswordHash(usr.email + newPass);

            // unset flag
            usr.settings &= ~(int)UserSettings.PASSWORD_RESET;
            repository.Save();

            return Json("Your password has been changed successfully".ToJsonOKMessage());
        }

        [RoleFilter(role = UserRole.CREATOR)]
        [HttpGet]
        public ActionResult Plan()
        {
            var usr = repository.GetUserById(sessionid.Value);
            var sd = usr.organisation1.MASTERsubdomain;
            var viewdata = new PlanViewData(baseviewmodel)
                               {
                                   hostName = accountHostname,
                                   accountType = sd.accountType.ToEnum<AccountPlanType>(),
                                   subdomainid = subdomainid.Value
                               };

            if (MASTERdomain.trialExpired)
            {
                viewdata.showPayTrialButton = true;
            }
            return View(viewdata);
        }

        /// <summary>
        /// sets new plan and sets status to pending
        /// </summary>
        /// <param name="id">name of plan</param>
        /// <returns></returns>
        [HttpPost]
        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult Plan(string id)
        {
            var plan = id.ToEnum<AccountPlanType>();
            if (plan == AccountPlanType.UNKNOWN)
            {
                return Json("Unknown Plan".ToJsonFail());
            }
            var sd = db.GetSubDomain(accountSubdomainName);
            sd.accountTypeNew = plan.ToString();
            sd.accountTypeStatus |= (int)AccountPlanPaymentStatus.PENDING;

            repository.Save();

            return Json("OK".ToJsonOKMessage());
        }

        [HttpPost]
        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult SSLOrder(string plans_ssl)
        {
            // get buyer
            var buyer = repository.GetUserById(sessionid.Value, subdomainid.Value);

            // check that user has all relevant fields filled up
            if (string.IsNullOrEmpty(buyer.firstName) ||
                string.IsNullOrEmpty(buyer.lastName) ||
                string.IsNullOrEmpty(buyer.organisation1.name) ||
                string.IsNullOrEmpty(buyer.organisation1.address) ||
                !buyer.organisation1.city.HasValue ||
                string.IsNullOrEmpty(buyer.organisation1.state) ||
                string.IsNullOrEmpty(buyer.organisation1.postcode) ||
                !buyer.organisation1.country.HasValue ||
                string.IsNullOrEmpty(buyer.organisation1.phone) ||
                string.IsNullOrEmpty(buyer.email))
            {
                return Json("Please ensure that your personal & organisation details are complete".ToJsonFail());
            }

            // get clear pixels domain
            var clearpixeldomain = repository.GetSubDomains().Single(x => x.name == "clearpixels");

            var transaction = new Transaction(clearpixeldomain, buyer, TransactionType.INVOICE, repository,
                                              sessionid.Value);

            var ordernumber = repository.GetNewOrderNumber(clearpixeldomain.id, TransactionType.INVOICE);
            transaction.CreateTransaction(ordernumber, DateTime.UtcNow, "", CurrencyHelper.GetCurrencies().Where(x => x.code == "USD").Single().id);
            transaction.UpdateOrderStatus(OrderStatus.SENT);

            var variant = repository.GetProductVariant(plans_ssl, clearpixeldomain.id, null);

            var orderitem = new orderItem()
            {
                description = variant.product.title,
                variantid = variant.id,
                unitPrice = variant.product.sellingPrice,
                tax = variant.product.tax,
                quantity = 1
            };

            transaction.AddOrderItem(orderitem, null);

            // update order total
            transaction.UpdateTotal();
            transaction.SaveNewTransaction();

            // send email to seller
            Syslog.Write(string.Format("{0} bought by subdomainid {1}", plans_ssl, subdomainid.Value));

            // handle payment
            var uniqueid = string.Format("{0}:{1}", accountSubdomainName, plans_ssl);
            var pworker = new PaypalWorker(uniqueid,
                                                       transaction,
                                                       repository,
                                                       clearpixeldomain.GetPaypalID(),
                                                       transaction.GetCurrency().id,
                                                       accountHostname.ToDomainUrl("/dashboard/account/SSLProcess?d=" + plans_ssl));

            var redirecturl = pworker.GetPaymentUrl();

            return Json(redirecturl.ToJsonOKData());
        }

        [HttpGet]
        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult SSLProcess(string d)
        {
            var viewmodel = new SSLRegistrationViewModel(baseviewmodel);
            viewmodel.ssl_plan = d;
            return View(viewmodel);
        }

        [HttpPost]
        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult SSLProcessPoll(string plan)
        {
            // check if payment has gone through
            var done = false;
            var uniqueid = string.Format("{0}:{1}", accountSubdomainName, plan);
            var payment = repository.GetPaymentByReference(uniqueid);
            if (payment == null)
            {
                return Json(accountHostname.ToDomainUrl("/dashboard").ToJsonOKData());
            }

            if (payment.status == PaymentStatus.Accepted.ToString())
            {
                done = true;
            }

            // payment has gone thru
            if (done)
            {
                var owner = MASTERdomain.organisation.users.First();
                var country = owner.organisation1.country.ToCountry();

                // get formatted phone numbers
                var phoneutil = PhoneNumberUtil.GetInstance();
                var parsedNumber = phoneutil.Parse(owner.organisation1.phone, country.code);

                PhoneNumber parsedFax = null;
                if (!string.IsNullOrEmpty(owner.organisation1.fax))
                {
                    parsedFax = phoneutil.Parse(owner.organisation1.fax, country.code);
                }

                var contact = new Contact(owner.firstName,
                                          owner.lastName,
                                          owner.organisation1.name,
                                          owner.organisation1.address,
                                          owner.organisation1.MASTERcity.name,
                                          owner.organisation1.state,
                                          owner.organisation1.postcode,
                                          country.code,
                                          string.Format("+{0}.{1}", parsedNumber.CountryCode, parsedNumber.NationalNumber),
                                          parsedFax == null ? "" : string.Format("+{0}.{1}", parsedFax.CountryCode, parsedFax.NationalNumber),
                                          owner.email);

                // now we start moving the domain
                int numberOfYears;
                switch (plan)
                {
                    case "one_year_ssl":
                        numberOfYears = 1;
                        break;
                    case "two_year_ssl":
                        numberOfYears = 2;
                        break;
                    case "three_year_ssl":
                        numberOfYears = 3;
                        break;
                    case "four_year_ssl":
                        numberOfYears = 4;
                        break;
                    case "five_year_ssl":
                        numberOfYears = 5;
                        break;
                    default:
                        throw new NotImplementedException();
                }


                var opensrs = new Trust();
                var respcode = opensrs.RegisterSSL(accountHostname, numberOfYears, contact);
                if (respcode == null)
                {
                    Syslog.Write("Unable to register SSL for " + subdomainid.Value);
                    return Json("/error".ToJsonOKData());
                }

                Syslog.Write(string.Format("{0}:{1}", subdomainid.Value, respcode.text));

                if (respcode.code == "200")
                {
                    // update user details and redirect to login page
                    return Json(accountHostname.ToDomainUrl("/dashboard").ToJsonOKData());
                }
            }
            return Json("".ToJsonFailData());
        }

        [HttpPost]
        [RoleFilter(role = UserRole.USER)]
        public ActionResult UpdateAddresses(int?[] country, string[] states_canadian, string[] states_other, string[] states_us,
            string billing_first_name, string billing_last_name, string billing_company, string billing_address, string billing_city, long? billing_citySelected,
            string billing_postcode, string billing_phone, string shipping_first_name, string shipping_last_name, string shipping_company,
            string shipping_address, string shipping_city, long? shipping_citySelected, string shipping_postcode, string shipping_phone)
        {
            var ownerid = sessionid.Value;
            var profile = repository.GetUserById(ownerid, subdomainid.Value);
            if (profile == null)
            {
                return SendJsonErrorResponse("Cannot find profile");
            }

            var addressHandler = new AddressHandler(profile.organisation1, repository);
            addressHandler.SetShippingAndBillingAddresses(billing_first_name,
                                                          billing_last_name,
                                                          billing_company,
                                                          billing_address,
                                                          billing_city,
                                                          billing_citySelected,
                                                          billing_postcode,
                                                          billing_phone,
                                                          country.ElementAtOrDefault(0),
                                                          states_canadian.ElementAtOrDefault(0),
                                                          states_other.ElementAtOrDefault(0),
                                                          states_us.ElementAtOrDefault(0),
                                                          shipping_first_name,
                                                          shipping_last_name,
                                                          shipping_company,
                                                          shipping_address,
                                                          shipping_city,
                                                          shipping_citySelected,
                                                          shipping_postcode,
                                                          shipping_phone,
                                                          country.ElementAtOrDefault(1),
                                                          states_canadian.ElementAtOrDefault(1),
                                                          states_other.ElementAtOrDefault(1),
                                                          states_us.ElementAtOrDefault(1),
                                                          false);

            repository.Save();
            CacheHelper.Instance.invalidate_dependency(DependencyType.products_subdomain, subdomainid.Value.ToString());
            CacheHelper.Instance.invalidate_dependency(DependencyType.organisation, subdomainid.Value.ToString());
#if LUCENE
            // update index
            var indexer = new LuceneWorker(db, profile.organisation1.MASTERsubdomain.ToIdName());
            indexer.AddToIndex(LuceneIndexType.CONTACTS, profile);
#endif
            return Json(OPERATION_SUCCESSFUL.ToJsonOKData());
        }

        [HttpPost]
        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult UpdateDomain(string domainName)
        {
            // custom domain
            if (!string.IsNullOrEmpty(domainName))
            {
                domainName = domainName.Replace("http://", "");
                domainName = domainName.Replace("https://", "");
                // check that domain does not already exist
                var domain = repository.GetSubDomains().Where(x => x.customDomain == domainName).SingleOrDefault();
                if (domain != null && domain.id != subdomainid.Value)
                {
                    return Json("Domain name is currently in use".ToJsonFail());
                }
                // check for invalid domain name
                if (domainName.EndsWith("tradelr.com"))
                {
                    return Json("Domain name is invalid".ToJsonFail());
                }
                // check that domain name resolves to our server ip
                try
                {
                    var addresses = Dns.GetHostAddresses(domainName).Select(x => x.ToString()).ToArray();
                    if (!addresses.Contains(GeneralConstants.SERVER_IP))
                    {
                        return
                            Json(string.Format("{0} has not been configured to point to {1}", domainName,
                                               GeneralConstants.SERVER_IP).ToJsonFail());
                    }
                }
                catch (Exception ex)
                {
                    Syslog.Write(ex);
                    return
                        Json(
                            "Invalid domain name entered. Domain name should be in the format www.yourdomain.com".ToJsonFail());
                }
            }

            MASTERdomain.customDomain = domainName;

            repository.Save();

            return Json(MASTERdomain.ToHostName().ToDomainUrl("/login").ToJsonOKData());
        }

        [HttpPost]
        [RoleFilter(role = UserRole.USER)]
        public ActionResult UpdateProfile(string address, string city, string citySelected, string coPhone,
            string companyName, int? country, string fax, string firstName,
            string gender, string lastName, string notes,
            string phone, string postcode, string title, string currency, string timezone,
            string email, string states_canadian, string states_other, string states_us)
        {
            var ownerid = sessionid.Value;

            try
            {
                var profile = repository.GetUserById(ownerid, subdomainid.Value);
                if (profile == null)
                {
                    return SendJsonErrorResponse("Cannot find profile");
                }

                // no need to take into account whether an organisation is there because it will always be created
                profile.organisation1.address = address;
                if (!string.IsNullOrEmpty(citySelected))
                {
                    profile.organisation1.city = int.Parse(citySelected);
                }
                else if (!string.IsNullOrEmpty(city))
                {
                    profile.organisation1.city = repository.AddCity(city).id;
                }
                profile.organisation1.phone = coPhone;
                profile.organisation1.name = companyName;
                profile.organisation1.fax = fax;

                if (country != null)
                {
                    profile.organisation1.country = country;
                    profile.organisation1.state = AddressHandler.GetState(country, states_us, states_canadian, states_other);
                }

                profile.firstName = firstName;
                profile.gender = gender;
                profile.lastName = lastName;
                profile.notes = notes;
                profile.phoneNumber = phone;
                profile.organisation1.postcode = postcode;
                profile.title = title;

                if (!string.IsNullOrEmpty(email) && email != profile.email)
                {
                    profile.email = email.Trim();

                    var password = Crypto.Utility.GetRandomString();

                    // save password hash
                    var hash = Crypto.Utility.ComputePasswordHash(email + password);
                    profile.passwordHash = hash;

                    // set flag
                    profile.settings |= (int)UserSettings.PASSWORD_RESET;

                    // email new password to user
                    var data = new ViewDataDictionary() { { "password", password } };
                    EmailHelper.SendEmail(EmailViewType.ACCOUNT_PASSWORD_RESET, data, "Password Reset", email, profile.ToFullName(), null);
                }

                if (permission.HasFlag(UserPermission.NETWORK_SETTINGS))
                {
                    if (!string.IsNullOrEmpty(timezone))
                    {
                        profile.timezone = timezone;
                    }

                    if (!string.IsNullOrEmpty(currency))
                    {
                        profile.organisation1.MASTERsubdomain.currency = int.Parse(currency);
                    }
                }

                repository.Save();
                CacheHelper.Instance.invalidate_dependency(DependencyType.products_subdomain, subdomainid.Value.ToString());
                CacheHelper.Instance.invalidate_dependency(DependencyType.organisation, subdomainid.Value.ToString());
#if LUCENE
                // update index
                var indexer = new LuceneWorker(db, profile.organisation1.MASTERsubdomain.ToIdName());
                indexer.AddToIndex(LuceneIndexType.CONTACTS, profile);
#endif
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            // will be intepreted as an error if Content() is used
            return Json(OPERATION_SUCCESSFUL.ToJsonOKData());
        }

    }
}
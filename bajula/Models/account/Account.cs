using System;
using System.Linq;
using System.Threading;
using clearpixels.Facebook.Resources;
using tradelr.Areas.dashboard.Models.theme;
using tradelr.Crypto;
using tradelr.DBML;
using tradelr.Libraries.Affiliate;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using tradelr.Library.payment;
using clearpixels.Logging;
using tradelr.Models.account.plans;
using tradelr.Models.address;
using tradelr.Models.shipping;
using tradelr.Models.subdomain;
using tradelr.Models.users;
using Utility = tradelr.Crypto.Utility;

namespace tradelr.Models.account
{
    public class Account
    {
        public MASTERsubdomain mastersubdomain { get; set; }
        public user usr { get; set; }

        private ITradelrRepository repository { get; set; }

        private string email { get; set; }
        private string password { get; set; }
        private string passwordConfirm { get; set; }
        private string loginPage { get; set; }
        private string affiliate { get; set; }
        private AccountPlanType plan { get; set; }

        // for fb
        public Account(ITradelrRepository repository, string email, string loginPage, AccountPlanType plan, string affiliate)
        {
            this.repository = repository;
            this.email = email;
            this.loginPage = loginPage;
            this.plan = plan;
            this.affiliate = affiliate;

            // create subdomain entry
            mastersubdomain = new MASTERsubdomain
                                  {
                                      flags = 0,
                                      name = loginPage,
                                      total_outofstock = 0,
                                      total_contacts_public = 0,
                                      total_contacts_private = 0,
                                      total_contacts_staff = 0,
                                      total_invoices_sent = 0,
                                      total_invoices_received = 0,
                                      total_orders_sent = 0,
                                      total_orders_received = 0,
                                      total_products_mine = 0,
                                      uniqueid = Utility.GetRandomString(10),
                                      accountType = plan.ToString(),
                                      accountTypeStatus = (int)AccountPlanPaymentStatus.TRIAL, // start date for 30-day trial campaign
                                      trialExpiry = DateTime.UtcNow.AddDays(30),
                                      affiliateID = AffiliateUtil.GenerateAffiliateID()
                                  };
        }

        // for email
        public Account(ITradelrRepository repository, string email, string password, string passwordConfirm, string loginPage, AccountPlanType plan, string affiliate)
            : this(repository, email, loginPage, plan, affiliate)
        {
            this.password = password;
            this.passwordConfirm = passwordConfirm;
        }

        public JsonData CreateAccountWithLoginPassword()
        {
            // check fields are not empty
            if (string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(passwordConfirm) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(loginPage))
            {
                return "Some fields are missing".ToJsonFail();
            }

            // verify subdomain is available
            loginPage = loginPage.Trim().ToLower();

            // BUG need to mark subdomain as not available in case of race condition
            if (!repository.IsDomainAvailable(loginPage))
            {
                return "The selected site name is not available. Please choose another name.".ToJsonFail();
            }

            // check passwords
            if (string.Compare(password, passwordConfirm) != 0)
            {
                return "Passwords do not match".ToJsonFail();
            }

            // verify that email has not been used to register another account
            if (repository.GetUsersByEmail(email).SingleOrDefault(x => (x.role & (int)UserRole.CREATOR) != 0) != null)
            {
                Syslog.Write("Email address in use: " + email);
                return "Email address is currently in use".ToJsonFail();
            }

            // check affiliate ID
            if (!string.IsNullOrEmpty(affiliate))
            {
                var referrer = repository.GetSubDomains().SingleOrDefault(x => x.affiliateID == affiliate);
                if (referrer == null)
                {
                    Syslog.Write("Invalid Affiliate ID: " + affiliate);
                    return "Invalid Affiliate ID".ToJsonFail(); 
                }
                mastersubdomain.affiliateReferrer = referrer.id;

                
            }

            usr = new user
            {
                role = (int)UserRole.TENTATIVE,
                viewid = Utility.GetRandomString(),
                permissions = (int)UserPermission.ADMIN,
                email = email,
                firstName = "",
                lastName = ""
            };

            repository.AddMasterSubdomain(mastersubdomain);
            // hash is created later

            // create organisation first
            var org = new organisation
            {
                subdomain = mastersubdomain.id,
                name = ""
            };
            usr.organisation = repository.AddOrganisation(org);
            org.users.Add(usr);
            mastersubdomain.organisation = org;
            CreateDataStructures();

            return mastersubdomain.ToHostName().ToDomainUrl("/login").ToJsonOKData();
        }

        public string CreateAccountWithFacebookLogin(User fb_usr)
        {
            if (!repository.IsDomainAvailable(mastersubdomain.name))
            {
                return "The selected site name is not available. Please choose another name.";
            }

            // check affiliate ID
            if (!string.IsNullOrEmpty(affiliate))
            {
                var referrer = repository.GetSubDomains().SingleOrDefault(x => x.affiliateID == affiliate);
                if (referrer == null)
                {
                    Syslog.Write("Invalid Affiliate ID: " + affiliate);
                    return "Invalid Affiliate ID";
                }
                mastersubdomain.affiliateReferrer = referrer.id;

                // valid id so give  60 days
                mastersubdomain.trialExpiry = DateTime.UtcNow.AddDays(60);
            }

            usr = new user
            {
                role = (int)UserRole.ADMIN,
                viewid = Utility.GetRandomString(),
                permissions = (int)UserPermission.ADMIN,
                FBID = fb_usr.id,
                email = email ?? "",
                externalProfileUrl = fb_usr.link,
                firstName = fb_usr.first_name,
                lastName = fb_usr.last_name,
                gender = fb_usr.gender,
                externalProfilePhoto = string.Format("{0}{1}/picture?type=large", GeneralConstants.FACEBOOK_GRAPH_HOST, fb_usr.id)
            };

            repository.AddMasterSubdomain(mastersubdomain);
            // hash is created later

            // create organisation first
            mastersubdomain.organisation = new organisation
            {
                subdomain = mastersubdomain.id,
                name = fb_usr.name
            };

            var addressHandler = new AddressHandler(mastersubdomain.organisation, repository);
            addressHandler.CopyShippingAndBillingAddressFromOrgAddress("", ""); 

            CreateDataStructures();

            return "";
        }

        private void CreateDataStructures()
        {
            // CREATE DEFAULT STRUCTURES
            // add default inventory location
            var loc = new inventoryLocation
            {
                name = GeneralConstants.INVENTORY_LOCATION_DEFAULT,
                subdomain = mastersubdomain.id,
                lastUpdate = DateTime.UtcNow
            };
            repository.AddInventoryLocation(loc, mastersubdomain.id);

            // add default shipping profile
            var shippingProfile = new shippingProfile()
            {
                title = "Default",
                type = ShippingProfileType.FLATRATE.ToString(),
                subdomainid = mastersubdomain.id,
                permanent = true
            };
            repository.AddShippingProfile(shippingProfile);

            // add default payment methods
            var method_bank = new paymentMethod
                             {
                                 method = PaymentMethod.BankTransfer.ToString(),
                                 name = "Bank Deposit", 
                                 instructions = "Please contact me for payment details"
                             };
            mastersubdomain.paymentMethods.Add(method_bank);

            var method_cod = new paymentMethod
            {
                method = PaymentMethod.COD.ToString(),
                name = "Cash On Delivery",
                instructions = ""
            };
            mastersubdomain.paymentMethods.Add(method_cod);

            usr.passwordHash = Utility.ComputePasswordHash(email + password);

            // if user exist then we still need to verify email
            Random rnd = RandomNumberGenerator.Instance;
            usr.confirmationCode = rnd.Next();

            repository.Save();

            // generate liquid stuff
            new Thread(() => ThemeHandler.GenerateDefaultStructures(mastersubdomain.id)).Start();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using tradelr.DBML.Helper;
using tradelr.Email;
using tradelr.Email.Models;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using clearpixels.Logging;
using tradelr.Models.account;
using tradelr.Models.account.viewmodel;
using tradelr.Models.address;
using tradelr.Models.register;
using tradelr.Models.subdomain;
using tradelr.Models.users;

namespace tradelr.Controllers.register
{
    [TradelrHttps]
    //[ElmahHandleError]
    public class registerController : baseController
    {
        [HttpPost]
        public ActionResult DomainAvailable(string loginPage)
        {
            var available = repository.IsDomainAvailable(loginPage);
            if (!available)
            {
                return Json(false.ToJsonOKData());
            }
            return Json(true.ToJsonOKData());
        }
#if !DEBUG
        [OutputCache(Duration = GeneralConstants.DURATION_1DAY_SECS, VaryByParam = "None")]
#endif
        public ActionResult Index()
        {
            var data = new Register { planName = AccountPlanType.ULTIMATE.ToDescriptionString() };
            return View("Index", data);
        }

        [OutputCache(Duration = GeneralConstants.DURATION_1DAY_SECS, VaryByParam = "None")]
        public ActionResult Single()
        {
            var data = new Register { planName = AccountPlanType.SINGLE.ToDescriptionString() };
            return View("Index", data);
        }

        [OutputCache(Duration = GeneralConstants.DURATION_1DAY_SECS, VaryByParam = "None")]
        public ActionResult Basic()
        {
            var data = new Register { planName = AccountPlanType.BASIC.ToDescriptionString() };
            return View("Index", data);
        }

        [OutputCache(Duration = GeneralConstants.DURATION_1DAY_SECS, VaryByParam = "None")]
        public ActionResult Pro()
        {
            var data = new Register { planName = AccountPlanType.PRO.ToDescriptionString() };
            return View("Index", data);
        }

        [OutputCache(Duration = GeneralConstants.DURATION_1DAY_SECS, VaryByParam = "None")]
        public ActionResult Ultimate()
        {
            var data = new Register { planName = AccountPlanType.ULTIMATE.ToDescriptionString() };
            return View("Index", data);
        }

        [HttpPost]
        public ActionResult Create(string affiliate, string email, string password, string passwordConfirm, string loginPage, string plan)
        {
            try
            {
                var account = new Account(repository, email, password, passwordConfirm, loginPage, plan.ToEnum<AccountPlanType>(), affiliate);
                var status = account.CreateAccountWithLoginPassword();
                if (!status.success)
                {
                    return Json(status);
                }
                var user =
                    repository.GetUsersByEmail(email).Single(x => x.organisation1.MASTERsubdomain.name == loginPage);

                // send confirmation email
                new Thread(() =>
                               {
                                   var viewdata = new ViewDataDictionary()
                                   {
                                       {"host", user.organisation1.MASTERsubdomain.ToHostName().ToDomainUrl()},
                                       {"confirmCode", user.confirmationCode},
                                       {"email", user.email}
                                   };
                                   EmailHelper.SendEmailNow(EmailViewType.ACCOUNT_CONFIRMATION, viewdata, "New Account Details and Email Verification Link",
                                                  user.email, user.ToFullName(), null);
                               }).Start();
                
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            return Json(loginPage.ToTradelrDomainUrl("/login?r=1").ToJsonOKData());
        }

        [RoleFilter(role = UserRole.CREATOR)]
        [HttpGet]
        [NoCache]
        public ActionResult Setup()
        {
            return View();
        }

        [RoleFilter(role = UserRole.CREATOR)]
        [HttpPost]
        public ActionResult Setup(string timezone, string currency, string organisation, string country,
            string address, string city, string citySelected, string postcode, string phone, string tos,
            string states_canadian, string states_other, string states_us)
        {
            if (sessionid == null)
            {
                return RedirectToLogin();
            }

            try
            {
                if (!string.IsNullOrEmpty(timezone)  &&
                    !string.IsNullOrEmpty(organisation) &&
                    !string.IsNullOrEmpty(country) &&
                    !string.IsNullOrEmpty(address) &&
                    (!string.IsNullOrEmpty(city) || !string.IsNullOrEmpty(citySelected)) &&
                    !string.IsNullOrEmpty(postcode) &&
                    !string.IsNullOrEmpty(tos))
                {
                    long? finalCity = null;
                    if (!string.IsNullOrEmpty(citySelected))
                    {
                        finalCity = long.Parse(citySelected);
                    }
                    else if (!string.IsNullOrEmpty(city))
                    {
                        finalCity = repository.AddCity(city).id;
                    }

                    var usr = repository.GetUserById(sessionid.Value);
                    if (usr == null)
                    {
                        throw new Exception("User not found");
                    }

                    // add organisation
                    var org = usr.organisation1;
                    org.name = organisation;
                    org.address = address;
                    org.city = finalCity;
                    org.postcode = postcode;
                    org.phone = phone;
                    org.country = int.Parse(country);
                    if (string.IsNullOrEmpty(currency))
                    {
                        currency = "432";
                    }
                    org.MASTERsubdomain.currency = int.Parse(currency);

                    if (!string.IsNullOrEmpty(country) && country == "185" && !string.IsNullOrEmpty(states_us))
                    {
                        org.state = states_us;
                    }
                    else if (!string.IsNullOrEmpty(country) && country == "32" && !string.IsNullOrEmpty(states_canadian))
                    {
                        org.state = states_canadian;
                    }
                    else
                    {
                        org.state = states_other;
                    }

                    // update shop name
                    org.MASTERsubdomain.storeName = org.name;

                    // update user information
                    usr.timezone = timezone;

                    // update shipping and billing addresses
                    var addressHandler = new AddressHandler(org, repository);
                    addressHandler.CopyShippingAndBillingAddressFromOrgAddress("","");   // first name and last name is null here

                    repository.Save();

                    var homeUrl = org.MASTERsubdomain.ToHostName().ToDomainUrl();
                    return Json(homeUrl.ToJsonOKData());
                }

                #region find out which field is 'missing'
                var missingfields = new List<string>();
                if (string.IsNullOrEmpty(timezone))
                {
                    missingfields.Add("timezone");
                }
                if (string.IsNullOrEmpty(currency))
                {
                    missingfields.Add("currency");
                }
                if (string.IsNullOrEmpty(organisation))
                {
                    missingfields.Add("organisation");
                }
                if (string.IsNullOrEmpty(country))
                {
                    missingfields.Add("country");
                }
                if (string.IsNullOrEmpty(address))
                {
                    missingfields.Add("address");
                }
                if (string.IsNullOrEmpty(city) && string.IsNullOrEmpty(citySelected))
                {
                    missingfields.Add("city");
                }
                if (string.IsNullOrEmpty(postcode))
                {
                    missingfields.Add("postcode");
                }
                if (string.IsNullOrEmpty(phone))
                {
                    missingfields.Add("phone");
                }
                if (string.IsNullOrEmpty(tos))
                {
                    missingfields.Add("tos");
                } 
                #endregion
                return SendJsonErrorResponse("Missing fields: " + string.Join(",", missingfields));
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
        }

        public ActionResult Verify(int? confirm, string email)
        {
            // look for the confirm code and update the role of the user
            var viewmodel = new AccountVerify {domainName = accountSubdomainName};
            if (confirm.HasValue)
            {
                var user = repository.VerifyAccount(confirm.Value);
                viewmodel.isValidCode = user != null;
                
                if (!viewmodel.isValidCode)
                {
                    Syslog.Write("Invalid code: " + confirm + ", email: " + email);
                }

                // set auth information
                if (user != null)
                {
                    SetAuthCookie(user, false);

                }
            }

            return View(viewmodel);
        }
    }
}
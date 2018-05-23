using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FacebookToolkit.Rest;
using Google.GData.Client;
using Google.GData.Contacts;
using tradelr.Areas.dashboard.Models.contact;
using tradelr.Areas.dashboard.Models.product;
using tradelr.Common;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.currency;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.DBML.Lucene;
using tradelr.Email;
using tradelr.Email.Models;
using tradelr.Libraries.Facebook;
using tradelr.Library;
using tradelr.Library.JSON;
using clearpixels.Logging;
using tradelr.Models.activity;
using tradelr.Models.address;
using tradelr.Models.comments;
using tradelr.Models.contacts;
using tradelr.Models.contacts.viewmodel;
using tradelr.Models.counter;
using tradelr.Libraries;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Extensions;
using tradelr.Libraries.Helpers;
using tradelr.Models.history;
using tradelr.Models.message;
using tradelr.Models.products;
using tradelr.Models.subdomain;
using tradelr.Models.transactions;
using tradelr.Models.users;
using Message = tradelr.Models.message.Message;
#if LUCENE

#endif

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class contactsController : baseController
    {
        [PermissionFilter(permission = UserPermission.CONTACTS_ADD)]
        [NoCache]
        public ActionResult Add()
        {
            if (MASTERdomain.trialExpired)
            {
                return RedirectToAction("TrialExpired", "Error", new { Area = ""});
            }
            var viewdata = new ContactViewModel(baseviewmodel)
                               {
                                   contact = new Contact()
                                                 {
                                                     canModify = true
                                                 },
                                   organisationList = MASTERdomain.organisations
                                       .OrderBy(x => x.name)
                                       .Select(x => new SelectListItem {Text = x.name, Value = x.id.ToString()})
                                       .ToSelectList(null, "Select ...", "")
                               };
            return View(viewdata);
        }

        [PermissionFilter(permission = UserPermission.CONTACTS_ADD)]
        public ActionResult Contacts()
        {
            var viewmodel = new ContactListViewModel(baseviewmodel)
            {
                contactTypes = typeof(ContactType).ToSelectList(true).ToFilterList()
            };
            viewmodel.PopulateContactGroups(repository, subdomainid.Value);

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Create(long? profilePhotoID, string password, string organisationPhotoID, string address, string city, 
            long? citySelected, string coPhone, int permissions,
            string companyName, string contactList, int? country, string email, string fax, string firstName,
            string gender, string lastName, string notes, string phone, string postcode, string title, long? existingOrg,
            string states_canadian, string states_other, string states_us, bool sendemail)
        {
            // add organisation even if fields are empty
            if (!string.IsNullOrEmpty(email))
            {
                // check if email already added
                var emailExist = repository.IsEmailInUse(email, subdomainid.Value);
                if (emailExist)
                {
                    return Json("Email has already been added".ToJsonFail());
                }
            }

            organisation o;
            long addedOrgID;
            if (!existingOrg.HasValue)
            {
                o = new organisation
                        {
                            subdomain = subdomainid.Value,
                            address = address.Trim(),
                            phone = coPhone,
                            name = companyName,
                            fax = fax,
                            postcode = postcode
                        };
                if (!string.IsNullOrEmpty(organisationPhotoID))
                {
                    o.logo = long.Parse(organisationPhotoID);
                }

                if (citySelected.HasValue)
                {
                    o.city = citySelected.Value;
                }
                else if (!string.IsNullOrEmpty(city))
                {
                    o.city = repository.AddCity(city).id;
                }

                if (country != null)
                {
                    o.country = country;
                    o.state = AddressHandler.GetState(country, states_us,
                                       states_canadian, states_other);
                }

                addedOrgID = repository.AddOrganisation(o);

                // update shipping and billing addresses
                var addressHandler = new AddressHandler(o, repository);
                addressHandler.CopyShippingAndBillingAddressFromOrgAddress("", ""); 

            }
            else
            {
                o = repository.GetOrganisation(existingOrg.Value, subdomainid.Value);
                if (o == null)
                {
                    return SendJsonErrorResponse("Company is invalid");
                }
                addedOrgID = o.id;
            }

            // add user
            var u = new user
            {
                created = DateTime.UtcNow,
                role = (int)UserRole.USER,
                email = email,
                passwordHash = Crypto.Utility.ComputePasswordHash(email + password),
                firstName = firstName,
                gender = gender,
                lastName = lastName,
                notes = notes,
                phoneNumber = phone,
                title = title,
                organisation = addedOrgID,
                viewid = Crypto.Utility.GetRandomString()
            };

            // only allow user to create user with permissions equal to or less than themselves
            var currentuser = repository.GetUserById(sessionid.Value, subdomainid.Value);
            var allowedPermission = currentuser.permissions & permissions;
            u.permissions = allowedPermission;

            try
            {
                if (profilePhotoID.HasValue)
                {
                    u.profilePhoto = profilePhotoID.Value;
                }

                repository.AddUser(u);

                
                // need to update entry in images table too since contextid will be the site creator's
                if (profilePhotoID.HasValue)
                {
                    var dbImage = repository.GetImage(profilePhotoID.Value);
                    if (dbImage != null)
                    {
                        dbImage.contextID = u.id;
                    }
                }

                // log activity
                repository.AddActivity(sessionid.Value,
                        new ActivityMessage(u.id, sessionid,
                            ActivityMessageType.CONTACT_NEW,
                             new HtmlLink(u.ToEmailName(true), u.id).ToContactString()), subdomainid.Value);

                // update total contacts count
                repository.UpdateCounters(subdomainid.Value, 1, CounterType.CONTACTS_PRIVATE);

                // add contact list filter
                if (!string.IsNullOrEmpty(contactList))
                {
                    var cf = new contactGroupMember()
                                 {
                                     groupid = long.Parse(contactList),
                                     userid = u.id
                                 };
                    repository.AddContactGroupMember(cf);
                }

                // email contact that was just added
                if (!string.IsNullOrEmpty(email) && sendemail)
                {
                    var me = repository.GetUserById(sessionid.Value, subdomainid.Value);
                    var viewmodel = new ContactNewViewModel
                    {
                        creatorEmail = me.email,
                        creatorName = me.ToEmailName(true),
                        hostName = accountHostname,
                        email = email,
                        password = password,
                        note = notes,
                        profile = u.ToProfileUrl()
                    };

                    // link to view profile
                    this.SendEmail(EmailViewType.CONTACT_NEWENTRY, viewmodel, "New Profile created", u.GetEmailAddress(), u.ToFullName(), u);
                }
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            // return contact ID & org ID
            return Json(new { uid = u.id, oid = addedOrgID}.ToJsonOKData());
        }

        [HttpPost]
        [PermissionFilter(permission = UserPermission.CONTACTS_MODIFY)]
        public ActionResult Delete(string id)
        {
            var contactids = id.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse);
            var successIDs = new List<long>();
            foreach (var contactid in contactids)
            {
                if (repository.IsContactInUse(contactid))
                {
                    continue;
                }
#if LUCENE
                var indexer = new LuceneWorker(db, MASTERdomain.ToIdName());
                indexer.DeleteFromIndex(LuceneIndexType.CONTACTS, contactid);
#endif
                // delete necessary data
                repository.DeleteUser(contactid, subdomainid.Value);
                successIDs.Add(contactid);
            }

            return Json(successIDs.ToJsonOKData());
        }

        [PermissionFilter(permission = UserPermission.CONTACTS_MODIFY)]
        public ActionResult Edit(long? id)
        {
            if (!id.HasValue)
            {
                return ReturnError("Contact edit:Bad Request");
            }
            var viewmodel = new ContactViewModel(baseviewmodel);
            viewmodel.editMode = true;
            try
            {
                var usr = repository.GetContact(subdomainid.Value, id.Value);
                viewmodel.contact = usr.ToModel(sessionid.Value, subdomainid.Value);
                viewmodel.contact.canModify = true;
                if (usr.organisation1.shippingAddressID.HasValue)
                {
                    viewmodel.addresses.shipping = usr.organisation1.address1.ToModel();

                }
                if (usr.organisation1.billingAddressID.HasValue)
                {
                    viewmodel.addresses.billing = usr.organisation1.address2.ToModel();
                }
                viewmodel.addresses.hideSameShippingCheckBox = true;

                viewmodel.organisationList = MASTERdomain.organisations
                    .OrderBy(x => x.name)
                    .Select(x => new SelectListItem {Text = x.name, Value = x.id.ToString()})
                    .ToSelectList();
                return View("Add", viewmodel);
            }
            catch (Exception ex)
            {
                return ReturnError(ex.Message);
            }
        }

        public ActionResult Export()
        {
            return View();
        }

        public ActionResult Find(string email, long? id)
        {
            // check if email already exist locally for private contacts
            var find = repository.GetUsersByEmail(email, subdomainid.Value);

            if (id.HasValue)
            {
                find = find.Where(x => x.id != id.Value);
            }

            var privateUser = find.SingleOrDefault();

            if (privateUser != null)
            {
                return Json(JavascriptReturnCodes.INUSE.ToJsonOKData());
            }

            // we only get creators
            var usr = repository.GetUsersByEmail(email).Where(x => (x.role & (int)UserRole.CREATOR) != 0).SingleOrDefault();

            // return detail of contact found
            if (usr != null && 
                subdomainid.HasValue &&
                usr.organisation1.subdomain != subdomainid.Value)
            {
                // check that we're not already linked
                var linked = repository.IsFriend(subdomainid.Value, usr.organisation1.subdomain);
                if (linked)
                {
                    return Json(JavascriptReturnCodes.ISLINKED.ToJsonOKData());
                }

                return Json(usr.ToModelBasic().ToJsonOKData());
            }
            return Json(JavascriptReturnCodes.NOTFOUND.ToJsonOKData());
        }

        [HttpPost]
        [JsonFilter(Param = "contacts", RootType = typeof(ContactBasic[]))]
        public ActionResult Import(ContactBasic[] contacts)
        {
            foreach (var contact in contacts)
            {
                // check if email already added
                var emailExist = repository.IsEmailInUse(contact.email, subdomainid.Value);
                if (emailExist)
                {
                    continue;
                }

                organisation o = new organisation
                                     {
                                         subdomain = subdomainid.Value,
                                         address = contact.address,
                                         name = contact.email
                                     };
                long addedOrgId = repository.AddOrganisation(o);

                // add user
                var u = new user
                            {
                                created = DateTime.UtcNow,
                                role = (int) UserRole.USER,
                                email = contact.email,
                                firstName = contact.firstName,
                                lastName = contact.lastName,
                                phoneNumber = contact.phone,
                                organisation = addedOrgId,
                                viewid = Crypto.Utility.GetRandomString(),
                                permissions = (int)UserPermission.USER
                            };

                repository.AddUser(u);

                // update total contacts count
                repository.UpdateCounters(subdomainid.Value, 1, CounterType.CONTACTS_PRIVATE);
            }
            repository.Save();

            return Json("".ToJsonOKMessage());
        }

        [HttpGet]
        public ActionResult Import(string type, string token)
        {
            var owner = db.GetUserById(sessionid.Value, subdomainid.Value);
            var viewdata = new ImportContactsViewData(baseviewmodel)
                               {
                                   hostName = accountHostname, 
                                   fbuid = owner.FBID, 
                                   subdomainid = subdomainid.Value, 
                                   appid = sessionid.Value
                               };

            // facebook
            var connectSession = UtilFacebook.GetConnectSession();
            if (connectSession.IsConnected())
            {
                var api = new Api(connectSession);
                IList<long> fdlist = api.Friends.GetAppUsers();
                string s = string.Empty;
                for (int i = 0; i < fdlist.Count; i++)
                {
                    s += fdlist[i].ToString();
                    if (i != fdlist.Count - 1)
                    {
                        s += ",";
                    }
                }
                viewdata.invitedFbuidList = s;
            }

            // if callback from authsub authorisation
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(token))
            {
                var importType = type.ToEnum<ContactImportType>();
                switch (importType)
                {
                    case ContactImportType.GOOGLE:
                        var requestFactory = new GAuthSubRequestFactory("cp", "tradelr");
                        requestFactory.Token = token;

                        var query = new ContactsQuery(ContactsQuery.CreateContactsUri("default"));
                        //query.OAuthRequestorId = string.Concat(sessionid.Value, '@', subdomainid.Value);

                        var service = new ContactsService(requestFactory.ApplicationName);
                        service.RequestFactory = requestFactory;
                        try
                        {
                            var feed = service.Query(query);
                            var contacts = new List<ContactBasic>();
                            foreach (ContactEntry entry in feed.Entries)
                            {
                                string email;
                                // primary email can be null
                                if (entry.PrimaryEmail == null)
                                {
                                    if (entry.Emails == null || entry.Emails.Count == 0)
                                    {
                                        continue; // skip entry, cannot find email
                                    }
                                    email = entry.Emails[0].Address;
                                }
                                else
                                {
                                    email = entry.PrimaryEmail.Address;
                                }

                                var contact = new ContactBasic()
                                {
                                    address = entry.BillingInformation,
                                    companyName = email,
                                    email = email
                                };

                                if (entry.Name != null)
                                {
                                    if (!string.IsNullOrEmpty(entry.Name.GivenName))
                                    {
                                        contact.firstName = entry.Name.GivenName;
                                    }

                                    if (!string.IsNullOrEmpty(entry.Name.FamilyName))
                                    {
                                        contact.lastName = entry.Name.FamilyName;
                                    }
                                }

                                if (entry.PrimaryPhonenumber == null)
                                {
                                    if (entry.Phonenumbers != null && entry.Phonenumbers.Count != 0)
                                    {
                                        contact.phone = entry.Phonenumbers[0].Value;
                                    }
                                }
                                else
                                {
                                    contact.phone = entry.PrimaryPhonenumber.Value;
                                }
                                
                                // now we need to format it nicely for display purposes

                                contacts.Add(contact);
                            }
                            viewdata.contacts = contacts.OrderBy(x => x.email);
                        }
                        catch (Exception ex)
                        {
                            Syslog.Write(ex);
                        }
                        
                        break;
                    default:
                        break;
                }
            }

            return View(viewdata);
        }

        [PermissionFilter(permission = UserPermission.CONTACTS_VIEW)]
        public ActionResult Index()
        {
            return View(baseviewmodel);
        }

        [HttpPost]
        [PermissionFilter(permission = UserPermission.CONTACTS_VIEW)]
        public ActionResult Grid(string cat, int rows, int page, string term,
                                 string sidx, string sord, ContactType? type, string letter)
        {
            if (!string.IsNullOrEmpty(cat))
            {
                cat = cat.Trim();
            }

            // to select all categories
            if (cat == "All")
            {
                cat = "";
            }

            IEnumerable<user> results = repository.GetContacts(subdomainid.Value, subdomainid.Value, cat, sidx, sord, type, letter);

#if LUCENE
            if (!string.IsNullOrEmpty(term))
            {
                var search = new LuceneSearch();
                var ids = search.ContactSearch(term.ToLowerInvariant(), accountSubdomainName);
                results = results.Where(x => ids.Select(y => y.id).Contains(x.id.ToString())).AsEnumerable();
                results = results.Join(ids, x => x.id.ToString(), y => y.id, (x, y) => new { x, y.score })
                    .OrderByDescending(x => x.score).Select(x => x.x);
            }
#endif

            var records = results.Count();
            var total = (records / rows);
            if (records % rows != 0)
            {
                total++;
            }

            // return in the format required for jqgrid
            results = results.Skip(rows * (page - 1)).Take(rows);
            var contacts = results.ToContactsJqGrid(subdomainid.Value);
            contacts.page = page;
            contacts.records = records;
            contacts.total = total;

            return Json(contacts);
        }

        [HttpPost]
        public ActionResult List(ContactType type)
        {
            IQueryable<organisation> orgs;
            switch (type)
            {
                case ContactType.PRIVATE:
                    orgs = repository.GetPrivateContacts(subdomainid.Value).Select(x => x.organisation1).Distinct();
                    break;
                case ContactType.NETWORK:
                    orgs = repository.GetPublicContacts(subdomainid.Value).Select(x => x.organisation1).Distinct();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }

            return
                Json(
                    orgs.OrderBy(x => x.name).Select(x => new SelectListItem() {Value = x.id.ToString(), Text = x.name})
                        .ToJsonOKData());
        }
        
        [HttpGet]
        public ActionResult listAdd()
        {
                var privatecontacts = repository.GetPrivateContacts(subdomainid.Value).ToModelBasic();
                var publiccontacts = repository.GetPublicContacts(subdomainid.Value).ToModelBasic();
                var contacts = privatecontacts.Union(publiccontacts)
                    .OrderBy(x => x.companyName)
                    .ThenBy(x => x.fullName);
                return View(contacts);
        }

        [HttpPost]
        public ActionResult listAdd(string filterTitle, string ids)
        {
            filterTitle = filterTitle.Trim();
            // if it's empty then
            if (string.IsNullOrEmpty(filterTitle))
            {
                return SendJsonErrorResponse("Please enter a name.");
            }

            var exist = MASTERdomain.contactGroups.Where(x => x.title == filterTitle).SingleOrDefault();
            if (exist != null)
            {
                return Json("Group already exist".ToJsonFail());
            }

            var group = new contactGroup()
            {
                title = filterTitle,
                subdomainid = subdomainid.Value
            };
            try
            {
                MASTERdomain.contactGroups.Add(group);

                if (!string.IsNullOrEmpty(ids))
                {
                    string[] userids = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    repository.UpdateContactGroupMembers(subdomainid.Value, group.id, userids);
                }
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            return Json(group.ToModel().ToJsonOKData());
        }
        
        [HttpPost]
        public ActionResult listDelete(long? id)
        {
            if (!id.HasValue)
            {
                return SendJsonErrorResponse("Invalid contact group ID");
            }
            try
            {
                repository.DeleteContactGroup(subdomainid.Value, id.Value);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            
            return Json(id.Value.ToJsonOKData());
        }

        [HttpGet]
        public ActionResult listEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return SendJsonErrorResponse("Bad Request");
            }

            var filterid = long.Parse(id);
            string listTitle = repository.GetContactGroup(filterid, subdomainid.Value).title;
            var privatecontacts = repository.GetPrivateContacts(subdomainid.Value).ToModelBasic();
            var publiccontacts = repository.GetPublicContacts(subdomainid.Value).ToModelBasic();
            var contactList = privatecontacts.Union(publiccontacts)
                .OrderBy(x => x.companyName)
                .ThenBy(x => x.fullName);

            var contactsIdsInCategory = string.Join(",", repository.GetContacts(subdomainid.Value, subdomainid.Value, id, "", "",null, "")
                                                             .Select(x => x.id.ToString())
                                                             .ToArray());
            var data = new Dictionary<string, object>();
            data.Add("listTitle", listTitle);
            data.Add("contactList", contactList);
            data.Add("filterid", filterid);
            data.Add("ids", contactsIdsInCategory);
            return View(data);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult listEdit(long id, string ids)
        {
            try
            {
                repository.UpdateContactGroupMembers(subdomainid.Value, id, ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                return Json(id.ToJsonOKData());
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
        }

        [HttpPost]
        [PermissionFilter(permission = UserPermission.CONTACTS_VIEW)]
        public ActionResult NetworkFind(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(JavascriptReturnCodes.NOTFOUND.ToJsonOKData());
            }

            Uri networkUri = null;
            try
            {

                if (id.IndexOf('.') == -1)
                {
                    // user only enter store name
                    id = string.Format("{0}.tradelr.com", id);
                }
                if (!id.StartsWith("http"))
                {
                    // user did not enter http
                    id = string.Format("http://{0}", id);
                }
                networkUri = new Uri(id);
            }
            catch (Exception ex)
            {
                Syslog.Write("Unable to parse " + id);
            }

            // we only get creators
            user usr;
            if (networkUri != null)
            {
                if (networkUri.Host.Split('.').Length < 3)
                {
                    return Json(JavascriptReturnCodes.NOTFOUND.ToJsonOKData());
                }

                if (networkUri.Host.Contains("tradelr.com"))
                {

                    var subdomainname = networkUri.Host.Substring(0, networkUri.Host.IndexOf('.'));
                    var sd = repository.GetSubDomains().Where(x => x.name == subdomainname).SingleOrDefault();
                    if (sd == null)
                    {
                        return Json(JavascriptReturnCodes.NOTFOUND.ToJsonOKData());
                    }
                    usr = sd.organisation.users.First();
                }
                else
                {
                    // custom domain
                    var sd = repository.GetSubDomains().Where(x => x.customDomain == networkUri.Host).SingleOrDefault();
                    if (sd == null)
                    {
                        return Json(JavascriptReturnCodes.NOTFOUND.ToJsonOKData());
                    }
                    usr = sd.organisation.users.First();
                }
            }
            else
            {
                usr = repository.GetUsersByEmail(id).Where(x => (x.role & (int)UserRole.CREATOR) != 0).SingleOrDefault();
            }
            

            // return detail of contact found
            if (usr != null &&
                subdomainid.HasValue &&
                usr.organisation1.subdomain != subdomainid.Value)
            {
                // check that we're not already linked
                var linked = repository.IsFriend(subdomainid.Value, usr.organisation1.subdomain);
                if (linked)
                {
                    return Json(JavascriptReturnCodes.ISLINKED.ToJsonOKData());
                }

                return Json(usr.ToModelBasic().ToJsonOKData());
            }
            return Json(JavascriptReturnCodes.NOTFOUND.ToJsonOKData());
        }

        [HttpGet]
        [PermissionFilter(permission = UserPermission.CONTACTS_ADD)]
        public ActionResult NetworkLink()
        {
            return View();
        }

        [PermissionFilter(permission = UserPermission.CONTACTS_VIEW)]
        public ActionResult ProductTransactions(long id, string term)
        {
            IEnumerable<LuceneHit> ids = null;
#if LUCENE
            if (!string.IsNullOrEmpty(term))
            {
                var search = new LuceneSearch();
                ids = search.ProductSearch(term.ToLowerInvariant(), accountSubdomainName);
            }
#endif
            // invoices
            var invoiceitems = repository.GetOrders()
                .Where(x => x.type == TransactionType.ORDER.ToString() && x.receiverUserid == id)
                .SelectMany(x => x.orderItems);

            if (ids != null)
            {
                invoiceitems = invoiceitems.Where(x => ids.Select(y => y.id).Contains(x.product_variant.productid.ToString()));
            }

            var bought = invoiceitems
                .GroupBy(x => new {x.unitPrice, x.product_variant, x.order.currency})
                .Select(y => y.Key)
                .ToArray()
                .Select(z => new ContactTransaction()
                {
                    productName = z.product_variant.ToProductFullTitle(),
                    productediturl = z.product_variant.product.ToProductEditUrl(),
                    unitPrice = z.unitPrice.HasValue ? z.unitPrice.Value.ToString("n" + z.currency.ToCurrency().decimalCount) : ""
                });

            // orders
            var orderitems = repository.GetOrders()
                .Where(x => x.type == TransactionType.INVOICE.ToString() && x.receiverUserid == id)
                .SelectMany(x => x.orderItems);

            if (ids != null)
            {
                orderitems = orderitems.Where(x => ids.Select(y => y.id).Contains(x.product_variant.productid.ToString()));
            }

            var sold = orderitems
                .GroupBy(x => new { x.unitPrice, x.product_variant, x.order.currency })
                .Select(y => y.Key)
                .ToArray()
                .Select(z => new ContactTransaction()
                {
                    productName = z.product_variant.ToProductFullTitle(),
                    productediturl = z.product_variant.product.ToProductEditUrl(),
                    unitPrice = z.unitPrice.HasValue ? z.unitPrice.Value.ToString("n" + z.currency.ToCurrency().decimalCount) : ""
                });

            var viewmodel = new ContactTransactionsViewModel()
                                {
                                    products_sold = sold,
                                    products_bought = bought
                                };

            return View(viewmodel);
        }

        public ActionResult RequestAccept(long id)
        {
            // need to verify that a request is present
            var linkRequest = repository.GetLinkRequest(id);
            if (linkRequest != null)
            {
                // delete the message
                repository.DeleteMessage(linkRequest.userid, linkRequest.friendid,
                                         MessageType.LINKREQUEST);

                var friend = linkRequest.user;
                var me = linkRequest.user1;
                
                repository.AddFriend(me.organisation1.subdomain, friend.organisation1.subdomain);

                // delete link request
                repository.DeleteLinkRequest(linkRequest);

                // add activity to notify requester
                repository.AddActivity(me.id,
                        new ActivityMessage(sessionid.Value, friend.id,
                            ActivityMessageType.NETWORK_NEW,
                             new HtmlLink(me.ToEmailName(true), me.id).ToContactString()), friend.organisation1.subdomain);

                repository.Save();
            }

            return Json("".ToJsonOKMessage());
        }

        public ActionResult RequestIgnore(long id)
        {
            var linkRequest = repository.GetLinkRequest(id);
            if (linkRequest != null)
            {
                // delete the message
                repository.DeleteMessage(linkRequest.userid, linkRequest.friendid,
                                         MessageType.LINKREQUEST);
                repository.Save();
            }
            return Json("".ToJsonOKMessage());
        }

        public ActionResult RequestSend(long? id)
        {
            if (!id.HasValue)
            {
                return SendJsonErrorResponse("Contact ID not specified");
            }
            // has a request already being sent?
            var existingRequest = repository.GetLinkRequest(sessionid.Value, id.Value);
            if (existingRequest == null)
            {
                // send request to friend
                var me = repository.GetUserById(sessionid.Value, subdomainid.Value);
                var friend = repository.GetUserById(id.Value);
                var viewdata = new ContactLinkRequestViewData
                {
                    notificationLink =
                        friend.organisation1.MASTERsubdomain.ToHostName().ToDomainUrl("/notifications"),
                    senderEmail = me.email,
                    senderName = string.IsNullOrEmpty(me.ToFullName().Trim()) ? accountSubdomainName : me.ToFullName()
                };

                // save link request
                existingRequest = new linkRequest
                {
                    userid = sessionid.Value,
                    friendid = id.Value
                };

                try
                {
                    repository.AddLinkRequest(existingRequest);
                }
                catch (Exception ex)
                {
                    return SendJsonErrorResponse(ex);
                }

                // send notification
                var notification = new Message(friend, me, subdomainid.Value);
                notification.SendMessage(this, repository, EmailViewType.CONTACT_LINKREQUEST, viewdata,
                                         string.Format(
                                             "<a target='_blank' href='{0}'>{1}</a> wishes to link networks with you.",
                                             me.ToProfileUrl(), viewdata.senderName), viewdata.notificationLink,
                                         MessageType.LINKREQUEST);
            }
            return Json("".ToJsonOKMessage());
        }

        [PermissionFilter(permission = UserPermission.CONTACTS_VIEW)]
        public ActionResult Show(long id, long? domainid)
        {
            var networkContact = true;
            if (!domainid.HasValue)
            {
                domainid = subdomainid.Value;
                networkContact = false;
            }
            var usr = repository.GetUserById(id, domainid.Value);
            var viewmodel = new ContactViewModel(baseviewmodel)
            {
                contact = usr.ToModel(sessionid, subdomainid.Value, Imgsize.SMALL)
            };
            viewmodel.contact.address = viewmodel.contact.address.ToCommaSeparator();

            // need to get all product comments, transaction comments
            if (!networkContact)
            {
                var productcomments = usr.product_comments.ToContextualModel();
                viewmodel.comments.AddRange(productcomments);
            }
            
            var transactioncomments = usr.comments.ToContextualModel(domainid.Value);
            viewmodel.comments.AddRange(transactioncomments);

            return View(viewmodel);
        }

        

        [HttpPost]
        public ActionResult Update(long? id, string email, string address, string city, long? citySelected, string coPhone,
            string companyName, IEnumerable<int?> country, string fax, string firstName, int? permissions,
            string gender, string lastName, string notes, string phone, string postcode, string title, string password,
            IEnumerable<string> states_canadian, IEnumerable<string> states_other, IEnumerable<string> states_us,
            string billing_first_name, string billing_last_name, string billing_company, string billing_address, string billing_city, long? billing_citySelected,
            string billing_postcode, string billing_phone,
            string shipping_first_name, string shipping_last_name, string shipping_company, string shipping_address, string shipping_city, long? shipping_citySelected, string shipping_postcode, string shipping_phone)
        {
            if (!id.HasValue)
            {
                return SendJsonErrorResponse("Missing ID");
            }

            try
            {
                var contact = repository.GetContact(subdomainid.Value, id.Value);

                if (contact == null)
                {
                    return SendJsonErrorResponse("Missing ID");
                }

                var original = contact.ToModel(sessionid, subdomainid.Value);
                
                // no need to take into account whether an organisation is there because it will always be created
                contact.organisation1.address = address.Trim();
                if (citySelected.HasValue)
                {
                    var mcity = repository.GetCity(citySelected.Value);
                    contact.organisation1.MASTERcity = mcity;
                }
                else if (!string.IsNullOrEmpty(city))
                {
                    contact.organisation1.MASTERcity = repository.AddCity(city);
                }
                if (coPhone != null)
                {
                    contact.organisation1.phone = coPhone;
                }
                if (companyName != null)
                {
                    contact.organisation1.name = companyName;
                }
                if (country != null)
                {
                    contact.organisation1.country = country.ElementAtOrDefault(0);
                    contact.organisation1.state = AddressHandler.GetState(country.ElementAtOrDefault(0), states_us.ElementAtOrDefault(0), states_canadian.ElementAtOrDefault(0), states_other.ElementAtOrDefault(0));
                }
                if (fax != null)
                {
                    contact.organisation1.fax = fax;
                }

                if (email != null)
                {
                    contact.email = email;
                }
                if (firstName != null)
                {
                    contact.firstName = firstName;
                }
                if (gender != null)
                {
                    contact.gender = gender;
                }
                if (lastName != null)
                {
                    contact.lastName = lastName;
                }
                if (phone != null)
                {
                    contact.phoneNumber = phone;
                }
                if (postcode != null)
                {
                    contact.organisation1.postcode = postcode;
                }
                
                // handle addresses
                var addressHandler = new AddressHandler(contact.organisation1, repository);
                addressHandler.SetShippingAndBillingAddresses(billing_first_name,
                                                              billing_last_name,
                                                              billing_company,
                                                              billing_address,
                                                              billing_city,
                                                              billing_citySelected,
                                                              billing_postcode,
                                                              billing_phone,
                                                              country.ElementAtOrDefault(1),
                                                              states_canadian.ElementAtOrDefault(1),
                                                              states_other.ElementAtOrDefault(1),
                                                              states_us.ElementAtOrDefault(1),
                                                              shipping_first_name,
                                                              shipping_last_name,
                                                              shipping_company,
                                                              shipping_address,
                                                              shipping_city,
                                                              shipping_citySelected,
                                                              shipping_postcode,
                                                              shipping_phone,
                                                              country.ElementAtOrDefault(2),
                                                              states_canadian.ElementAtOrDefault(2),
                                                              states_other.ElementAtOrDefault(2),
                                                              states_us.ElementAtOrDefault(2),
                                                              false);
                
                if (title != null)
                {
                    contact.title = title;
                }

                if (!string.IsNullOrEmpty(password))
                {
                    // password specified
                    contact.passwordHash = Crypto.Utility.ComputePasswordHash(email + password);
                }
                else
                {
                    // password removed
                    contact.passwordHash = null;
                }
                
                // list of fields that are allowed to be modified
                if (notes != null)
                {
                    contact.notes = notes;
                }

                // handle permissions
                if(permissions.HasValue)
                {
                    contact.permissions = permissions;
                }

                repository.AddActivity(sessionid.Value,
                        new ActivityMessage(id.Value, sessionid,
                            ActivityMessageType.CONTACT_UPDATED,
                             new HtmlLink(contact.ToEmailName(true), id.Value).ToContactString()), subdomainid.Value);
                
                repository.Save();
#if LUCENE
                // update search index
                var indexer = new LuceneWorker(db, MASTERdomain.ToIdName());
                indexer.AddToIndex(LuceneIndexType.CONTACTS, contact);
#endif
                // get changed and store in database
                var changed = contact.ToModel(sessionid, subdomainid.Value);
                var comparer = new CompareObject();
                var diff = comparer.Compare(original, changed);
                if (diff.Count != 0)
                {
                    repository.AddChangeHistory(sessionid.Value, contact.id, ChangeHistoryType.CONTACT, diff);
                }
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            return Json(id.ToJsonOKData());
        }
    }
}
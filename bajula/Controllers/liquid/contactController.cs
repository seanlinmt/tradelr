using System.Linq;
using System.Web.Mvc;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Email;
using tradelr.Email.Models;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Models.email.store;
using tradelr.Models.liquid.models.Form;
using tradelr.Models.users;

namespace tradelr.Controllers.liquid
{
    public class contactController : baseController
    {
        [HttpPost]
        public ActionResult Index()
        {
            var errors = new FormErrors();
            foreach (string entry in Request.Form.Keys)
            {
                var value = Request.Form[entry];

                if (string.IsNullOrEmpty(value))
                {
                    errors.messages[entry] = "Value is missing";
                    errors.Add(entry);
                }
            }

            var form = new Form();
            if (errors.Count != 0)
            {
                form.posted_successfully = false;
                form.errors = errors;
            }
            else
            {
                form.posted_successfully = true;
            }

            TempData["form"] = form;

            var storeOwner = MASTERdomain.organisation.users.First();
            var formType = Request.Form["form_type"];
            var email = Request.Form["email"];
            if (!string.IsNullOrEmpty(formType) && 
                formType == "customer")
            {
                // add contact to contact list
                // check if email exists
                var usr = repository.GetUsersByEmail(email, subdomainid.Value).SingleOrDefault();
                if (usr == null)
                {
                    // create org
                    var org = new organisation
                                  {
                                      name = email, 
                                      subdomain = subdomainid.Value
                                  };

                    var orgid = repository.AddOrganisation(org);

                    // create user
                    usr = new user
                    {
                        role = (int)UserRole.USER,
                        email = email,
                        viewid = Crypto.Utility.GetRandomString(),
                        permissions = (int)UserPermission.USER,
                        organisation = orgid
                    };
                    repository.AddUser(usr);
                }

                // add to contact group
                if (!string.IsNullOrEmpty(Request.Form["group"]))
                {
                    var group = MASTERdomain.contactGroups.Where(x => x.title == Request.Form["group"]).SingleOrDefault();
                    if (group == null)
                    {
                        group = new contactGroup()
                        {
                            title = Request.Form["group"]
                        };
                        MASTERdomain.contactGroups.Add(group);
                    }

                    if (group.contactGroupMembers.AsQueryable().Where(x => x.userid == usr.id).SingleOrDefault() == null)
                    {
                        group.contactGroupMembers.Add(new contactGroupMember
                                     {
                                         userid = usr.id
                                     });
                    }
                }
                repository.Save();
            }
            else
            {
                //  send notification email to store owner
                var viewdata = new NewMessage
                                   {
                                       name = Request.Form["name"],
                                       email = email, 
                                       message = Request.Form["body"].ToHtmlBreak()
                                   };

                this.SendEmail(EmailViewType.STORE_NEWMESSAGE, viewdata, "New Store Message", storeOwner.GetEmailAddress(), storeOwner.ToFullName(), null);
            }
            
            return Redirect(Request.UrlReferrer.ToString());
        }

    }
}

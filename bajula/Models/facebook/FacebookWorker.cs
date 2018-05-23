using System;
using FacebookToolkit.Rest;
using FacebookToolkit.Schema;
using FacebookToolkit.Session;
using tradelr.Common.Library.Imaging;
using tradelr.Crypto;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Email.Models;
using tradelr.Library;
using tradelr.Library.Constants;
using clearpixels.Logging;
using tradelr.Models.counter;
using tradelr.Models.photos;
using tradelr.Models.users;
using user = tradelr.DBML.user;

namespace tradelr.Models.facebook
{
    public class FacebookWorker
    {
        private readonly ConnectSession session;
        private readonly Api api;
        private readonly long subdomainid;
        private readonly long ownerid;
        private readonly ITradelrRepository repository;

        public FacebookWorker(ITradelrRepository repository, long ownerid, long subdomainid)
        {
            session = new ConnectSession(GeneralConstants.FACEBOOK_API_KEY,
                GeneralConstants.FACEBOOK_API_SECRET);
            api = new Api(session);
            this.subdomainid = subdomainid;
            this.repository = repository;
            this.ownerid = ownerid;
        }

        public void ImportFacebookContacts()
        {
            var query =
                string.Format(
                    "SELECT uid, first_name, last_name, current_location, pic_big, profile_url, proxied_email, sex FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1 = {0})",
                    api.Application.Session.UserId);

            var rows = api.Fql.Query<users_getInfo_response>(query);
            foreach (var row in rows.user)
            {
                try
                {
                    var usr = repository.GetUserByFBID(row.uid.Value.ToString(), subdomainid);
                    if (usr != null)
                    {
                        continue;
                    }

                    var firstname = row.first_name;
                    var lastname = row.last_name;

                    if(string.IsNullOrEmpty(firstname) && string.IsNullOrEmpty(lastname))
                    {
                        continue;
                    }

                    var org = new organisation
                    {
                        subdomain = subdomainid,
                        name = string.Format("{0} {1}", firstname, lastname)
                    };

                    var friend = new user()
                    {
                        role = UserRole.USER.ToInt(),
                        email = "",
                        proxiedEmail = row.proxied_email,
                        firstName = firstname,
                        lastName = lastname,
                        gender = row.sex,
                        organisation = repository.AddOrganisation(org),
                        externalProfilePhoto = row.pic_big,
                        externalProfileUrl = row.profile_url,
                        FBID = row.uid.Value.ToString(),
                        viewid = Crypto.Utility.GetRandomString(),
                        permissions = (int)UserPermission.USER
                    };

                    repository.UpdateCounters(subdomainid, 1, CounterType.CONTACTS_PRIVATE);

                    repository.AddUser(friend);

                    friend.externalProfilePhoto.ReadAndSaveFromUrl(subdomainid, friend.id, friend.id, PhotoType.PROFILE);
                }
                catch (Exception ex)
                {
                    Syslog.Write(ex);
                }
            }

            // mail user
            var owner = repository.GetUserById(ownerid, subdomainid);
            var msg = new message.Message(owner, null, subdomainid);
            msg.SendMessage(null, repository, EmailViewType.GENERIC,
                                         "Your Facebook contacts have been successfully imported", "Import Contacts");

        }
    }
}
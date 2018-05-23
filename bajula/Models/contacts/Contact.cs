using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using tradelr.Common;
using tradelr.Common.Constants;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.photos;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Libraries.Imaging;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.geo;
using clearpixels.Logging;
using tradelr.Models.activity;
using tradelr.Models.address;
using tradelr.Models.google;
using tradelr.Models.jqgrid;
using tradelr.Models.subdomain;
using tradelr.Models.users;
using tradelr.Models.yahoo;


namespace tradelr.Models.contacts
{
    public class Contact : ContactBasic
    {
        // account bits
        public string timezone { get; set; }
        public string currency { get; set; }
        public bool isOwner { get; set; } // this belongs to the current viewer
        public bool isPrivate { get; set; } // this contact is a private editable contact
        public bool isFbConnected { get; set; }
        
        // user bit
        public string gender  { get; set; }
        public string notes { get; set; }
        public string title { get; set; }
        public UserRole role { get; set; }
        public string fbuserid { get; set; }
        public string contactTypeLink { get; set; }
        public Photo profilePhoto { get; set; }
        public UserPermission permissions { get; set; }

        // company bit
        public string orgid { get; set; }
        public string city { get; set; }
        public string coPhone { get; set; }
        public int? country { get; set; }
        
        public string fax { get; set; }  
        public string postcode { get; set; }
        public Photo companyLogo { get; set; }
        public SubdomainFlags domainFlags { get; set; }
        
        public bool canModify { get; set; }

        public GoogleMapData mapData { get; set; }

        // addresses
        public string billingAddress { get; set; }
        public string shippingAddress { get; set; }

    }

    public static class ContactHelper
    {
        private const string SPLITSTRING = ",";
        private static string ToContactLink(this user usr, long viewer_domain)
        {
            var name = !string.IsNullOrEmpty(usr.ToFullName().Trim()) // use fullname if avaiable
                           ? usr.ToFullName()
                           : (!string.IsNullOrEmpty(usr.organisation1.name) // use organisation name if available
                                  ? usr.organisation1.name
                                  : usr.organisation1.MASTERsubdomain.name); // else use subdomain name
            if (viewer_domain == usr.organisation1.subdomain)
            {
                // on the same domain so can just return contact with id
                return string.Format("<a href='/dashboard/contacts/{0}'>{1}</a>", usr.id, name);
            }

            return string.Format("<a href='/dashboard/contacts/{0}/{1}'>{2}</a>", usr.id, usr.organisation1.subdomain, name);
        }

        private static string ToContactLink(this user value)
        {
            if (!string.IsNullOrEmpty(value.email))
            {
                return "<a href='mailto:" + value.email + "'>" + value.email + "</a>";
            }
            
            return "";
        }
        private static string ToContactTypeLink(this user value)
        {
            if ((value.role & (int)UserRole.CREATOR) != 0)
            {
                var subDomainUrl = value.organisation1.MASTERsubdomain.ToHostName().ToDomainUrl(true);
                // changing this will affect javascript of whether to show edit button or not
                return string.Concat("<a target='_blank' href='", subDomainUrl, "' class='link_tradelr'>view network</a>");
            }

            if (!string.IsNullOrEmpty(value.FBID))
            {
                return "<a target='_blank' href='" + value.externalProfileUrl + "' class='link_facebook'>view profile</a>";
            }
            
            return "private";
        }

        public static JqgridTable ToContactsJqGrid(this IEnumerable<user> values, long subdomainid)
        {
            var grid = new JqgridTable();
            foreach (var value in values)
            {
                var entry = new JqgridRow
                                {
                                    id = value.id.ToString(),
                                    cell = new object[]
                                               {
                                                   value.id,
                                                   "",
                                                   value.ToProfilePhoto(Imgsize.THUMB, true).url.ToHtmlImage(),
                                                   string.Concat(value.ToContactLink(subdomainid), SPLITSTRING, value.ToContactTypeLink()),
                                                   value.organisation1.name,
                                                   value.phoneNumber,
                                                   value.ToContactLink(),
                                                   value.organisation1.subdomain == subdomainid?string.Format("<a class='jqedit' href='/dashboard/contacts/edit/{0}'>edit</a>",value.id):""
                                               }
                                };
                grid.rows.Add(entry);
            }
            return grid;
        }

        public static IEnumerable<Contact> ToModel(this IEnumerable<user> values, long sessionid, long subdomainid)
        {
            foreach (var value in values)
            {
                yield return value.ToModel(sessionid, subdomainid);
            }
        }

        // convert from yahoo contacts to our model, email is stored in company name
        public static IEnumerable<ContactBasic> ToModel(this Contacts values)
        {
            var contacts = new List<ContactBasic>();
            foreach (var value in values.contact)
            {
                var contact = new ContactBasic();
                foreach (var field in value.fields)
                {
                    if (field.type == FieldType.name)
                    {
                        contact.firstName = field.value["givenName"];
                        if (!string.IsNullOrEmpty(field.value["familyName"]))
                        {
                            contact.lastName = field.value["familyName"];
                        }
                        else if (!string.IsNullOrEmpty(field.value["middleName"]))
                        {
                            contact.lastName = field.value["middleName"];
                        }
                    }

                    if (field.type == FieldType.email)
                    {
                        contact.companyName = field.value;
                    }

                    if (field.type == FieldType.address)
                    {
                        var sb = new StringBuilder();
                        sb.Append(field.value["street"]);
                        sb.Append(",");
                        sb.Append(field.value["city"]);
                        sb.Append(" ");
                        sb.Append(field.value["postalCode"]);
                        sb.Append(",");
                        sb.Append(field.value["stateOrProvince"]);
                        sb.Append(",");
                        sb.Append(field.value["country"]);
                        contact.address = sb.ToString();
                    }
                }
                
                if (string.IsNullOrEmpty(contact.lastName) || string.IsNullOrEmpty(contact.firstName))
                {
                    Syslog.Write("yahoo contacts schema may have changed");
                }
                contacts.Add(contact);
            }
            return contacts;
        }

        public static IEnumerable<ContactBasic> ToModelBasic(this IEnumerable<user> values)
        {
            foreach (var value in values)
            {
                yield return value.ToModelBasic();
            }
        }

        public static ContactBasic ToModelBasic(this user usr)
        {
            return new ContactBasic()
            {
                profileThumbnail = 
                    usr.profilePhoto.HasValue
                        ? Img.by_size(usr.image.url, Imgsize.THUMB).ToHtmlImage()
                        : GeneralConstants.PHOTO_NO_THUMBNAIL.ToHtmlImage(),
                companyName = string.IsNullOrEmpty(usr.organisation1.name) ? usr.organisation1.MASTERsubdomain.name : usr.organisation1.name,
                countryName = usr.organisation1.country.HasValue?Country.GetCountry(usr.organisation1.country.Value).name:"",
                state = usr.organisation1.state,
                fullName = usr.ToEmailName(true),
                id = usr.id,
                email = usr.email,
                address = usr.organisation1.address,
                firstName = usr.firstName,
                lastName = usr.lastName,
                phone = usr.phoneNumber
            };
        }

        public static Contact ToModel(this user value, long? sessionid, long subdomainid, Imgsize imgsize = Imgsize.MEDIUM)
        {
            Debug.Assert(value.organisation.HasValue);

            var contact = new Contact()
                              {
                                  isOwner = value.id == sessionid,
                                  isPrivate = value.organisation1.subdomain == subdomainid,
                                  id = value.id,
                                  email = value.email,
                                  fbuserid = value.FBID,
                                  firstName = value.firstName,
                                  lastName = value.lastName,
                                  fullName = value.ToEmailName(true),
                                  gender = value.gender,
                                  notes = value.notes,
                                  phone = value.phoneNumber,
                                  title = value.title,
                                  profilePhoto = value.ToProfilePhoto(imgsize),
                                  contactTypeLink = value.ToContactTypeLink(),

                                  orgid = value.organisation.HasValue?value.organisation.Value.ToString():"",
                                  companyName = value.organisation1.name,
                                  address = value.organisation1.address,
                                  city = value.organisation1.city.HasValue ? value.organisation1.MASTERcity.name : "",
                                  coPhone = value.organisation1.phone,
                                  country = value.organisation1.country,
                                  countryName =
                                      value.organisation1.country.HasValue
                                          ? Country.GetCountry(value.organisation1.country.Value).name
                                          : "",
                                  fax = value.organisation1.fax,
                                  postcode = value.organisation1.postcode,
                                  companyLogo =
                                      value.organisation1.logo.HasValue
                                          ? value.organisation1.image.ToModel(imgsize)
                                          : null,
                                  mapData = new GoogleMapData()
                                                {
                                                    latitude =
                                                        value.organisation1.latitude.HasValue
                                                            ? value.organisation1.latitude.Value
                                                            : 0,
                                                    longtitude =
                                                        value.organisation1.longtitude.HasValue
                                                            ? value.organisation1.longtitude.Value
                                                            : 0,
                                                    mapZoom =
                                                        value.organisation1.zoom.HasValue
                                                            ? value.organisation1.zoom.Value
                                                            : 0
                                                },
                                  state = value.organisation1.state,
                                  timezone = value.timezone,
                                  currency = value.organisation1.MASTERsubdomain.currency.ToString(),
                                  domainFlags = (SubdomainFlags) value.organisation1.MASTERsubdomain.flags,
                                  isFbConnected = !string.IsNullOrEmpty(value.FBID),
                                  shippingAddress = value.organisation1.shippingAddressID.HasValue? value.organisation1.address1.ToHtmlString():"",
                                  billingAddress = value.organisation1.billingAddressID.HasValue ? value.organisation1.address2.ToHtmlString():"",
                                  role = (UserRole) value.role
                              };
            if (value.permissions.HasValue)
            {
                contact.permissions = (UserPermission) value.permissions.Value;
            }
            else
            {
                if((contact.role & UserRole.CREATOR) == UserRole.CREATOR)
                {
                    contact.permissions = UserPermission.ADMIN;
                }
                else
                {
                    contact.permissions = UserPermission.USER;
                }
            }
            return contact;
        }

        public static Photo ToProfilePhoto(this user usr, Imgsize size, bool returnNoPhotoThumbnail = false)
        {
            Photo photo = null;
            if (usr.profilePhoto.HasValue)
            {
                photo = usr.image.ToModel(size);
            }
            else if (!string.IsNullOrEmpty(usr.externalProfilePhoto))
            {
                // detect if twitter or facebook
                if (!string.IsNullOrEmpty(usr.FBID))
                {
                    photo = new Photo {bigUrl = usr.externalProfilePhoto, url = usr.externalProfilePhoto};
                }
            }
            else if (returnNoPhotoThumbnail)
            {
                photo = new Photo
                            {
                                bigUrl = GeneralConstants.PHOTO_NO_PROFILE,
                                url = GeneralConstants.PHOTO_NO_PROFILE
                            };
            }
            return photo;
        }
    }
}
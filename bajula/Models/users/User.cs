using System;
using Shipwire.order;
using tradelr.Common.Library.Imaging;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Libraries.Imaging;
using tradelr.Library;
using tradelr.Library.Constants;
using clearpixels.Logging;
using tradelr.Models.subdomain;

namespace tradelr.Models.users
{
    public static class UserHelper
    {
        public static string GetEmailAddress(this user usr)
        {
            if (!string.IsNullOrEmpty(usr.email))
            {
                return usr.email;
            }
            if (string.IsNullOrEmpty(usr.proxiedEmail))
            {
                Syslog.Write("No email address, userid:" + usr.id);
            }
            return usr.proxiedEmail;
        }

        public static DateTime GetDisplayTime(this DateTime utcTime, TimeZoneInfo timezoneinfo)
        {
            utcTime = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTime(utcTime, timezoneinfo);
        }

        public static string GetProfilePhoto(this user usr, Imgsize sz = Imgsize.SMALL)
        {
            if (usr.profilePhoto.HasValue)
            {
                return Img.by_size(usr.image.url, sz).ToHtmlImage();
            }

            return GeneralConstants.PHOTO_NO_PROFILE.ToHtmlImage();
        }

        public static bool HasValidShippingAddress(this user usr)
        {
            var shippingAddress = usr.organisation1.address1;
            if ( shippingAddress == null ||
                string.IsNullOrEmpty(usr.organisation1.address) || 
                !usr.organisation1.city.HasValue ||
                !usr.organisation1.country.HasValue)
            {
                return false;
            }
            return true;
        }

        public static string ToEmailName(this user usr, bool returnEmail)
        {
            // get first name else
            if (!string.IsNullOrEmpty(usr.firstName))
            {
                if (!string.IsNullOrEmpty(usr.lastName))
                {
                    return string.Format("{0} {1}", usr.firstName, usr.lastName);
                }
                return usr.firstName;
            }
            // get organisation else
            if (usr.organisation1.name != usr.email)
            {
                if (string.IsNullOrEmpty(usr.organisation1.name))
                {
                    return "";
                }
                return usr.organisation1.name;
            }

            if (returnEmail)
            {
                return usr.email;
            }

            // empty string
            return "";
        }

        public static string ToProfileUrl(this user usr)
        {
            var viewid = usr.viewid;
            var hostname = usr.organisation1.MASTERsubdomain.ToHostName();

            return hostname.ToDomainUrl(string.Concat("/browse/profile/", viewid));
        }
    }
}

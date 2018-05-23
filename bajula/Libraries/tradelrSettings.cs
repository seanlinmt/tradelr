using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;

namespace tradelr.Libraries
{
    public sealed class tradelrSettings : IConfigurationSectionHandler
    {
        static tradelrSettings() 
        {
            Object obj = ConfigurationManager.GetSection("tradelr");
        }
        public Object Create(Object parent, object configContext, XmlNode section) 
        {
            NameValueCollection settings;

            try 
            {
                NameValueSectionHandler baseHandler = new NameValueSectionHandler();
                settings = (NameValueCollection) baseHandler.Create(parent, configContext, section);
            } 
            catch
            {
                settings = null;
            }

            if (settings != null) 
            {
                language = ReadSetting(settings, "language", "en_US");
                web_prefix = ReadSetting(settings, "web_prefix", "");
                container = ReadSetting(settings, "container", "default");
                ifr_uri = ReadSetting(settings, "ifr_uri", "/gadgets/ifr?");
                gadget_server = ReadSetting(settings, "gadget_server", "http://localhost/");
                st_max_age = ReadSetting(settings, "st_max_age", "3600");
                site_root = ReadSetting(settings, "site_root", "");
                enable_caching = string.Compare(bool.TrueString, ReadSetting(settings, "enable_caching", "true")) == 0;
                enable_facebookConnect = string.Compare(bool.TrueString, ReadSetting(settings, "enable_facebookConnect", "true")) == 0;
                enable_googleFriendConnect = string.Compare(bool.TrueString, ReadSetting(settings, "enable_googleFriendConnect", "true")) == 0;
                fb_api_session_key = ReadSetting(settings, "fb_api_session_key", "");
                fb_api_session_userid = long.Parse(ReadSetting(settings, "fb_api_session_userid", ""));
                gfc_key = ReadSetting(settings, "gfc_key", "");
            }

            return null;
        }

        private static String ReadSetting(NameValueCollection settings, String key, String defaultValue)
        {
            if (settings == null || key == null)
                throw new ArgumentNullException();

            try
            {
                Object setting = settings[key];
                return (setting == null) ? defaultValue : (String)setting;
            }
            catch
            {
                return defaultValue;
            }
        }

        private static string language;
        private static string web_prefix;
        private static string container;
        private static string ifr_uri;
        private static string gadget_server;
        private static string st_max_age;
        private static string site_root;
        private static bool enable_caching;
        private static bool enable_facebookConnect;
        private static bool enable_googleFriendConnect;
        private static string fb_api_session_key;
        private static long fb_api_session_userid;
        private static string gfc_key;

        public static bool Enable_facebookConnect
        {
            get { return enable_facebookConnect; }
            set { enable_facebookConnect = value; }
        }

        public static bool Enable_googleFriendConnect
        {
            get { return enable_googleFriendConnect; }
            set { enable_googleFriendConnect = value; }
        }

        public static String Language 
        {
            get { return language; }
            set { language = value; }
        }
        public static String Web_prefix
        {
            get { return web_prefix; }
            set { web_prefix = value; }
        }
        public static String Container
        {
            get { return container; }
            set { container = value; }
        }
        public static String Ifr_uri
        {
            get { return ifr_uri; }
            set { ifr_uri = value; }
        }
        public static String Gadget_server
        {
            get { return gadget_server; }
            set { gadget_server = value; }
        }
        public static String St_max_age
        {
            get { return st_max_age; }
            set { st_max_age = value; }
        }

        public static String Site_root
        {
            get { return site_root; }
            set { site_root = value; }
        }

        public static bool Enable_caching
        {
            get { return enable_caching; }
            set { enable_caching = value; }
        }

        public static String Fb_api_session_key
        {
            get { return fb_api_session_key; }
            set { fb_api_session_key = value; }
        }

        public static long Fb_api_session_userid
        {
            get { return fb_api_session_userid; }
            set { fb_api_session_userid = value; }
        }

        public static String Gfc_key
        {
            get { return gfc_key; }
            set { gfc_key = value; }
        }
    }
}
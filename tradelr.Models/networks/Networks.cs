using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.networks
{
    public abstract class Networks
    {
        // DO NOT CHANGE THIS. VALUES IN DB WILL BE DIFFERENT
        public const string LOCATIONNAME_ETSY = "Etsy Sync";
        public const string LOCATIONNAME_EBAY = "Ebay Sync";
        public const string LOCATIONNAME_GBASE = "Google Base Sync";
        public static readonly string[] SYNC_NETWORKS = new[]
                                                            {
                                                                LOCATIONNAME_EBAY, 
                                                                LOCATIONNAME_ETSY,
                                                                LOCATIONNAME_GBASE
                                                            };

        public abstract long subdomainid { get; set; }
        public abstract long sessionid { get; set; }
        public abstract void StartSynchronisation(bool? upload);
    }
}
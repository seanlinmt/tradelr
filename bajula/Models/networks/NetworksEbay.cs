using System;
using System.Linq;
using clearpixels.OAuth;
using Ebay;
using tradelr.DBML;

namespace tradelr.Models.networks
{
    public sealed class NetworksEbay : Networks
    {
        public override long subdomainid { get; set; }

        public override long sessionid { get; set; }

        public NetworksEbay(long subdomainid)
        {
            this.subdomainid = subdomainid;
        }

        public override void StartSynchronisation(bool? upload)
        {
            using (var repository = new TradelrRepository())
            {
                // create default shipping profile
                var sd = repository.GetSubDomain(subdomainid);

                if (sd == null)
                {
                    throw new ArgumentException("Cannot find domain for subdomainid " + subdomainid);
                }

                // create network location
                var inventoryLocation = new inventoryLocation
                                            {
                                                name = LOCATIONNAME_EBAY,
                                                subdomain = subdomainid,
                                                lastUpdate = DateTime.UtcNow
                                            };
                repository.AddInventoryLocation(inventoryLocation, subdomainid);

                // add shipping profile if there's none
                foreach (var site in EbayService.SupportedSites)
                {
                    if (sd.ebay_shippingprofiles.Count(x => x.siteid == site.ToString()) != 0)
                    {
                        continue;
                    }

                    var shippingprofile = new ebay_shippingprofile
                    {
                        title = "Default",
                        siteid = site.ToString()
                    };

                    sd.ebay_shippingprofiles.Add(shippingprofile);

                    repository.Save("ebay.StartSynchronisation");
                }

            }
        }

        public void ClearSynchronisation()
        {
            using (var repository = new TradelrRepository())
            {
                repository.DeleteOAuthToken(subdomainid, OAuthTokenType.EBAY);

                // delete inventory location
                repository.DeleteInventoryLocation(Networks.LOCATIONNAME_EBAY, subdomainid);
                repository.Save();
            }
        }
    }
}
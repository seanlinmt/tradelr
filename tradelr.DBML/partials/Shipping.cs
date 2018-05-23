using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DBML;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public long AddShippingRule(shippingRule rule)
        {
            var existing =
                db.shippingRules.SingleOrDefault(x => x.profileid == rule.profileid && x.ruletype == rule.ruletype && x.matchvalue == rule.matchvalue &&
                                                      x.cost == rule.cost && x.secondaryCost == rule.secondaryCost && x.country == rule.country &&
                                                      x.state == rule.state && x.etsy_templateentryid == rule.etsy_templateentryid);

            if (existing == null)
            {
                db.shippingRules.InsertOnSubmit(rule);
                db.SubmitChanges();
                return rule.id;
            }
            else
            {
                return existing.id;
            }
        }

        public void AddShippingProfile(shippingProfile profile)
        {
            // check if entry already exist
            var existing =
                db.shippingProfiles.SingleOrDefault(x => x.title == profile.title && 
                                                         x.subdomainid == profile.subdomainid && 
                                                         x.type == profile.type &&
                                                         x.etsy_templateid == profile.etsy_templateid);
            if (existing == null)
            {
                db.shippingProfiles.InsertOnSubmit(profile);
                db.SubmitChanges();
            }
            else
            {
                profile.id = existing.id;
            }
        }

        public void DeleteShippingRule(long id, long profileid)
        {
            var existing = db.shippingRules.SingleOrDefault(x => x.id == id && x.profileid == profileid);
            if (existing != null)
            {
                db.shippingRules.DeleteOnSubmit(existing);
                db.SubmitChanges();
            }
        }

        public void DeleteShippingRules(long profileid, long subdomainid)
        {
            var rules = GetShippingRules(profileid, subdomainid);
            db.shippingRules.DeleteAllOnSubmit(rules);
            db.SubmitChanges();
        }

        public void DeleteShippingProfile(long id, long subdomainid)
        {
            var existing = db.shippingProfiles.SingleOrDefault(x => x.id == id && x.subdomainid == subdomainid);
            if (existing != null)
            {
                // get default shipping profile
                var defaultprofile = db.shippingProfiles.First(x => x.subdomainid == subdomainid && x.permanent);

                var rules = existing.shippingRules;
                db.shippingRules.DeleteAllOnSubmit(rules);
                var products = db.products.Where(x => x.shippingProfileID == id);
                foreach (var product in products)
                {
                    product.shippingProfileID = defaultprofile.id;
                }
                db.shippingProfiles.DeleteOnSubmit(existing);
                db.SubmitChanges();
            }
        }

        public bool ExistDifferentRuleType(shippingRule rule)
        {
            var rowcount =
                db.shippingRules.Count(x => x.country == rule.country && x.state == rule.state && x.profileid == rule.profileid && x.ruletype != rule.ruletype);
            if (rowcount == 0)
            {
                return false;
            }
            return true;
        }

        public bool ExistShippingRule(shippingRule rule)
        {
            var existing =
                db.shippingRules.SingleOrDefault(x => x.ruletype == rule.ruletype &&
                                                      x.name == rule.name &&
                                                      x.matchvalue == rule.matchvalue &&
                                                      x.country == rule.country &&
                                                      x.state == rule.state &&
                                                      x.profileid == rule.profileid);
            if (existing == null)
            {
                return false;
            }
            return true;
        }

        public ebay_shippingprofile GetEbayShippingProfile(long id)
        {
            return db.ebay_shippingprofiles.Single(x => x.id == id);
        }

        public IQueryable<shippingRule> GetShippingRule(long id)
        {
            return db.shippingRules.Where(x => x.id == id);
        }

        public IQueryable<shippingRule> GetShippingRules(long profileid, long subdomainid)
        {
            return db.shippingRules.Where(x => x.profileid == profileid && x.shippingProfile.subdomainid == subdomainid).OrderBy(x => x.name);
        }

        public shippingProfile GetShippingProfile(long id)
        {
            return db.shippingProfiles.SingleOrDefault(x => x.id == id);
        }

        public IQueryable<shippingProfile> GetShippingProfiles(long subdomainid)
        {
            return db.shippingProfiles.Where(x => x.subdomainid == subdomainid);
        }
    }
}
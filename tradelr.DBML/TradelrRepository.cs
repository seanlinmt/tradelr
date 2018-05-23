using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using clearpixels.Logging;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Models.counter;
using tradelr.Models.history;
using tradelr.Models.subdomain;
using tradelr.Models.users;

namespace tradelr.DBML
{
    public partial class TradelrRepository : ITradelrRepository, IDisposable
    {
        readonly tradelrDataContext db;

        public TradelrRepository()
        {
            db = new tradelrDataContext();
#if DEBUG
            //db.Log = new DebuggerWriter();
#endif
        }

        public TradelrRepository(tradelrDataContext _db)
        {
            db = _db;
        }

        public void Dispose()
        {
            if (db.Connection != null)
            {
                if (db.Connection.State != System.Data.ConnectionState.Closed)
                {
                    db.Connection.Close();
                    db.Connection.Dispose();
                }
            }
                
        }

        public void SetIsolationToNoLock()
        {
            db.Connection.Open();
            db.ExecuteCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
        }

        /// <summary>
        /// all exceptions are caught and not thrown
        /// </summary>
        /// <param name="method"></param>
        public void Save(string method = "")
        {
            try
            {
                db.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException cce)
            {
                var sb = new StringBuilder();
                sb.AppendLine(cce.Message);
                sb.AppendFormat("\tMethod: {0}", method);

                foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                {
                    var metatable = db.Mapping.GetTable(occ.Object.GetType());
                    sb.AppendFormat("\nTable name: {0}\n", metatable.TableName);
                    foreach (MemberChangeConflict mcc in occ.MemberConflicts)
                    {
                        sb.AppendFormat("Member: {0}", mcc.Member);
                        sb.AppendFormat("\tCurrent  value: {0}", mcc.CurrentValue);
                        sb.AppendFormat("\tOriginal value: {0}", mcc.OriginalValue);
                        sb.AppendFormat("\tDatabase value: {0}", mcc.DatabaseValue);
                    }

                    occ.Resolve(RefreshMode.KeepChanges);
                }
                Syslog.Write(sb.ToString());
                db.SubmitChanges(ConflictMode.FailOnFirstConflict);

            }
            catch (Exception ex)
            {
                Syslog.Write(ex);    
                Syslog.Write("Submit Exception: previous method = " + method);
            }
        }


        public void CopyDataMembers(object sourceEntity, object targetEntity)
        {
            //get entity members
            IEnumerable<MetaDataMember> dataMembers = from mem in db.Mapping.GetTable(sourceEntity.GetType()).RowType.DataMembers
                                                      where mem.IsAssociation == false
                                                      select mem;

            //go through the list of members and compare values
            foreach (MetaDataMember mem in dataMembers)
            {
                // don't copy primary key
                if (mem.IsPrimaryKey)
                {
                    continue;
                }
                object originalValue = mem.StorageAccessor.GetBoxedValue(targetEntity);
                object newValue = mem.StorageAccessor.GetBoxedValue(sourceEntity);

                //check if the value has changed
                if (newValue == null && originalValue != null || newValue != null && !newValue.Equals(originalValue))
                {
                    //use reflection to update the target
                    System.Reflection.PropertyInfo propInfo = targetEntity.GetType().GetProperty(mem.Name);
                    propInfo.SetValue(targetEntity, propInfo.GetValue(sourceEntity, null), null);

                    //setboxedvalue bypasses change tracking - otherwise mem.StorageAccessor.SetBoxedValue(ref targetEntity, newValue); could be used instead of reflection
                }
            }
        }

        public user VerifyAccount(int confirm)
        {
            var exist = db.users.SingleOrDefault(x => x.confirmationCode == confirm);
            if (exist == null)
            {
                return null;
            }


            // valid id so give additional 30 days but only for first time
            if (exist.role == UserRole.TENTATIVE.ToInt())
            {
                exist.organisation1.MASTERsubdomain.trialExpiry = DateTime.UtcNow.AddDays(60);
            }

            exist.role = (int)UserRole.ADMIN;
            db.SubmitChanges();
            return exist;
        }

        public bool IsDomainAvailable(string name)
        {
            name = name.ToLower();

            // also check special domain list
            if (GeneralConstants.SUBDOMAIN_RESTRICTED.Contains(name))
            {
                return false;
            }

            var domainCount = db.MASTERsubdomains.Count(x => x.name == name);
            if (domainCount == 0)
            {
                return true;
            }
            return false;
        }

        public MASTERcity AddCity(string city)
        {
            var exist = db.MASTERcities.SingleOrDefault(x => x.name == city);
            if (exist != null)
            {
                return exist;
            }
            var mcity = new MASTERcity();
            mcity.name = city;
            db.MASTERcities.InsertOnSubmit(mcity);
            db.SubmitChanges();
            return mcity;
        }

        public IQueryable<MASTERcity> FindMASTERCity(string s)
        {
            return db.MASTERcities.Where(x => x.name.StartsWith(s));
        }

        public MASTERcity GetCity(long id)
        {
            return db.MASTERcities.Where(x => x.id == id).SingleOrDefault();
        }

        public void AddOrderComment(comment comment)
        {
            db.comments.InsertOnSubmit(comment);
        }

        public void DeleteGoogleBlogsProductPosts(long id)
        {
            var entries = db.googleBlogsProductPosts.Where(x => x.productid == id);
            db.googleBlogsProductPosts.DeleteAllOnSubmit(entries);
            db.SubmitChanges();
        }

        public void AddTwitterSearch(twitterSearch search)
        {
            var exist =
                db.twitterSearches.Where(
                    x =>
                    x.subdomainid == search.subdomainid && x.keyword == search.keyword &&
                    x.parameters == search.parameters).SingleOrDefault();
            if (exist == null)
            {
                db.twitterSearches.InsertOnSubmit(search);
                db.SubmitChanges();
            }
        }
        
        public void UpdatePasswordHash(long userid, string hash)
        {
            var exist = db.users.Where(x => x.id == userid).SingleOrDefault();
            if (exist != null)
            {
                exist.passwordHash = hash;
            }
            db.SubmitChanges();
        }

        public void UpdateOpenID(string openid, long ownerid)
        {
            var result = db.users.Where(x => x.id == ownerid).SingleOrDefault();
            if (result == null)
            {
                throw new Exception("Attempt to update openid:" + openid + " of non-existant user:" + ownerid);
            }
            result.openID = openid;
            db.SubmitChanges();
        }

        public void UpdateCounters(long subdomainid, long count, CounterType type)
        {
            var dom = GetSubDomain(subdomainid);
            switch (type)
            {
                case CounterType.PRODUCTS_MINE:
                    dom.total_products_mine += count;
                    break;
                case CounterType.ORDERS_SENT:
                    dom.total_orders_sent += count;
                    break;
                case CounterType.ORDERS_RECEIVED:
                    dom.total_orders_received += count;
                    break;
                case CounterType.INVOICES_RECEIVED:
                    dom.total_invoices_received += count;
                    break;
                case CounterType.INVOICES_SENT:
                    dom.total_invoices_sent += count;
                    break;
                case CounterType.CONTACTS_PUBLIC:
                    dom.total_contacts_public += count;
                    break;
                case CounterType.CONTACTS_PRIVATE:
                    dom.total_contacts_private += count;
                    break;
                case CounterType.CONTACTS_STAFF:
                    dom.total_contacts_staff += count;
                    break;
                case CounterType.OUTOFSTOCK:
                    dom.total_outofstock += count;
                    break;
            }
        }

        // use to check if contact email is already in use
        public bool IsEmailInUse(string email, long subdomain)
        {
            var count = db.users.Count(x => x.organisation1.subdomain == subdomain && x.email == email);
            if (count != 0)
            {
                return true;
            }
            return false;
        }

        public IQueryable<changeHistoryItem> GetChangeHistory(ChangeHistoryType type, long contextid, long subdomain)
        {
            return db.changeHistoryItems.Where(x => x.changeHistory.historyType == type.ToString() && x.changeHistory.contextID == contextid);
        }

        public void AddChangeHistory(long changer, long contextID, ChangeHistoryType changeType, Dictionary<string, Pair<object, object>> difference)
        {
            string[] ignoredFields = new[]
                                         {
                                             "amountDue",
                                             "orderTotal",
                                             "subTotalString",
                                             "subTotal",
                                             "orderItems"
                                         };
        
            var change =
                db.changeHistories.SingleOrDefault(x => x.contextID == contextID && x.historyType == changeType.ToString());
            if (change == null)
            {
                change = new changeHistory
                             {
                                 historyType = changeType.ToString(), 
                                 contextID = contextID
                             };
                db.changeHistories.InsertOnSubmit(change);
                db.SubmitChanges();
            }

            var changelist = new List<changeHistoryItem>();
            if (difference == null)
            {
                // to record creation date
                var changeItem = new changeHistoryItem
                                     {
                                         changer = changer,
                                         changeDate = DateTime.UtcNow,
                                         changedField = changeType.ToString().ToLowerInvariant(),
                                         changeID = change.id
                                     };
                changelist.Add(changeItem);
            }
            else
            {
                foreach (var row in difference)
                {
                    // ignore these fields
                    if (ignoredFields.Contains(row.Key))
                    {
                        continue;
                    }
                    var fieldname = row.Key.ToFieldDisplay();
                    var value = row.Value;
                    var changeItem = new changeHistoryItem
                                         {
                                             changer = changer,
                                             changeDate = DateTime.UtcNow,
                                             changedField = fieldname,
                                             changeID = change.id,
                                             oldValue = value.First == null ? "":value.First.ToString(),
                                             newValue = value.Second == null ? "":value.Second.ToString()
                                         };
                    changelist.Add(changeItem);
                }
            }
            
            db.changeHistoryItems.InsertAllOnSubmit(changelist);
            db.SubmitChanges();
        }

        public void AddSupportMessage(adminSupportMessage message)
        {
            db.adminSupportMessages.InsertOnSubmit(message);
            db.SubmitChanges();
        }

       public void ToggleOfflineAccess(long subdomainid)
        {
            var sd = db.MASTERsubdomains.SingleOrDefault(x => x.id == subdomainid);
            sd.flags ^= ((int)SubdomainFlags.OFFLINE_ENABLED);
            db.SubmitChanges();
        }

        public void SetMetric(long owner, bool ismetric)
        {
            var usr = db.users.Single(x => x.id == owner);
            if (ismetric)
            {
                usr.settings |= ((int)UserSettings.METRIC_VIEW);
            }
            else
            {
                usr.settings &= ((int)~UserSettings.METRIC_VIEW);
            }
            db.SubmitChanges();
        }

        public IQueryable<MASTERsubdomain> GetSubDomains()
        {
            return db.MASTERsubdomains;
        }

        public void AddMasterSubdomain(MASTERsubdomain subdomain)
        {
            db.MASTERsubdomains.InsertOnSubmit(subdomain);
            db.SubmitChanges();
        }

        public IQueryable<twitterSearch> GetTwitterSearches(long subdomainid)
        {
            return db.twitterSearches.Where(x => x.subdomainid == subdomainid);
        }
    }
}

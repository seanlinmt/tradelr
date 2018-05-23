using System;
using System.Linq;
using tradelr.DBML.Lucene;
using tradelr.DBML;
using clearpixels.Logging;
using tradelr.DBML.Lucene.IndexingQueue;

namespace tradelr.Libraries.scheduler
{
    public static partial class ScheduledTask
    {
        public static void PollEbayOrders()
        {
            var myLock = new object();
            lock (myLock)
            {
                try
                {
                    using (var repository = new TradelrRepository())
                    {
                        EbayPollForOrders(repository);
                    }
                }
                catch (Exception ex)
                {
                    Syslog.Write(ex);
                }
            }
        }

        public static void PollIndexingQueue()
        {
            var myLock = new object();
            lock (myLock)
            {
                using (var db = new tradelrDataContext())
                {
                    try
                    {
                        var queueItems = db.indexingQueues.Take(10);
                        foreach (var item in queueItems)
                        {
                            var action = item.ToModel();

                            if (LuceneUtil.Instance.ModifyIndex(action))
                            {
                                db.indexingQueues.DeleteOnSubmit(item);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Syslog.Write(ex);
                    }
                    finally
                    {
                        db.SubmitChanges();
                    }
                        
                }
            }
        }

        public static void PollPaypalPaymentDetails()
        {
            var myLock = new object();
            lock (myLock)
            {
                try
                {
                    using (var repository = new TradelrRepository())
                    {
                        bool haveChanges = PaypalHandleBuyerPaidAndWeNeedToVerify(repository);
                        if (haveChanges)
                        {
                            repository.Save("PollPaypalPaymentDetails");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Syslog.Write(ex);
                }
                
            }
        }

        public static void PollForShipwireShippedStatus()
        {
            var myLock = new object();
            lock (myLock)
            {
                try
                {
                    using (var repository = new TradelrRepository())
                    {

                        bool haveChanges = ShipwirePollForShippedStatus(repository);
                        if (haveChanges)
                        {
                            repository.Save("PollForShipwireShippedStatus");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Syslog.Write(ex);
                }
                
            }
        }

        public static void SendEmails()
        {
            var myLock = new object();
            lock (myLock)
            {
                try
                {
                    using (var repository = new TradelrRepository())
                    {
                        bool haveChanges = false;
                        var mails = repository.GetMails();
                        foreach (var mail in mails.ToList())
                        {
                            Email.Email.SendMail(mail, true, false);
                            repository.DeleteMail(mail);
                            haveChanges = true;
                        }
                        if (haveChanges)
                        {
                            repository.Save("SendEmails");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Syslog.Write(ex);
                }
                
            }
        }
    }
}

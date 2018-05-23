using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Caching;
using clearpixels.Logging;
using tradelr.Library;
using tradelr.Library.Constants;

namespace tradelr.Libraries.scheduler
{
    public sealed class CacheScheduler
    {
        private enum TaskType
        {
            Gbase,
            Paypal,
            Ebay,
            Email,
            SearchIndexer,
            Shipwire,
            Shipwire_Inventory
        }
        
        public readonly static CacheScheduler Instance = new CacheScheduler();
        private readonly Dictionary<TaskType,Thread> runningThreads = new Dictionary<TaskType, Thread>();
        private CacheScheduler()
        {
            
        }

        public void RegisterCacheEntry()
        {
            Debug.WriteLine("RegisterCacheEntry .....");
            // Prevent duplicate key addition
            if (HttpRuntime.Cache[CacheTimerType.Seconds10.ToString()] == null)
            {
                HttpRuntime.Cache.Add(CacheTimerType.Seconds10.ToString(), "10s", null, DateTime.Now.AddSeconds(10),
                    Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable,
                    CacheItemRemovedCallback);
            }

            if (HttpRuntime.Cache[CacheTimerType.Minute1.ToString()] == null)
            {
                HttpRuntime.Cache.Add(CacheTimerType.Minute1.ToString(), 1, null, DateTime.Now.AddSeconds(60),
                    Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable,
                    CacheItemRemovedCallback);
            }

            if (HttpRuntime.Cache[CacheTimerType.Minute5.ToString()] == null)
            {
                HttpRuntime.Cache.Add(CacheTimerType.Minute5.ToString(), 5, null, DateTime.Now.AddMinutes(5),
                        Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable,
                        CacheItemRemovedCallback);
            }

            if (HttpRuntime.Cache[CacheTimerType.Minute10.ToString()] == null)
            {
                HttpRuntime.Cache.Add(CacheTimerType.Minute10.ToString(), 10, null, DateTime.Now.AddMinutes(10),
                        Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable,
                        CacheItemRemovedCallback);
            }

            if (HttpRuntime.Cache[CacheTimerType.Minute60.ToString()] == null)
            {
                HttpRuntime.Cache.Add(CacheTimerType.Minute60.ToString(), 60, null, DateTime.Now.AddMinutes(60),
                         Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable,
                        CacheItemRemovedCallback);
            }
        }

        private void CacheItemRemovedCallback(
            string key,
            object value,
            CacheItemRemovedReason reason
            )
        {
            //if (reason != CacheItemRemovedReason.Expired)
            //{
            //    eJException newex = new eJException();
            //    newex.logException("cacheExpired: " + key + " " + reason.ToString(), null);
            //}
            Debug.WriteLine("Cache Expired: " + key);
            switch (key.ToEnum<CacheTimerType>())
            {
#if DEBUG
                case CacheTimerType.Seconds10:
                    {
                        var thread = new Thread(ScheduledTask.PollIndexingQueue) { Name = TaskType.SearchIndexer.ToString() };
                        if (!runningThreads.ContainsKey(TaskType.SearchIndexer))
                        {
                            runningThreads.Add(TaskType.SearchIndexer, thread);
                            thread.Start();
                        }
                        else
                        {
                            if (!runningThreads[TaskType.SearchIndexer].IsAlive)
                            {
                                runningThreads[TaskType.SearchIndexer] = thread;
                                thread.Start();
                            }
                        }
                    }
                    break;

                case CacheTimerType.Minute1:
                    {
                        var thread = new Thread(ScheduledTask.PollPaypalPaymentDetails) { Name = TaskType.Paypal.ToString() };
                        if (!runningThreads.ContainsKey(TaskType.Paypal))
                        {
                            runningThreads.Add(TaskType.Paypal, thread);
                            thread.Start();
                        }
                        else
                        {
                            if (!runningThreads[TaskType.Paypal].IsAlive)
                            {
                                runningThreads[TaskType.Paypal] = thread;
                                thread.Start();
                            }
                        }

                        thread = new Thread(ScheduledTask.SendEmails) { Name = TaskType.Email.ToString() };
                        if (!runningThreads.ContainsKey(TaskType.Email))
                        {
                            runningThreads.Add(TaskType.Email, thread);
                            thread.Start();
                        }
                        else
                        {
                            if (!runningThreads[TaskType.Email].IsAlive)
                            {
                                runningThreads[TaskType.Email] = thread;
                                thread.Start();
                            }
                        }
                    }
                    /*
                    thread = new Thread(ScheduledTask.ShipwirePollForInventoryUpdates) { Name = TaskType.Shipwire_Inventory.ToString() };
                    if (!runningThreads.ContainsKey(TaskType.Shipwire_Inventory))
                    {
                        runningThreads.Add(TaskType.Shipwire_Inventory, thread);
                        thread.Start();
                    }
                    else
                    {
                        if (!runningThreads[TaskType.Shipwire_Inventory].IsAlive)
                        {
                            runningThreads[TaskType.Shipwire_Inventory] = thread;
                            thread.Start();
                        }
                    }

                    thread = new Thread(ScheduledTask.PollForShipwireShippedStatus) { Name = TaskType.Shipwire.ToString() };
                    if (!runningThreads.ContainsKey(TaskType.Shipwire))
                    {
                        runningThreads.Add(TaskType.Shipwire, thread);
                        thread.Start();
                    }
                    else
                    {
                        if (!runningThreads[TaskType.Shipwire].IsAlive)
                        {
                            runningThreads[TaskType.Shipwire] = thread;
                            thread.Start();
                        }
                    }

                    thread = new Thread(ScheduledTask.PollGoogleBase) { Name = TaskType.Gbase.ToString() };
                    if (!runningThreads.ContainsKey(TaskType.Gbase))
                    {
                        runningThreads.Add(TaskType.Gbase, thread);
                        thread.Start();
                    }
                    else
                    {
                        if (!runningThreads[TaskType.Gbase].IsAlive)
                        {
                            runningThreads[TaskType.Gbase] = thread;
                            thread.Start();
                        }
                    }
                   
                     * */
                    break;
                case CacheTimerType.Minute5:
                    /*
                    {
                        var thread = new Thread(ScheduledTask.PollEbayOrders) { Name = TaskType.Ebay.ToString() };
                        if (!runningThreads.ContainsKey(TaskType.Ebay))
                        {
                            runningThreads.Add(TaskType.Ebay, thread);
                            thread.Start();
                        }
                        else
                        {
                            if (!runningThreads[TaskType.Ebay].IsAlive)
                            {
                                runningThreads[TaskType.Ebay] = thread;
                                thread.Start();
                            }
                        }
                    }
                     * */
                    break;
#else
                    case CacheTimerType.Seconds10:
                    {
                        var thread = new Thread(ScheduledTask.PollIndexingQueue) { Name = TaskType.SearchIndexer.ToString() };
                        if (!runningThreads.ContainsKey(TaskType.SearchIndexer))
                        {
                            runningThreads.Add(TaskType.SearchIndexer, thread);
                            thread.Start();
                        }
                        else
                        {
                            if (!runningThreads[TaskType.SearchIndexer].IsAlive)
                            {
                                runningThreads[TaskType.SearchIndexer] = thread;
                                thread.Start();
                            }
                        }
                    }
                    break;

                case CacheTimerType.Minute1:
                    {
                        var thread = new Thread(ScheduledTask.SendEmails) { Name = TaskType.Email.ToString() };
                        if (!runningThreads.ContainsKey(TaskType.Email))
                        {
                            runningThreads.Add(TaskType.Email, thread);
                            thread.Start();
                        }
                        else
                        {
                            if (!runningThreads[TaskType.Email].IsAlive)
                            {
                                runningThreads[TaskType.Email] = thread;
                                thread.Start();
                            }
                        }
                    }
                    break;
                case CacheTimerType.Minute5:
                    {
                        // paypal
                        var thread = new Thread(ScheduledTask.PollPaypalPaymentDetails) { Name = TaskType.Paypal.ToString() };
                        if (!runningThreads.ContainsKey(TaskType.Paypal))
                        {
                            runningThreads.Add(TaskType.Paypal, thread);
                            thread.Start();
                        }
                        else
                        {
                            if (!runningThreads[TaskType.Paypal].IsAlive)
                            {
                                runningThreads[TaskType.Paypal] = thread;
                                thread.Start();
                            }
                        }

                        
                    }
                    break;
                case CacheTimerType.Minute10:
                    {
                        // shipwire inventory updates
                        var thread = new Thread(ScheduledTask.ShipwirePollForInventoryUpdates) { Name = TaskType.Shipwire_Inventory.ToString() };
                        if (!runningThreads.ContainsKey(TaskType.Shipwire_Inventory))
                        {
                            runningThreads.Add(TaskType.Shipwire_Inventory, thread);
                            thread.Start();
                        }
                        else
                        {
                            if (!runningThreads[TaskType.Shipwire_Inventory].IsAlive)
                            {
                                runningThreads[TaskType.Shipwire_Inventory] = thread;
                                thread.Start();
                            }
                        }
                    }
                    break;
                case CacheTimerType.Minute60:
                    {
                        var thread = new Thread(ScheduledTask.PollForShipwireShippedStatus) { Name = TaskType.Shipwire.ToString() };
                        if (!runningThreads.ContainsKey(TaskType.Shipwire))
                        {
                            runningThreads.Add(TaskType.Shipwire, thread);
                            thread.Start();
                        }
                        else
                        {
                            if (!runningThreads[TaskType.Shipwire].IsAlive)
                            {
                                runningThreads[TaskType.Shipwire] = thread;
                                thread.Start();
                            }
                        }

                        thread = new Thread(ScheduledTask.PollGoogleBase) { Name = TaskType.Gbase.ToString() };
                        if (!runningThreads.ContainsKey(TaskType.Gbase))
                        {
                            runningThreads.Add(TaskType.Gbase, thread);
                            thread.Start();
                        }
                        else
                        {
                            if (!runningThreads[TaskType.Gbase].IsAlive)
                            {
                                runningThreads[TaskType.Gbase] = thread;
                                thread.Start();
                            }
                        }

                        thread = new Thread(ScheduledTask.PollEbayOrders) { Name = TaskType.Ebay.ToString() };
                        if (!runningThreads.ContainsKey(TaskType.Ebay))
                        {
                            runningThreads.Add(TaskType.Ebay, thread);
                            thread.Start();
                        }
                        else
                        {
                            if (!runningThreads[TaskType.Ebay].IsAlive)
                            {
                                runningThreads[TaskType.Ebay] = thread;
                                thread.Start();
                            }
                        }
                    }
                    break;
#endif
                default:
#if !DEBUG
                    Syslog.Write("CacheScheduler ERROR: " + key);
#endif
                    break;
            }
            HitPage();
        }

        private void HitPage()
        {
            using (var client = new WebClient())
            {
                var data = client.DownloadData(GeneralConstants.HTTP_CACHEURL);    
            }
        }
    }
}

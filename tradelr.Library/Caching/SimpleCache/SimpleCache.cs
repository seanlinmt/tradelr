using System;
using System.Web;
using System.Web.Caching;

namespace tradelr.Library.Caching.SimpleCache
{

    public static class SimpleCache
    {
        private static string CreateCacheKey(string key, SimpleCacheType type)
        {
            return string.Concat(type, "-", key);
        }

        public static object Get(string key, SimpleCacheType type)
        {
            var cachekey = CreateCacheKey(key, type);
            return HttpRuntime.Cache.Get(cachekey);
        }

        private static void Add(string key, object value, SimpleCacheType type, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            var cachekey = CreateCacheKey(key, type);
            HttpRuntime.Cache.Insert(cachekey,value,null, absoluteExpiration, slidingExpiration);
        }

        public static void Add(string key, object value, SimpleCacheType type, DateTime absoluteExpiration)
        {
            Add(key,value, type, absoluteExpiration, Cache.NoSlidingExpiration);
        }

        public static void Add(string key, object value, SimpleCacheType type, TimeSpan slidingExpiration)
        {
            Add(key, value, type, Cache.NoAbsoluteExpiration, slidingExpiration);
        }

        public static void Add(string key, object value, SimpleCacheType type)
        {
            Add(key, value, type, Cache.NoAbsoluteExpiration, new TimeSpan(0,10,10));
        }

        public static bool Remove(string key, SimpleCacheType type)
        {
            var cachekey = CreateCacheKey(key, type);
            var removed = HttpRuntime.Cache.Remove(cachekey);
            if (removed == null)
            {
                return false;
            }
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Web;

namespace tradelr.Library.Caching
{
    public sealed class CacheHelper 
    {
        /// <summary>
        /// dependency map key is the dependency type and the hashset is keys for all cached items
        /// </summary>
        private Dictionary<string, HashSet<string>> dep_map;
        public static readonly CacheHelper Instance = new CacheHelper();
        private CacheHelper()
        {
            dep_map = new Dictionary<string, HashSet<string>>();
        }

        private string CreateCacheItemKey(CacheItemType type, string id)
        {
            return type + "_res:" + id;
        }

        private string CreateDependencyKey(DependencyType type, string id)
        {
            return type + "_deps:" + id;
        }

        public bool TryGetCache(CacheItemType type, string id, out object value)
        {
            string key = CreateCacheItemKey(type, id);
            
            // anything in cache
            object data = HttpRuntime.Cache.Get(key);

            // yes, return cached entry
            if (data == null)
            {
                value = null;
                return false;
            }
            value = data;
            return true;
        }

        public string Insert(CacheItemType type, string id, object value)
        {
            if (value == null)
            {
                return null;
            }

            string key = CreateCacheItemKey(type, id);
            // add with no sliding or absolute expiration
            // cache dependencies are manually handled
            HttpRuntime.Cache.Insert(key,value);

            return key;
        }

        public void add_dependency(DependencyType depType, string depid, CacheItemType itemType, string itemid)
        {
            // if null then just add independent cache entry
            string cacheKey = CreateCacheItemKey(itemType, itemid);
            string dependencyKey = CreateDependencyKey(depType, depid);
            if (!dep_map.ContainsKey(dependencyKey))
            {
                dep_map.Add(dependencyKey, new HashSet<string>());
            }
            dep_map[dependencyKey].Add(cacheKey);
        }

        public void invalidate_dependency(DependencyType type, string id)
        {
            string dependencyKey = CreateDependencyKey(type, id);
            if (!dep_map.ContainsKey(dependencyKey))
            {
                return;
            }
            // remove affected cached entries
            HashSet<string> keys = dep_map[dependencyKey];
            foreach (var key in keys)
            {
                HttpRuntime.Cache.Remove(key);
            }
            // remove dependency
            dep_map.Remove(dependencyKey);
        }

        public static string GetKey(string id, long? sessionid)
        {
            return string.Format("{0}:{1}", id, sessionid);
        }

        public object Remove(CacheItemType type, string id)
        {
            string key = CreateCacheItemKey(type, id);
            return HttpRuntime.Cache.Remove(key);
        }
    }
}
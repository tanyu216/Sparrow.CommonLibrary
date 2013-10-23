using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Sparrow.CommonLibrary.Cache
{
    public class LocalCache : ICache
    {
        private readonly string regionName;

        public LocalCache(string regionName)
        {
            this.regionName = regionName;
        }

        private string BuildKey(string key)
        {
            return string.Concat(regionName, ">", key);
        }

        public bool Contains(string key)
        {
            return HttpRuntime.Cache.Get(BuildKey(key)) != null;
        }

        public object Get(string key)
        {
            return HttpRuntime.Cache.Get(BuildKey(key));
        }

        public T Get<T>(string key)
        {
            return (T)HttpRuntime.Cache.Get(BuildKey(key));
        }

        public IDictionary<string, object> Get(string[] keys)
        {
            var dic = new Dictionary<string, object>();
            foreach (var key in keys)
                dic.Add(key, Get<object>(key));
            //
            return dic;
        }

        public IDictionary<string, object> Get(IEnumerable<string> keys)
        {
            return Get(keys.ToArray());
        }

        public void Set<T>(string key, T value)
        {
            HttpRuntime.Cache.Insert(BuildKey(key), value);
        }

        public void Set<T>(string key, T value, DateTime expireAt)
        {
            HttpRuntime.Cache.Insert(BuildKey(key), value, null, expireAt, TimeSpan.Zero);
        }
    }
}

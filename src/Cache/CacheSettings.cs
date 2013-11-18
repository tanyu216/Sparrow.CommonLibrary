using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Cache.Configuration;

namespace Sparrow.CommonLibrary.Cache
{
    /// <summary>
    /// 缓存配置
    /// </summary>
    public static class CacheSettings
    {
        public static string DefaultRegionName { get; private set; }

        private static readonly ConcurrentDictionary<string, KeyValuePair<Type, string>> _caches;

        static CacheSettings()
        {
            _caches = new ConcurrentDictionary<string, KeyValuePair<Type, string>>();
            LoadConfig();
            if (string.IsNullOrEmpty(DefaultRegionName))
                DefaultRegionName = "sprregion_default";
        }

        static void LoadConfig()
        {
            var configuration = CacheConfigurationSection.GetSection();
            if (configuration == null)
                return;

            foreach (CacheElement element in configuration.Caches)
            {
                SetCache(element.RegionName, element.Type, element.ConnectionString, element.Default);
            }
        }

        public static void SetCache(string regionName, Type type, string connectionString, bool isDefault)
        {
            if (isDefault && string.IsNullOrEmpty(DefaultRegionName))
            {
                DefaultRegionName = regionName;
            }
            _caches.AddOrUpdate(regionName, new KeyValuePair<Type, string>(type, connectionString), (key, val) => new KeyValuePair<Type, string>(type, connectionString));
        }

        public static void GetCache(string regionName, out Type type, out string connectionString)
        {
            KeyValuePair<Type, string> item;
            if (!string.IsNullOrEmpty(regionName) && _caches.TryGetValue(regionName, out item))
            {
                type = item.Key;
                connectionString = item.Value;
                return;
            }
            type = typeof(LocalCache);
            connectionString = null;
        }
    }
}

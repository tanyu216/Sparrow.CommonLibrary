using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Cache.Configuration;

namespace Sparrow.CommonLibrary.Cache.Configuration
{
    /// <summary>
    /// 缓存配置
    /// </summary>
    public class CacheSettings
    {
        private static readonly object syncObj = new object();
        private static CacheSettings _settings;

        public static CacheSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    lock (syncObj)
                    {
                        if (_settings == null)
                        {
                            _settings = new CacheSettings();
                            _settings.LoadConfig();
                        }
                    }
                }
                return _settings;
            }
        }

        public string DefaultRegionName { get; private set; }

        private readonly ConcurrentDictionary<string, KeyValuePair<Type, string>> _caches;

        public CacheSettings()
        {
            _caches = new ConcurrentDictionary<string, KeyValuePair<Type, string>>();
        }

        public CacheSettings(CacheConfigurationSection configuration)
            : this()
        {
            LoadConfig(configuration);
        }

        protected virtual void LoadConfig()
        {
            var configuration = CacheConfigurationSection.GetSection();

            if (configuration == null)
            {
                configuration = new CacheConfigurationSection();
                configuration.Caches = new CacheElementCollection();
                configuration.Caches.Add(new CacheElement() { Default = true, RegionName = "spr_region_default", Type = typeof(LocalCache) });
            }

            LoadConfig(configuration);
        }

        public virtual void LoadConfig(CacheConfigurationSection configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            foreach (CacheElement element in configuration.Caches)
            {
                SetCache(element.RegionName, element.Type, element.ConnectionString, element.Default);
            }
        }

        public virtual void SetCache(string regionName, Type type, string connectionString, bool isDefault)
        {
            if (isDefault && string.IsNullOrEmpty(DefaultRegionName))
            {
                DefaultRegionName = regionName;
            }
            _caches.AddOrUpdate(regionName, new KeyValuePair<Type, string>(type, connectionString), (key, val) => new KeyValuePair<Type, string>(type, connectionString));
        }

        public virtual void GetCache(string regionName, out Type type, out string connectionString)
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

        public static void ResetSettings(CacheSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            lock (syncObj)
            {
                _settings = settings;
            }
        }
    }
}

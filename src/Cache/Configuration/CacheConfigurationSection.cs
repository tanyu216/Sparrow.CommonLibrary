using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Logging;

namespace Sparrow.CommonLibrary.Cache.Configuration
{
    public class CacheConfigurationSection : ConfigurationSection
    {
        private readonly ConfigurationPropertyCollection _properties;
        private readonly ConfigurationProperty _clientProperty1;

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return _properties;
            }
        }

        public CacheElementCollection Caches
        {
            get { return (CacheElementCollection)this[_clientProperty1]; }
        }

        public CacheConfigurationSection()
        {
            _properties = new ConfigurationPropertyCollection();
            _clientProperty1 = new ConfigurationProperty("caches", typeof(CacheElementCollection), null, ConfigurationPropertyOptions.None);
            _properties.Add(_clientProperty1);
        }

        public static string DefaultSectionName
        {
            get
            {
                if (string.IsNullOrEmpty(DefaultSettings.ConfigurationName))
                    return "cache";
                return string.Concat(DefaultSettings.ConfigurationName, "/", "cache");
            }
        }

        private static bool nonConfiguration = false;

        public static CacheConfigurationSection GetSection()
        {
            if (nonConfiguration)
            {
                return null;
            }
            try
            {
                var configuration = ((CacheConfigurationSection)ConfigurationManager.GetSection(DefaultSectionName));
                nonConfiguration = false;
                return configuration;
            }
            catch (Exception ex)
            {
                nonConfiguration = true;
                Logging.Log.GetLog(LoggingSettings.SparrowCategory).Warning("CacheConfiguration加载失败。", ex);
            }
            return null;
        }

    }
}

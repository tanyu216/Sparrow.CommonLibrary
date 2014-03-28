using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

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
            set { this[_clientProperty1] = value; }
        }

        public CacheConfigurationSection()
        {
            _properties = new ConfigurationPropertyCollection();
            _clientProperty1 = new ConfigurationProperty("caches", typeof(CacheElementCollection), null, ConfigurationPropertyOptions.None);
            _properties.Add(_clientProperty1);
        }

        public static string SectionName
        {
            get
            {
                return "sparrow.CommonLibrary/cache";
            }
        }

        public static CacheConfigurationSection GetSection()
        {
            var configuration = ((CacheConfigurationSection)ConfigurationManager.GetSection(SectionName));
            return configuration;
        }

    }
}

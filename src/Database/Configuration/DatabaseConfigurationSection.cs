using Sparrow.CommonLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Database.Configuration
{
    public class DatabaseConfigurationSection : ConfigurationSection
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

        public ProviderElementCollection Providers
        {
            get { return (ProviderElementCollection)this[_clientProperty1]; }
            set { this[_clientProperty1] = value; }
        }

        public DatabaseConfigurationSection()
        {
            _properties = new ConfigurationPropertyCollection();
            _clientProperty1 = new ConfigurationProperty("providers", typeof(ProviderElementCollection), null, ConfigurationPropertyOptions.None);
            _properties.Add(_clientProperty1);
        }

        public static string DefaultSectionName
        {
            get
            {
                if (string.IsNullOrEmpty(DefaultSettings.ConfigurationName))
                    return "database";
                return string.Concat(DefaultSettings.ConfigurationName, "/", "database");
            }
        }

        private static bool nonConfiguration = false;

        public static DatabaseConfigurationSection GetSection()
        {
            if (nonConfiguration)
            {
                return null;
            }
            try
            {
                var configuration = (DatabaseConfigurationSection)System.Configuration.ConfigurationManager.GetSection(DefaultSectionName);
                nonConfiguration = false;
                return configuration;
            }
            catch (Exception ex)
            {
                nonConfiguration = true;
                Log.GetLog(LoggingSettings.SparrowCategory).Warning("DataConfiguration加载失败。", ex);
            }
            return null;
        }

    }
}

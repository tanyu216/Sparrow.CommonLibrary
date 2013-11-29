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

        public static string SectionName
        {
            get
            {
                return "sparrow.CommonLibrary/database";
            }
        }

        public static DatabaseConfigurationSection GetSection()
        {
            var configuration = (DatabaseConfigurationSection)System.Configuration.ConfigurationManager.GetSection(SectionName);
            return configuration;
        }

    }
}

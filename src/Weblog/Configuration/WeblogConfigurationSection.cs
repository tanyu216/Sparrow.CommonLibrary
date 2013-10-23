using Sparrow.CommonLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Weblog.Configuration
{
    public class WeblogConfigurationSection : ConfigurationSection
    {
        private readonly ConfigurationPropertyCollection _properties;
        private readonly ConfigurationProperty _clientProperty;
        private readonly ConfigurationProperty _clientProperty1;
        private readonly ConfigurationProperty _clientProperty2;
        private readonly ConfigurationProperty _clientProperty3;

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return _properties;
            }
        }

        public string Version { get { return (string)this[_clientProperty]; } }

        public IgnoreElementCollection Ignores { get { return (IgnoreElementCollection)this[_clientProperty1]; } }

        public WriterElement Writer { get { return (WriterElement)this[_clientProperty2]; } }

        public CollectElement Collect { get { return (CollectElement)this[_clientProperty3]; } }

        public WeblogConfigurationSection()
        {
            _properties = new ConfigurationPropertyCollection();
            _clientProperty = new ConfigurationProperty("version", typeof(string), "1.0");
            _clientProperty1 = new ConfigurationProperty("ignores", typeof(IgnoreElementCollection), null, ConfigurationPropertyOptions.None);
            _clientProperty2 = new ConfigurationProperty("writer", typeof(WriterElement), null, ConfigurationPropertyOptions.IsRequired);
            _clientProperty3 = new ConfigurationProperty("collect", typeof(CollectElement), null, ConfigurationPropertyOptions.IsRequired);
            _properties.Add(_clientProperty);
            _properties.Add(_clientProperty1);
            _properties.Add(_clientProperty2);
            _properties.Add(_clientProperty3);
        }

        public static string DefaultSectionName
        {
            get
            {
                if (string.IsNullOrEmpty(DefaultSettings.ConfigurationName))
                    return "weblog";
                return string.Concat(DefaultSettings.ConfigurationName, "/", "weblog");
            }
        }

        private static bool nonConfiguration = false;

        public static WeblogConfigurationSection GetSection()
        {
            if (nonConfiguration)
            {
                return null;
            }
            try
            {
                var configuration = (WeblogConfigurationSection)ConfigurationManager.GetSection(DefaultSectionName);
                nonConfiguration = false;
                return configuration;
            }
            catch (Exception ex)
            {
                nonConfiguration = true;
                Log.GetLog(LoggingSettings.SparrowCategory).Warning("WeblogConfiguration加载失败。", ex);
            }
            return null;
        }
    }
}

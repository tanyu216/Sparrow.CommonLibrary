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

        public string Version
        {
            get { return (string)this[_clientProperty]; }
            set { this[_clientProperty] = value; }
        }

        public IgnoreElementCollection Ignores
        {
            get { return (IgnoreElementCollection)this[_clientProperty1]; }
            set { this[_clientProperty1] = value; }
        }

        public WriterElement Writer
        {
            get { return (WriterElement)this[_clientProperty2]; }
            set { this[_clientProperty2] = value; }
        }

        public CollectElement Collect
        {
            get { return (CollectElement)this[_clientProperty3]; }
            set { this[_clientProperty3] = value; }
        }

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

        public static string SectionName
        {
            get
            {
                return "sparrow.CommonLibrary/weblog";
            }
        }

        public static WeblogConfigurationSection GetSection()
        {
            var configuration = (WeblogConfigurationSection)ConfigurationManager.GetSection(SectionName);
            return configuration;
        }
    }
}

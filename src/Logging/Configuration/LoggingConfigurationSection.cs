using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Logging.Filter;
using Sparrow.CommonLibrary.Logging.Writer;

namespace Sparrow.CommonLibrary.Logging.Configuration
{
    public class LoggingConfigurationSection : ConfigurationSection
    {
        private readonly ConfigurationPropertyCollection _properties;
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

        public FilterElementCollection Filters { get { return (FilterElementCollection)this[_clientProperty1]; } }

        public WriterElementCollection Writers { get { return (WriterElementCollection)this[_clientProperty2]; } }

        public LogLevel LowLevel { get { return (LogLevel)this[_clientProperty3]; } }

        public LoggingConfigurationSection()
        {
            _properties = new ConfigurationPropertyCollection();
            _clientProperty1 = new ConfigurationProperty("filters", typeof(FilterElementCollection), null, ConfigurationPropertyOptions.None);
            _clientProperty2 = new ConfigurationProperty("writers", typeof(WriterElementCollection), null, ConfigurationPropertyOptions.IsRequired);
            _clientProperty3 = new ConfigurationProperty("lowLevel", typeof(LogLevel), LogLevel.Debug, new GenericEnumConverter(typeof(LogLevel)), null, ConfigurationPropertyOptions.None);
            _properties.Add(_clientProperty1);
            _properties.Add(_clientProperty2);
            _properties.Add(_clientProperty3);
        }

        #region

        public static string DefaultSectionName
        {
            get
            {
                if (string.IsNullOrEmpty(DefaultSettings.ConfigurationName))
                    return "logging";
                return string.Concat(DefaultSettings.ConfigurationName, "/", "logging");
            }
        }

        private static bool nonConfiguration = false;

        public static LoggingConfigurationSection GetSection()
        {
            if (nonConfiguration)
            {
                return null;
            }
            try
            {
                var configuration = ((LoggingConfigurationSection)ConfigurationManager.GetSection(DefaultSectionName));
                nonConfiguration = false;
                return configuration;
            }
            catch (Exception ex)
            {
                nonConfiguration = true;
                Logging.Log.GetLog(LoggingSettings.SparrowCategory).Warning("LoggingConfiguration加载失败。", ex);
            }
            return null;
        }

        #endregion
    }
}

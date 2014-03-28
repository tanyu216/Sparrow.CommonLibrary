using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Database.Configuration
{
    public class ProviderElement : ConfigurationElement
    {
        private readonly ConfigurationProperty _builderProperty;
        private readonly ConfigurationProperty _dbProperty;

        public ProviderElement()
        {
            _builderProperty = new ConfigurationProperty("builder", typeof(BuilderElement), null);
            _dbProperty = new ConfigurationProperty("database", typeof(DatabaseElement), null);
            Properties.Add(_builderProperty);
            Properties.Add(_dbProperty);
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        public BuilderElement Builder
        {
            get { return (BuilderElement)this[_builderProperty]; }
            set { this[_builderProperty] = value; }
        }

        public DatabaseElement Database
        {
            get { return (DatabaseElement)this[_dbProperty]; }
            set { this[_dbProperty] = value; }
        }
    }
}

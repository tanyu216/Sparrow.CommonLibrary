using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Data.Configuration
{
    public class ProviderElement : ConfigurationElement
    {
        private readonly ConfigurationProperty _builderProperty;
        private readonly ConfigurationProperty _executerProperty;
        private readonly ConfigurationProperty _importerProperty;

        public ProviderElement()
        {
            _builderProperty = new ConfigurationProperty("builder", typeof(BuilderElement));
            _executerProperty = new ConfigurationProperty("executer", typeof(ExecuterElement));
            _importerProperty = new ConfigurationProperty("importer", typeof(ImporterElement));
            Properties.Add(_builderProperty);
            Properties.Add(_executerProperty);
            Properties.Add(_importerProperty);
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

        public ExecuterElement Executer
        {
            get { return (ExecuterElement)this[_builderProperty]; }
            set { this[_builderProperty] = value; }
        }

        public ImporterElement Importer
        {
            get { return (ImporterElement)this[_builderProperty]; }
            set { this[_builderProperty] = value; }
        }
    }
}

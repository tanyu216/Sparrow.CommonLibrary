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
        private readonly ConfigurationProperty _executerProperty;
        private readonly ConfigurationProperty _importerProperty;

        public ProviderElement()
        {
            _builderProperty = new ConfigurationProperty("builder", typeof(BuilderElement), null);
            _executerProperty = new ConfigurationProperty("executer", typeof(ExecuterElement), null);
            _importerProperty = new ConfigurationProperty("importer", typeof(ImporterElement), null);
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
            get { return (ExecuterElement)this[_executerProperty]; }
            set { this[_executerProperty] = value; }
        }

        public ImporterElement Importer
        {
            get { return (ImporterElement)this[_importerProperty]; }
            set { this[_importerProperty] = value; }
        }
    }
}

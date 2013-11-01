using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Database.Configuration
{
    public class ProviderElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        public new ProviderElement this[string name]
        {
            get { return (ProviderElement)BaseGet(name); }
        }

        protected override string ElementName
        {
            get
            {
                return "provider";
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ProviderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProviderElement)(element)).Name;
        }
    }
}

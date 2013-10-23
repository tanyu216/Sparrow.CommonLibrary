using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Logging.Configuration
{
    public class FilterElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        public new FilterElement this[string name]
        {
            get { return (FilterElement)BaseGet(name); }
        }

        protected override string ElementName
        {
            get
            {
                return "filter";
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FilterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FilterElement)(element)).Name;
        }
    }
}

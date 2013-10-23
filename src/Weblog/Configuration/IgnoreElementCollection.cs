using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Weblog.Configuration
{
    public class IgnoreElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override string ElementName
        {
            get
            {
                return "ignore";
            }
        }

        public new IgnoreElement this[string name]
        {
            get { return (IgnoreElement)BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new IgnoreElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IgnoreElement)(element)).Name;
        }
    }
}

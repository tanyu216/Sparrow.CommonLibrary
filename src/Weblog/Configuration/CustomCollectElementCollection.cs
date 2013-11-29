using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Weblog.Configuration
{
    public class CustomCollectElementCollection : ConfigurationElementCollection
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
                return "add";
            }
        }

        public new CustomCollectElement this[string name]
        {
            get { return (CustomCollectElement)BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CustomCollectElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CustomCollectElement)(element)).Name;
        }

        public void Add(CustomCollectElement element)
        {
            BaseAdd(element);
        }
    }
}

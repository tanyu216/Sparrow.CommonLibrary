using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Cache.Configuration
{
    public class CacheElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        public new CacheElement this[string regionName]
        {
            get { return (CacheElement)BaseGet(regionName); }
        }

        protected override string ElementName
        {
            get
            {
                return "add";
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CacheElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CacheElement)(element)).RegionName;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Logging.Configuration
{
    public class ParamElementCollection : ConfigurationElementCollection
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

        public new ParamElement this[string name]
        {
            get { return (ParamElement)BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ParamElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ParamElement)(element)).Name;
        }
    }
}

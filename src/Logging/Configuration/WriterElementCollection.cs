using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Logging.Configuration
{
    public class WriterElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        public new WriterElement this[string name]
        {
            get { return (WriterElement)BaseGet(name); }
        }

        protected override string ElementName
        {
            get
            {
                return "writer";
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new WriterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WriterElement)(element)).Name;
        }

        public void Add(WriterElement element)
        {
            BaseAdd(element);
        }
    }
}

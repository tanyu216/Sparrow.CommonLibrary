using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Data.Configuration
{
    public class ExecuterElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        [TypeConverter(typeof(TypeNameConverter))]
        public Type Type
        {
            get { return (Type)this["type"]; }
            set { this["type"] = value; }
        }
    }
}

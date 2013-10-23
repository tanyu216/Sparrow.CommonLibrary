using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Weblog.Configuration
{
    public class CustomCollectElement : ConfigurationElement
    {
        private const string ElementName = "name";
        private const string ElementType = "type";

        /// <summary>
        /// 名称
        /// </summary>
        [ConfigurationProperty(ElementName, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this[ElementName]; }
            set { this[ElementName] = value; }
        }

        /// <summary>
        /// 实现<see cref="Sparrow.CommonLibrary.Weblog.Collect.ICollecter"/>的实例类。
        /// </summary>
        [ConfigurationProperty(ElementType)]
        [TypeConverter(typeof(TypeNameConverter))]
        public Type Type
        {
            get { return (Type)this[ElementType]; }
            set { this[ElementType] = value; }
        }
    }
}

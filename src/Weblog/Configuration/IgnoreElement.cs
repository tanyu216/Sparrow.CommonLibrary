using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Weblog.Configuration
{
    public class IgnoreElement : ConfigurationElement
    {
        private const string ElementName = "name";
        private const string ElementMatch = "match";

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
        /// 正则匹配
        /// </summary>
        [ConfigurationProperty(ElementMatch, IsRequired = true)]
        public string Match
        {
            get { return (string)this[ElementMatch]; }
            set { this[ElementMatch] = value; }
        }
    }
}

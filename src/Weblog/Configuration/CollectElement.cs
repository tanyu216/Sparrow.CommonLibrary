using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Weblog.Configuration
{
    public class CollectElement : ConfigurationElement
    {
        private const string ElementItems = "items";

        private ConfigurationProperty _CustomsProperty;

        public CollectElement()
        {
            _CustomsProperty = new ConfigurationProperty("customs", typeof(CustomCollectElementCollection), null, ConfigurationPropertyOptions.None);
            Properties.Add(_CustomsProperty);
        }

        /// <summary>
        /// 数据采集选项
        /// </summary>
        [ConfigurationProperty(ElementItems, IsRequired = true)]
        public string Value
        {
            get { return (string)this[ElementItems]; }
            set { this[ElementItems] = value; }
        }

        /// <summary>
        /// 自定义数据采集器
        /// </summary>
        public CustomCollectElementCollection  Customs
        {
            get
            {
                var param = (CustomCollectElementCollection)this[_CustomsProperty];
                if (param == null)
                {
                    param = new CustomCollectElementCollection();
                    this[_CustomsProperty] = param;
                }
                return param;
            }
            set { this[_CustomsProperty] = value; }
        }
    }
}

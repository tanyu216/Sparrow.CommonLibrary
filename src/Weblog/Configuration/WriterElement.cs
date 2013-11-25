using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Weblog.Writer;

namespace Sparrow.CommonLibrary.Weblog.Configuration
{
    public class WriterElement : ConfigurationElement
    {
        private const string ElementType = "type";

        private ConfigurationProperty _ParamsProperty;

        public WriterElement()
        {
            _ParamsProperty = new ConfigurationProperty("params", typeof(ParamElementCollection), null, ConfigurationPropertyOptions.None);
            Properties.Add(_ParamsProperty);
        }

        /// <summary>
        /// 实现<see cref="Sparrow.CommonLibrary.Weblog.Writer.IWeblogWriter"/>接口的实例类
        /// </summary>
        [ConfigurationProperty(ElementType, DefaultValue = typeof(TextWeblogWriter))]
        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationValidator(typeof(ConfigurationIWeblogWriterValidator))]
        public Type Type
        {
            get { return (Type)this[ElementType]; }
            set { this[ElementType] = value; }
        }

        /// <summary>
        /// 参数信息
        /// </summary>
        public ParamElementCollection Params
        {
            get
            {
                var param = (ParamElementCollection)this[_ParamsProperty];
                if (param == null)
                {
                    param = new ParamElementCollection();
                    this[_ParamsProperty] = param;
                }
                return param;
            }
            set { this[_ParamsProperty] = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Logging.Writer;

namespace Sparrow.CommonLibrary.Logging.Configuration
{
    public class WriterElement : ConfigurationElement
    {
        private const string ElementName = "name";
        private const string ElementFilter = "filter";
        private const string ElementMode = "mode";
        private const string ElementType = "type";

        private ConfigurationProperty _ParamsProperty;

        public WriterElement()
        {
            _ParamsProperty = new ConfigurationProperty("params", typeof(ParamElementCollection), null, ConfigurationPropertyOptions.None);
            Properties.Add(_ParamsProperty);
        }

        public WriterElement(string name)
            : this()
        {
            this[ElementName] = name;
        }

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
        /// 引用的日志筛选器
        /// </summary>
        [ConfigurationProperty(ElementFilter)]
        public string FilterName
        {
            get { return (string)this[ElementFilter]; }
            set { this[ElementFilter] = value; }
        }

        /// <summary>
        /// 写日志的模式（db/text/custom)
        /// </summary>
        [ConfigurationProperty(ElementMode, IsRequired = true)]
        public string Mode
        {
            get { return (string)this[ElementMode]; }
            set { this[ElementMode] = value; }
        }

        /// <summary>
        /// 实现<see cref="Sparrow.CommonLibrary.Logging.Writer.ILogWriter"/>接口的实例类
        /// </summary>
        [ConfigurationProperty(ElementType)]
        [TypeConverter(typeof(TypeNameConverter))]
        public Type Type
        {
            get
            {
                switch ((Mode ?? string.Empty).ToLower())
                {
                    case "db":
                        return typeof(DbLogWriter);
                    case "text":
                        return typeof(TextLogWriter);
                }
                return (Type)this[ElementType];
            }
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

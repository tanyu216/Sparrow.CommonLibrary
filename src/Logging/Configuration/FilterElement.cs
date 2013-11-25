using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Logging.Filter;

namespace Sparrow.CommonLibrary.Logging.Configuration
{
    public class FilterElement : ConfigurationElement
    {
        private const string ElementName = "name";
        private const string ElementCategories = "categories";
        private const string ElementLogLevel = "logLevel";
        private const string ElementType = "type";

        public FilterElement()
        {

        }

        public FilterElement(string name)
            : this()
        {
            this[ElementName] = name;
        }

        /// <summary>
        /// 日志筛选器名称
        /// </summary>
        [ConfigurationProperty(ElementName, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this[ElementName]; }
            set { this[ElementName] = value; }
        }

        /// <summary>
        /// 日志筛选器筛选日志依据的日志分类
        /// </summary>
        [ConfigurationProperty(ElementCategories)]
        public string Categories
        {
            get { return (string)this[ElementCategories]; }
            set { this[ElementCategories] = value; }
        }

        /// <summary>
        /// 日志筛选器筛选日志依据的日志级别
        /// </summary>
        [ConfigurationProperty(ElementLogLevel, DefaultValue = LogLevel.Debug)]
        [TypeConverter(typeof(GenericEnumConverter))]
        public LogLevel LogLevel
        {
            get { return (LogLevel)this[ElementLogLevel]; }
            set { this[ElementLogLevel] = value; }
        }

        /// <summary>
        /// 日志筛选器类型（实现<see cref="Sparrow.CommonLibrary.Logging.Filter.ILogFilter"/>接口的实例类）
        /// </summary>
        [ConfigurationProperty(ElementType, DefaultValue = typeof(LogFilter))]
        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationValidator(typeof(ConfigurationILogFilterValidator))]
        public Type Type
        {
            get { return (Type)this[ElementType]; }
            set { this[ElementType] = value; }
        }
    }
}

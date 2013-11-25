using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Cache.Configuration
{
    public class CacheElement : ConfigurationElement
    {
        private const string ElementRegionName = "regionName";
        private const string ElementDefault = "default";
        private const string ElementType = "type";
        private const string ElementConn = "connectionString";

        public CacheElement()
        {

        }

        public CacheElement(string regionName)
            : this()
        {
            this[ElementRegionName] = regionName;
        }

        /// <summary>
        /// 缓存区域名称
        /// </summary>
        [ConfigurationProperty(ElementRegionName, IsRequired = true, IsKey = true)]
        public string RegionName
        {
            get { return (string)this[ElementRegionName]; }
            set { this[ElementRegionName] = value; }
        }

        /// <summary>
        /// 是否设置为默认的缓存
        /// </summary>
        [ConfigurationProperty(ElementDefault)]
        public bool Default
        {
            get { return (bool)this[ElementDefault]; }
            set { this[ElementDefault] = value; }
        }

        /// <summary>
        /// 缓存实例类型
        /// </summary>
        [ConfigurationProperty(ElementType)]
        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationValidator(typeof(ConfigurationICacheValidator))]
        public Type Type
        {
            get { return (Type)this[ElementType]; }
            set { this[ElementType] = value; }
        }

        /// <summary>
        /// 缓存服务链接信息
        /// </summary>
        [ConfigurationProperty(ElementConn)]
        public string ConnectionString
        {
            get { return (string)this[ElementConn]; }
            set { this[ElementConn] = value; }
        }

    }
}

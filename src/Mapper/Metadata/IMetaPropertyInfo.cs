using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Mapper.Metadata
{
    /// <summary>
    /// 元数据属性描述
    /// </summary>
    public interface IMetaPropertyInfo
    {
        /// <summary>
        /// <see cref="IMetaInfo"/>实例
        /// </summary>
        IMetaInfo MetaInfo { get; }
        /// <summary>
        /// 属性名称
        /// </summary>
        string PropertyName { get; }
        /// <summary>
        /// 映射的对象属性
        /// </summary>
        PropertyInfo PropertyInfo { get; }
    }
}

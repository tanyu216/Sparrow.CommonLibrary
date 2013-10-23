using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Data.Mapper.Metadata
{
    /// <summary>
    /// 成员字段描述
    /// </summary>
    public interface IMetaFieldInfo
    {
        /// <summary>
        /// <see cref="IMetaInfo"/>实例
        /// </summary>
        IMetaInfo MetaInfo { get; }
        /// <summary>
        /// 成员字段映射的属性
        /// </summary>
        PropertyInfo PropertyInfo { get; }
        /// <summary>
        /// 成员名称
        /// </summary>
        string FieldName { get; }
        /// <summary>
        /// 主键标识
        /// </summary>
        bool IsKey { get; }
        /// <summary>
        /// 默认值
        /// </summary>
        object DefaultValue { get; }
        /// <summary>
        /// 指示字段成员是否有默认值。
        /// </summary>
        /// <returns></returns>
        bool HasDefaultValue();
        /// <summary>
        /// 成员字段扩展
        /// </summary>
        IMetaFieldExtend[] GetExtends();
    }
}

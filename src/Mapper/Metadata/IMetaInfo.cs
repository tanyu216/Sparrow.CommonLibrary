using System.Reflection;

namespace Sparrow.CommonLibrary.Mapper.Metadata
{
    /// <summary>
    /// 实体的源数据描述
    /// </summary>
    public interface IMetaInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 具有主键标识的成员字段数量
        /// </summary>
        int KeyCount { get; }
        /// <summary>
        /// 成员字段个数
        /// </summary>
        int FieldCount { get; }
        /// <summary>
        /// 获取指定的成员字段
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        IMetaFieldInfo this[string field] { get; }
        /// <summary>
        /// 获取指定的成员字段
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        IMetaFieldInfo this[PropertyInfo propertyInfo] { get; }
        /// <summary>
        /// 主键检测
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        bool IsKey(string field);
        /// <summary>
        /// 具有主键标识的成员字段名称
        /// </summary>
        string[] GetKeys();
        /// <summary>
        /// 所有成员字段名称
        /// </summary>
        string[] GetFieldNames();
        /// <summary>
        /// 获取所有的成员字段
        /// </summary>
        IMetaFieldInfo[] GetFields();
        /// <summary>
        /// 元数据扩张
        /// </summary>
        /// <returns></returns>
        IMetaInfoExtend[] GetExtends();
    }
}

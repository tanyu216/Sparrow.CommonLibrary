using System;
using System.Reflection;

namespace Sparrow.CommonLibrary.Mapper.Metadata
{
    /// <summary>
    /// 元数据描述
    /// </summary>
    public interface IMetaInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 实体类型
        /// </summary>
        Type EntityType { get; }
        /// <summary>
        /// 属性个数
        /// </summary>
        int PropertyCount { get; }
        /// <summary>
        /// 获取指定的属性
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IMetaPropertyInfo this[int index] { get; }
        /// <summary>
        /// 获取指定的属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        IMetaPropertyInfo this[string propertyName] { get; }
        /// <summary>
        /// 获取指定的属性
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        IMetaPropertyInfo this[PropertyInfo propertyInfo] { get; }
        /// <summary>
        /// 获取指定属性的下标
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        int IndexOf(string propertyName);
        /// <summary>
        /// 所有属性名称
        /// </summary>
        string[] GetPropertyNames();
        /// <summary>
        /// 获取所有的属性
        /// </summary>
        IMetaPropertyInfo[] GetProperties();
        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="metaPropertyInfo"></param>
        void AddPropertyInfo(IMetaPropertyInfo metaPropertyInfo);
        /// <summary>
        /// 移除属性
        /// </summary>
        /// <param name="metaPropertyInfo"></param>
        void RemovePropertyInfo(IMetaPropertyInfo metaPropertyInfo);
        /// <summary>
        /// 元数据标记为只读
        /// </summary>
        void MakeReadonly();
    }
}

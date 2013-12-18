using Sparrow.CommonLibrary.Mapper.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sparrow.CommonLibrary.Mapper
{
    /// <summary>
    /// 对象访问器
    /// </summary>
    public interface IObjectAccessor
    {
        /// <summary>
        /// 获取对象指定成员对象的属性读写器
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        IPropertyAccessor this[string propertyName] { get; }
        /// <summary>
        /// 获取对象指定成员对象的属性读写器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IPropertyAccessor this[int index] { get; }
        /// <summary>
        /// 对象元数据
        /// </summary>
        /// <returns></returns>
        IMetaInfo MetaInfo { get; }
        /// <summary>
        /// 创建一个实体对象
        /// </summary>
        /// <returns></returns>
        object Create();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        object MapSingle(object dataSource);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        IList MapList(object dataSource);
    }

    /// <summary>
    /// 对象访问器
    /// </summary>
    /// <typeparam name="T">实体类型<see cref="T"/></typeparam>
    public interface IObjectAccessor<T> : IObjectAccessor
    {
        /// <summary>
        /// 获取对象指定成员对象的值
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        new IPropertyAccessor<T> this[string field] { get; }
        /// <summary>
        /// 获取对象指定成员对象的属性读写器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        new IPropertyAccessor<T> this[int index] { get; }
        /// <summary>
        /// 创建一个类型为<see cref="T"/>实体对象
        /// </summary>
        /// <returns></returns>
        new T Create();
        /// <summary>
        /// 映射
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        new T MapSingle(object dataSource);
        /// <summary>
        /// 映射
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        new List<T> MapList(object dataSource);
    }

}

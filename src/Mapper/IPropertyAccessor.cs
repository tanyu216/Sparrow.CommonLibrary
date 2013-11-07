using System;
using System.Linq.Expressions;
using System.Reflection;
using Sparrow.CommonLibrary.Database;

namespace Sparrow.CommonLibrary.Mapper
{
    /// <summary>
    /// 实体对象的取值或赋值
    /// </summary>
    public interface IPropertyAccessor
    {
        /// <summary>
        /// 向对象的目标属性赋值
        /// </summary>
        /// <param name="handler">引用的对象</param>
        /// <param name="value">将<paramref name="value"/>赋值给对象的属性</param>
        void SetValue(object handler, object value);
        /// <summary>
        /// 获取对象的目标属性的值
        /// </summary>
        /// <param name="handler">引用的对象</param>
        /// <returns></returns>
        object GetValue(object handler);
    }

    /// <summary>
    /// 实体对象的取值或赋值
    /// </summary>
    public interface IPropertyAccessor<T> : IPropertyAccessor
    {
        /// <summary>
        /// 向对象的目标属性赋值
        /// </summary>
        /// <param name="handler">引用的对象</param>
        /// <param name="value">将<paramref name="value"/>赋值给对象的属性</param>
        void SetValue(T handler, object value);
        /// <summary>
        /// 获取对象的目标属性的值
        /// </summary>
        /// <param name="handler">引用的对象</param>
        /// <returns></returns>
        object GetValue(T handler);
    }

}

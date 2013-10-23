﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using Sparrow.CommonLibrary.Data.Database;
using Sparrow.CommonLibrary.Data.Entity;
using Sparrow.CommonLibrary.Data.Mapper.Metadata;

namespace Sparrow.CommonLibrary.Data.Mapper
{

    /// <summary>
    /// 实体对象内部的取值或赋值实现。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>此对象通过Lambda表达示树编译成委托实现对象属性的取值或赋值。</remarks>
    public class ObjectPropertyValue<T> : IPropertyValue<T>
    {
        readonly Func<T, object> _getter;
        readonly Action<T, object> _setter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        public ObjectPropertyValue(PropertyInfo propertyInfo)
        {
            var param1 = Expression.Parameter(typeof(T), "handler");
            var param2 = Expression.Parameter(typeof(object), "value");
            // 获取属性的值的方法
            var accessExp = Expression.MakeUnary(ExpressionType.Convert, Expression.MakeMemberAccess(param1, propertyInfo), typeof(object));
            _getter = Expression.Lambda<Func<T, object>>(accessExp, param1).Compile();
            // 向属性赋值的方法
            var assignExp = Expression.Assign(
                    Expression.Property(param1, propertyInfo),
                    Expression.Call(this.GetType(), "To", new Type[] { propertyInfo.PropertyType }, new ParameterExpression[] { param2 })
                );
            _setter = Expression.Lambda<Action<T, object>>(assignExp, param1, param2).Compile();
        }

        public void SetValue(object handler, object value)
        {
            SetValue((T)handler, value);
        }

        public void SetValue(T handler, object value)
        {
            _setter(handler, value);
        }

        public object GetValue(object handler)
        {
            return GetValue((T)handler);
        }

        public object GetValue(T handler)
        {
            return _getter(handler);
        }

        protected virtual TPropertyType To<TPropertyType>(object value)
        {
            return DbValueCast.Cast<TPropertyType>(value);
        }
    }

}

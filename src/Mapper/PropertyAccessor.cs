using System;
using System.Linq.Expressions;
using System.Reflection;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Entity;
using Sparrow.CommonLibrary.Mapper.Metadata;
using Sparrow.CommonLibrary.Mapper.TypeMappers;

namespace Sparrow.CommonLibrary.Mapper
{

    /// <summary>
    /// 实体对象内部的取值或赋值实现。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>此对象通过Lambda表达示树编译成委托实现对象属性的取值或赋值。</remarks>
    public class PropertyAccessor<T> : IPropertyAccessor<T>
    {
        readonly Func<T, object> _getter;
        readonly Action<T, object> _setter;
        readonly ITypeMapper _typeMapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            _typeMapper = NativeTypeMapper.GetTypeMapper(propertyInfo.PropertyType);

            var param1 = Expression.Parameter(typeof(T), "handler");
            var param2 = Expression.Parameter(typeof(object), "value");
            // 获取属性的值的方法
            var accessExp = Expression.MakeUnary(ExpressionType.Convert, Expression.MakeMemberAccess(param1, propertyInfo), typeof(object));
            _getter = Expression.Lambda<Func<T, object>>(accessExp, param1).Compile();
            // 向属性赋值的方法
            var assignExp = Expression.Assign(
                    Expression.Property(param1, propertyInfo),
                    Expression.Call(Expression.Convert(Expression.Constant(_typeMapper), typeof(ITypeMapper).MakeGenericType(propertyInfo.PropertyType)), "Cast", new Type[0], new ParameterExpression[] { param2 })
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
    }

}

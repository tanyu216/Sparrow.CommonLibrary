using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMappers
{
    /// <summary>
    /// 字典转换的基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DictionaryTypeMapperBase<T> : ITypeMapper<T>
    {
        public DictionaryTypeMapperBase()
        {
            bool validated = false;

            if (DesctinationType.IsGenericType && DesctinationType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                validated = true;
            }
            else if (DesctinationType == typeof(IDictionary))
            {
                validated = true;
            }
            else if (DesctinationType.GetInterfaces().Any(x => (x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>)) || x == typeof(IDictionary)))
            {
                validated = true;
            }

            if (!validated)
                throw new MapperException(string.Format("类型{0}未实现字典接口：IDictionary/IDictionary<TKey,TValue>。", typeof(T).FullName));

        }

        public T Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(T); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        protected abstract T Create();

        protected abstract T Convert(object value);

        protected virtual T Convert(IDictionary source)
        {
            var dictionary = (IDictionary)Create();

            foreach (DictionaryEntry keyVal in source)
            {
                var type = keyVal.Key.GetType();
                if (keyVal.Value == null)
                    dictionary.Add(NativeTypeMapper.GetTypeMapper(type).Cast(keyVal.Key), null);
                else
                {
                    var type2 = keyVal.Value.GetType();
                    dictionary.Add(NativeTypeMapper.GetTypeMapper(type).Cast(keyVal.Key), NativeTypeMapper.GetTypeMapper(type2).Cast(keyVal.Value));
                }
            }

            return (T)dictionary;
        }

        protected virtual T Convert<TKey, TValue>(IDictionary source)
        {
            var dictionary = (IDictionary<TKey, TValue>)Create();

            var typeMapper = NativeTypeMapper.GetTypeMapper<TKey>();
            var typeMapper2 = NativeTypeMapper.GetTypeMapper<TValue>();

            foreach (DictionaryEntry keyVal in source)
                dictionary.Add(typeMapper.Cast(keyVal.Key), typeMapper2.Cast(keyVal.Value));

            return (T)dictionary;
        }

        protected virtual T Convert<TKey, TValue, TSourceKey, TSourceValue>(IDictionary<TSourceKey, TSourceValue> source)
        {
            var dictionary = (IDictionary<TKey, TValue>)Create();

            var typeMapper = NativeTypeMapper.GetTypeMapper<TKey>();
            var typeMapper2 = NativeTypeMapper.GetTypeMapper<TValue>();

            foreach (KeyValuePair<TSourceKey, TSourceValue> keyVal in source)
                dictionary.Add(typeMapper.Cast(keyVal.Key), typeMapper2.Cast(keyVal.Value));

            return (T)dictionary;
        }

        protected virtual T Convert<TSourceKey, TSourceValue>(IDictionary<TSourceKey, TSourceValue> source)
        {
            var dictionary = (IDictionary)Create();

            var typeMapper = NativeTypeMapper.GetTypeMapper<TSourceKey>();
            var typeMapper2 = NativeTypeMapper.GetTypeMapper<TSourceValue>();

            foreach (KeyValuePair<TSourceKey, TSourceValue> keyVal in source)
                dictionary.Add(typeMapper.Cast(keyVal.Key), typeMapper2.Cast(keyVal.Value));

            return (T)dictionary;
        }
    }

    /// <summary>
    /// 针对<see cref="Dictionary&lt;TKey,TValue&gt;"/>转换的优化
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DictionaryTypeMapper<TKey, TValue> : DictionaryTypeMapperBase<Dictionary<TKey, TValue>>
    {
        protected override Dictionary<TKey, TValue> Create()
        {
            return new Dictionary<TKey, TValue>();
        }

        protected override Dictionary<TKey, TValue> Convert(object value)
        {
            if (value == null || !(value is IEnumerable))
                return null;

            if (value is IDictionary<TKey, TValue>)
            {
                return Convert((IDictionary<TKey, TValue>)value);
            }

            if (value is IDictionary)
            {
                return Convert((IDictionary)value);
            }

            var iDicType = value.GetType().GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));
            if (iDicType != null)
            {
                var param1 = Expression.Parameter(typeof(object));
                var args = iDicType.GetGenericArguments();
                var method = this.GetType().GetMethod("Convert", new Type[] { typeof(IDictionary<,>).MakeGenericType(args) }).MakeGenericMethod(typeof(TKey), typeof(TValue), args[0], args[1]);
                var caller = Expression.Call(Expression.Constant(this), method, Expression.Convert(param1, typeof(IDictionary<,>).MakeGenericType(args)));
                var genericDicConvert = Expression.Lambda<Func<object, Dictionary<TKey, TValue>>>(caller, param1).Compile();

                return genericDicConvert(value);
            }

            return null;
        }
    }

    /// <summary>
    /// 针对<see cref="Hashtable"/>转换的优化
    /// </summary>
    public class HashtableTypeMapper : DictionaryTypeMapperBase<Hashtable>
    {
        protected override Hashtable Create()
        {
            return new Hashtable();
        }

        protected override Hashtable Convert(object value)
        {
            if (value == null || !(value is IEnumerable))
                return null;

            if (value is IDictionary)
            {
                return Convert((IDictionary)value);
            }

            var iDicType = value.GetType().GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));
            if (iDicType != null)
            {
                var param1 = Expression.Parameter(typeof(object));
                var args = iDicType.GetGenericArguments();
                var method = this.GetType().GetMethod("Convert", new Type[] { typeof(IDictionary<,>).MakeGenericType(args) }).MakeGenericMethod(args);
                var caller = Expression.Call(Expression.Constant(this), method, Expression.Convert(param1, typeof(IDictionary<,>).MakeGenericType(args)));
                var genericDicConvert = Expression.Lambda<Func<object, Hashtable>>(caller, param1).Compile();

                return genericDicConvert(value);
            }

            return null;
        }
    }

    /// <summary>
    /// 通用字典的转换（包括支持<see cref="HashtableTypeMapper"/>/<see cref="DictionaryTypeMapper&lt;TKey,TValue&gt;"/>所支持的）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommonDictionaryTypeMapper<T> : DictionaryTypeMapperBase<T>
    {
        /// <summary>
        /// 字典接口信息描述
        /// </summary>
        private Type iDicType;
        /// <summary>
        /// 适用于泛型的集合的初始化
        /// </summary>
        private readonly Func<IDictionary, T> genericDicConvert;

        public CommonDictionaryTypeMapper()
        {
            if (!DesctinationType.IsClass)
            {
                throw new MapperException(string.Format("类型{0}不是一个有效的实例类型。", DesctinationType.FullName));
            }

            if (iDicType == null)
                iDicType = DesctinationType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));

            if (iDicType == null)
                iDicType = DesctinationType.GetInterfaces().FirstOrDefault(x => x == typeof(IDictionary));

            if (iDicType == null)
                throw new MapperException(string.Format("类型{0}未实现字典接口：IDictionary/IDictionary<TKey,TValue>。", typeof(T).FullName));

            if (iDicType.IsGenericType)
            {
                var param1 = Expression.Parameter(typeof(IDictionary));
                var args = iDicType.GetGenericArguments();
                var caller = Expression.Call(Expression.Constant(this), this.GetType().GetMethod("Convert", new Type[] { typeof(IDictionary) }).MakeGenericMethod(args), param1);
                genericDicConvert = Expression.Lambda<Func<IDictionary, T>>(caller, param1).Compile();
            }
        }

        protected override T Create()
        {
            return (T)Activator.CreateInstance(DesctinationType);
        }

        protected override T Convert(object value)
        {
            if (value == null || !(value is IEnumerable))
                return default(T);

            var sourceDicType = value.GetType().GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));
            if (sourceDicType != null)
            {
                var param1 = Expression.Parameter(typeof(object));
                var args = sourceDicType.GetGenericArguments();
                System.Reflection.MethodInfo method;
                if (iDicType.IsGenericType)
                {
                    method = this.GetType().GetMethod("Convert", new Type[] { typeof(IDictionary<,>).MakeGenericType(args) }).MakeGenericMethod(iDicType.GetGenericArguments().Zip(args, (x, y) => y).ToArray());
                }
                else
                {
                    method = this.GetType().GetMethod("Convert", new Type[] { typeof(IDictionary<,>).MakeGenericType(args) }).MakeGenericMethod(args);
                }

                var caller = Expression.Call(Expression.Constant(this), method, Expression.Convert(param1, typeof(IDictionary<,>).MakeGenericType(args)));
                var convert = Expression.Lambda<Func<object, T>>(caller, param1).Compile();

                return convert(value);
            }

            if (value is IDictionary)
            {
                if (genericDicConvert != null)
                {
                    return genericDicConvert((IDictionary)value);
                }
                return Convert((IDictionary)value);
            }

            return default(T);
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class DictionaryTypeMapper<T> : ITypeMapper<T> where T : class
    {
        /// <summary>
        /// 创建字典实例的类型描述
        /// </summary>
        private Type instanceType;
        /// <summary>
        /// 字典接口信息描述
        /// </summary>
        private Type iDicType;
        /// <summary>
        /// 适用于泛型的集合的初始化
        /// </summary>
        private readonly Action<T, object> dicInit;

        public DictionaryTypeMapper()
        {
            if (DesctinationType.IsInterface)
            {
                if (DesctinationType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    instanceType = typeof(Dictionary<,>).MakeGenericType(DesctinationType.GetGenericArguments());
                    iDicType = DesctinationType;
                }

                if (DesctinationType == typeof(IDictionary))
                {
                    instanceType = typeof(Hashtable);
                    iDicType = DesctinationType;
                }
            }
            else if (!DesctinationType.IsClass)
            {
                throw new MapperException(string.Format("类型{0}不是一个有效的实例类型。", DesctinationType.FullName));
            }

            if (instanceType == null)
                instanceType = DesctinationType;

            if (iDicType == null)
                iDicType = DesctinationType.GetInterfaces().FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(IDictionary<,>));

            if (iDicType == null)
                iDicType = DesctinationType.GetInterfaces().FirstOrDefault(x => x == typeof(IDictionary));

            if (iDicType == null)
                throw new MapperException(string.Format("类型{0}未实现字典接口：IDictionary/IDictionary<TKey,TValue>。", typeof(T).FullName));

            if (iDicType.IsGenericType)
            {
                var param1 = Expression.Parameter(DesctinationType);
                var param2 = Expression.Parameter(typeof(object));
                var args = iDicType.GetGenericArguments();
                var caller = Expression.Call(null, this.GetType().GetMethod("Convert", new Type[] { typeof(IDictionary<,>).MakeGenericType(args), typeof(object) }).MakeGenericMethod(args), Expression.Convert(param1, typeof(IDictionary<,>).MakeGenericType(args)), param2);
                dicInit = Expression.Lambda<Action<T, object>>(caller, param1, param2).Compile();
            }
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

        private T Create()
        {
            return (T)Activator.CreateInstance(instanceType);
        }

        private T Convert(object value)
        {
            if (value == null || !(value is IEnumerable))
                return null;

            var dictionary = Create();

            if (value is IDictionary)
            {
                if (dicInit != null)
                {
                    var argType = iDicType.GetGenericArguments()[0];
                    var argType2 = iDicType.GetGenericArguments()[1];
                    var typeMapper = NativeTypeMapper.GetTypeMapper(argType);
                    var typeMapper2 = NativeTypeMapper.GetTypeMapper(argType2);

                    //foreach (DictionaryEntry keyVal in (IDictionary)value)
                    //    dicAddCaller(dictionary, typeMapper.Cast(keyVal.Key), typeMapper2.Cast(keyVal.Value));

                }
                else
                {
                    var hash = (IDictionary)dictionary;
                    foreach (DictionaryEntry keyVal in (IDictionary)value)
                    {
                        var type = keyVal.Key.GetType();
                        if (keyVal.Value == null)
                            hash.Add(NativeTypeMapper.GetTypeMapper(type).Cast(keyVal.Key), null);
                        else
                        {
                            var type2 = keyVal.Value.GetType();
                            hash.Add(NativeTypeMapper.GetTypeMapper(type).Cast(keyVal.Key), NativeTypeMapper.GetTypeMapper(type2).Cast(keyVal.Value));
                        }
                    }
                }

                return (T)dictionary;
            }
            return default(T);
        }

        private T Convert<TKey, TValue>(IDictionary source)
        {
            var dictionary = (IDictionary<TKey, TValue>)Create();

            var typeMapper = NativeTypeMapper.GetTypeMapper<TKey>();
            var typeMapper2 = NativeTypeMapper.GetTypeMapper<TValue>();

            foreach (DictionaryEntry keyVal in source)
                dictionary.Add(typeMapper.Cast(keyVal.Key), typeMapper2.Cast(keyVal.Value));

            return (T)dictionary;
        }

        private T Convert<TKey, TValue, TSourceKey, TSourceValue>(IDictionary<TSourceKey, TSourceValue> source)
        {
            var dictionary = (IDictionary<TKey, TValue>)Create();

            var typeMapper = NativeTypeMapper.GetTypeMapper<TKey>();
            var typeMapper2 = NativeTypeMapper.GetTypeMapper<TValue>();

            foreach (KeyValuePair<TSourceKey, TSourceValue> keyVal in source)
                dictionary.Add(typeMapper.Cast(keyVal.Key), typeMapper2.Cast(keyVal.Value));

            return (T)dictionary;
        }

        private T Convert<TSourceKey, TSourceValue>(IDictionary<TSourceKey, TSourceValue> source)
        {
            var dictionary = (IDictionary)Create();

            var typeMapper = NativeTypeMapper.GetTypeMapper<TSourceKey>();
            var typeMapper2 = NativeTypeMapper.GetTypeMapper<TSourceValue>();

            foreach (KeyValuePair<TSourceKey, TSourceValue> keyVal in source)
                dictionary.Add(typeMapper.Cast(keyVal.Key), typeMapper2.Cast(keyVal.Value));

            return (T)dictionary;
        }
    }

    public class DictionaryTypeMapper<TKey, TValue> : ITypeMapper<Dictionary<TKey, TValue>>
    {
        public Dictionary<TKey, TValue> Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Dictionary<TKey, TValue>); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private Dictionary<TKey, TValue> Convert(object value)
        {
            if (value == null)
                return null;

            var dictionary = new Dictionary<TKey, TValue>();

            var typeMapper = NativeTypeMapper.GetTypeMapper<TKey>();
            var typeMapper2 = NativeTypeMapper.GetTypeMapper<TValue>();

            if (value is IDictionary<TKey, TValue>)
            {
                foreach (KeyValuePair<TKey, TValue> keyVal in (IDictionary<TKey, TValue>)value)
                    dictionary.Add(typeMapper.Cast(keyVal.Key), typeMapper2.Cast(keyVal.Value));

                return dictionary;
            }

            if (value is IDictionary)
            {
                foreach (DictionaryEntry keyVal in (IDictionary)value)
                    dictionary.Add(typeMapper.Cast(keyVal.Key), typeMapper2.Cast(keyVal.Value));

                return dictionary;
            }

            return default(Dictionary<TKey, TValue>);
        }
    }
}

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
        private Type newType;
        private Type iDicType;
        /// <summary>
        /// 适用于泛型的集合
        /// </summary>
        private Action<T, object, object> dicAddCaller;

        public DictionaryTypeMapper()
        {
            if (DesctinationType.IsInterface)
            {
                if (DesctinationType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    newType = typeof(Dictionary<,>).MakeGenericType(DesctinationType.GetGenericArguments());
                    iDicType = DesctinationType;
                }

                if (DesctinationType == typeof(IDictionary))
                {
                    newType = typeof(Hashtable);
                    iDicType = DesctinationType;
                }
            }

            if (!DesctinationType.IsAbstract)
                newType = typeof(T);

            if (iDicType == null)
                iDicType = DesctinationType.GetInterfaces().FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(IDictionary<,>));

            if (iDicType == null)
                iDicType = DesctinationType.GetInterfaces().FirstOrDefault(x => x == typeof(IDictionary));

            if (iDicType != null && iDicType.IsGenericType)
            {
                var param1 = Expression.Parameter(typeof(T));
                var param2 = Expression.Parameter(typeof(object));
                var param3 = Expression.Parameter(typeof(object));
                var args = iDicType.GetGenericArguments();
                dicAddCaller = Expression.Lambda<Action<T, object, object>>(Expression.Call(param1, iDicType.GetMethod("Add", new Type[] { args[0], args[1] }), Expression.Convert(param2, args[0]), Expression.Convert(param3, args[1])), param1, param2, param3).Compile();
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
            if (newType == null)
                return null;
            return Activator.CreateInstance(newType) as T;
        }

        private T Convert(object value)
        {
            if (value is IEnumerable)
            {
                var dictionary = Create();
                if (dictionary == null)
                    return null;

                if (iDicType.IsGenericType)
                {
                    var argType = iDicType.GetGenericArguments()[0];
                    var argType2 = iDicType.GetGenericArguments()[1];
                    var typeMapper = NativeTypeMapper.GetTypeMapper(argType);
                    var typeMapper2 = NativeTypeMapper.GetTypeMapper(argType2);

                    foreach (DictionaryEntry keyVal in (IDictionary)value)
                        dicAddCaller(dictionary, typeMapper.Cast(keyVal.Key), typeMapper2.Cast(keyVal.Value));

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
            if (value is IDictionary)
            {
                var dictionary = new Dictionary<TKey, TValue>();

                var typeMapper = NativeTypeMapper.GetTypeMapper<TKey>();
                var typeMapper2 = NativeTypeMapper.GetTypeMapper<TValue>();

                foreach (DictionaryEntry keyVal in (IDictionary)value)
                    dictionary.Add(typeMapper.Cast(keyVal.Key), typeMapper2.Cast(keyVal.Value));

                return dictionary;
            }
            return default(Dictionary<TKey, TValue>);
        }
    }
}

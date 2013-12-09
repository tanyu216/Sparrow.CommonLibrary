using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    /// <summary>
    /// 自定义字典的转换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomDictionaryTypeMapper<T> : ITypeMapper<T> where T : class
    {
        private Type iDicType;
        /// <summary>
        /// 适用于泛型的集合
        /// </summary>
        private Action<T, object, object> dicAddCaller;

        public CustomDictionaryTypeMapper()
        {
            if (!DesctinationType.IsClass)
                throw new MapperException(string.Format("类型{0}不是一个有效的实例类型", DesctinationType.FullName));

            if (iDicType == null)
                iDicType = DesctinationType.GetInterfaces().FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(IDictionary<,>));

            if (iDicType == null)
                iDicType = DesctinationType.GetInterfaces().FirstOrDefault(x => x == typeof(IDictionary));

            if (iDicType == null)
                throw new MapperException(string.Format("类型{0}未实现字典接口：IDictionary/IDictionary<TKey,TValue>。", DesctinationType.FullName));

            if (iDicType.IsGenericType)
            {
                var param1 = Expression.Parameter(DesctinationType);
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
            return Activator.CreateInstance<T>();
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

}

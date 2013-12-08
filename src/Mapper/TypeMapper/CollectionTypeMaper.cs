using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class CollectionTypeMaper<T> : ITypeMapper<T> where T : class
    {
        private Type newType;
        private Type collectionType;
        /// <summary>
        /// 适用于泛型的集合
        /// </summary>
        private Action<T, object> collectionAddCaller;

        public CollectionTypeMaper()
        {
            if (DesctinationType.IsInterface)
            {
                if (DesctinationType.GetGenericTypeDefinition() == typeof(IList<>) ||
                    DesctinationType.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                    DesctinationType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    newType = typeof(List<>).MakeGenericType(DesctinationType.GetGenericArguments()[0]);
                    collectionType = DesctinationType;
                }

                if (DesctinationType == typeof(IList) ||
                    DesctinationType == typeof(ICollection) ||
                    DesctinationType == typeof(IEnumerable))
                {
                    newType = typeof(ArrayList);
                    collectionType = DesctinationType;
                }
            }

            if (!DesctinationType.IsAbstract)
                newType = typeof(T);

            if (collectionType == null)
                collectionType = DesctinationType.GetInterfaces().FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (collectionType == null)
                collectionType = DesctinationType.GetInterfaces().FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(IEnumerable));

            if (collectionType != null && collectionType.IsGenericType)
            {
                var param1 = Expression.Parameter(typeof(T));
                var param2 = Expression.Parameter(typeof(object));
                collectionAddCaller = Expression.Lambda<Action<T, object>>(Expression.Call(param1, collectionType.GetMethod("Add", new Type[] { collectionType.GetGenericArguments()[0] }), Expression.Convert(param2, collectionType.GetGenericArguments()[0])), param1, param2).Compile();
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
                var collection = Create();
                if (collection == null)
                    return default(T);

                if (collectionType.IsGenericType)
                {
                    var argType = collectionType.GetGenericArguments()[0];
                    var typeMapper = NativeTypeMapper.GetTypeMapper(argType);
                    foreach (var obj in (IEnumerable)value)
                        collectionAddCaller(collection, typeMapper.Cast(obj));

                }
                else
                {
                    var list = (IList)collection;
                    foreach (var obj in (IEnumerable)value)
                    {
                        if (obj == null)
                            list.Add(null);
                        else
                        {
                            var type = obj.GetType();
                            if (type.IsPrimitive || type.IsValueType)
                                list.Add(obj);
                            else
                                list.Add(NativeTypeMapper.GetTypeMapper(type).Cast(obj));
                        }
                    }
                }

                return (T)collection;
            }
            return default(T);
        }
    }
}

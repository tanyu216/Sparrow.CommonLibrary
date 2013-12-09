using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class CollectionTypeMaper<T> : ITypeMapper<T> where T : class
    {
        /// <summary>
        /// 创建集合实例的类型描述
        /// </summary>
        private readonly Type instanceType;
        /// <summary>
        /// 集合接口类型描述
        /// </summary>
        private readonly Type iCollectionType;
        /// <summary>
        /// 适用于泛型的集合转换
        /// </summary>
        private readonly Func<IEnumerable, T> genericCollectionConvert;

        public CollectionTypeMaper()
        {
            if (DesctinationType.IsInterface)
            {
                if (DesctinationType.GetGenericTypeDefinition() == typeof(IList<>) ||
                    DesctinationType.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                    DesctinationType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    instanceType = typeof(List<>).MakeGenericType(DesctinationType.GetGenericArguments()[0]);
                    iCollectionType = DesctinationType;
                }

                if (DesctinationType == typeof(IList) ||
                    DesctinationType == typeof(ICollection) ||
                    DesctinationType == typeof(IEnumerable))
                {
                    instanceType = typeof(ArrayList);
                    iCollectionType = DesctinationType;
                }
            }
            else if (!DesctinationType.IsClass)
            {
                throw new MapperException(string.Format("类型{0}不是一个有效的实例类型", DesctinationType.FullName));
            }

            if (instanceType == null)
                instanceType = DesctinationType;

            if (iCollectionType == null)
                iCollectionType = DesctinationType.GetInterfaces().FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(ICollection<>));

            if (iCollectionType == null)
                iCollectionType = DesctinationType.GetInterfaces().FirstOrDefault(x => x == typeof(IList));

            if (iCollectionType == null)
                throw new MapperException(string.Format("实现类型{0}未实现接口：IList/ICollection<>", DesctinationType.FullName));

            if (iCollectionType.IsGenericType)
            {
                var param1 = Expression.Parameter(typeof(IEnumerable));
                var caller = Expression.Call(Expression.Constant(this), this.GetType().GetMethod("Convert", new Type[] { typeof(IEnumerable) }).MakeGenericMethod(iCollectionType.GetGenericArguments()[0]), param1);
                genericCollectionConvert = Expression.Lambda<Func<IEnumerable, T>>(caller, param1).Compile();
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
            if (value is IEnumerable)
            {
                if (genericCollectionConvert != null)
                {
                    return genericCollectionConvert((IEnumerable)value);
                }
                else
                {
                    var collection = Create();
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
                    return (T)collection;
                }
            }
            return default(T);
        }

        private T Convert<TElementType>(IEnumerable source)
        {
            ICollection<TElementType> dest = (ICollection<TElementType>)Create();
            var typeMapper = NativeTypeMapper.GetTypeMapper<TElementType>();
            foreach (object element in source)
            {
                dest.Add(typeMapper.Cast(element));
            }
            return (T)dest;
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMappers
{
    public class HashSetTypeMapper<T> : ITypeMapper<T>
    {
        private Type instanceType;
        private Type hashSetType;
        private Func<IEnumerable, T> hashSetConvert;

        public HashSetTypeMapper()
        {
            if (DesctinationType.IsInterface)
            {
                if (DesctinationType.GetGenericTypeDefinition() == typeof(ISet<>))
                {
                    instanceType = typeof(HashSet<>).MakeGenericType(DesctinationType.GetGenericArguments()[0]);
                    hashSetType = DesctinationType;
                }
            }
            else if (DesctinationType.IsAbstract)
            {
                throw new MapperException(string.Format("{0}不是一个有效的实例类型", DesctinationType.FullName));
            }

            if (instanceType == null)
                instanceType = DesctinationType;

            if (hashSetType == null)
                hashSetType = DesctinationType.GetInterfaces().FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(ISet<>));

            if (hashSetType == null)
            {
                throw new MapperException(string.Format("{0}未实现接口ISet<>", DesctinationType.FullName));
            }

            var param1 = Expression.Parameter(typeof(IEnumerable));
            var caller = Expression.Call(Expression.Constant(this), hashSetType.GetMethod("Convert").MakeGenericMethod(hashSetType.GetGenericArguments()), param1);
            hashSetConvert = Expression.Lambda<Func<IEnumerable, T>>(caller, param1).Compile();

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
                return hashSetConvert((IEnumerable)value);
            }
            return default(T);
        }

        private T Convert<TElementType>(IEnumerable source)
        {
            var dest = (ISet<TElementType>)Create();
            var typeMapper = NativeTypeMapper.GetTypeMapper<TElementType>();

            foreach (var element in source)
                dest.Add(typeMapper.Cast(element));

            return (T)dest;
        }
    }
}

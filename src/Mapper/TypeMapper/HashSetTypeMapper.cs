using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class HashSetTypeMapper<T> : ITypeMapper<T> where T : class
    {
        private Type newType;
        private Type hashSetType;
        private Action<T, object> hashSetAddCaller;

        public HashSetTypeMapper()
        {
            if (DesctinationType.IsInterface)
            {
                if (DesctinationType.GetGenericTypeDefinition() == typeof(ISet<>))
                {
                    newType = typeof(HashSet<>).MakeGenericType(DesctinationType.GetGenericArguments()[0]);
                    hashSetType = DesctinationType;
                }
            }

            if (!DesctinationType.IsAbstract)
                newType = typeof(T);

            if (hashSetType == null)
                hashSetType = DesctinationType.GetInterfaces().FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(ISet<>));

            if (hashSetType != null)
            {
                var param1 = Expression.Parameter(typeof(T));
                var param2 = Expression.Parameter(typeof(object));
                hashSetAddCaller = Expression.Lambda<Action<T, object>>(Expression.Call(param1, hashSetType.GetMethod("Add", new Type[] { hashSetType.GetGenericArguments()[0] }), Expression.Convert(param2, hashSetType.GetGenericArguments()[0])), param1, param2).Compile();
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
                var hashSet = Create();
                if (hashSet == null)
                    return default(T);

                var argType = hashSetType.GetGenericArguments()[0];
                var typeMapper = NativeTypeMapper.GetTypeMapper(argType);
                foreach (var obj in (IEnumerable)value)
                    hashSetAddCaller(hashSet, typeMapper.Cast(obj));

                return hashSet;
            }
            return default(T);
        }
    }
}

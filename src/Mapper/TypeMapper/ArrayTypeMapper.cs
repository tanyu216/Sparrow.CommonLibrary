using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMappers
{
    public class ArrayTypeMapper<T> : ITypeMapper<T[]>
    {
        public T[] Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(T[]); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private T[] Convert(object value)
        {
            var type = typeof(T);
            if (type.IsPrimitive || type.IsValueType)
            {
                if (value is ICollection<T>)
                {
                    var source = (ICollection<T>)value;
                    var array = new T[source.Count];
                    source.CopyTo(array, 0);
                    return array;
                }
                if (value is IEnumerable<T>)
                {
                    var list = new List<T>();
                    foreach (T obj in (IEnumerable<T>)value)
                        list.Add(obj);
                    return list.ToArray();
                }
            }
            if (value is IEnumerable)
            {
                var list = new List<T>();
                var typeMapper = NativeTypeMapper.GetTypeMapper<T>();
                foreach (object obj in (IEnumerable)value)
                    list.Add(typeMapper.Cast(obj));
                return list.ToArray();
            }
            return null;
        }
    }
}

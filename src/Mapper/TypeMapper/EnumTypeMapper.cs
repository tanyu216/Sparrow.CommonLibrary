using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMappers
{
    public class EnumTypeMapper<T> : ITypeMapper<T> where T : struct
    {
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
            if (value is T)
                return value;

            return Convert(value);
        }

        private T Convert(object value)
        {
            if (value is T)
                return (T)value;

            T output;
            if (value is string && Enum.TryParse((string)value, out output))
                return output;

            return default(T);
        }
    }

    public class Enum2TypeMapper<T> : ITypeMapper<T?> where T : struct
    {
        public T? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(T?); }
        }

        object ITypeMapper.Cast(object value)
        {
            if (value is T?)
                return value;

            return Convert(value);
        }

        private T? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is T)
                return (T)value;

            T output;
            if (value is string && Enum.TryParse((string)value, out output))
                return output;

            return default(T?);
        }
    }
}

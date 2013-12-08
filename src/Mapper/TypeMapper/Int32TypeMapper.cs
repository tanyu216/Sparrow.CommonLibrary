using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class Int32TypeMapper : ITypeMapper<Int32>
    {
        public int Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Int32); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private int Convert(object value)
        {
            if (value is int)
                return (int)value;

            int output;
            if (value is string && int.TryParse((string)value, out output))
                return output;

            return default(int);
        }
    }

    public class Int32_2TypeMapper : ITypeMapper<Int32?>
    {
        public int? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Int32?); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private int? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is int)
                return (int)value;

            int output;
            if (value is string && int.TryParse((string)value, out output))
                return output;

            return default(int?);
        }
    }
}

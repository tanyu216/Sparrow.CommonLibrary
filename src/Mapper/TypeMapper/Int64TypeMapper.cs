using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class Int64TypeMapper : ITypeMapper<Int64>
    {
        public long Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Int64); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private long Convert(object value)
        {
            if (value is long)
                return (long)value;

            long output;
            if (value is string && long.TryParse((string)value, out output))
                return output;

            return default(long);
        }
    }

    public class Int64_2TypeMapper : ITypeMapper<Int64?>
    {
        public long? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Int64?); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private long? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is long)
                return (long)value;

            long output;
            if (value is string && long.TryParse((string)value, out output))
                return output;

            return default(long?);
        }
    }
}

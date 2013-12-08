using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class UInt64TypeMapper : ITypeMapper<UInt64>
    {
        public ulong Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(UInt64); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private ulong Convert(object value)
        {
            if (value is ulong)
                return (ulong)value;

            ulong output;
            if (value is string && ulong.TryParse((string)value, out output))
                return output;

            return default(ulong);
        }
    }

    public class UInt64_2TypeMapper : ITypeMapper<UInt64?>
    {
        public ulong? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(UInt64?); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private ulong? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is ulong)
                return (ulong)value;

            ulong output;
            if (value is string && ulong.TryParse((string)value, out output))
                return output;

            return default(ulong?);
        }
    }
}

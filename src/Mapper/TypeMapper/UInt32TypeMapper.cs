using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class UInt32TypeMapper : ITypeMapper<UInt32>
    {
        public uint Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(UInt32); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private uint Convert(object value)
        {
            if (value is uint)
                return (uint)value;

            uint output;
            if (value is string && uint.TryParse((string)value, out output))
                return output;

            return default(uint);
        }
    }

    public class UInt32_2TypeMapper : ITypeMapper<UInt32?>
    {
        public uint? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(UInt32?); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private uint? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is uint)
                return (uint)value;

            uint output;
            if (value is string && uint.TryParse((string)value, out output))
                return output;

            return default(uint?);
        }
    }
}

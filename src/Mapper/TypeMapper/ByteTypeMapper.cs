using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class ByteTypeMapper : ITypeMapper<Byte>
    {
        public byte Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Byte); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private byte Convert(object value)
        {
            if (value is byte)
                return (byte)value;

            byte output;
            if (value is string && byte.TryParse((string)value, out output))
                return output;

            return default(byte);
        }
    }

    public class Byte2TypeMapper : ITypeMapper<Byte?>
    {
        public byte? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Byte?); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private byte? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is byte)
                return (byte)value;

            byte output;
            if (value is string && byte.TryParse((string)value, out output))
                return output;

            return default(byte?);
        }
    }
}

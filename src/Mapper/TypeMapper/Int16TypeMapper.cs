using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class Int16TypeMapper : ITypeMapper<Int16>
    {
        public short Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Int16); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private short Convert(object value)
        {
            if (value is short)
                return (short)value;

            short output;
            if (value is string && short.TryParse((string)value, out output))
                return output;

            return default(short);
        }
    }

    public class Int16_2TypeMapper : ITypeMapper<Int16?>
    {
        public short? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Int16?); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private short? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is short)
                return (short)value;

            short output;
            if (value is string && short.TryParse((string)value, out output))
                return output;

            return default(short?);
        }
    }
}

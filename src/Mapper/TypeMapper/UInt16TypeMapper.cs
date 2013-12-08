using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class UInt16TypeMapper : ITypeMapper<UInt16>
    {
        public ushort Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(UInt16); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private ushort Convert(object value)
        {
            if (value is ushort)
                return (ushort)value;

            ushort output;
            if (value is string && ushort.TryParse((string)value, out output))
                return output;

            return default(ushort);
        }
    }

    public class UInt16_2TypeMapper : ITypeMapper<UInt16?>
    {
        public ushort? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(UInt16?); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private ushort? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is ushort)
                return (ushort)value;

            ushort output;
            if (value is string && ushort.TryParse((string)value, out output))
                return output;

            return default(ushort?);
        }
    }
}

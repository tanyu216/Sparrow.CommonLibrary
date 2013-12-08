using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class SByteTypeMapper : ITypeMapper<SByte>
    {
        public sbyte Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(SByte); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private sbyte Convert(object value)
        {
            if (value is sbyte)
                return (sbyte)value;

            sbyte output;
            if (value is string && sbyte.TryParse((string)value, out output))
                return output;

            return default(sbyte);
        }
    }

    public class SByte2TypeMapper : ITypeMapper<SByte?>
    {
        public sbyte? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(SByte?); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private sbyte? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is sbyte)
                return (sbyte)value;

            sbyte output;
            if (value is string && sbyte.TryParse((string)value, out output))
                return output;

            return default(sbyte?);
        }
    }
}

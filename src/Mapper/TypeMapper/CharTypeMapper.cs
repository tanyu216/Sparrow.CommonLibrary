using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class CharTypeMapper : ITypeMapper<Char>
    {
        public char Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Char); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private char Convert(object value)
        {
            if (value is char)
                return (char)value;

            char output;
            if (value is string && char.TryParse((string)value, out output))
                return output;

            return default(char);
        }
    }

    public class Char2TypeMapper : ITypeMapper<Char?>
    {
        public char? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Char?); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private char? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is char)
                return (char)value;

            char output;
            if (value is string && char.TryParse((string)value, out output))
                return output;

            return default(char?);
        }
    }
}

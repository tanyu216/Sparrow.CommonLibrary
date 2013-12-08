using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class BooleanTypeMapper : ITypeMapper<Boolean>
    {
        public bool Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Boolean); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private bool Convert(object value)
        {
            if (value is bool)
                return (bool)value;

            bool output;
            if (value is string && bool.TryParse((string)value, out output))
                return output;

            return false;
        }
    }

    public class Boolean2TypeMapper : ITypeMapper<Boolean?>
    {
        public bool? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Boolean?); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private bool? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is bool)
                return (bool)value;

            bool output;
            if (value is string && bool.TryParse((string)value, out output))
                return output;

            return default(bool?);
        }
    }
}

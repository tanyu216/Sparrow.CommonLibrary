using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class SingleTypeMapper : ITypeMapper<Single>
    {
        public float Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Single); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private float Convert(object value)
        {
            if (value is float)
                return (float)value;

            float output;
            if (value is string && float.TryParse((string)value, out output))
                return output;

            return default(float);
        }
    }

    public class Single2TypeMapper : ITypeMapper<Single?>
    {
        public float? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Single?); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private float? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is float)
                return (float)value;

            float output;
            if (value is string && float.TryParse((string)value, out output))
                return output;

            return default(float?);
        }
    }
}

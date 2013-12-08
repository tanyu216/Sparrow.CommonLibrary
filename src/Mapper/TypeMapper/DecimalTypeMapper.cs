using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class DecimalTypeMapper : ITypeMapper<Decimal>
    {
        public decimal Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Decimal); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private decimal Convert(object value)
        {
            if (value is decimal)
                return (decimal)value;

            decimal output;
            if (value is string && decimal.TryParse((string)value, out output))
                return output;

            return default(decimal);
        }
    }

    public class Decimal2TypeMapper : ITypeMapper<Decimal?>
    {
        public decimal? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Decimal?); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private decimal? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is decimal)
                return (decimal)value;

            decimal output;
            if (value is string && decimal.TryParse((string)value, out output))
                return output;

            return default(decimal?);
        }
    }
}

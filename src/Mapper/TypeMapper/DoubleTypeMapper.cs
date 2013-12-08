using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class DoubleTypeMapper : ITypeMapper<Double>
    {
        public double Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Double); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private double Convert(object value)
        {
            if (value is double)
                return (double)value;

            double output;
            if (value is string && double.TryParse((string)value, out output))
                return output;

            return default(double);
        }
    }

    public class Double2TypeMapper : ITypeMapper<Double?>
    {
        public double? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Double?); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private double? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is double)
                return (double)value;

            double output;
            if (value is string && double.TryParse((string)value, out output))
                return output;

            return default(double?);
        }
    }
}

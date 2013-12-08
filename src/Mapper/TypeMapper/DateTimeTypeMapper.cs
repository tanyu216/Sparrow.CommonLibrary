using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class DateTimeTypeMapper : ITypeMapper<DateTime>
    {
        public DateTime Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(DateTime); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private DateTime Convert(object value)
        {
            if (value is DateTime)
                return (DateTime)value;

            DateTime output;
            if (value is string && DateTime.TryParse((string)value, out output))
                return output;

            return default(DateTime);
        }
    }

    public class DateTime2TypeMapper : ITypeMapper<DateTime?>
    {
        public DateTime? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(DateTime?); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private DateTime? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is DateTime)
                return (DateTime)value;

            DateTime output;
            if (value is string && DateTime.TryParse((string)value, out output))
                return output;

            return default(DateTime?);
        }
    }
}

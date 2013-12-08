using Sparrow.CommonLibrary.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class TimestampTypeMapper : ITypeMapper<Timestamp>
    {
        public Timestamp Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Timestamp); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private Timestamp Convert(object value)
        {
            if (value is Timestamp)
                return (Timestamp)value;

            Timestamp output;
            if (value is string && Timestamp.TryParse((string)value, out output))
                return output;

            return default(Timestamp);
        }
    }

    public class Timestamp2TypeMapper : ITypeMapper<Timestamp?>
    {
        public Timestamp? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Timestamp?); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private Timestamp? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is Timestamp)
                return (Timestamp)value;

            Timestamp output;
            if (value is string && Timestamp.TryParse((string)value, out output))
                return output;

            return default(Timestamp?);
        }
    }
}

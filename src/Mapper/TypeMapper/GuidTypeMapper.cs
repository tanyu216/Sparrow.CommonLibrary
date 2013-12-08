using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class GuidTypeMapper : ITypeMapper<Guid>
    {
        public Guid Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Guid); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private Guid Convert(object value)
        {
            if (value is Guid)
                return (Guid)value;

            Guid output;
            if (value is string && Guid.TryParse((string)value, out output))
                return output;

            return default(Guid);
        }
    }

    public class Guid2TypeMapper : ITypeMapper<Guid?>
    {
        public Guid? Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Guid?); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private Guid? Convert(object value)
        {
            if (value == null)
                return null;

            if (value is Guid)
                return (Guid)value;

            Guid output;
            if (value is string && Guid.TryParse((string)value, out output))
                return output;

            return default(Guid?);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class ObjectTypeMapper<T> : ITypeMapper<T> where T : class
    {

        public T Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(T); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private T Convert(object value)
        {
            return null;
        }
    }
}

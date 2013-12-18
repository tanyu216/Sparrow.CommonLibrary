using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMappers
{
    public class ValueTypeTypeMapper<T> : ITypeMapper<T>
    {
        
        public ValueTypeTypeMapper()
        {

        }

        public T Cast(object value)
        {
            if (value is T)
                return (T)value;

            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(T); }
        }

        object ITypeMapper.Cast(object value)
        {
            if (value is T)
                return value;//值类型，避免反复拆箱和装箱带来的性能损耗。

            return Convert(value);
        }

        protected virtual T Convert(object value)
        {
            if (value == null || value == DBNull.Value)
                return default(T);

            return (T)System.Convert.ChangeType(value, typeof(T));
        }
    }

}

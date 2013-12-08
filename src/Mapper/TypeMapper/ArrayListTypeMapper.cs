using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class ArrayListTypeMapper : ITypeMapper<ArrayList>
    {
        public ArrayList Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(ArrayList); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private ArrayList Convert(object value)
        {
            if (value is IEnumerable)
            {
                ArrayList list;
                if (value is ICollection)
                {
                    var source = (ICollection)value;
                    list = new ArrayList(source.Count);
                }
                else
                {
                    list = new ArrayList();
                }

                foreach (var obj in (IEnumerable)value)
                {
                    if (obj == null)
                        list.Add(null);
                    else
                    {
                        var type = obj.GetType();
                        if (type.IsPrimitive || type.IsValueType)
                            list.Add(obj);
                        else
                            list.Add(NativeTypeMapper.GetTypeMapper(obj.GetType()).Cast(obj));
                    }
                }
                return list;
            }
            return null;
        }

    }
}

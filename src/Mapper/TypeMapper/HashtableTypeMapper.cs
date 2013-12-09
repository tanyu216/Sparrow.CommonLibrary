using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class HashtableTypeMapper : ITypeMapper<Hashtable>
    {
        public Hashtable Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(Hashtable); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private Hashtable Convert(object value)
        {
            if (value == null || !(value is IEnumerable))
                return null;

            Hashtable hash = new Hashtable();

            if (value is IDictionary)
            {
                foreach (DictionaryEntry keyVal in (IDictionary)value)
                {
                    var type = keyVal.Key.GetType();
                    if (keyVal.Value == null)
                        hash.Add(NativeTypeMapper.GetTypeMapper(type).Cast(keyVal.Key), null);
                    else
                    {
                        var type2 = keyVal.Value.GetType();
                        hash.Add(NativeTypeMapper.GetTypeMapper(type).Cast(keyVal.Key), NativeTypeMapper.GetTypeMapper(type2).Cast(keyVal.Value));
                    }
                }
                return hash;
            }

            var sourceInterfaces = value.GetType().GetInterfaces();
            if (sourceInterfaces.Any(x => x.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
            {

            }

            return null;
        }
    }
}

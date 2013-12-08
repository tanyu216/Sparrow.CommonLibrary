using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class HashtableTypeMapper : ITypeMapper<Hashtable>
    {
        public System.Collections.Hashtable Cast(object value)
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
            if (value is IDictionary)
            {
                Hashtable hash = new Hashtable();

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
            return null;
        }
    }
}

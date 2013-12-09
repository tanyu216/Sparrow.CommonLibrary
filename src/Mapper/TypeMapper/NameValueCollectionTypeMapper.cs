using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public class NameValueCollectionTypeMapper : ITypeMapper<NameValueCollection>
    {
        public NameValueCollection Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(NameValueCollection); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private NameValueCollection Convert(object value)
        {
            if (value is NameValueCollection)
            {
                var collection = new NameValueCollection();
                var source = (NameValueCollection)value;
                foreach (string key in source.AllKeys)
                {
                    collection.Add(key, source[key]);
                }
                return collection;
            }

            if (value is IDictionary)
            {
                var collection = new NameValueCollection();
                var source = (IDictionary)value;
                var typeMapper = NativeTypeMapper.GetTypeMapper<string>();
                foreach (DictionaryEntry keyVal in source)
                {
                    collection.Add(typeMapper.Cast(keyVal.Key), typeMapper.Cast(keyVal.Value));
                }
                return collection;
            }

            return null;
        }
    }
}

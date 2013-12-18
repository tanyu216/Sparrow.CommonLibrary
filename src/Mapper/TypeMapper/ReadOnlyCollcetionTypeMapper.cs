using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMappers
{
    public class ReadOnlyCollcetionTypeMapper<TElementType> : ITypeMapper<ReadOnlyCollection<TElementType>>
    {
        public ReadOnlyCollection<TElementType> Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(ReadOnlyCollection<TElementType>); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        private ReadOnlyCollection<TElementType> Convert(object value)
        {
            if (value is IEnumerable)
            {
                var collection = new Collection<TElementType>();
                var typeMapper = NativeTypeMapper.GetTypeMapper<TElementType>();
                foreach (var obj in (IEnumerable)value)
                    collection.Add(typeMapper.Cast(obj));
                return new ReadOnlyCollection<TElementType>(collection);
            }
            return null;
        }
    }
}

using Sparrow.CommonLibrary.Mapper.TypeMappers;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper
{
    public static class NativeTypeMapper
    {
        private static readonly ITypeMapper[] _initialMappers = new ITypeMapper[] 
        { 
            new ArrayListTypeMapper(),
            new StringTypeMapper(),
            new HashtableTypeMapper(),
            new NameValueCollectionTypeMapper(),
            new TimestampTypeMapper(),
            new Timestamp2TypeMapper(),
            new ValueTypeTypeMapper<Boolean>(),
            new ValueTypeTypeMapper<Boolean?>(),
            new ValueTypeTypeMapper<Byte>(),
            new ValueTypeTypeMapper<Byte?>(),
            new ValueTypeTypeMapper<Char>(),
            new ValueTypeTypeMapper<Char?>(),
            new ValueTypeTypeMapper<DateTime>(),
            new ValueTypeTypeMapper<DateTime?>(),
            new ValueTypeTypeMapper<Decimal>(),
            new ValueTypeTypeMapper<Decimal?>(),
            new ValueTypeTypeMapper<Double>(),
            new ValueTypeTypeMapper<Double?>(),
            new ValueTypeTypeMapper<Guid>(),
            new ValueTypeTypeMapper<Guid?>(),
            new ValueTypeTypeMapper<Int16>(),
            new ValueTypeTypeMapper<Int16?>(),
            new ValueTypeTypeMapper<Int32>(),
            new ValueTypeTypeMapper<Int32>(),
            new ValueTypeTypeMapper<Int64>(),
            new ValueTypeTypeMapper<Int64?>(),
            new ValueTypeTypeMapper<SByte>(),
            new ValueTypeTypeMapper<SByte?>(),
            new ValueTypeTypeMapper<Single>(),
            new ValueTypeTypeMapper<Single?>(),
            new ValueTypeTypeMapper<UInt16>(),
            new ValueTypeTypeMapper<UInt16?>(),
            new ValueTypeTypeMapper<UInt32>(),
            new ValueTypeTypeMapper<UInt32?>(),
            new ValueTypeTypeMapper<UInt64>(),
            new ValueTypeTypeMapper<UInt64?>()
        };

        private static readonly ConcurrentDictionary<Type, ITypeMapper> _typeMappers;

        static NativeTypeMapper()
        {
            _typeMappers = new ConcurrentDictionary<Type, ITypeMapper>();
            Reset();
        }

        public static void Register<T>(ITypeMapper<T> typeMapper)
        {
            if (typeMapper == null)
                throw new ArgumentNullException("typeMapper");

            _typeMappers.AddOrUpdate(typeof(T), typeMapper, (x, y) => typeMapper);
            if (typeMapper is ObjectTypeMapper<T>)
                _typeMappers.AddOrUpdate(((ObjectTypeMapper<T>)typeMapper).ObjAccessor.InstanceType, typeMapper, (x, y) => typeMapper);
        }

        public static void Reset()
        {
            lock (_typeMappers)
            {
                _typeMappers.Clear();
                foreach (var typeMapper in _initialMappers)
                    _typeMappers.TryAdd(typeMapper.DesctinationType, typeMapper);

                _typeMappers.TryAdd(typeof(NameObjectCollectionBase), new NameValueCollectionTypeMapper());
            }
        }

        public static ITypeMapper GetTypeMapper(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            ITypeMapper typeMapper;
            if (_typeMappers.TryGetValue(type, out typeMapper))
                return typeMapper;

            if (type.IsEnum)
            {
                var enumTypeMapperType = typeof(EnumTypeMapper<>).MakeGenericType(type);
                return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(enumTypeMapperType));
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && type.GetGenericArguments()[0].IsEnum)
            {
                var enumTypeMapperType = typeof(Enum2TypeMapper<>).MakeGenericType(type);
                return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(enumTypeMapperType));
            }

            if (type.IsArray)
            {
                var arrayTypeMapperType = typeof(ArrayTypeMapper<>).MakeGenericType(type);
                return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(arrayTypeMapperType));
            }

            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(Dictionary<,>) || genericType == typeof(IDictionary<,>))
                {
                    var readOnlyTypeMapperType = typeof(DictionaryTypeMapper<,>).MakeGenericType(type.GetGenericArguments());
                    return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(readOnlyTypeMapperType));
                }

                if (genericType == typeof(ReadOnlyCollection<>))
                {
                    var readOnlyTypeMapperType = typeof(ReadOnlyCollcetionTypeMapper<>).MakeGenericType(type.GetGenericArguments());
                    return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(readOnlyTypeMapperType));
                }
            }

            // 接口转换
            var interfaces = type.GetInterfaces();

            if (interfaces.Any(x => x == typeof(IDictionary) || (x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>))))
            {
                var dictionaryTypeMapperType = typeof(CommonDictionaryTypeMapper<>).MakeGenericType(type);
                return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(dictionaryTypeMapperType));
            }

            if (interfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISet<>)))
            {
                var hashSetTypeMapperType = typeof(HashSetTypeMapper<>).MakeGenericType(type);
                return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(hashSetTypeMapperType));
            }

            if (interfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                var collectionTypeMapperType = typeof(CollectionTypeMaper<>).MakeGenericType(type);
                return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(collectionTypeMapperType));
            }

            if (interfaces.Any(x => x == typeof(IEnumerable)))
            {
                var collectionTypeMapperType = typeof(CollectionTypeMaper<>).MakeGenericType(type);
                return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(collectionTypeMapperType));
            }

            if (type.IsClass)
            {
                var accessor = Map.GetAccessor(type);
                if (accessor != null)
                {
                    var objectTypeMapper = typeof(ObjectTypeMapper<>).MakeGenericType(type);
                    typeMapper = _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(objectTypeMapper, accessor));
                    _typeMappers.GetOrAdd(accessor.InstanceType, typeMapper);
                    return typeMapper;
                }
            }

            return null;
        }

        public static ITypeMapper<T> GetTypeMapper<T>()
        {
            return (ITypeMapper<T>)GetTypeMapper(typeof(T));
        }
    }
}

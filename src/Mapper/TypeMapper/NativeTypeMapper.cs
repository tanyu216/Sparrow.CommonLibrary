using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public static class NativeTypeMapper
    {
        private static readonly ITypeMapper[] _initialMappers = new ITypeMapper[] 
        { 
            new ArrayListTypeMapper(),
            new BooleanTypeMapper(),
            new Boolean2TypeMapper(),
            new ByteTypeMapper(),
            new Byte2TypeMapper(),
            new CharTypeMapper(),
            new Char2TypeMapper(),
            new DateTimeTypeMapper(),
            new DateTime2TypeMapper(),
            new DecimalTypeMapper(),
            new Decimal2TypeMapper(),
            new DoubleTypeMapper(),
            new Double2TypeMapper(),
            new GuidTypeMapper(),
            new Guid2TypeMapper(),
            new Int16TypeMapper(),
            new Int16_2TypeMapper(),
            new Int32TypeMapper(),
            new Int32_2TypeMapper(),
            new Int64TypeMapper(),
            new Int64_2TypeMapper(),
            new SByteTypeMapper(),
            new SByte2TypeMapper(),
            new SingleTypeMapper(),
            new Single2TypeMapper(),
            new StringTypeMapper(),
            new TimestampTypeMapper(),
            new Timestamp2TypeMapper(),
            new UInt16TypeMapper(),
            new UInt16_2TypeMapper(),
            new UInt32TypeMapper(),
            new UInt32_2TypeMapper(),
            new UInt64TypeMapper(),
            new UInt64_2TypeMapper(),
            new HashtableTypeMapper(),
            new NameValueCollectionTypeMapper()
        };

        private static readonly ConcurrentDictionary<Type, ITypeMapper> _typeMappers;

        static NativeTypeMapper()
        {
            _typeMappers = new ConcurrentDictionary<Type, ITypeMapper>();
            Reset();
        }

        public static void Register<T>(ITypeMapper<T> typeMapper)
        {
            _typeMappers.AddOrUpdate(typeof(T), typeMapper, (x, y) => typeMapper);
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
            ITypeMapper typeMapper;
            if (_typeMappers.TryGetValue(type, out typeMapper))
                return typeMapper;

            if (type.IsEnum)
            {
                var enumTypeMapperType = typeof(EnumTypeMapper<>).MakeGenericType(type);
                return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(enumTypeMapperType));
            }

            if (type.GetGenericTypeDefinition() == typeof(Nullable<>) && type.GetGenericArguments()[0].IsEnum)
            {
                var enumTypeMapperType = typeof(Enum2TypeMapper<>).MakeGenericType(type);
                return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(enumTypeMapperType));
            }

            if (type.IsArray)
            {
                var arrayTypeMapperType = typeof(ArrayTypeMapper<>).MakeGenericType(type);
                return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(arrayTypeMapperType));
            }

            if (type.GetGenericTypeDefinition() == typeof(ReadOnlyCollection<>))
            {
                var readOnlyTypeMapperType = typeof(ReadOnlyCollcetionTypeMapper<>).MakeGenericType(type.GetGenericArguments()[0]);
                return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(readOnlyTypeMapperType));
            }

            // 接口转换
            var interfaces = type.GetInterfaces();

            if (interfaces.Any(x => x.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
            {
                var dictionaryTypeMapperType = typeof(DictionaryTypeMapper<>).MakeGenericType(type);
                return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(dictionaryTypeMapperType));
            }

            if (interfaces.Any(x => x.GetGenericTypeDefinition() == typeof(ISet<>)))
            {
                var hashSetTypeMapperType = typeof(HashSetTypeMapper<>).MakeGenericType(type);
                return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(hashSetTypeMapperType));
            }

            if (interfaces.Any(x => x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                var collectionTypeMapperType = typeof(CollectionTypeMaper<>).MakeGenericType(type);
                return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(collectionTypeMapperType));
            }

            if (interfaces.Any(x => x == typeof(IDictionary)))
            {
                var dictionaryTypeMapperType = typeof(DictionaryTypeMapper<>).MakeGenericType(type);
                return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(dictionaryTypeMapperType));
            }

            if (interfaces.Any(x => x == typeof(IEnumerable)))
            {
                var collectionTypeMapperType = typeof(CollectionTypeMaper<>).MakeGenericType(type);
                return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(collectionTypeMapperType));
            }

            // 收尾
            var objectTypeMapper = typeof(ObjectTypeMapper<>).MakeGenericType(type);
            return _typeMappers.GetOrAdd(type, x => (ITypeMapper)Activator.CreateInstance(objectTypeMapper));
        }

        public static ITypeMapper<T> GetTypeMapper<T>()
        {
            return (ITypeMapper<T>)GetTypeMapper(typeof(T));
        }
    }
}

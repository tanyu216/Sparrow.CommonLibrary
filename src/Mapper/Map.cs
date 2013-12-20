using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Sparrow.CommonLibrary.Mapper.Metadata;
using Sparrow.CommonLibrary.Database;
using System.Collections.Generic;
using Sparrow.CommonLibrary.Mapper.TypeMappers;
using Sparrow.CommonLibrary.Mapper.DataSource;

namespace Sparrow.CommonLibrary.Mapper
{
    /// <summary>
    /// 
    /// </summary>
    public static class Map
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static IObjectAccessor GetAccessor(Type entityType)
        {
            return (IObjectAccessor)typeof(Map).GetMethod("GetAccessor", new Type[0]).MakeGenericMethod(entityType).Invoke(null, new object[] { });
        }

        /// <summary>
        /// 获取IMapper&lt;<typeparamref name="T"/>&gt;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IObjectAccessor<T> GetAccessor<T>()
        {
            var typeMapper = NativeTypeMapper.GetTypeMapper<T>() as ObjectTypeMapper<T>;
            if (typeMapper != null)
                return typeMapper.ObjAccessor;

            return default(IObjectAccessor<T>);
        }

        /// <summary>
        /// 创建类型<paramref name="T"/>的实例。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Create<T>()
        {
            return GetAccessor<T>().Create();
        }

        /// <summary>
        /// 复制<paramref name="source"/>的副本。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(T source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var mapper = GetAccessor<T>();
            var dest = mapper.Create();

            if (dest is IMappingTrigger)
                ((IMappingTrigger)dest).Begin();

            for (var i = mapper.MetaInfo.PropertyCount - 1; i > -1; i--)
            {
                var property = mapper[i];
                property.SetValue(dest, property.GetValue(source));
            }

            if (dest is IMappingTrigger)
                ((IMappingTrigger)dest).End();

            return dest;
        }

        /// <summary>
        /// 映射数据源中的一行数据到目标对象
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDestination Single<TDestination>(object source)
        {
            var accessor = GetAccessor<TDestination>();
            if (accessor == null)
                throw new MapperException(string.Format("未找到适用{0}的对象访问器。", typeof(TDestination).FullName));

            var dataSourceReader = NativeDataSourceReader.GetReader(source);
            var fields = accessor.MetaInfo.GetPropertyNames();
            dataSourceReader.Ordinal(fields);

            var initData = dataSourceReader.Read();
            if (initData != null)
                return NativeTypeMapper.GetTypeMapper<TDestination>().Cast(initData);

            return default(TDestination);
        }

        /// <summary>
        /// 映射数据源中的数据到目标对象
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<TDestination> List<TDestination>(object source)
        {
            var accessor = GetAccessor<TDestination>();
            if (accessor == null)
                throw new MapperException(string.Format("未找到适用{0}的对象访问器。", typeof(TDestination).FullName));

            var typeMapper = NativeTypeMapper.GetTypeMapper<TDestination>();

            var dataSourceReader = NativeDataSourceReader.GetReader(source);
            var fields = accessor.MetaInfo.GetPropertyNames();
            dataSourceReader.Ordinal(fields);

            var list = new List<TDestination>(dataSourceReader.Count > 0 ? dataSourceReader.Count : 8);

            object[] initData;
            while (null != (initData = dataSourceReader.Read()))
            {
                TDestination destObj = typeMapper.Cast(initData);
                if (destObj != null)
                    list.Add(destObj);
            }
            return list;
        }

    }
}


﻿using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Sparrow.CommonLibrary.Data.Mapper.Metadata;
using Sparrow.CommonLibrary.Data.Database;
using System.Collections.Generic;

namespace Sparrow.CommonLibrary.Data.Mapper
{
    /// <summary>
    /// 
    /// </summary>
    public static class MapperManager
    {
        private static readonly ConcurrentDictionary<Type, IMapper> Container =
            new ConcurrentDictionary<Type, IMapper>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <typeparam name="T"></typeparam>
        public static bool Register<T>(IMapper<T> mapper)
        {
            if (mapper == null)
                throw new ArgumentNullException("mapper");

            int i = 0;
            while (i++ < 3)
            {
                if (Container.TryAdd(typeof(T), mapper))
                    return true;
                if (Container.ContainsKey(typeof(T)))
                    return false;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static IMapper GetIMapper(Type entityType)
        {
            IMapper output;
            if (Container.TryGetValue(entityType, out output))
                return output;

            var mapper = typeof(MapperManager).GetMethod("GetIMapper", new Type[0]).MakeGenericMethod(entityType).Invoke(null, new object[] { });
            if (mapper != null)
                return (IMapper)mapper;
            //
            throw new MapperException(string.Format("无法获取到实体对象的映射信息，请确认[{0}]的架构信息和实体的约定。", entityType.FullName));
        }

        /// <summary>
        /// 获取IMapper&lt;<typeparamref name="T"/>&gt;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IMapper<T> GetIMapper<T>()
        {
            IMapper output;
            int i = 0;
        begin:
            if (Container.TryGetValue(typeof(T), out output))
                return (IMapper<T>)output;

            var mapper = MapperFinder.GetIMapper<T>();
            if (mapper != null)
            {
                if (Register<T>(mapper))
                    return mapper;

                i++;
                if (i < 3)
                    goto begin;
            }

            //
            throw new MapperException(string.Format("无法获取到实体对象的映射信息，请确认[{0}]的架构信息和实体的约定。", typeof(T).FullName));
        }

        /// <summary>
        /// 创建类型<paramref name="T"/>的实例。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Create<T>()
        {
            return GetIMapper<T>().Create();
        }

        /// <summary>
        /// 映射数据源中的一行数据到目标对象
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDestination MapSingle<TDestination, TSource>(TSource source)
        {
            return Converter.DataConverter.CreateConverter<TDestination>(GetIMapper<TDestination>(), source).Next();
        }

        /// <summary>
        /// 映射数据源中的数据到目标对象
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<TDestination> Map<TDestination, TSource>(TSource source)
        {
            return Converter.DataConverter.CreateConverter<TDestination>(GetIMapper<TDestination>(), source).Cast();
        }
    }
}


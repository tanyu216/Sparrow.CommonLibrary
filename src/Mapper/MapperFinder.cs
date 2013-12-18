using System;
using System.Collections.Generic;
using System.Reflection;
using Sparrow.CommonLibrary.Mapper.Metadata;
using System.Linq.Expressions;

namespace Sparrow.CommonLibrary.Mapper
{
    /// <summary>
    /// <see cref="IObjectAccessor"/>映射对象搜索器
    /// </summary>
    public abstract class MapperFinder
    {
        private static readonly List<MapperFinder> Finders;

        static MapperFinder()
        {
            Finders = new List<MapperFinder>
                          {
                              new MappingFinderForClass(),
                              new MappingFinderForNamespace(),
                              new MappingFinderAuto()
                          };
        }

        /// <summary>
        /// 新增一个映射对象搜索器
        /// </summary>
        /// <param name="finder"></param>
        public static void AddFinder(MapperFinder finder)
        {
            if (finder == null) 
                throw new ArgumentNullException("finder");

            lock (Finders)
            {
                Finders.Insert(Finders.Count - 1, finder);
            }
        }

        /// <summary>
        /// 重置默认的映射对象搜索器
        /// </summary>
        /// <param name="finder"></param>
        public static void ResetLastFinder(MapperFinder finder)
        {
            if (finder == null)
                throw new ArgumentNullException("finder");

            lock (Finders)
            {
                Finders[Finders.Count - 1] = finder;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IObjectAccessor<T> GetIMapper<T>()
        {
            MapperFinder[] finders = null;
            lock (Finders)
            {
                finders = Finders.ToArray();
            }

            Exception lastException = null;
            foreach (var mapperFinder in finders)
            {
                try
                {
                    var mapper = mapperFinder.FindIMapper<T>();
                    if (mapper != null)
                        return mapper;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }
            }
            if (lastException != null) throw lastException;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="MapperException"></exception>
        public abstract IObjectAccessor<T> FindIMapper<T>();

        private class MappingFinderForNamespace : MapperFinder
        {
            public override IObjectAccessor<T> FindIMapper<T>()
            {
                var type = Type.GetType(typeof(T).Namespace + ".Mappers." + typeof(T).Name + "MapperProvider");
                if (type != null)
                {
                    var mapper = (IObjectAccessor<T>)type.GetMethod("GetIMapper").Invoke(null, new object[] { });
                    return mapper;
                }
                return null;
            }
        }

        private class MappingFinderForClass : MapperFinder
        {
            public override IObjectAccessor<T> FindIMapper<T>()
            {
                var method = typeof(T).GetMethod("GetIMapper");
                if (method == null)
                    return null;
                var mapper = (IObjectAccessor<T>)method.Invoke(null, new object[] { });
                return mapper;
            }
        }

        private class MappingFinderAuto : MapperFinder
        {
            public override IObjectAccessor<T> FindIMapper<T>()
            {
                var mapper = new ObjectAccessor<T>();
                mapper.AutoAppendProperty();
                if (mapper.MetaInfo.PropertyCount == 0)
                    return null;
                //
                return mapper.Complete();
            }
        }
    }
}

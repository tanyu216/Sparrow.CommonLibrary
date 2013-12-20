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
    public abstract class ObjectAccessorFinder
    {
        private static readonly List<ObjectAccessorFinder> Finders;

        static ObjectAccessorFinder()
        {
            Finders = new List<ObjectAccessorFinder>
                          {
                              new ObjectAccessorFinderForClass(),
                              new ObjectAccessorFinderForNamespace(),
                              new ObjectAccessorFinderAuto()
                          };
        }

        /// <summary>
        /// 新增一个映射对象搜索器
        /// </summary>
        /// <param name="finder"></param>
        public static void AddFinder(ObjectAccessorFinder finder)
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
        public static void ResetLastFinder(ObjectAccessorFinder finder)
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
        public static IObjectAccessor<T> FindObjAccessor<T>()
        {
            ObjectAccessorFinder[] finders = null;
            lock (Finders)
            {
                finders = Finders.ToArray();
            }

            Exception lastException = null;
            foreach (var mapperFinder in finders)
            {
                try
                {
                    var mapper = mapperFinder.FindAccessor<T>();
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
        public abstract IObjectAccessor<T> FindAccessor<T>();

        private class ObjectAccessorFinderForNamespace : ObjectAccessorFinder
        {
            public override IObjectAccessor<T> FindAccessor<T>()
            {
                var type = Type.GetType(typeof(T).Namespace + ".Accessors." + typeof(T).Name + "ObjectAccessorProvider");
                if (type != null)
                {
                    var mapper = (IObjectAccessor<T>)type.GetMethod("GetObjectAccessor").Invoke(null, new object[] { });
                    return mapper;
                }
                return null;
            }
        }

        private class ObjectAccessorFinderForClass : ObjectAccessorFinder
        {
            public override IObjectAccessor<T> FindAccessor<T>()
            {
                var method = typeof(T).GetMethod("GetObjectAccessor");
                if (method == null)
                    return null;
                var mapper = (IObjectAccessor<T>)method.Invoke(null, new object[] { });
                return mapper;
            }
        }

        private class ObjectAccessorFinderAuto : ObjectAccessorFinder
        {
            public override IObjectAccessor<T> FindAccessor<T>()
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

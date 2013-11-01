using Sparrow.CommonLibrary.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Entity
{
    /// <summary>
    /// 实体对象复制
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class EntityClone<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone(T source)
        {
            var mapper = MapperManager.GetIMapper<T>();
            var target = mapper.Create();
            for (var i = 0; i < mapper.MetaInfo.FieldCount; i++)
            {
                var property = mapper[i];
                property.SetValue(target, property.GetValue(source));
            }
            return target;
        }
    }
}

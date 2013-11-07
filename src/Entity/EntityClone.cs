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
            var mapper = Map.GetIMapper<T>();
            var target = mapper.Create();
            for (var i = mapper.MetaInfo.FieldCount - 1; i > -1; i--)
            {
                var property = mapper[i];
                property.SetValue(target, property.GetValue(source));
            }
            return target;
        }
    }
}

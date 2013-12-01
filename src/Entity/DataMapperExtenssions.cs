using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Sparrow.CommonLibrary.Mapper;

namespace Sparrow.CommonLibrary.Entity
{
    public static class DataMapperExtenssions
    {
        /// <summary>
        /// 新建一个<see cref="IMapper"/>对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DataMapper<T> Create<T>()
        {
            return new DataMapper<T>(new DbMetaInfo(null, typeof(T)));
        }
        /// <summary>
        /// 新建一个<see cref="IMapper"/>对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataMapper<T> Create<T>(string tableName)
        {
            return new DataMapper<T>(new DbMetaInfo(tableName, typeof(T)));
        }
        /// <summary>
        /// 添加属性与表字段的映射
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataMapper"></param>
        /// <param name="propertyExp"></param>
        /// <param name="propertyName"></param>
        /// <param name="isKey"></param>
        /// <returns></returns>
        public static DataMapper<T> AppendProperty<T>(this DataMapper<T> dataMapper, Expression<Func<T, object>> propertyExp, string propertyName, bool isKey)
        {
            if (propertyExp == null)
                throw new ArgumentNullException("propertyExp");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            var propertyInfo = PropertyExpression.ExtractMemberExpression(propertyExp);
            dataMapper.MetaInfo.AddPropertyInfo(new DbMetaPropertyInfo(dataMapper.MetaInfo, propertyName, (PropertyInfo)propertyInfo.Member, isKey));

            return dataMapper;
        }
        /// <summary>
        /// 标识当前添加的字段为自动增长标识
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataMapper"></param>
        /// <returns></returns>
        public static DataMapper<T> Increment<T>(this DataMapper<T> dataMapper)
        {
            if (dataMapper.MetaInfo.PropertyCount == 0)
                throw new MapperException("未添加任何可以设置自动增长标识的属性。");

            var property = dataMapper.MetaInfo[dataMapper.MetaInfo.PropertyCount - 1];

            dataMapper.MetaInfo.RemovePropertyInfo(property);
            dataMapper.MetaInfo.AddPropertyInfo(new DbIncrementMetaPropertyInfo(property.MetaInfo, property.PropertyName, property.PropertyInfo, (property is DbMetaPropertyInfo) ? ((DbMetaPropertyInfo)property).IsKey : false));

            return dataMapper;
        }
        /// <summary>
        /// 标识当前添加的字段为自动增长标识
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataMapper"></param>
        /// <param name="incrementName"></param>
        /// <returns></returns>
        public static DataMapper<T> Increment<T>(this DataMapper<T> dataMapper, string incrementName)
        {
            return dataMapper.Increment<T>(incrementName, 1);
        }
        /// <summary>
        /// 标识当前添加的字段为自动增长标识
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataMapper"></param>
        /// <param name="incrementName"></param>
        /// <param name="startVal"></param>
        /// <returns></returns>
        public static DataMapper<T> Increment<T>(this DataMapper<T> dataMapper, string incrementName, int startVal)
        {
            if (dataMapper.MetaInfo.PropertyCount == 0)
                throw new MapperException("未添加任何可以设置自动增长标识的属性。");

            var property = dataMapper.MetaInfo[dataMapper.MetaInfo.PropertyCount - 1];

            dataMapper.MetaInfo.RemovePropertyInfo(property);
            dataMapper.MetaInfo.AddPropertyInfo(new DbIncrementMetaPropertyInfo(property.MetaInfo, property.PropertyName, property.PropertyInfo, ((property is DbMetaPropertyInfo) ? ((DbMetaPropertyInfo)property).IsKey : false), incrementName, startVal));

            return dataMapper;
        }
        /// <summary>
        /// 完成映射，生成一个继承于<typeparamref name="T"/>和<see cref="IEntity"/>的子类。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataMapper"></param>
        /// <returns></returns>
        public static DataMapper<T> ComplieWithEntity<T>(this DataMapper<T> dataMapper)
        {
            var subType = EntityBuilder.BuildEntityClass(typeof(T), dataMapper.MetaInfo);
            var func = Expression.Lambda<Func<T>>(Expression.New(subType)).Compile();
            return dataMapper.Complete(x => func);
        }
    }
}

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
        public static DataMapper<T> Create<T>(string tableName)
        {
            return new DataMapper<T>(new DbMetaInfo(tableName, typeof(T)));
        }

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

        public static DataMapper<T> Increment<T>(this DataMapper<T> dataMapper)
        {
            if (dataMapper.MetaInfo.PropertyCount == 0)
                throw new MapperException("未添加任何可以设置自动增长标识的属性。");

            var property = dataMapper.MetaInfo[dataMapper.MetaInfo.PropertyCount - 1];

            dataMapper.MetaInfo.RemovePropertyInfo(property);
            dataMapper.MetaInfo.AddPropertyInfo(new DbIncrementMetaPropertyInfo(property.MetaInfo, property.PropertyName, property.PropertyInfo, (property is DbMetaPropertyInfo) ? ((DbMetaPropertyInfo)property).IsKey : false));

            return dataMapper;
        }

        public static DataMapper<T> Increment<T>(this DataMapper<T> dataMapper, string incrementName)
        {
            return dataMapper.Increment<T>(incrementName, 1);
        }

        public static DataMapper<T> Increment<T>(this DataMapper<T> dataMapper, string incrementName, int startVal)
        {
            if (dataMapper.MetaInfo.PropertyCount == 0)
                throw new MapperException("未添加任何可以设置自动增长标识的属性。");

            var property = dataMapper.MetaInfo[dataMapper.MetaInfo.PropertyCount - 1];

            dataMapper.MetaInfo.RemovePropertyInfo(property);
            dataMapper.MetaInfo.AddPropertyInfo(new DbIncrementMetaPropertyInfo(property.MetaInfo, property.PropertyName, property.PropertyInfo, ((property is DbMetaPropertyInfo) ? ((DbMetaPropertyInfo)property).IsKey : false), incrementName, startVal));

            return dataMapper;
        }
    }
}

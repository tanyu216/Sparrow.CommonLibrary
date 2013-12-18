using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Mapper.DataSource;

namespace Sparrow.CommonLibrary.Mapper.TypeMappers
{
    public class ObjectTypeMapper<T> : ITypeMapper<T> where T : class
    {
        private readonly IObjectAccessor<T> objAccessor;
        private ITypeMapper[] typeMappers;
        private string[] propertyNames;
        private IPropertyAccessor<T>[] propertyAccessors;

        public ObjectTypeMapper()
        {
            objAccessor = MapperFinder.GetIMapper<T>();
            if (objAccessor == null)
                throw new MapperException(string.Format("未能查找到{0}的访问器[{1}]。", typeof(T).FullName, typeof(IObjectAccessor<T>).FullName));

        }

        public ObjectTypeMapper(IObjectAccessor<T> objAccessor)
        {
            if (objAccessor == null)
                throw new ArgumentNullException("objAccessor");

            this.objAccessor = objAccessor;
        }

        private void Init()
        {
            var properties = objAccessor.MetaInfo.GetProperties();
            typeMappers = new ITypeMapper[properties.Length];
            for (var i = 0; i < properties.Length; i++)
            {
                typeMappers[i] = NativeTypeMapper.GetTypeMapper(properties[i].PropertyInfo.PropertyType);
            }

            propertyNames = objAccessor.MetaInfo.GetPropertyNames();

            propertyAccessors = propertyNames.Select(x => objAccessor[x]).ToArray();
        }

        public T Cast(object value)
        {
            return Convert(value);
        }

        public Type DesctinationType
        {
            get { return typeof(T); }
        }

        object ITypeMapper.Cast(object value)
        {
            return Convert(value);
        }

        protected T Create(object[] initData)
        {
            for (var i = 0; i < initData.Length; i++)
            {

            }
            return default(T);
        }

        protected virtual T Convert(object value)
        {
            if (value is Array)
            {
                return Create((object[])value);
            }
            else
            {

            }
            return default(T);
        }

    }
}

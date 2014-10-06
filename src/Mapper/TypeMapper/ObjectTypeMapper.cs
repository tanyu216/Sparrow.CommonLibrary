using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Mapper.DataSource;

namespace Sparrow.CommonLibrary.Mapper.TypeMappers
{
    public class ObjectTypeMapper<T> : ITypeMapper<T>
    {
        private readonly IObjectAccessor<T> objAccessor;
        private string[] propertyNames;
        private IPropertyAccessor<T>[] propertyAccessors;

        public IObjectAccessor<T> ObjAccessor { get { return objAccessor; } }

        public ObjectTypeMapper()
        {
            objAccessor = ObjectAccessorFinder.FindObjAccessor<T>();
            if (objAccessor == null)
                throw new MapperException(string.Format("未能查找到{0}的访问器[{1}]。", typeof(T).FullName, typeof(IObjectAccessor<T>).FullName));

            Init();
        }

        public ObjectTypeMapper(IObjectAccessor<T> objAccessor)
        {
            if (objAccessor == null)
                throw new ArgumentNullException("objAccessor");

            this.objAccessor = objAccessor;
            Init();
        }

        private void Init()
        {
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
            T obj = objAccessor.Create();
            for (var i = 0; i < initData.Length; i++)
            {
                propertyAccessors[i].SetValue(obj, initData[i]);
            }
            return obj;
        }

        protected virtual T Convert(object value)
        {
            if (value == null)
                return default(T);

            if (value is Array)
            {
                return Create((object[])value);
            }
            else if (value is ICollection)
            {
                var initData = new object[((ICollection)value).Count];
                if (initData.Length < propertyAccessors.Length)
                    return default(T);
                ((ICollection)value).CopyTo(initData, 0);
                return Create(initData);
            }

            var sourceAccessor = Map.GetAccessor(value.GetType());
            if (sourceAccessor != null)
            {
                var initData = new object[propertyAccessors.Length];
                for (var i = 0; i < propertyNames.Length; i++)
                {
                    var getter = sourceAccessor[propertyNames[i]];
                    if (getter != null)
                    {
                        initData[i] = getter.GetValue(value);
                    }
                }
                return Create(initData);
            }

            return default(T);
        }

    }
}

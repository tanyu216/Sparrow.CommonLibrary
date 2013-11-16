using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.Metadata
{
    public class MetaPropertyInfo : IMetaPropertyInfo
    {
        private readonly IMetaInfo _metaInfo;
        public IMetaInfo MetaInfo
        {
            get { return _metaInfo; }
        }

        private readonly string _propertyName;
        public string PropertyName
        {
            get { return _propertyName; }
        }

        private readonly System.Reflection.PropertyInfo _propertyInfo;
        public System.Reflection.PropertyInfo PropertyInfo
        {
            get { return _propertyInfo; }
        }

        public MetaPropertyInfo(IMetaInfo metaInfo, string propertyName, System.Reflection.PropertyInfo propertyInfo)
        {
            if (metaInfo == null)
                throw new ArgumentNullException("metaInfo");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            _metaInfo = metaInfo;
            _propertyName = propertyName;
            _propertyInfo = propertyInfo;
        }
    }
}

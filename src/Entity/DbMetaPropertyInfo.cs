using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Mapper.Metadata;

namespace Sparrow.CommonLibrary.Entity
{
    public class DbMetaPropertyInfo : MetaPropertyInfo, IDbMetaPropertyInfo
    {
        public string ColumnName { get { return base.PropertyName; } }

        private readonly bool _isKey;

        public bool IsKey { get { return _isKey; } }

        public DbMetaPropertyInfo(IMetaInfo metaInfo, string columnName, System.Reflection.PropertyInfo propertyInfo)
            : base(metaInfo, columnName, propertyInfo)
        {
        }

        public DbMetaPropertyInfo(IMetaInfo metaInfo, string columnName, System.Reflection.PropertyInfo propertyInfo, bool isKey)
            : base(metaInfo, columnName, propertyInfo)
        {
            _isKey = isKey;
        }
        
    }
}

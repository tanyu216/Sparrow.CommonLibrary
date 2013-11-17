using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Mapper.Metadata;

namespace Sparrow.CommonLibrary.Entity
{
    public class DbIncrementMetaPropertyInfo : DbMetaPropertyInfo, IDbIncrementMetaPropertyInfo
    {
        private readonly string _incrementName;

        public string IncrementName { get { return _incrementName; } }

        private readonly int _startVal;

        public int StartVal { get { return _startVal; } }

        public DbIncrementMetaPropertyInfo(IMetaInfo metaInfo, string columnName, System.Reflection.PropertyInfo propertyInfo)
            : base(metaInfo, columnName, propertyInfo)
        {
        }

        public DbIncrementMetaPropertyInfo(IMetaInfo metaInfo, string columnName, System.Reflection.PropertyInfo propertyInfo, bool isKey)
            : base(metaInfo, columnName, propertyInfo, isKey)
        {
        }

        public DbIncrementMetaPropertyInfo(IMetaInfo metaInfo, string columnName, System.Reflection.PropertyInfo propertyInfo, bool isKey, string incrementName)
            : base(metaInfo, columnName, propertyInfo, isKey)
        {
            _incrementName = incrementName;
        }

        public DbIncrementMetaPropertyInfo(IMetaInfo metaInfo, string columnName, System.Reflection.PropertyInfo propertyInfo, bool isKey, string incrementName, int startVal)
            : base(metaInfo, columnName, propertyInfo, isKey)
        {
            _incrementName = incrementName;
            _startVal = startVal;
        }

    }
}

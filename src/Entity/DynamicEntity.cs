using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using Sparrow.CommonLibrary.Mapper.Metadata;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Mapper;

namespace Sparrow.CommonLibrary.Entity
{
    /// <summary>
    /// 动态生成的数据表的映射实体类.适用于编码时表结构不确定,运行时表结构为动态定义的环境.
    /// </summary>
    public sealed class DynamicEntity : DynamicObject, IEntityExplain
    {

        private readonly Dictionary<string, object> _fieldValues;
        private readonly HashSet<string> _fieldModified;

        private readonly string _tableName;
        private readonly string[] _keys;
        private readonly IDbIncrementMetaPropertyInfo _increment;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public DynamicEntity()
        {
            _fieldValues = new Dictionary<string, object>();
            _fieldModified = new HashSet<string>();
        }

        public DynamicEntity(string tableName)
            : this(tableName, null, null, null)
        {
        }

        public DynamicEntity(string tableName, string[] keys)
            : this(tableName, keys, null, null)
        {
        }

        public DynamicEntity(string tableName, string[] keys, string incrementColumn, string incrementName)
            : this()
        {
            _tableName = tableName;
            _keys = keys ?? new string[0];
            if (!string.IsNullOrEmpty(incrementColumn))
                _increment = new DbIncrementMetaPropertyInfo(incrementName, 1, incrementColumn, keys.Any(x => x == incrementColumn));
        }

        /// <summary>
        /// 获取属性成员的值
        /// </summary>
        /// <param name="field">成员属性名称</param>
        /// <returns></returns>
        private object GetFieldValue(string field)
        {
            if (string.IsNullOrEmpty(field)) throw new ArgumentNullException("fields");

            object val;
            return _fieldValues.TryGetValue(field, out val) ? val : null;
        }

        /// <summary>
        /// 设置属性成员的值
        /// </summary>
        /// <param name="field">成员属性名称</param>
        /// <param name="value">属性成员的值</param>
        private void SetFieldValue(string field, object value)
        {
            if (string.IsNullOrEmpty(field)) throw new ArgumentNullException("fields");

            if (value == DBNull.Value || value == null)
            {
                if (_isImporting == false || _fieldValues.ContainsKey(field))
                    _fieldValues[field] = null;
            }
            else
            {
                _fieldValues[field] = value;
            }
            //
            if (_isImporting == false && _fieldModified.Contains(field) == false)
                _fieldModified.Add(field);
        }

        private bool _isImporting;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        public void Import(IDataRecord record)
        {
            if (record == null)
                throw new ArgumentNullException("record");
            _isImporting = true;
            _fieldModified.Clear();
            for (int i = record.FieldCount - 1; i > -1; i--)
            {
                SetFieldValue(record.GetName(i), record[i]);
            }
            _isImporting = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        public void Import(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            _isImporting = true;
            _fieldModified.Clear();
            for (var i = row.ItemArray.Length - 1; i > -1; i--)
            {
                SetFieldValue(row.Table.Columns[i].ColumnName, row.ItemArray[i]);
            }
            _isImporting = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        public void Import(string[] fields, object[] values)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");
            if (values == null)
                throw new ArgumentNullException("values");
            if (fields.Length != values.Length)
                throw new ArgumentException("fields、values的元素数量不一致。");

            _isImporting = true;
            _fieldModified.Clear();
            for (var i = values.Length; i > -1; i--)
            {
                SetFieldValue(fields[i], values[i]);
            }
            _isImporting = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValues"></param>
        public void Import(IDictionary<string, object> keyValues)
        {
            if (keyValues == null)
                throw new ArgumentNullException("keyValues");

            _isImporting = true;
            _fieldModified.Clear();
            foreach (var item in keyValues)
                SetFieldValue(item.Key, item.Value);
            _isImporting = false;
        }

        #region DynamicObject

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _fieldValues.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetFieldValue(binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetFieldValue(binder.Name, value);
            return true;
        }

        #endregion

        #region IEntityFieldAccessor

        object IEntityFieldAccessor.this[string field]
        {
            get { return GetFieldValue(field); }
            set { SetFieldValue(field, value); }
        }

        IEnumerable<string> IEntityFieldAccessor.GetSettedFields()
        {
            return _fieldModified.ToArray();
        }

        IEnumerable<ItemValue> IEntityFieldAccessor.GetFieldValues(IEnumerable<string> fields)
        {
            if (fields == null) throw new ArgumentNullException("fields");

            return from field in fields select new ItemValue(field, ((IEntityFieldAccessor)this)[field]);
        }

        #endregion

        #region IEntityExplain

        object IEntityExplain.EntityData
        {
            get { return this; }
        }

        #endregion

        #region IDbMetaInfo

        string IDbMetaInfo.TableName
        {
            get { return _tableName; }
        }

        IDbIncrementMetaPropertyInfo IDbMetaInfo.Increment
        {
            get { return _increment; }
        }

        int IDbMetaInfo.KeyCount { get { return _keys.Length; } }

        int IDbMetaInfo.ColumnCount { get { return _fieldValues.Count; } }

        bool IDbMetaInfo.IsKey(string columnName)
        {
            return _keys.Any(x => x == columnName);
        }

        string[] IDbMetaInfo.GetKeys()
        {
            var keys = new string[_keys.Length];
            _keys.CopyTo(keys, 0);
            return keys;
        }

        string[] IDbMetaInfo.GetColumnNames()
        {
            return _fieldValues.Keys.ToArray();
        }

        #endregion

        #region IEntity

        DataState IEntity.OperationState
        {
            get;
            set;
        }

        Type IEntity.EntityType
        {
            get { return typeof(DynamicEntity); }
        }

        bool IEntity.IsSetted(int index)
        {
            if (_fieldValues.Count - 1 < index)
                return false;
            return _fieldModified.Contains(_fieldValues.Keys.Skip(index).First());
        }

        bool IEntity.AnySetted()
        {
            return _fieldModified.Count > 0;
        }

        #endregion

        #region IMappingTrigger

        public void Begin()
        {
            _isImporting = true;
        }

        public void End()
        {
            _isImporting = false;
        }

        #endregion

        private class DbIncrementMetaPropertyInfo : IDbIncrementMetaPropertyInfo
        {
            private readonly string _incrementName;
            private readonly int _startVal;
            private readonly string _columnName;
            private readonly bool _isKey;

            public DbIncrementMetaPropertyInfo(string incrementName, int startVal, string columnName, bool isKey)
            {
                _incrementName = incrementName;
                _startVal = startVal;
                _columnName = columnName;
                _isKey = isKey;
            }

            public string IncrementName
            {
                get { return _incrementName; }
            }

            public int StartVal
            {
                get { return _startVal; }
            }

            public string ColumnName
            {
                get { return _columnName; }
            }

            public bool IsKey
            {
                get { return _isKey; }
            }
        }
    }
}

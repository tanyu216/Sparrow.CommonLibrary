using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using Sparrow.CommonLibrary.Mapper.Metadata;
using Sparrow.CommonLibrary.Database;

namespace Sparrow.CommonLibrary.Entity
{
    /// <summary>
    /// 动态生成的数据表的映射实体类.适用于编码时表结构不确定,运行时表结构为动态定义的环境.
    /// </summary>
    public sealed class DynamicEntity : DynamicObject, IEntityFieldAccessor
    {
        #region IEntityFieldAccessor

        public object this[string field]
        {
            get { return GetFieldValue(field); }
            set { SetFieldValue(field, value); }
        }

        public IEnumerable<string> GetSettedFields()
        {
            return _fieldModified.ToArray();
        }

        public IEnumerable<ItemValue> GetValues(IEnumerable<string> fields)
        {
            if (fields == null) throw new ArgumentNullException("fields");

            return from field in fields select new ItemValue(field, this[field]);
        }

        #endregion

        private readonly Dictionary<string, object> _fieldValues;
        private readonly HashSet<string> _fieldModified;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public DynamicEntity()
        {
            _fieldValues = new Dictionary<string, object>();
            _fieldModified = new HashSet<string>();
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
        public DynamicEntity Import(IDataRecord record)
        {
            if (record == null)
                throw new ArgumentNullException("record");
            _isImporting = true;
            _fieldModified.Clear();
            for (int i = 0; i < record.FieldCount; i++)
            {
                SetFieldValue(record.GetName(i), record[i]);
            }
            _isImporting = false;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        public DynamicEntity Import(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            _isImporting = true;
            _fieldModified.Clear();
            for (var i = 0; i < row.ItemArray.Length; i++)
            {
                SetFieldValue(row.Table.Columns[i].ColumnName, row.ItemArray[i]);
            }
            _isImporting = false;
            return this;
        }

        public DynamicEntity Import(string[] fields, object[] values)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");
            if (values == null)
                throw new ArgumentNullException("values");
            if (fields.Length != values.Length)
                throw new ArgumentException("fields、values的元素数量不一致。");

            _isImporting = true;
            _fieldModified.Clear();
            for (var i = 0; i < values.Length; i++)
            {
                SetFieldValue(fields[i], values[i]);
            }
            _isImporting = false;
            return this;
        }

        public DynamicEntity Import(IDictionary<string, object> keyValues)
        {
            if (keyValues == null)
                throw new ArgumentNullException("keyValues");

            _isImporting = true;
            _fieldModified.Clear();
            foreach (var item in keyValues)
                SetFieldValue(item.Key, item.Value);
            _isImporting = false;
            return this;
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

    }
}

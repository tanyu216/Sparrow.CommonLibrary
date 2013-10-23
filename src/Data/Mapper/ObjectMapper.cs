using System;
using System.Linq;
using System.Linq.Expressions;
using Sparrow.CommonLibrary.Data.Mapper.Metadata;
using Sparrow.CommonLibrary.Data.Entity;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Sparrow.CommonLibrary.Data.Mapper
{

    /// <summary>
    /// 内部数据映射的实现。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectMapper<T> : IMapper<T>, IMetaInfo, IMetaInfoForDbTable
    {

        /// <summary>
        /// 默认初始化
        /// </summary>
        public ObjectMapper()
            : this(null)
        { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="name">元数据名称</param>
        public ObjectMapper(string name)
        {
            Type type = typeof(T);
            if (type.IsClass == false)
                throw new MapperException(string.Format("类型[{0}]不是一个合法的实体类型。", type.FullName));

            var ctor = type.GetConstructor(new Type[0]);
            if (ctor == null)
                throw new MapperException(string.Format("类型[{0}]无默认构造函数。", type.FullName));
            //
            _name = name;
            _fields = new ConcurrentDictionary<string, IMetaFieldInfo>();
            _extends = new ConcurrentBag<IMetaInfoExtend>();
            _fieldIndex = new Dictionary<string, int>();
            @lock = new ReaderWriterLock();
        }

        #region IMapper<T>

        private IPropertyValue<T>[] _propertyValues;
        private string[] _fieldNames;
        private Func<T> _creator;
        private readonly IDictionary<string, int> _fieldIndex;
        private ReaderWriterLock @lock;

        IPropertyValue IMapper.this[string field]
        {
            get
            {
                int i = IndexOf(field);
                return i > -1 ? _propertyValues[i] : null;
            }
        }

        IPropertyValue IMapper.this[int index]
        {
            get
            {
                if (index < 0 || _propertyValues.Length <= index)
                    return null;
                return _propertyValues[index];
            }
        }

        public IPropertyValue<T> this[string field]
        {
            get
            {
                int i = IndexOf(field);
                return i > -1 ? _propertyValues[i] : null;
            }
        }

        public IPropertyValue<T> this[int index]
        {
            get
            {
                if (index < 0 || _propertyValues.Length <= index)
                    return null;
                return _propertyValues[index];
            }
        }

        public IMetaInfo MetaInfo
        {
            get { return this; }
        }

        public Type EntityType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        /// <remarks>唯一一个不区分大小写的方法</remarks>
        public int IndexOf(string field)
        {
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException("field");
            //
            @lock.AcquireReaderLock(-1);
            try
            {
                int idx;
                if (_fieldIndex.TryGetValue(field.ToLower(), out idx))
                    return idx;
            }
            finally
            {
                @lock.ReleaseReaderLock();
            }
            return -1;
        }

        public string FieldName(int index)
        {
            if (index < 0 || _fieldNames.Length <= index)
                return null;
            return _fieldNames[index];
        }

        object IMapper.Create()
        {
            return Create();
        }

        public T Create()
        {
            return _creator();
        }

        #endregion

        #region IMetaInfo

        private readonly ConcurrentDictionary<string, IMetaFieldInfo> _fields;
        private IMetaFieldInfo[] _keys;
        private readonly ConcurrentBag<IMetaInfoExtend> _extends;

        private readonly string _name;

        string IMetaInfo.Name
        {
            get { return _name; }
        }

        private int _keyCount;

        int IMetaInfo.KeyCount
        {
            get { return _keyCount; }
        }

        int IMetaInfo.FieldCount { get { return _fields.Count; } }

        IMetaFieldInfo IMetaInfo.this[string field]
        {
            get
            {
                if (string.IsNullOrEmpty(field))
                    throw new ArgumentNullException("field");
                IMetaFieldInfo metaField;
                if (_fields.TryGetValue(field, out metaField))
                    return metaField;
                return null;
            }
        }

        bool IMetaInfo.IsKey(string field)
        {
            for (var i = 0; i < _keys.Length; i++)
                if (field == _keys[i].FieldName)
                    return _keys[i].IsKey;
            return false;
        }

        string[] IMetaInfo.GetKeys()
        {
            var keys = new string[_keys.Length];
            for (var i = 0; i < keys.Length; i++)
                keys[i] = _keys[i].FieldName;
            return keys;
        }

        string[] IMetaInfo.GetFieldNames()
        {
            var fields = new string[_fields.Count];
            _fields.Keys.CopyTo(fields, 0);
            return fields;
        }

        IMetaFieldInfo[] IMetaInfo.GetFields()
        {
            var fields = new IMetaFieldInfo[_fields.Count];
            _fields.Values.CopyTo(fields, 0);
            return fields;
        }

        IMetaInfoExtend[] IMetaInfo.GetExtends()
        {
            if (_extends.Count == 0)
                return null;
            var extends = new IMetaInfoExtend[_extends.Count];
            _extends.CopyTo(extends, 0);
            return extends;
        }

        #endregion

        #region IMetaDbTable

        private IdentityMetaFieldExtend _identity;
        public IdentityMetaFieldExtend Identity { get { return _identity; } }

        #endregion

        #region Field

        private MetaField _currentField;
        /// <summary>
        /// 添加成员字段。
        /// </summary>
        /// <param name="propertyExp"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public ObjectMapper<T> AppendField(Expression<Func<T, object>> propertyExp, string field)
        {
            TestReadonly();
            _currentField = new MetaField(this, field, propertyExp);
            if (!_fields.TryAdd(field, _currentField))
            {
                throw new MapperException("添加成员字段失败。");
            }
            return this;
        }

        /// <summary>
        /// 操作最后添加的成员字段，向成员字段中添加扩展信息。
        /// </summary>
        /// <param name="extend"></param>
        /// <returns></returns>
        public ObjectMapper<T> AddFieldExtend(IMetaFieldExtend extend)
        {
            TestReadonly();
            if (_currentField == null)
                throw new MapperException("未包含任何成员字段。");
            _currentField.AddExtend(extend);
            return this;
        }

        /// <summary>
        /// 操作最后添加的成员字段，将成员字段标识为主键字段。
        /// </summary>
        /// <returns></returns>
        public ObjectMapper<T> MakeKey()
        {
            TestReadonly();
            if (_currentField == null)
                throw new MapperException("未包含任何成员字段。");
            _currentField.MakeKey();
            return this;
        }

        /// <summary>
        /// 操作最后添加的成员字段，设置成员字段的默认值。
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public ObjectMapper<T> SetDefaultValue<TValue>(TValue value)
        {
            TestReadonly();
            if (_currentField == null)
                throw new MapperException("未包含任何成员字段。");
            Convert.ChangeType(value, _currentField.PropertyInfo.PropertyType);//类型校验
            _currentField.SetDefaultValue(value);
            return this;
        }

        #endregion

        private bool _ignoreInherirtr;
        public ObjectMapper<T> IgnoreInherirtr()
        {
            _ignoreInherirtr = true;
            return this;
        }

        /// <summary>
        /// 增加对象的扩展信息。
        /// </summary>
        /// <param name="extend"></param>
        /// <returns></returns>
        public ObjectMapper<T> AddExtend(IMetaInfoExtend extend)
        {
            TestReadonly();
            if (extend == null)
                throw new ArgumentNullException("extend");
            _extends.Add(extend);
            return this;
        }

        private bool _isReadonly;
        /// <summary>
        /// 只读检测
        /// </summary>
        protected void TestReadonly()
        {
            if (_isReadonly)
                throw new InvalidOperationException(string.Format("{0}在只读状态下无法完成修改操作。", typeof(IMetaFieldInfo).FullName));
        }

        /// <summary>
        /// 完成映射（生命周期内只能调用一次）。
        /// </summary>
        /// <returns></returns>
        public ObjectMapper<T> Complete()
        {
            lock (this)
            {
                TestReadonly();
                //
                if (_fields.Count == 0)
                    throw new MapperException("未设置任何成员字段。");
                //
                _propertyValues = new IPropertyValue<T>[_fields.Count];
                var keys = new List<IMetaFieldInfo>();
                _fieldNames = new string[_fields.Count];

                var i = 0;
                foreach (var field in _fields.Values)
                {
                    _propertyValues[i] = new ObjectPropertyValue<T>(field.PropertyInfo);
                    if (field.IsKey)
                    {
                        keys.Add(field);
                    }
                    _fieldNames[i] = field.FieldName;
                    //
                    _fieldIndex.Add(field.FieldName.ToLower(), i);
                    //
                    var identity = ((IdentityMetaFieldExtend)field.GetExtends().FirstOrDefault(x => x is IdentityMetaFieldExtend));
                    if (identity != null)
                    {
                        if (_identity != null)
                            throw new MapperException("一个实体映射只能包含一个自增长标识。");
                        _identity = identity;
                    }
                    //
                    ((MetaField)field).MakeReadonly();
                    //
                    i++;
                }
                _keys = keys.ToArray();
                _keyCount = keys.Count;
                //

                if (this._name == null || _ignoreInherirtr)
                {
                    var be = Expression.Block(Expression.New(typeof(T)));
                    _creator = Expression.Lambda<Func<T>>(be).Compile();
                }
                else
                {
                    var _newEntityType = EntityBuilder.BuilderEntityClass(typeof(T), this);
                    var be = Expression.Block(Expression.New(_newEntityType));
                    _creator = Expression.Lambda<Func<T>>(be).Compile();
                }
                //
                _isReadonly = true;
            }
            //
            return this;
        }

        internal protected class MetaField : IMetaFieldInfo
        {
            private readonly IMapper _mapper;
            private bool _isReadonly;
            private readonly Expression<Func<T, object>> _expression;

            public MetaField(IMapper<T> metaInfo, string field, Expression<Func<T, object>> propertyExp)
            {
                if (_mapper == null)
                    throw new ArgumentNullException("metaInfo");
                if (string.IsNullOrWhiteSpace(_feild))
                    throw new ArgumentNullException("field");
                //
                _mapper = metaInfo;
                _feild = field;
                _expression = propertyExp;
                _propertyInfo = (PropertyInfo)PropertyExpression.ExtractMemberExpression(propertyExp).Member;
                _extends = new ConcurrentBag<IMetaFieldExtend>();
            }

            #region IMetaFieldInfo

            public IMetaInfo MetaInfo
            {
                get { return _mapper.MetaInfo; }
            }

            private readonly PropertyInfo _propertyInfo;
            public PropertyInfo PropertyInfo { get { return _propertyInfo; } }

            private readonly string _feild;
            public string FieldName
            {
                get { return _feild; }
            }

            private bool _isKey;
            public bool IsKey
            {
                get { return _isKey; }
            }

            private object _defaultValue;
            public object DefaultValue { get { return _defaultValue; } }

            private bool _hasDefaultValue;
            public bool HasDefaultValue()
            {
                return _hasDefaultValue;
            }

            private readonly ConcurrentBag<IMetaFieldExtend> _extends;
            public IMetaFieldExtend[] GetExtends()
            {
                var extends = new IMetaFieldExtend[_extends.Count];
                _extends.CopyTo(extends, 0);
                return extends;
            }

            #endregion

            public Expression<Func<T, object>> Expression { get { return _expression; } }

            protected void TestReadonly()
            {
                if (_isReadonly)
                    throw new InvalidOperationException(string.Format("{0}在只读状态下无法完成修改操作。", typeof(IMetaFieldInfo).FullName));
            }

            public void MakeReadonly()
            {
                _isReadonly = true;
            }

            public void MakeKey()
            {
                TestReadonly();
                _isKey = true;
            }

            public void AddExtend(IMetaFieldExtend extend)
            {
                TestReadonly();
                if (extend == null)
                    throw new ArgumentNullException("extend");
                _extends.Add(extend);
            }

            public void SetDefaultValue<TValue>(TValue value)
            {
                TestReadonly();
                _defaultValue = value;
                _hasDefaultValue = true;
            }

            public TValue GetDefaultValue<TValue>()
            {
                if (!_hasDefaultValue)
                    return default(TValue);
                return (TValue)_defaultValue;
            }

        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sparrow.CommonLibrary.Mapper;
using Sparrow.CommonLibrary.Mapper.Metadata;

namespace Sparrow.CommonLibrary.Entity
{
    /// <summary>
    /// 拥有映射信息的实体对象，当实体对象未从<see cref="Sparrow.CommonLibrary.Entity.IEntity"/>继承时，使用此对象解析实体映射信息。
    /// </summary>
    public class EntityExplain : IEntityExplain
    {
        #region IEntity

        public DataState OperationState
        {
            get
            {
                var entity = EntityData as IEntity;
                // 如果Data不是IEntity类型,则始终返回DataState.New
                return entity == null ? DataState.NewOrModify : entity.OperationState;
            }
            set
            {
                var entity = EntityData as IEntity;
                if (entity != null) { entity.OperationState = value; }
            }
        }

        public Type EntityType
        {
            get
            {
                var entity = EntityData as IEntity;
                if (entity == null)
                    return this.GetType();
                return entity.EntityType;
            }
        }

        public bool IsSetted(int index)
        {
            var entity = EntityData as IEntity;
            if (entity == null)
                return true;
            return entity.IsSetted(index);
        }

        public bool AnySetted()
        {
            var entity = EntityData as IEntity;
            if (entity == null)
                return true;
            return entity.AnySetted();
        }

        #endregion

        #region IMappingTrigger

        public void Begin()
        {
            var entity = EntityData as IMappingTrigger;
            if (entity == null)
                return;
            entity.Begin();
        }

        public void End()
        {
            var entity = EntityData as IMappingTrigger;
            if (entity == null)
                return;
            entity.End();
        }

        #endregion

        #region IEntityFieldAccessor

        public object this[string field]
        {
            get
            {
                var property = _mapper[field];
                if (property == null)
                    return null;
                return property.GetValue(EntityData);
            }
            set
            {
                var property = _mapper[field];
                if (property == null)
                    return;
                property.SetValue(EntityData, value);
            }
        }

        public IEnumerable<string> GetSettedFields()
        {
            var entity = EntityData as IEntity;
            if (entity == null)
                return _mapper.MetaInfo.GetPropertyNames();
            //
            var list = new List<string>();
            for (var i = 0; i < _mapper.MetaInfo.PropertyCount; i++)
                if (entity.IsSetted(i))
                    list.Add(_mapper.MetaInfo[i].PropertyName);
            return list;
        }

        public IEnumerable<ItemValue> GetFieldValues(IEnumerable<string> fields)
        {
            return from field in fields select new ItemValue(field, this[field]);
        }

        #endregion

        #region IDbMetaInfo

        public string TableName
        {
            get { return _mapper.MetaInfo.Name; }
        }

        public IDbIncrementMetaPropertyInfo Increment
        {
            get
            {
                var dbMeta = _mapper.MetaInfo as IDbMetaInfo;
                if (dbMeta != null)
                    return dbMeta.Increment;
                return null;
            }
        }

        public int KeyCount
        {
            get
            {
                var dbMeta = _mapper.MetaInfo as IDbMetaInfo;
                if (dbMeta != null)
                    return dbMeta.KeyCount;
                return 0;
            }
        }

        public int ColumnCount { get { return _mapper.MetaInfo.PropertyCount; } }

        public bool IsKey(string columnName)
        {
            var dbMeta = _mapper.MetaInfo as IDbMetaInfo;
            if (dbMeta != null)
                return dbMeta.IsKey(columnName);
            return false;
        }

        public string[] GetKeys()
        {
            var dbMeta = _mapper.MetaInfo as IDbMetaInfo;
            if (dbMeta != null)
                return dbMeta.GetKeys();
            return new string[0];
        }

        public string[] GetColumnNames()
        {
            return _mapper.MetaInfo.GetPropertyNames();
        }

        #endregion

        #region IEntityExplain

        public object EntityData { get; private set; }

        #endregion

        private IObjectAccessor _mapper;

        public IObjectAccessor Mapper { get { return _mapper; } }

        public IMetaInfo MetaInfo
        {
            get { return _mapper.MetaInfo; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public EntityExplain(object data)
        {
            Switch(data);
        }

        /// <summary>
        /// 切换一个新的实体对象
        /// </summary>
        /// <param name="data"></param>
        public void Switch(object data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (data is IEntityExplain)
                throw new ArgumentException(string.Format("类型[{0}]已经继承[{1}].", data.GetType().FullName, typeof(IEntityExplain).FullName));
            if (EntityData == null || data.GetType() != EntityData.GetType())
            {
                if (data is IEntity)
                {
                    _mapper = Map.GetAccessor(((IEntity)data).EntityType);
                }
                else
                {
                    _mapper = Map.GetAccessor(data.GetType());
                }
            }
            EntityData = data;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class EntityExplain<T> : EntityExplain, IEntityExplain<T>
    {

        public new T EntityData { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public EntityExplain(T data)
            : base(data)
        {
            Switch(data);
        }

        /// <summary>
        /// 切换一个新的实体对象
        /// </summary>
        /// <param name="data"></param>
        public void Switch(T data)
        {
            base.Switch(data);
            EntityData = data;
        }

        public new IObjectAccessor<T> Mapper
        {
            get { return (IObjectAccessor<T>)base.Mapper; }
        }
    }
}

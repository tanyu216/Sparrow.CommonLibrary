using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Collections;
using Sparrow.CommonLibrary.Data.Database;
using Sparrow.CommonLibrary.Data.SqlBuilder;
using Sparrow.CommonLibrary.Data.DbCommon;

namespace Sparrow.CommonLibrary.Data.Database
{
    /// <summary>
    /// SQL参数(<see cref="System.Data.Common.DbParameter"/>)集合
    /// </summary>
    public class ParameterCollection : IEnumerable<DbParameter>
    {
        private readonly List<DbParameter> _parameterList;
        private readonly INameBuilder _nameBuilder;
        private readonly DbProvider _dbProvider;

        /// <summary>
        /// 获取集合中对象的个数。
        /// </summary>
        public int Count
        {
            get { return _parameterList.Count; }
        }

        /// <summary>
        /// 获取参数对象
        /// </summary>
        /// <param name="index">指定下标,从0开始的index</param>
        /// <returns>参数对象</returns>
        public DbParameter this[int index]
        {
            get
            {
                return _parameterList[index];
            }
        }

        /// <summary>
        /// 获取参数对象
        /// </summary>
        /// <param name="parameterName">指定参数名称 </param>
        /// <returns>参数对象</returns>
        public DbParameter this[string parameterName]
        {
            get
            {
                parameterName = _nameBuilder.BuildParameterName(parameterName);
                return _parameterList.FirstOrDefault(dbParam => dbParam.ParameterName == parameterName);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="nameBuilder"></param>
        /// <param name="dbProvider"></param>
        public ParameterCollection(ISqlBuilder nameBuilder, DbProvider dbProvider)
            : this(nameBuilder, dbProvider, 4)
        {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="nameBuilder"></param>
        /// <param name="dbProvider"></param>
        /// <param name="capacity">集合每次分配空间的增量大小</param>
        public ParameterCollection(ISqlBuilder nameBuilder, DbProvider dbProvider, int capacity)
        {
            if (nameBuilder == null) throw new ArgumentNullException("nameBuilder");
            //
            _parameterList = new List<DbParameter>(capacity);
            _nameBuilder = nameBuilder;
            _dbProvider = dbProvider;
        }

        #region CreateParameter

        /// <summary>
        /// 创建一个参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>返回一个参数对象</returns>
        protected DbParameter CreateParameter(string name, object value)
        {
            var dbParam = _dbProvider.CreateParameter();
            dbParam.ParameterName = _nameBuilder.BuildParameterName(name);
            dbParam.Value = value ?? DBNull.Value;
            return dbParam;
        }

        /// <summary>
        /// 创建一个参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="suffix">参数名称的后缀（下标，人为的控制参数惟一性）</param>
        /// <returns>返回一个参数对象</returns>
        protected DbParameter CreateParameter(string name, object value, bool suffix)
        {
            var dbParam = _dbProvider.CreateParameter();
            dbParam.ParameterName = _nameBuilder.BuildParameterName(suffix ? string.Concat(name, "_", this.Count) : name);
            dbParam.Value = value ?? DBNull.Value;
            return dbParam;
        }

        #endregion

        #region Fill

        /// <summary>
        /// 向集合中添加参数值
        /// </summary>
        /// <param name="values">参数值</param>
        /// <returns>参数名称，与values中的参数值的下标一一对应</returns>
        public string[] Fill(ICollection values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            // 因为给的values只有值，没有参数无名称，所以默认情况下将参数的名称用@param0,@param1,@param2...
            var list = new List<string>(values.Count);
            foreach (var value in values)
            {
                var name = this.Append("p", value, true).ParameterName;
                list.Add(name);
            }
            return list.ToArray();
        }

        #endregion

        #region Append

        /// <summary>
        /// 向集合中添加一个参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>新建在集合中的参数对象</returns>
        public DbParameter Append(string name, object value)
        {
            DbParameter param = CreateParameter(name, value);
            this._parameterList.Add(param);
            return param;
        }

        /// <summary>
        /// 向集合中添加一个参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="suffix">参数名称的后缀（下标，人为的控制参数惟一性）</param>
        /// <returns>新建在集合中的参数对象</returns>
        public DbParameter Append(string name, object value, bool suffix)
        {
            DbParameter param = CreateParameter(name, value, suffix);
            this._parameterList.Add(param);
            return param;
        }

        /// <summary>
        /// 向集合中添加一个参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数映射至数据库中的类型</param>
        /// <returns>新建在集合中的参数对象</returns>
        public DbParameter Append(string name, object value, DbType type)
        {
            DbParameter param = CreateParameter(name, value);
            param.DbType = type;
            this._parameterList.Add(param);
            return param;
        }

        /// <summary>
        /// 向集合中添加一个参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数映射至数据库中的类型</param>
        /// <param name="suffix">为参数名称加后缀（人为的控制参数惟一性）</param>
        /// <returns>新建在集合中的参数对象</returns>
        public DbParameter Append(string name, object value, DbType type, bool suffix)
        {
            DbParameter param = CreateParameter(name, value, suffix);
            param.DbType = type;
            this._parameterList.Add(param);
            return param;
        }

        /// <summary>
        /// 向集合中添加一个参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数映射至数据库中的类型</param>
        /// <param name="direction">参数方向</param>
        /// <returns>新建在集合中的参数对象</returns>
        public DbParameter Append(string name, object value, DbType type, ParameterDirection direction)
        {
            DbParameter param = CreateParameter(name, value);
            param.DbType = type;
            param.Direction = direction;
            this._parameterList.Add(param);
            return param;
        }

        /// <summary>
        /// 向集合中添加一个参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数映射至数据库中的类型</param>
        /// <param name="direction">参数方向</param>
        /// <param name="suffix">为参数名称加后缀（人为的控制参数惟一性）</param>
        /// <returns>新建在集合中的参数对象</returns>
        public DbParameter Append(string name, object value, DbType type, ParameterDirection direction, bool suffix)
        {
            DbParameter param = CreateParameter(name, value, suffix);
            param.DbType = type;
            param.Direction = direction;
            this._parameterList.Add(param);
            return param;
        }

        /// <summary>
        /// 向集合中添加一个参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数映射至数据库中的类型</param>
        /// <param name="direction">参数方向</param>
        /// <param name="size">参数大小</param>
        /// <returns>新建在集合中的参数对象</returns>
        public DbParameter Append(string name, object value, DbType type, ParameterDirection direction, int size)
        {
            DbParameter param = CreateParameter(name, value);
            param.DbType = type;
            param.Direction = direction;
            param.Size = size;
            this._parameterList.Add(param);
            return param;
        }

        /// <summary>
        /// 向集合中添加一个参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数映射至数据库中的类型</param>
        /// <param name="direction">参数方向</param>
        /// <param name="size">参数大小</param>
        /// <param name="suffix">为参数名称加后缀（人为的控制参数惟一性）</param>
        /// <returns>新建在集合中的参数对象</returns>
        public DbParameter Append(string name, object value, DbType type, ParameterDirection direction, int size, bool suffix)
        {
            DbParameter param = CreateParameter(name, value, suffix);
            param.DbType = type;
            param.Direction = direction;
            param.Size = size;
            this._parameterList.Add(param);
            return param;
        }

        #endregion

        #region implement IEnumerable<DbParameter>

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DbParameter> GetEnumerator()
        {
            return _parameterList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parameterList.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// 清空所有的参数
        /// </summary>
        public void Clear()
        {
            _parameterList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var content = new StringBuilder();
            foreach (var item in _parameterList)
            {
                content.Append("[").Append(item.ParameterName).Append("=").Append(item.Value).AppendLine("]");
            }
            content.Append(";");
            return content.ToString();
        }
    }
}

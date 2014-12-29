using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using Sparrow.CommonLibrary.Entity;
using Sparrow.CommonLibrary.Database.SqlBuilder;

namespace Sparrow.CommonLibrary.Database
{
    /// <summary>
    /// SQL语句集合,用于将多条Sql或多个实体对象，一次性执行。以减与数据库的多次链接而提高效率。
    /// </summary>
    /// <remarks>这是一个支持迭代的对象，通过迭代将获取所有的Sql语句（它也包括实体对象的sql）。</remarks>
    public class SqlBatch : IEnumerable<SqlBatchItem>
    {
        /// <summary>
        /// 待执行的SQL集合
        /// </summary>
        private readonly List<SqlBatchItem> _list;

        /// <summary>
        /// SQL语句批量的条数
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public SqlBatch()
        {
            _list = new List<SqlBatchItem>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="capacity">集合数组预分配大小(不能小于0)</param>
        public SqlBatch(int capacity)
        {
            _list = new List<SqlBatchItem>(capacity);
        }

        /// <summary>
        /// 向SQL语句集合中追加一个sql
        /// </summary>
        /// <param name="sql">被执行的sql语句</param>
        /// <returns>返回对象本身，它的目的是让编码更顺畅更轻松（Fluent）。</returns>
        public SqlBatch Append(string sql)
        {
            _list.Add(new SqlBatchItem(sql, null));
            return this;
        }

        /// <summary>
        /// 向SQL语句集合中追加一个sql
        /// </summary>
        /// <param name="sql">被执行的sql语句</param>
        /// <param name="values">sql参数</param>
        /// <example>sql语句示例：INSERT INTO TableName(col1,col2,col3) VALUES({0},{1},{2});</example>
        /// <returns>返回对象本身，它的目的是让编码更顺畅更轻松（Fluent）。</returns>
        /// <remarks></remarks>
        public virtual SqlBatch AppendFormat(string sql, params object[] values)
        {
            _list.Add(new SqlBatchItem(sql, values));
            return this;
        }

        /// <summary>
        /// 向SQL语句集合中追加一个sql
        /// </summary>
        /// <param name="sql">被执行的sql语句</param>
        /// <param name="values">sql参数</param>
        /// <returns>返回对象本身，它的目的是让编码更顺畅更轻松（Fluent）。</returns>
        /// <example>sql语句示例：INSERT INTO TableName(col1,col2,col3) VALUES({0},{1},{2});</example>
        /// <remarks></remarks>
        public virtual SqlBatch AppendFormat(string sql, ICollection values)
        {
            var array = new object[values.Count];
            array.CopyTo(array, 0);
            _list.Add(new SqlBatchItem(sql, array));
            return this;
        }

        /// <summary>
        /// 向SQL语句集合中追加一个实体
        /// </summary>
        /// <param name="entity">数据对象</param>
        /// <returns>返回对象本身，它的目的是让编码更顺畅更轻松（Fluent）。</returns>
        /// <remarks>当实体对象(<paramref name="entity"/>)添加到<see cref="SqlBatch"/>之后,<paramref name="entity"/>会通过<see cref="EntityToSqlStatement"/>生成Sql语句。</remarks>
        public virtual SqlBatch Append(Object entity)
        {
            _list.Add(new SqlBatchItem(entity));
            return this;
        }

        /// <summary>
        /// 向SQL语句集合中追加一个实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities">数据对象集合</param>
        /// <returns></returns>
        public virtual SqlBatch Append<T>(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                _list.Add(new SqlBatchItem(entity));
            }
            return this;
        }

        /// <summary>
        /// 清空集合
        /// </summary>
        public void Clear() { _list.Clear(); }

        #region IEnumerable Members

        public IEnumerator<SqlBatchItem> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public enum ItemCommandType
    {
        /// <summary>
        /// 
        /// </summary>
        Text,
        /// <summary>
        /// 
        /// </summary>
        Entity
    }

    /// <summary>
    /// 
    /// </summary>
    public class SqlBatchItem
    {
        /// <summary>
        /// 
        /// </summary>
        public ItemCommandType ItemType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public object Command { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public object[] Parameters { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        public SqlBatchItem(string sql, object[] parameters)
        {
            ItemType = ItemCommandType.Text;
            Command = sql;
            Parameters = parameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public SqlBatchItem(object entity)
        {
            ItemType = ItemCommandType.Entity;
            Command = entity;
        }
    }
}

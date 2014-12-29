using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Database.SqlBuilder;
using Sparrow.CommonLibrary.Entity;
using Sparrow.CommonLibrary.Mapper;
using Sparrow.CommonLibrary.Mapper.Metadata;
using System.Linq.Expressions;
using System.Reflection;
using Sparrow.CommonLibrary.Query;
using Sparrow.CommonLibrary.Extenssions;

namespace Sparrow.CommonLibrary.Repository
{
    /// <summary>
    /// 基于数据库的<see cref="IRepository"/>实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RepositoryDatabase<T> : IRepository<T> where T : class
    {
        private readonly DatabaseHelper _dbReader;
        /// <summary>
        /// 数据库只读对象
        /// </summary>
        public DatabaseHelper DbReader { get { return _dbReader; } }

        private readonly DatabaseHelper _dbWriter;
        /// <summary>
        /// 数据库只写对象
        /// </summary>
        public DatabaseHelper DbWriter { get { return _dbWriter; } }

        private readonly IDbMetaInfo dbMetaInfo;
        private readonly IMetaInfo metaInfo;

        /// <summary>
        /// Repository初始化
        /// </summary>
        /// <param name="database">数据库读写访问对象</param>
        public RepositoryDatabase(Database.DatabaseHelper database)
        {
            if (database == null)
                throw new ArgumentNullException("database");

            if (typeof(T) == typeof(DynamicEntity))
                throw new ArgumentException(string.Format("泛型T不能是{0}", typeof(DynamicEntity).FullName));

            _dbReader = database;
            _dbWriter = database;
            var accessor = Map.GetCheckedAccessor<T>();
            dbMetaInfo = accessor.MetaInfo as IDbMetaInfo;
            metaInfo = accessor.MetaInfo;
        }

        /// <summary>
        /// Repository初始化
        /// </summary>
        /// <param name="databaseReader">数据库读访问对象</param>
        /// <param name="databaseWriter">数据库写访问对象</param>
        public RepositoryDatabase(Database.DatabaseHelper databaseReader, Database.DatabaseHelper databaseWriter)
            : this(databaseReader)
        {
            if (databaseWriter == null)
                throw new ArgumentNullException("databaseWriter");

            if (databaseReader.DbProvider.ProviderName != databaseWriter.DbProvider.ProviderName)
                throw new ArgumentException("数据库读写连接对象的驱动不一致");

            _dbWriter = databaseWriter;
        }

        /// <summary>
        /// sql语句生成对象（依据不同数据库驱动生成符合各数据库语法的sql语句）
        /// </summary>
        protected ISqlBuilder SqlBuilder { get { return _dbReader.Builder; } }

        /// <summary>
        /// 数据实体转换成可执行的Sql语句对象
        /// </summary>
        protected EntityToSqlStatement EntityToSql { get { return _dbReader.EntityToSql; } }

        /// <summary>
        /// 创建一个数据库参数对象
        /// </summary>
        /// <returns></returns>
        protected ParameterCollection CreateParamterCollection()
        {
            return _dbReader.CreateParamterCollection();
        }

        /// <summary>
        /// 获取实体对象中成员属性映射的数据库字段名称
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        protected string FieldName(Expression<Func<T, object>> field)
        {
            var propertyInfo = (PropertyInfo)PropertyExpression.ExtractMemberExpression(field).Member;
            var fieldMap = metaInfo[propertyInfo];
            if (fieldMap != null)
                return fieldMap.PropertyName;

            throw new ArgumentException("参数不支持作为查询条件，因为无法获取该属性所映射的成员字段。");
        }

        #region BuildDmlSql
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="output"></param>
        /// <param name="incrementEntity"></param>
        /// <returns></returns>
        protected string BuildDmlSql(T entity, ParameterCollection output, ref IDictionary<string, T> incrementEntity)
        {
            var expl = new EntityExplain<T>(entity);
            return BuildDmlSql(expl, expl.OperationState, output, ref incrementEntity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="state"></param>
        /// <param name="output"></param>
        /// <param name="incrementEntity"></param>
        /// <returns></returns>
        protected string BuildDmlSql(T entity, DataState state, ParameterCollection output, ref IDictionary<string, T> incrementEntity)
        {
            var expl = new EntityExplain<T>(entity);
            return BuildDmlSql(expl, state, output, ref incrementEntity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="output"></param>
        /// <param name="incrementEntity"></param>
        /// <returns></returns>
        protected string BuildDmlSql(IEnumerable<T> entities, ParameterCollection output, ref IDictionary<string, T> incrementEntity)
        {
            StringBuilder sql = new StringBuilder();
            foreach (T entity in entities)
            {
                sql.AppendLine(BuildDmlSql(entity, output, ref incrementEntity));
            }
            return sql.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="state"></param>
        /// <param name="output"></param>
        /// <param name="incrementEntity"></param>
        /// <returns></returns>
        protected string BuildDmlSql(IEnumerable<T> entities, DataState state, ParameterCollection output, ref IDictionary<string, T> incrementEntity)
        {
            StringBuilder sql = new StringBuilder();
            foreach (T entity in entities)
            {
                sql.AppendLine(BuildDmlSql(entity, state, output, ref incrementEntity));
            }
            return sql.ToString();
        }

        private string BuildDmlSql(EntityExplain<T> expl, DataState state, ParameterCollection output, ref IDictionary<string, T> incrementEntity)
        {
            //
            string identityFieldName = null;
            if (expl.Increment != null)
            {
                if (incrementEntity == null)
                    incrementEntity = new Dictionary<string, T>();
                identityFieldName = expl.Increment.ColumnName + incrementEntity.Count;
            }
            //
            string innerSql;
            bool hasIncrement = false;
            switch (state)
            {
                case DataState.New:
                    innerSql = EntityToSql.GenerateInsert(expl, output, true, out hasIncrement, identityFieldName);
                    break;
                case DataState.Modify:
                    innerSql = EntityToSql.GenerateUpdate(expl, output);
                    break;
                case DataState.NewOrModify:
                    innerSql = EntityToSql.GenerateInsertOrUpdate(expl, output, true, out hasIncrement, identityFieldName);
                    break;
                default:
                    throw new ArgumentException("不支持枚举：" + expl.OperationState);
            }
            if (hasIncrement)
                incrementEntity.Add(identityFieldName, expl.EntityData);
            return innerSql;
        }

        #endregion

        #region ReceiveIncrement

        /// <summary>
        /// 接收返回的增量标识，按<paramref name="incrementReader"/>自增序列中的别名取出数据写入对应的<paramref name="entities"/>实体中。
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="incrementReader"></param>
        /// <returns></returns>
        internal protected bool ReceiveIncrement(IDictionary<string, T> entities, IDataReader incrementReader)
        {
            EntityExplain entityExplain = null;
            var received = false;
            while (incrementReader.Read())
            {
                T entity;
                if (entities.TryGetValue(incrementReader.GetName(0), out entity) == false)
                    continue;
                var entyExpl = entity as IEntityExplain;
                if (entyExpl != null)
                {
                    entyExpl[entyExpl.Increment.ColumnName] = incrementReader.GetValue(0);
                }
                else
                {
                    if (entityExplain == null)
                        entityExplain = new EntityExplain(entity);
                    else
                        entityExplain.Switch(entity);
                    //
                    entityExplain[entityExplain.Increment.ColumnName] = incrementReader.GetValue(0);
                }

                if (received == false)
                    received = true;
                if (incrementReader.NextResult() == false) break;
            }
            return received;
        }

        #endregion

        #region DoExecute
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterCollection"></param>
        /// <returns></returns>
        protected int DoExecuteByDbWriter(string sql, ParameterCollection parameterCollection)
        {
            return DoExecuteByDbWriter(sql, parameterCollection, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterCollection"></param>
        /// <param name="incrementEntity"></param>
        /// <returns></returns>
        protected int DoExecuteByDbWriter(string sql, ParameterCollection parameterCollection, IDictionary<string, T> incrementEntity)
        {
            if (string.IsNullOrEmpty(sql))
                return 0;
            if (incrementEntity == null || incrementEntity.Count == 0)
            {
                return DbWriter.ExecuteNonQuery(sql, parameterCollection);
            }
            using (var reader = DbWriter.ExecuteReader(sql, parameterCollection))
            {
                ReceiveIncrement(incrementEntity, reader);
                return reader.RecordsAffected;
            }
        }

        #endregion

        #region IRepository<T>.DML
        /// <summary>
        /// 将实体生成一条Insert语句到数据库执行
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (entity is IEntity)
                ((IEntity)entity).OperationState = DataState.New;

            IDictionary<string, T> incrementEntity = null;
            var parameters = CreateParamterCollection();
            var sql = BuildDmlSql(entity, DataState.New, parameters, ref incrementEntity);
            //
            return DoExecuteByDbWriter(sql, parameters, incrementEntity);
        }
        /// <summary>
        /// 将实体生成一组Insert语句到数据库执行
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Insert(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entity");

            foreach (var entity in entities)
                if (entity is IEntity)
                    ((IEntity)entity).OperationState = DataState.New;

            var parameters = CreateParamterCollection();
            IDictionary<string, T> incrementEntity = null;
            var sql = BuildDmlSql(entities, DataState.New, parameters, ref incrementEntity);
            //
            return DoExecuteByDbWriter(sql, parameters, incrementEntity);
        }
        /// <summary>
        /// 将实体生成一条Update语句到数据库执行
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (entity is IEntity)
                ((IEntity)entity).OperationState = DataState.Modify;

            var parameters = CreateParamterCollection();
            IDictionary<string, T> incrementEntity = null;
            var sql = BuildDmlSql(entity, DataState.Modify, parameters, ref incrementEntity);
            //
            return DoExecuteByDbWriter(sql, parameters, incrementEntity);
        }
        /// <summary>
        /// 将实体生成一组Update语句到数据库执行
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Update(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            foreach (var entity in entities)
                if (entity is IEntity)
                    ((IEntity)entity).OperationState = DataState.Modify;

            var parameters = CreateParamterCollection();
            IDictionary<string, T> incrementEntity = null;
            var sql = BuildDmlSql(entities, DataState.Modify, parameters, ref incrementEntity);
            //
            return DoExecuteByDbWriter(sql, parameters, incrementEntity);
        }
        /// <summary>
        /// 将实体保存到数据库，如果实体存在（依据主键）则执行Update否则执行Insert
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Save(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (entity is IEntity)
                ((IEntity)entity).OperationState = DataState.NewOrModify;

            var parameters = CreateParamterCollection();
            IDictionary<string, T> incrementEntity = null;
            var sql = BuildDmlSql(entity, parameters, ref incrementEntity);
            //
            return DoExecuteByDbWriter(sql, parameters, incrementEntity);
        }
        /// <summary>
        /// 将实体保存到数据库，如果实体存在（依据主键）则执行Update否则执行Insert
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Save(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            foreach (var entity in entities)
                if (entity is IEntity)
                    ((IEntity)entity).OperationState = DataState.NewOrModify;

            var parameters = CreateParamterCollection();
            IDictionary<string, T> incrementEntity = null;
            var sql = BuildDmlSql(entities, parameters, ref incrementEntity);
            //
            return DoExecuteByDbWriter(sql, parameters, incrementEntity);
        }
        /// <summary>
        /// 依据实体主键，生成Delete语句到数据库执行。
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            //
            var parameters = CreateParamterCollection();
            var expl = new EntityExplain<T>(entity);
            var sql = EntityToSql.GenerateDelete(expl, parameters);
            //
            return DoExecuteByDbWriter(sql, parameters, null);
        }
        /// <summary>
        /// 依据Lambda表达式，生成Delete语句到数据库执行。
        /// </summary>
        /// <param name="logical">Lambda表达式</param>
        /// <returns></returns>
        public int Delete(Expression<Func<T, bool>> logical)
        {
            if (logical == null)
                throw new ArgumentNullException("logical");
            //
            var parameters = CreateParamterCollection();
            var sql = SqlBuilder.DeleteFormat(metaInfo.Name, LogicalBinaryExpression.Expression(logical).OutputSqlString(SqlBuilder, parameters), SqlOptions.None);
            //
            return DoExecuteByDbWriter(sql, parameters, null);
        }
        /// <summary>
        /// 依据表达式，生成Delete语句到数据库执行。
        /// </summary>
        /// <param name="logical">Sparrow.Query.SqlExpression表达式</param>
        /// <returns></returns>
        public int Delete(LogicalBinaryExpression logical)
        {
            if (logical == null)
                throw new ArgumentNullException("logical");
            //
            var parameters = CreateParamterCollection();
            var sql = SqlBuilder.DeleteFormat(metaInfo.Name, logical.OutputSqlString(SqlBuilder, parameters), SqlOptions.None);
            //
            return DoExecuteByDbWriter(sql, parameters, null);
        }

        #endregion

        #region IRepository<T>.Database.Query
        /// <summary>
        /// 获取表中的所有数据
        /// </summary>
        /// <returns></returns>
        public IList<T> GetList()
        {
            return new Queryable<T>(DbReader).ExecuteList();
        }
        /// <summary>
        /// 获取表中指定行的数据
        /// </summary>
        /// <param name="startIndex">数据起始行，从0开始</param>
        /// <param name="rowCount">返回的总行数</param>
        /// <returns></returns>
        public IList<T> GetList(int startIndex, int rowCount)
        {
            return new Queryable<T>(DbReader).RowLimit(startIndex, rowCount).ExecuteList();
        }
        /// <summary>
        /// 获取表中指定行的数据
        /// </summary>
        /// <param name="startIndex">数据起始行，从0开始</param>
        /// <param name="rowCount">返回的总行数</param>
        /// <param name="orderby">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending">排序方式（true倒序，false升序）</param>
        /// <returns></returns>
        public IList<T> GetList(int startIndex, int rowCount, Expression<Func<T, object>> orderby, bool descending)
        {
            if (orderby == null)
                throw new ArgumentNullException("orderby");

            return new Queryable<T>(DbReader)
                .RowLimit(startIndex, rowCount)
                .OrderBy(orderby, descending)
                .ExecuteList();
        }
        /// <summary>
        /// 获取表中指定行的数据
        /// </summary>
        /// <param name="startIndex">数据起始行，从0开始</param>
        /// <param name="rowCount">返回的总行数</param>
        /// <param name="orderby">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending">排序方式（true倒序，false升序）</param>
        /// <param name="orderby2">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending2">排序方式（true倒序，false升序）</param>
        /// <returns></returns>
        public IList<T> GetList(int startIndex, int rowCount, Expression<Func<T, object>> orderby, bool descending, Expression<Func<T, object>> orderby2, bool descending2)
        {
            if (orderby == null)
                throw new ArgumentNullException("orderby");

            if (orderby2 == null)
                throw new ArgumentNullException("orderby2");

            return new Queryable<T>(DbReader)
                .RowLimit(startIndex, rowCount)
                .OrderBy(orderby, descending)
                .OrderBy(orderby2, descending2)
                .ExecuteList();
        }
        /// <summary>
        /// 获取表中指定行的数据
        /// </summary>
        /// <param name="startIndex">数据起始行，从0开始</param>
        /// <param name="rowCount">返回的总行数</param>
        /// <param name="orderby">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending">排序方式（true倒序，false升序）</param>
        /// <param name="orderby2">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending2">排序方式（true倒序，false升序）</param>
        /// <param name="orderby3">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending3">排序方式（true倒序，false升序）</param>
        /// <returns></returns>
        public IList<T> GetList(int startIndex, int rowCount, Expression<Func<T, object>> orderby, bool descending, Expression<Func<T, object>> orderby2, bool descending2, Expression<Func<T, object>> orderby3, bool descending3)
        {
            if (orderby == null)
                throw new ArgumentNullException("orderby");

            if (orderby2 == null)
                throw new ArgumentNullException("orderby2");

            if (orderby3 == null)
                throw new ArgumentNullException("orderby3");

            return new Queryable<T>(DbReader)
                .RowLimit(startIndex, rowCount)
                .OrderBy(orderby, descending)
                .OrderBy(orderby2, descending2)
                .OrderBy(orderby3, descending3)
                .ExecuteList();
        }
        /// <summary>
        /// 依据Lambda表达式，查询表中的数据
        /// </summary>
        /// <param name="logical">Lambda条件表达式，如：x=&gt;x.id&gt;3 &amp;&amp; x.id&lt;5 || (object)x.id==new[]{12,13,18}  </param>
        /// <returns></returns>
        public IList<T> GetList(Expression<Func<T, bool>> logical)
        {
            if (logical == null)
                throw new ArgumentNullException("logical");

            return new Queryable<T>(DbReader).Where(logical).ExecuteList();
        }
        /// <summary>
        /// 据Lambda表达式，查询表中的数据
        /// </summary>
        /// <param name="logical">Lambda条件表达式，如：x=&gt;x.id&gt;3 &amp;&amp; x.id&lt;5 || (object)x.id==new[]{12,13,18}  </param>
        /// <param name="startIndex">数据起始行，从0开始</param>
        /// <param name="rowCount">返回的总行数</param>
        /// <returns></returns>
        public IList<T> GetList(Expression<Func<T, bool>> logical, int startIndex, int rowCount)
        {
            if (logical == null)
                throw new ArgumentNullException("logical");
            
            return new Queryable<T>(DbReader).Where(logical).RowLimit(startIndex, rowCount).ExecuteList();
        }
        /// <summary>
        /// 据Lambda表达式，查询表中的数据
        /// </summary>
        /// <param name="logical">Lambda条件表达式，如：x=&gt;x.id&gt;3 &amp;&amp; x.id&lt;5 || (object)x.id==new[]{12,13,18}  </param>
        /// <param name="startIndex">数据起始行，从0开始</param>
        /// <param name="rowCount">返回的总行数</param>
        /// <param name="orderby">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending">排序方式（true倒序，false升序）</param>
        /// <returns></returns>
        public IList<T> GetList(Expression<Func<T, bool>> logical, int startIndex, int rowCount, Expression<Func<T, object>> orderby, bool descending)
        {
            if (logical == null)
                throw new ArgumentNullException("logical");

            if (orderby == null)
                throw new ArgumentNullException("orderby");

            return new Queryable<T>(DbReader)
                .Where(logical)
                .RowLimit(startIndex, rowCount)
                .OrderBy(orderby, descending)
                .ExecuteList();
        }
        /// <summary>
        /// 据Lambda表达式，查询表中的数据
        /// </summary>
        /// <param name="logical">Lambda条件表达式，如：x=&gt;x.id&gt;3 &amp;&amp; x.id&lt;5 || (object)x.id==new[]{12,13,18}  </param>
        /// <param name="startIndex">数据起始行，从0开始</param>
        /// <param name="rowCount">返回的总行数</param>
        /// <param name="orderby">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending">排序方式（true倒序，false升序）</param>
        /// <param name="orderby2">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending2">排序方式（true倒序，false升序）</param>
        /// <returns></returns>
        public IList<T> GetList(Expression<Func<T, bool>> logical, int startIndex, int rowCount, Expression<Func<T, object>> orderby, bool descending, Expression<Func<T, object>> orderby2, bool descending2)
        {
            if (logical == null)
                throw new ArgumentNullException("logical");

            if (orderby == null)
                throw new ArgumentNullException("orderby");

            if (orderby2 == null)
                throw new ArgumentNullException("orderby2");

            return new Queryable<T>(DbReader)
                .Where(logical)
                .RowLimit(startIndex, rowCount)
                .OrderBy(orderby, descending)
                .OrderBy(orderby2, descending2)
                .ExecuteList();
        }
        /// <summary>
        /// 据Lambda表达式，查询表中的数据
        /// </summary>
        /// <param name="logical">Lambda条件表达式，如：x=&gt;x.id&gt;3 &amp;&amp; x.id&lt;5 || (object)x.id==new[]{12,13,18}  </param>
        /// <param name="startIndex">数据起始行，从0开始</param>
        /// <param name="rowCount">返回的总行数</param>
        /// <param name="orderby">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending">排序方式（true倒序，false升序）</param>
        /// <param name="orderby2">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending2">排序方式（true倒序，false升序）</param>
        /// <param name="orderby3">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending3">排序方式（true倒序，false升序）</param>
        /// <returns></returns>
        public IList<T> GetList(Expression<Func<T, bool>> logical, int startIndex, int rowCount, Expression<Func<T, object>> orderby, bool descending, Expression<Func<T, object>> orderby2, bool descending2, Expression<Func<T, object>> orderby3, bool descending3)
        {
            if (logical == null)
                throw new ArgumentNullException("logical");

            if (orderby == null)
                throw new ArgumentNullException("orderby");

            if (orderby2 == null)
                throw new ArgumentNullException("orderby2");

            if (orderby3 == null)
                throw new ArgumentNullException("orderby3");

            return new Queryable<T>(DbReader)
                .Where(logical)
                .RowLimit(startIndex, rowCount)
                .OrderBy(orderby, descending)
                .OrderBy(orderby2, descending2)
                .OrderBy(orderby3, descending3)
                .ExecuteList();
        }
        /// <summary>
        /// 依据Lambda表达式，获取表中一行数据
        /// </summary>
        /// <param name="logical">Lambda表达式，如：x=&gt;x.Name&gt;"123" &amp;&amp; x.sex=1  </param>
        /// <returns></returns>
        public T Get(Expression<Func<T, bool>> logical)
        {
            if (logical == null)
                throw new ArgumentNullException("logical");

            using (var read = new Queryable<T>(DbReader).Where(logical).ExecuteReader())
            {
                return Map.Single<T>(read);
            }
        }
        /// <summary>
        /// 依据Lambda表达式，获取表中一行数据
        /// </summary>
        /// <param name="logical">Lambda表达式，如：x=&gt;x.Name&gt;"123" &amp;&amp; x.sex=1 </param>
        /// <param name="orderby">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending">排序方式（true倒序，false升序）</param>
        /// <returns></returns>
        public T Get(Expression<Func<T, bool>> logical, Expression<Func<T, object>> orderby, bool descending)
        {
            if (logical == null)
                throw new ArgumentNullException("logical");

            if (orderby == null)
                throw new ArgumentNullException("orderby");

            using (var read = new Queryable<T>(DbReader).Where(logical).OrderBy(orderby, descending).ExecuteReader())
            {
                return Map.Single<T>(read);
            }
        }
        /// <summary>
        /// 依据Lambda表达式，获取表中一行数据
        /// </summary>
        /// <param name="logical">Lambda表达式，如：x=&gt;x.Name&gt;"123" &amp;&amp; x.sex=1 </param>
        /// <param name="orderby">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending">排序方式（true倒序，false升序）</param>
        /// <param name="orderby2">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending2">排序方式（true倒序，false升序）</param>
        /// <returns></returns>
        public T Get(Expression<Func<T, bool>> logical, Expression<Func<T, object>> orderby, bool descending, Expression<Func<T, object>> orderby2, bool descending2)
        {
            if (logical == null)
                throw new ArgumentNullException("logical");

            if (orderby == null)
                throw new ArgumentNullException("orderby");

            if (orderby2 == null)
                throw new ArgumentNullException("orderby2");

            using (var read = new Queryable<T>(DbReader).Where(logical).OrderBy(orderby, descending).OrderBy(orderby2, descending2).ExecuteReader())
            {
                return Map.Single<T>(read);
            }
        }
        /// <summary>
        /// 依据Lambda表达式，获取表中一行数据
        /// </summary>
        /// <param name="logical">Lambda表达式，如：x=&gt;x.Name&gt;"123" &amp;&amp; x.sex=1 </param>
        /// <param name="orderby">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending">排序方式（true倒序，false升序）</param>
        /// <param name="orderby2">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending2">排序方式（true倒序，false升序）</param>
        /// <param name="orderby3">Lambda指定排序字段，如：x=&gt;x.id</param>
        /// <param name="descending3">排序方式（true倒序，false升序）</param>
        /// <returns></returns>
        public T Get(Expression<Func<T, bool>> logical, Expression<Func<T, object>> orderby, bool descending, Expression<Func<T, object>> orderby2, bool descending2, Expression<Func<T, object>> orderby3, bool descending3)
        {
            if (logical == null)
                throw new ArgumentNullException("logical");

            if (orderby == null)
                throw new ArgumentNullException("orderby");

            if (orderby2 == null)
                throw new ArgumentNullException("orderby2");

            if (orderby3 == null)
                throw new ArgumentNullException("orderby3");

            using (var read = new Queryable<T>(DbReader).Where(logical).OrderBy(orderby, descending).OrderBy(orderby2, descending2).OrderBy(orderby3, descending3).ExecuteReader())
            {
                return Map.Single<T>(read);
            }
        }
        /// <summary>
        /// 依据主键获取表中的数据（不支持复合主键）
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        public T Get(object id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            if (dbMetaInfo == null)
                throw new MapperException(string.Format("实体{0}缺少数据库映射信息", typeof(T).FullName));
            if (dbMetaInfo.KeyCount < 1)
                throw new MapperException("缺少主键信息");
            if (dbMetaInfo.KeyCount != 1)
                throw new MapperException("复合主键的实体对象，无法使用该方法。");

            var condition = SqlExpression.Equal(dbMetaInfo.GetKeys()[0], id);
            using (var read = new Queryable<T>(DbReader).Where(condition).ExecuteReader())
            {
                return Map.Single<T>(read);
            }
        }

        #endregion

        #region IRepository<T>.Groupby

        public TValue Sum<TValue>(System.Linq.Expressions.Expression<Func<T, object>> field)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Sum(field)
                .OutputSqlString(parameters);
            return DbReader.ExecuteScalar<TValue>(sql, parameters);
        }

        public TValue Sum<TValue>(Expression<Func<T, object>> field, Expression<Func<T, bool>> logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Sum(field)
                .Where(logical)
                .OutputSqlString(parameters);
            return DbReader.ExecuteScalar<TValue>(sql, parameters);
        }

        public TValue Sum<TValue>(System.Linq.Expressions.Expression<Func<T, object>> field, LogicalBinaryExpression logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Sum(field)
                .Where(logical)
                .OutputSqlString(parameters);
            return DbReader.ExecuteScalar<TValue>(sql, parameters);
        }

        public TValue Min<TValue>(System.Linq.Expressions.Expression<Func<T, object>> field)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Min(field)
                .OutputSqlString(parameters);
            return DbReader.ExecuteScalar<TValue>(sql, parameters);
        }

        public TValue Min<TValue>(Expression<Func<T, object>> field, Expression<Func<T, bool>> logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Min(field)
                .Where(logical)
                .OutputSqlString(parameters);
            return DbReader.ExecuteScalar<TValue>(sql, parameters);
        }

        public TValue Min<TValue>(System.Linq.Expressions.Expression<Func<T, object>> field, LogicalBinaryExpression logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Min(field)
                .Where(logical)
                .OutputSqlString(parameters);
            return DbReader.ExecuteScalar<TValue>(sql, parameters);
        }

        public TValue Max<TValue>(System.Linq.Expressions.Expression<Func<T, object>> field)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Max(field)
                .OutputSqlString(parameters);
            return DbReader.ExecuteScalar<TValue>(sql, parameters);
        }

        public TValue Max<TValue>(Expression<Func<T, object>> field, Expression<Func<T, bool>> logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Max(field)
                .Where(logical)
                .OutputSqlString(parameters);
            return DbReader.ExecuteScalar<TValue>(sql, parameters);
        }

        public TValue Max<TValue>(System.Linq.Expressions.Expression<Func<T, object>> field, LogicalBinaryExpression logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Max(field)
                .Where(logical)
                .OutputSqlString(parameters);
            return DbReader.ExecuteScalar<TValue>(sql, parameters);
        }

        public TValue Avg<TValue>(System.Linq.Expressions.Expression<Func<T, object>> field)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Avg(field)
                .OutputSqlString(parameters);
            return DbReader.ExecuteScalar<TValue>(sql, parameters);
        }

        public TValue Avg<TValue>(Expression<Func<T, object>> field, Expression<Func<T, bool>> condition)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Avg(field)
                .Where(condition)
                .OutputSqlString(parameters);
            return DbReader.ExecuteScalar<TValue>(sql, parameters);
        }

        public TValue Avg<TValue>(System.Linq.Expressions.Expression<Func<T, object>> field, LogicalBinaryExpression logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Avg(field)
                .Where(logical)
                .OutputSqlString(parameters);
            return DbReader.ExecuteScalar<TValue>(sql, parameters);
        }

        public int Count(System.Linq.Expressions.Expression<Func<T, object>> field)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Count(field)
                .OutputSqlString(parameters);
            return DbReader.ExecuteScalar<int>(sql, parameters);
        }

        public int Count(Expression<Func<T, object>> field, Expression<Func<T, bool>> logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Count(field)
                .Where(logical)
                .OutputSqlString(parameters);
            return DbReader.ExecuteScalar<int>(sql, parameters);
        }

        public int Count(System.Linq.Expressions.Expression<Func<T, object>> field, LogicalBinaryExpression logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Count(field)
                .Where(logical)
                .OutputSqlString(parameters);
            return DbReader.ExecuteScalar<int>(sql, parameters);
        }

        public IDictionary<TKey, TValue> GroupbySum<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Select(keyField)
                .Sum(valueField)
                .GroupBy(keyField)
                .OutputSqlString(parameters);
            using (var reader = DbReader.ExecuteReader(sql, parameters))
            {
                return reader.ToDictionary<TKey, TValue>();
            }
        }

        public IDictionary<TKey, TValue> GroupbySum<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, Expression<Func<T, bool>> logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Select(keyField)
                .Sum(valueField)
                .Where(logical)
                .GroupBy(keyField)
                .OutputSqlString(parameters);
            using (var reader = DbReader.ExecuteReader(sql, parameters))
            {
                return reader.ToDictionary<TKey, TValue>();
            }
        }

        public IDictionary<TKey, TValue> GroupbySum<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, LogicalBinaryExpression logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Select(keyField)
                .Sum(valueField)
                .Where(logical)
                .GroupBy(keyField)
                .OutputSqlString(parameters);
            using (var reader = DbReader.ExecuteReader(sql, parameters))
            {
                return reader.ToDictionary<TKey, TValue>();
            }
        }

        public IDictionary<TKey, TValue> GroupbyMin<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Select(keyField)
                .Min(valueField)
                .GroupBy(keyField)
                .OutputSqlString(parameters);
            using (var reader = DbReader.ExecuteReader(sql, parameters))
            {
                return reader.ToDictionary<TKey, TValue>();
            }
        }

        public IDictionary<TKey, TValue> GroupbyMin<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, Expression<Func<T, bool>> logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Select(keyField)
                .Min(valueField)
                .Where(logical)
                .GroupBy(keyField)
                .OutputSqlString(parameters);
            using (var reader = DbReader.ExecuteReader(sql, parameters))
            {
                return reader.ToDictionary<TKey, TValue>();
            }
        }

        public IDictionary<TKey, TValue> GroupbyMin<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, LogicalBinaryExpression logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Select(keyField)
                .Min(valueField)
                .Where(logical)
                .GroupBy(keyField)
                .OutputSqlString(parameters);
            using (var reader = DbReader.ExecuteReader(sql, parameters))
            {
                return reader.ToDictionary<TKey, TValue>();
            }
        }

        public IDictionary<TKey, TValue> GroupbyMax<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Select(keyField)
                .Max(valueField)
                .GroupBy(keyField)
                .OutputSqlString(parameters);
            using (var reader = DbReader.ExecuteReader(sql, parameters))
            {
                return reader.ToDictionary<TKey, TValue>();
            }
        }

        public IDictionary<TKey, TValue> GroupbyMax<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, Expression<Func<T, bool>> logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Select(keyField)
                .Max(valueField)
                .Where(logical)
                .GroupBy(keyField)
                .OutputSqlString(parameters);
            using (var reader = DbReader.ExecuteReader(sql, parameters))
            {
                return reader.ToDictionary<TKey, TValue>();
            }
        }

        public IDictionary<TKey, TValue> GroupbyMax<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, LogicalBinaryExpression logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Select(keyField)
                .Max(valueField)
                .Where(logical)
                .GroupBy(keyField)
                .OutputSqlString(parameters);
            using (var reader = DbReader.ExecuteReader(sql, parameters))
            {
                return reader.ToDictionary<TKey, TValue>();
            }
        }

        public IDictionary<TKey, TValue> GroupbyAvg<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Select(keyField)
                .Avg(valueField)
                .GroupBy(keyField)
                .OutputSqlString(parameters);
            using (var reader = DbReader.ExecuteReader(sql, parameters))
            {
                return reader.ToDictionary<TKey, TValue>();
            }
        }

        public IDictionary<TKey, TValue> GroupbyAvg<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, Expression<Func<T, bool>> logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Select(keyField)
                .Avg(valueField)
                .Where(logical)
                .GroupBy(keyField)
                .OutputSqlString(parameters);
            using (var reader = DbReader.ExecuteReader(sql, parameters))
            {
                return reader.ToDictionary<TKey, TValue>();
            }
        }

        public IDictionary<TKey, TValue> GroupbyAvg<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, LogicalBinaryExpression logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Select(keyField)
                .Avg(valueField)
                .Where(logical)
                .GroupBy(keyField)
                .OutputSqlString(parameters);
            using (var reader = DbReader.ExecuteReader(sql, parameters))
            {
                return reader.ToDictionary<TKey, TValue>();
            }
        }

        public IDictionary<TKey, int> GroupbyCount<TKey>(Expression<Func<T, object>> field)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Select(field)
                .Count(field)
                .GroupBy(field)
                .OutputSqlString(parameters);
            using (var reader = DbReader.ExecuteReader(sql, parameters))
            {
                return reader.ToDictionary<TKey, int>();
            }
        }

        public IDictionary<TKey, int> GroupbyCount<TKey>(Expression<Func<T, object>> field, Expression<Func<T, bool>> logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Select(field)
                .Count(field)
                .Where(logical)
                .GroupBy(field)
                .OutputSqlString(parameters);
            using (var reader = DbReader.ExecuteReader(sql, parameters))
            {
                return reader.ToDictionary<TKey, int>();
            }
        }

        public IDictionary<TKey, int> GroupbyCount<TKey>(Expression<Func<T, object>> field, LogicalBinaryExpression logical)
        {
            var parameters = CreateParamterCollection();
            string sql = new Queryable<T>(DbReader)
                .Select(field)
                .Count(field)
                .Where(logical)
                .GroupBy(field)
                .OutputSqlString(parameters);
            using (var reader = DbReader.ExecuteReader(sql, parameters))
            {
                return reader.ToDictionary<TKey, int>();
            }
        }

        #endregion

    }
}

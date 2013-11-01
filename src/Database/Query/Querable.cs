using Sparrow.CommonLibrary.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using Sparrow.CommonLibrary.Mapper;
using Sparrow.CommonLibrary.Mapper.Metadata;
using System.Data;
using Sparrow.CommonLibrary.Database.Query;

namespace Sparrow.CommonLibrary.Database.Query
{
    /// <summary>
    /// 查询表达式
    /// </summary>
    public class Queryable<T> : Sparrow.CommonLibrary.Database.Query.Expression
    {
        private readonly DatabaseHelper database;

        private readonly IMapper<T> mapper;
        private readonly IMetaFieldInfo[] fields;

        public Queryable(DatabaseHelper database)
        {
            this.database = database;
            mapper = MapperManager.GetIMapper<T>();
            fields = mapper.MetaInfo.GetFields();
        }

        public Queryable<T> Select(Expression<Func<T, object>> field)
        {
            return this;
        }

        public Queryable<T> Select(Expression<Func<T, object>> field, string alias)
        {
            return this;
        }

        public Queryable<T> Select(params Expression<Func<T, object>>[] fields)
        {
            return this;
        }

        public Queryable<T> Select(Query.Expression expression)
        {
            return this;
        }

        public Queryable<T> Select(Query.Expression expression, string alias)
        {
            return this;
        }

        public Queryable<T> Select(params Query.Expression[] expressions)
        {
            return this;
        }

        public Queryable<T> Sum(Expression<Func<T, object>> field)
        {
            return this;
        }

        public Queryable<T> Sum(Expression<Func<T, object>> field, string alias)
        {
            return this;
        }

        public Queryable<T> Sum(Query.Expression field)
        {
            return this;
        }

        public Queryable<T> Sum(Query.Expression field, string alias)
        {
            return this;
        }

        public Queryable<T> Min(Expression<Func<T, object>> field)
        {
            return this;
        }

        public Queryable<T> Min(Expression<Func<T, object>> field, string alias)
        {
            return this;
        }

        public Queryable<T> Min(Query.Expression field)
        {
            return this;
        }

        public Queryable<T> Min(Query.Expression field, string alias)
        {
            return this;
        }

        public Queryable<T> Max(Expression<Func<T, object>> field)
        {
            return this;
        }

        public Queryable<T> Max(Expression<Func<T, object>> field, string alias)
        {
            return this;
        }

        public Queryable<T> Max(Query.Expression field)
        {
            return this;
        }

        public Queryable<T> Max(Query.Expression field, string alias)
        {
            return this;
        }

        public Queryable<T> Avg(Expression<Func<T, object>> field)
        {
            return this;
        }

        public Queryable<T> Avg(Expression<Func<T, object>> field, string alias)
        {
            return this;
        }

        public Queryable<T> Avg(Query.Expression field)
        {
            return this;
        }

        public Queryable<T> Avg(Query.Expression field, string alias)
        {
            return this;
        }

        public Queryable<T> Count(Expression<Func<T, object>> field)
        {
            return this;
        }

        public Queryable<T> Count(Expression<Func<T, object>> field, string alias)
        {
            return this;
        }

        public Queryable<T> Count(Query.Expression field)
        {
            return this;
        }

        public Queryable<T> Count(Query.Expression field, string alias)
        {
            return this;
        }

        public Queryable<T> Distinct()
        {
            distinct = true;
            return this;
        }

        public Queryable<T> Take(int top)
        {
            this.top = top;
            return this;
        }

        public Queryable<T> Where(Expression<Func<T, object>> field, object value)
        {
            return this;
        }

        public Queryable<T> Where(Expression<Func<T, object>> field, Operator op, object value)
        {
            return this;
        }

        public Queryable<T> Where(ConditionExpression condition)
        {
            return this;
        }

        public Queryable<T> GroupBy(Expression<Func<T, object>> field)
        {
            return this;
        }

        public Queryable<T> GroupBy(params Expression<Func<T, object>>[] fields)
        {
            return this;
        }

        public Queryable<T> GroupBy(Expression<Func<T, object>> field, ConditionExpression condition)
        {
            return this;
        }

        public Queryable<T> GroupBy(Expression field)
        {
            return this;
        }

        public Queryable<T> GroupBy(params Expression[] fields)
        {
            return this;
        }

        public Queryable<T> GroupBy(Expression field, ConditionExpression condition)
        {
            return this;
        }

        public Queryable<T> OrderBy(Expression<Func<T, object>> field)
        {
            return this;
        }

        public Queryable<T> OrderBy(Expression<Func<T, object>> field, bool descending)
        {
            return this;
        }

        public Queryable<T> OrderBy(Expression field)
        {
            return this;
        }

        public Queryable<T> OrderBy(Expression field, bool descending)
        {
            return this;
        }

        public override ExpressionType NodeType
        {
            get { return ExpressionType.Query; }
        }

        public override string OutputSqlString(SqlBuilder.ISqlBuilder builder, ParameterCollection output)
        {
            throw new NotImplementedException();
        }

        public string OutputSqlString(ParameterCollection output)
        {
            return OutputSqlString(database.Builder, output);
        }

        public IDataReader ExecuteReader()
        {
            var parameters = database.CreateParamterCollection();
            var sql = OutputSqlString(parameters);
            return database.ExecuteReader(sql, parameters);
        }

        public IList<T> ExecuteList()
        {
            var parameters = database.CreateParamterCollection();
            var sql = OutputSqlString(parameters);
            return database.ExecuteList<T>(sql, parameters);
        }

        public IList<TEntity> ExecuteList<TEntity>()
        {
            var parameters = database.CreateParamterCollection();
            var sql = OutputSqlString(parameters);
            return database.ExecuteList<TEntity>(sql, parameters);
        }

        #region DQL

        private bool distinct;
        private int top;

        private CollectionExpression _fields;
        private CollectionExpression Fields { get { return _fields = _fields ?? new CollectionExpression(); } }

        private CollectionExpression _expressionsForCondition;
        private CollectionExpression ExpressionsForCondition { get { return _expressionsForCondition = _expressionsForCondition ?? new CollectionExpression(); } }

        private ConditionExpression _condition;

        private CollectionExpression _groups;
        private CollectionExpression Groups { get { return _groups = _groups ?? new CollectionExpression(); } }

        private ConditionExpression _having;



        #endregion
    }
}

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
using Sparrow.CommonLibrary.Query;
using System.Collections;
using Sparrow.CommonLibrary.Database.SqlBuilder;
using System.Data.Common;

namespace Sparrow.CommonLibrary.Query
{
    /// <summary>
    /// 查询表达式
    /// </summary>
    public class Queryable<T> : SqlExpression
    {
        private readonly DatabaseHelper database;

        private readonly IObjectAccessor<T> accessor;
        private readonly IMetaPropertyInfo[] fields;

        public Queryable(DatabaseHelper database)
        {
            if (database == null)
                throw new ArgumentNullException("database");

            this.database = database;
            accessor = Map.GetCheckedAccessor<T>();
            fields = accessor.MetaInfo.GetProperties();
            Options = SqlOptions.NoLock;
        }

        public Queryable<T> Select(Expression<Func<T, object>> field)
        {
            Fields.Add(SqlExpression.Field(field));
            return this;
        }

        public Queryable<T> Select(Expression<Func<T, object>> field, string alias)
        {
            Fields.Add(SqlExpression.Field(field, alias));
            return this;
        }

        public Queryable<T> Select(params Expression<Func<T, object>>[] fields)
        {
            foreach (var field in fields)
                Fields.Add(SqlExpression.Field(field));
            return this;
        }

        public Queryable<T> Select(SqlExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Fields.Add(expression);
            return this;
        }

        public Queryable<T> Select(params SqlExpression[] expressions)
        {
            foreach (var exp in expressions)
                Fields.Add(exp);
            return this;
        }

        public Queryable<T> Sum(Expression<Func<T, object>> field)
        {
            return Sum(field, GetFieldName(field));
        }

        public Queryable<T> Sum(Expression<Func<T, object>> field, string alias)
        {
            Fields.Add(SqlExpression.Alias(SqlExpression.Function("SUM", SqlExpression.Field(field)), alias));
            return this;
        }

        public Queryable<T> Sum(SqlExpression expression, string alias)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Fields.Add(SqlExpression.Alias(SqlExpression.Function("SUM", expression), alias));
            return this;
        }

        public Queryable<T> Min(Expression<Func<T, object>> field)
        {
            return Min(field, GetFieldName(field));
        }

        public Queryable<T> Min(Expression<Func<T, object>> field, string alias)
        {
            Fields.Add(SqlExpression.Alias(SqlExpression.Function("MIN", SqlExpression.Field(field)), alias));
            return this;
        }

        public Queryable<T> Min(SqlExpression expression, string alias)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Fields.Add(SqlExpression.Alias(SqlExpression.Function("MIN", expression), alias));
            return this;
        }

        public Queryable<T> Max(Expression<Func<T, object>> field)
        {
            return Max(field, GetFieldName(field));
        }

        public Queryable<T> Max(Expression<Func<T, object>> field, string alias)
        {
            Fields.Add(SqlExpression.Alias(SqlExpression.Function("MAX", SqlExpression.Field(field)), alias));
            return this;
        }

        public Queryable<T> Max(SqlExpression expression, string alias)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Fields.Add(SqlExpression.Alias(SqlExpression.Function("MAX", expression), alias));
            return this;
        }

        public Queryable<T> Avg(Expression<Func<T, object>> field)
        {
            return Avg(field, GetFieldName(field));
        }

        public Queryable<T> Avg(Expression<Func<T, object>> field, string alias)
        {
            Fields.Add(SqlExpression.Alias(SqlExpression.Function("AVG", SqlExpression.Field(field)), alias));
            return this;
        }

        public Queryable<T> Avg(SqlExpression expression, string alias)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Fields.Add(SqlExpression.Alias(SqlExpression.Function("AVG", expression), alias));
            return this;
        }

        public Queryable<T> Count(Expression<Func<T, object>> field)
        {
            return Count(field, GetFieldName(field));
        }

        public Queryable<T> Count(Expression<Func<T, object>> field, string alias)
        {
            Fields.Add(SqlExpression.Alias(SqlExpression.Function("COUNT", SqlExpression.Field(field)), alias));
            return this;
        }

        public Queryable<T> Count(SqlExpression expression, string alias)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Fields.Add(SqlExpression.Alias(SqlExpression.Function("COUNT", expression), alias));
            return this;
        }

        public Queryable<T> Distinct()
        {
            distinct = true;
            return this;
        }

        public Queryable<T> Take(int top)
        {
            if (top < 1)
                throw new ArgumentException("top不能小于1。");
            this.top = top;
            return this;
        }

        public Queryable<T> Where(Expression<Func<T, object>> field, object value)
        {
            return Where(SqlExpression.Equal(field, value));
        }

        public Queryable<T> Where(Expression<Func<T, object>> field, Operator op, object value)
        {
            LogicalBinaryExpression logical;
            switch (op)
            {
                case Operator.Equal:
                    logical = SqlExpression.Equal(field, value);
                    break;
                case Operator.LessThan:
                    logical = SqlExpression.LessThan(field, value);
                    break;
                case Operator.LessThanOrEqual:
                    logical = SqlExpression.LessThanOrEqual(field, value);
                    break;
                case Operator.GreaterThan:
                    logical = SqlExpression.GreaterThan(field, value);
                    break;
                case Operator.GreaterThanOrEqual:
                    logical = SqlExpression.GreaterThanOrEqual(field, value);
                    break;
                case Operator.In:
                    logical = SqlExpression.In(field, value);
                    break;
                case Operator.StartWith:
                    logical = SqlExpression.Like(field, value, true, false);
                    break;
                case Operator.EndWith:
                    logical = SqlExpression.Like(field, value, false, true);
                    break;
                case Operator.Like:
                    logical = SqlExpression.Like(field, value, true, true);
                    break;
                case Operator.NotEqual:
                    logical = SqlExpression.NotEqual(field, value);
                    break;
                case Operator.Between:
                    var values = ((ICollection)value).Cast<object>();
                    if (values.Count() < 2)
                        throw new ArgumentException("value集合未包含两个元素。");
                    logical = SqlExpression.Between(field, values.First(), values.Skip(1).First());
                    break;
                case Operator.NotIn:
                    logical = SqlExpression.NotIn(field, value);
                    break;
                default:
                    throw new NotSupportedException(string.Format("不受支持的{0}", op));
            }
            return Where(logical);
        }

        public Queryable<T> Where(LogicalBinaryExpression logical)
        {
            if (logical == null)
                throw new ArgumentNullException("condition");

            if (_where == null)
            {
                _where = logical;
            }
            else
            {
                _where = SqlExpression.AndAlso(_where, logical);
            }
            return this;
        }

        public Queryable<T> Where(Expression<Func<T, bool>> logical)
        {
            return Where(LogicalBinaryExpression.Expression(logical));
        }

        public Queryable<T> GroupBy(Expression<Func<T, object>> field)
        {
            Groups.Add(SqlExpression.Field(field));
            return this;
        }

        public Queryable<T> GroupBy(params Expression<Func<T, object>>[] fields)
        {
            Groups.AddRang(fields.Select(x => SqlExpression.Field(x)));
            return this;
        }

        public Queryable<T> GroupBy(Expression<Func<T, object>> field, LogicalBinaryExpression having)
        {
            Groups.Add(SqlExpression.Field(field));
            if (having != null)
                _having = SqlExpression.AndAlso(_having, having);
            return this;
        }

        public Queryable<T> GroupBy(SqlExpression expression)
        {
            Groups.Add(expression);
            return this;
        }

        public Queryable<T> GroupBy(params SqlExpression[] fields)
        {
            Groups.AddRang(fields);
            return this;
        }

        public Queryable<T> GroupBy(SqlExpression field, LogicalBinaryExpression having)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            Groups.Add(field);
            if (having != null)
                _having = SqlExpression.AndAlso(_having, having);
            return this;
        }

        public Queryable<T> OrderBy(Expression<Func<T, object>> field)
        {
            return OrderBy(field, false);
        }

        public Queryable<T> OrderBy(Expression<Func<T, object>> field, bool descending)
        {
            return OrderBy(SqlExpression.Field(field), descending);
        }

        public Queryable<T> OrderBy(SqlExpression expression)
        {
            return OrderBy(expression, false);
        }

        public Queryable<T> OrderBy(SqlExpression expression, bool descending)
        {
            Orders[expression] = descending;
            return this;
        }

        /// <summary>
        /// 启用分页查询
        /// </summary>
        /// <param name="startIndex">从0行开始的起始行，小于0则表示不使用分页查询。</param>
        /// <param name="rowCount">数据返回的行数</param>
        /// <returns></returns>
        public Queryable<T> RowLimit(int startIndex, int rowCount)
        {
            this.startIndex = startIndex;
            this.rowCount = rowCount;
            return this;
        }

        public SqlOptions Options { get; set; }

        public override ExpressionType NodeType
        {
            get { return ExpressionType.Query; }
        }

        public override string OutputSqlString(ISqlBuilder builder, ParameterCollection output)
        {
            if (_fields == null || _fields.Count == 0)
                Fields.AddRang(accessor.MetaInfo.GetPropertyNames().Select(x => SqlExpression.Field(x)));

            var topExpressions = top > 0 ? builder.Constant(top) : string.Empty;
            var fieldExpressions = Fields.OutputSqlString(builder, output);
            var tableExpression = builder.BuildTableName(accessor.MetaInfo.Name);
            var conditionExpressions = _where != null ? _where.OutputSqlString(builder, output) : string.Empty;
            var groupbyExpression = _groups != null && _groups.Count > 0 ? _groups.OutputSqlString(builder, output) : string.Empty;
            var havingExpression = _groups != null && _groups.Count > 0 && _having != null ? _having.OutputSqlString(builder, output) : string.Empty;
            var orderbyExpression = _orders != null && _orders.Count > 0 ? string.Join(",", _orders.Select(x => string.Concat(x.Key.OutputSqlString(builder, output), " ", x.Value ? "DESC" : "ASC"))) : string.Empty;

            if (startIndex < 0)
            {
                return builder.QueryFormat(
                    topExpressions,//top(10)
                    fieldExpressions,//id,name,...
                    tableExpression,//tableName
                    conditionExpressions,//where id>1 and...
                    groupbyExpression,//group by name....
                    havingExpression,//having count(name)>1 ...
                    orderbyExpression,//order id
                    distinct ? Options | SqlOptions.Distinct : Options
                    );
            }
            else
            {
                return builder.QueryFormat(
                    fieldExpressions,//id,name,...
                    tableExpression,//tableName
                    conditionExpressions,//where id>1 and...
                    groupbyExpression,//group by name....
                    havingExpression,//having count(name)>1 ...
                    orderbyExpression,//order id
                    startIndex,
                    rowCount,
                    distinct ? Options | SqlOptions.Distinct : Options
                    );
            }
        }

        public string OutputSqlString(ParameterCollection output)
        {
            return OutputSqlString(database.Builder, output);
        }

        #region Execute

        public IDataReader ExecuteReader()
        {
            var parameters = database.CreateParamterCollection();
            var sql = OutputSqlString(parameters);
            return database.ExecuteReader(sql, parameters);
        }

        public IDataReader ExecuteReader(DbTransaction dbTransaction)
        {
            var parameters = database.CreateParamterCollection();
            var sql = OutputSqlString(parameters);
            return database.ExecuteReader(sql, parameters, dbTransaction);
        }

        public IList<T> ExecuteList()
        {
            var parameters = database.CreateParamterCollection();
            var sql = OutputSqlString(parameters);
            return database.ExecuteList<T>(sql, parameters);
        }

        public IList<T> ExecuteList(DbTransaction dbTransaction)
        {
            var parameters = database.CreateParamterCollection();
            var sql = OutputSqlString(parameters);
            return database.ExecuteList<T>(sql, parameters, dbTransaction);
        }

        public IList<TEntity> ExecuteList<TEntity>()
        {
            var parameters = database.CreateParamterCollection();
            var sql = OutputSqlString(parameters);
            return database.ExecuteList<TEntity>(sql, parameters);
        }

        public IList<TEntity> ExecuteList<TEntity>(DbTransaction dbTransaction)
        {
            var parameters = database.CreateParamterCollection();
            var sql = OutputSqlString(parameters);
            return database.ExecuteList<TEntity>(sql, parameters, dbTransaction);
        }

        public TType ExecuteScalar<TType>()
        {
            var parameters = database.CreateParamterCollection();
            var sql = OutputSqlString(parameters);
            return database.ExecuteScalar<TType>(sql, parameters);
        }

        public object ExecuteScalar()
        {
            var parameters = database.CreateParamterCollection();
            var sql = OutputSqlString(parameters);
            var cmd = database.BuildDbCommand(CommandType.Text, sql, parameters);
            return database.ExecuteScalar(cmd);
        }

        #endregion

        private string GetFieldName(Expression<Func<T, object>> field)
        {
            var propertyInfo = (PropertyInfo)PropertyExpression.ExtractMemberExpression(field).Member;
            var fieldInfo = Map.GetCheckedAccessor<T>().MetaInfo[propertyInfo];
            if (fieldInfo != null)
                return fieldInfo.PropertyName;
            throw new ArgumentException("无法获取该属性所映射的成员字段。");
        }

        #region DQL

        private bool distinct;
        private int top;

        private int startIndex = -1;
        private int rowCount = 0;

        private CollectionExpression _fields;
        private CollectionExpression Fields { get { return _fields = _fields ?? new CollectionExpression(); } }

        private LogicalBinaryExpression _where;

        private CollectionExpression _groups;
        private CollectionExpression Groups { get { return _groups = _groups ?? new CollectionExpression(); } }

        private LogicalBinaryExpression _having;

        private IDictionary<SqlExpression, bool> _orders;
        private IDictionary<SqlExpression, bool> Orders { get { return _orders = _orders ?? new Dictionary<SqlExpression, bool>(); } }

        #endregion
    }
}

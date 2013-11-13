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

namespace Sparrow.CommonLibrary.Query
{
    /// <summary>
    /// 查询表达式
    /// </summary>
    public class Queryable<T> : SqlExpression
    {
        private readonly DatabaseHelper database;

        private readonly IMapper<T> mapper;
        private readonly IMetaFieldInfo[] fields;

        public Queryable(DatabaseHelper database)
        {
            if (database == null)
                throw new ArgumentNullException("database");

            this.database = database;
            mapper = Map.GetIMapper<T>();
            fields = mapper.MetaInfo.GetFields();
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
            Fields.Add(SqlExpression.Alias(SqlExpression.Function("Count", SqlExpression.Field(field)), alias));
            return this;
        }

        public Queryable<T> Count(SqlExpression expression, string alias)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Fields.Add(SqlExpression.Alias(SqlExpression.Function("Count", expression), alias));
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
            CompareExpression compare;
            switch (op)
            {
                case Operator.Equal:
                    compare = SqlExpression.Equal(field, value);
                    break;
                case Operator.LessThan:
                    compare = SqlExpression.LessThan(field, value);
                    break;
                case Operator.LessThanOrEqual:
                    compare = SqlExpression.LessThanOrEqual(field, value);
                    break;
                case Operator.GreaterThan:
                    compare = SqlExpression.GreaterThan(field, value);
                    break;
                case Operator.GreaterThanOrEqual:
                    compare = SqlExpression.GreaterThanOrEqual(field, value);
                    break;
                case Operator.In:
                    compare = SqlExpression.In(field, value);
                    break;
                case Operator.StartWith:
                    compare = SqlExpression.Like(field, value, true, false);
                    break;
                case Operator.EndWith:
                    compare = SqlExpression.Like(field, value, false, true);
                    break;
                case Operator.Like:
                    compare = SqlExpression.Like(field, value, true, true);
                    break;
                case Operator.NotEqual:
                    compare = SqlExpression.NotEqual(field, value);
                    break;
                case Operator.Between:
                    var values = ((ICollection)value).Cast<object>();
                    if (values.Count() < 2)
                        throw new ArgumentException("value集合未包含两个元素。");
                    compare = SqlExpression.Between(field, values.First(), values.Skip(1).First());
                    break;
                default:
                    throw new NotSupportedException(string.Format("不受支持的{0}", op));
            }
            return Where(compare);
        }

        public Queryable<T> Where(CompareExpression condition)
        {
            if (condition == null)
                throw new ArgumentNullException("condition");

            if (_condition == null)
                _condition = SqlExpression.AndAlso(condition, null);
            else
                _condition = SqlExpression.AndAlso(_condition, condition);
            return this;
        }

        public Queryable<T> Where(ConditionExpression condition)
        {
            if (condition == null)
                throw new ArgumentNullException("condition");

            if (_condition == null)
            {
                _condition = condition;
            }
            else
            {
                _condition = SqlExpression.AndAlso(_condition, condition);
            }
            return this;
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

        public Queryable<T> GroupBy(Expression<Func<T, object>> field, ConditionExpression having)
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

        public Queryable<T> GroupBy(SqlExpression field, ConditionExpression having)
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
                Fields.AddRang(mapper.MetaInfo.GetFieldNames().Select(x => SqlExpression.Field(x)));

            var topExpressions = top > 0 ? builder.Constant(top) : string.Empty;
            var fieldExpressions = Fields.OutputSqlString(builder, output);
            var tableExpression = builder.BuildTableName(mapper.MetaInfo.Name);
            var conditionExpressions = _condition != null ? _condition.OutputSqlString(builder, output) : string.Empty;
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
                    distinct ? Options & SqlOptions.Distinct : Options
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
                    distinct ? Options & SqlOptions.Distinct : Options
                    );
            }
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

        private string GetFieldName(Expression<Func<T, object>> field)
        {
            var propertyInfo = (PropertyInfo)PropertyExpression.ExtractMemberExpression(field).Member;
            var fieldInfo = Map.GetIMapper<T>().MetaInfo[propertyInfo];
            if (fieldInfo != null)
                return fieldInfo.FieldName;
            throw new ArgumentException("无法获取该属性所映射的成员字段。");
        }

        #region DQL

        private bool distinct;
        private int top;

        private int startIndex = -1;
        private int rowCount = 0;

        private CollectionExpression _fields;
        private CollectionExpression Fields { get { return _fields = _fields ?? new CollectionExpression(); } }

        private ConditionExpression _condition;

        private CollectionExpression<T> _groups;
        private CollectionExpression<T> Groups { get { return _groups = _groups ?? new CollectionExpression<T>(); } }

        private ConditionExpression _having;

        private IDictionary<SqlExpression, bool> _orders;
        private IDictionary<SqlExpression, bool> Orders { get { return _orders = _orders ?? new Dictionary<SqlExpression, bool>(); } }

        #endregion
    }
}

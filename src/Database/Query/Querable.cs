﻿using Sparrow.CommonLibrary.Database;
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
using System.Collections;

namespace Sparrow.CommonLibrary.Database.Query
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
            this.database = database;
            mapper = MapperManager.GetIMapper<T>();
            fields = mapper.MetaInfo.GetFields();
        }

        public Queryable<T> Select(Expression<Func<T, object>> field)
        {
            _fields.Add(SqlExpression.Field(field));
            return this;
        }

        public Queryable<T> Select(Expression<Func<T, object>> field, string alias)
        {
            _fields.Add(SqlExpression.Field(field, alias));
            return this;
        }

        public Queryable<T> Select(params Expression<Func<T, object>>[] fields)
        {
            foreach (var field in fields)
                _fields.Add(SqlExpression.Field(field));
            return this;
        }

        public Queryable<T> Select(SqlExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            _fields.Add(expression);
            return this;
        }

        public Queryable<T> Select(params SqlExpression[] expressions)
        {
            foreach (var exp in expressions)
                _fields.Add(exp);
            return this;
        }

        public Queryable<T> Sum(Expression<Func<T, object>> field)
        {
            return Sum(field, GetFieldName(field));
        }

        public Queryable<T> Sum(Expression<Func<T, object>> field, string alias)
        {
            _fields.Add(SqlExpression.Alias(SqlExpression.Function("SUM", SqlExpression.Field(field)), alias));
            return this;
        }

        public Queryable<T> Sum(SqlExpression expression, string alias)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            _fields.Add(SqlExpression.Alias(SqlExpression.Function("SUM", expression), alias));
            return this;
        }

        public Queryable<T> Min(Expression<Func<T, object>> field)
        {
            return Min(field, GetFieldName(field));
        }

        public Queryable<T> Min(Expression<Func<T, object>> field, string alias)
        {
            _fields.Add(SqlExpression.Alias(SqlExpression.Function("MIN", SqlExpression.Field(field)), alias));
            return this;
        }

        public Queryable<T> Min(SqlExpression expression, string alias)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            _fields.Add(SqlExpression.Alias(SqlExpression.Function("MIN", expression), alias));
            return this;
        }

        public Queryable<T> Max(Expression<Func<T, object>> field)
        {
            return Max(field, GetFieldName(field));
        }

        public Queryable<T> Max(Expression<Func<T, object>> field, string alias)
        {
            _fields.Add(SqlExpression.Alias(SqlExpression.Function("MAX", SqlExpression.Field(field)), alias));
            return this;
        }

        public Queryable<T> Max(SqlExpression expression, string alias)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            _fields.Add(SqlExpression.Alias(SqlExpression.Function("MAX", expression), alias));
            return this;
        }

        public Queryable<T> Avg(Expression<Func<T, object>> field)
        {
            return Avg(field, GetFieldName(field));
        }

        public Queryable<T> Avg(Expression<Func<T, object>> field, string alias)
        {
            _fields.Add(SqlExpression.Alias(SqlExpression.Function("AVG", SqlExpression.Field(field)), alias));
            return this;
        }

        public Queryable<T> Avg(SqlExpression expression, string alias)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            _fields.Add(SqlExpression.Alias(SqlExpression.Function("AVG", expression), alias));
            return this;
        }

        public Queryable<T> Count(Expression<Func<T, object>> field)
        {
            return Count(field, GetFieldName(field));
        }

        public Queryable<T> Count(Expression<Func<T, object>> field, string alias)
        {
            _fields.Add(SqlExpression.Alias(SqlExpression.Function("Count", SqlExpression.Field(field)), alias));
            return this;
        }

        public Queryable<T> Count(SqlExpression expression, string alias)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            _fields.Add(SqlExpression.Alias(SqlExpression.Function("Count", expression), alias));
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
            _expressionsForCondition.Add(SqlExpression.Equal(field, value));
            return this;
        }

        public Queryable<T> Where(Expression<Func<T, object>> field, Operator op, object value)
        {
            switch (op)
            {
                case Operator.Equal:
                    _expressionsForCondition.Add(SqlExpression.Equal(field, value));
                    break;
                case Operator.LessThan:
                    _expressionsForCondition.Add(SqlExpression.LessThan(field, value));
                    break;
                case Operator.LessThanOrEqual:
                    _expressionsForCondition.Add(SqlExpression.LessThanOrEqual(field, value));
                    break;
                case Operator.GreaterThan:
                    _expressionsForCondition.Add(SqlExpression.GreaterThan(field, value));
                    break;
                case Operator.GreaterThanOrEqual:
                    _expressionsForCondition.Add(SqlExpression.GreaterThanOrEqual(field, value));
                    break;
                case Operator.In:
                    _expressionsForCondition.Add(SqlExpression.In(field, value));
                    break;
                case Operator.StartWith:
                    _expressionsForCondition.Add(SqlExpression.Like(field, value, true, false));
                    break;
                case Operator.EndWith:
                    _expressionsForCondition.Add(SqlExpression.Like(field, value, false, true));
                    break;
                case Operator.Like:
                    _expressionsForCondition.Add(SqlExpression.Like(field, value, true, true));
                    break;
                case Operator.NotEqual:
                    _expressionsForCondition.Add(SqlExpression.NotEqual(field, value));
                    break;
                case Operator.Between:
                    var values = ((ICollection)value).Cast<object>();
                    if (values.Count() < 2)
                        throw new ArgumentException("value集合未包含两个元素。");
                    _expressionsForCondition.Add(SqlExpression.Between(field, values.First(), values.Skip(1).First()));
                    break;
                default:
                    throw new NotSupportedException(string.Format("不受支持的{0}", op));
            }
            return this;
        }

        public Queryable<T> Where(CompareExpression condition)
        {
            if (condition == null)
                throw new ArgumentNullException("condition");

            _expressionsForCondition.Add(condition);
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

        public Queryable<T> GroupBy(SqlExpression field)
        {
            return this;
        }

        public Queryable<T> GroupBy(params SqlExpression[] fields)
        {
            return this;
        }

        public Queryable<T> GroupBy(SqlExpression field, ConditionExpression condition)
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

        public Queryable<T> OrderBy(SqlExpression field)
        {
            return this;
        }

        public Queryable<T> OrderBy(SqlExpression field, bool descending)
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

        private string GetFieldName(Expression<Func<T, object>> field)
        {
            var propertyInfo = (PropertyInfo)PropertyExpression.ExtractMemberExpression(field).Member;
            var fieldInfo = MapperManager.GetIMapper<T>().MetaInfo[propertyInfo];
            if (fieldInfo != null)
                return fieldInfo.FieldName;
            throw new ArgumentException("无法获取该属性所映射的成员字段。");
        }

        #region DQL

        private bool distinct;
        private int top;

        private CollectionExpression _fields;
        private CollectionExpression Fields { get { return _fields = _fields ?? new CollectionExpression(); } }

        private CollectionExpression<T> _expressionsForCondition;
        private CollectionExpression<T> ExpressionsForCondition { get { return _expressionsForCondition = _expressionsForCondition ?? new CollectionExpression<T>(); } }

        private ConditionExpression _condition;

        private CollectionExpression<T> _groups;
        private CollectionExpression<T> Groups { get { return _groups = _groups ?? new CollectionExpression<T>(); } }

        private ConditionExpression _having;



        #endregion
    }
}

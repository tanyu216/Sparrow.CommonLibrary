using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Database.SqlBuilder;

namespace Sparrow.CommonLibrary.Database.Query
{
    /// <summary>
    /// Sql表达式最终类
    /// </summary>
    public abstract class SqlExpression
    {
        /// <summary>
        /// 表达式类型
        /// </summary>
        public abstract ExpressionType NodeType { get; }

        /// <summary>
        /// 表达式输出成符合指定数据库的Sql脚本
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public abstract string OutputSqlString(ISqlBuilder builder, ParameterCollection output);

        public static BinaryExpression Add(SqlExpression left, SqlExpression right)
        {
            return BinaryExpression.Expression(ExpressionType.Add, left, right);
        }

        public static BinaryExpression Divide(SqlExpression left, SqlExpression right)
        {
            return BinaryExpression.Expression(ExpressionType.Divide, left, right);
        }

        public static BinaryExpression Modulo(SqlExpression left, SqlExpression right)
        {
            return BinaryExpression.Expression(ExpressionType.Modulo, left, right);
        }

        public static BinaryExpression Multiply(SqlExpression left, SqlExpression right)
        {
            return BinaryExpression.Expression(ExpressionType.Multiply, left, right);
        }

        public static BinaryExpression Subtract(SqlExpression left, SqlExpression right)
        {
            return BinaryExpression.Expression(ExpressionType.Subtract, left, right);
        }

        public static ConditionExpression AndAlso(SqlExpression left, SqlExpression right)
        {
            return ConditionExpression.Condition(ExpressionType.AndAlso, left, right);
        }

        public static ConditionExpression OrElse(SqlExpression left, SqlExpression right)
        {
            return ConditionExpression.Condition(ExpressionType.OrElse, left, right);
        }

        public static CompareExpression Equal(SqlExpression left, SqlExpression right)
        {
            return CompareExpression.Expression(ExpressionType.Equal, left, right);
        }

        public static CompareExpression Equal(string fieldName, object value)
        {
            return Equal(SqlExpression.Field(fieldName), SqlExpression.Constant(value));
        }

        public static CompareExpression Equal<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return Equal(Field<T>(field), Constant(value));
        }

        public static CompareExpression IsNull(SqlExpression left)
        {
            return CompareExpression.Expression(ExpressionType.IsNull, left, DbNullExpression.Instance);
        }

        public static CompareExpression IsNull(string fieldName)
        {
            return IsNull(SqlExpression.Field(fieldName));
        }

        public static CompareExpression IsNull<T>(System.Linq.Expressions.Expression<Func<T, object>> field)
        {
            return IsNull(Field<T>(field));
        }

        public static CompareExpression IsNotNull(SqlExpression left)
        {
            return CompareExpression.Expression(ExpressionType.IsNotNull, left, DbNullExpression.Instance);
        }

        public static CompareExpression IsNotNull(string fieldName)
        {
            return IsNotNull(SqlExpression.Field(fieldName));
        }

        public static CompareExpression IsNotNull<T>(System.Linq.Expressions.Expression<Func<T, object>> field)
        {
            return IsNotNull(Field<T>(field));
        }

        public static CompareExpression GreaterThan(SqlExpression left, SqlExpression right)
        {
            return CompareExpression.Expression(ExpressionType.GreaterThan, left, right);
        }

        public static CompareExpression GreaterThan(string fieldName, object value)
        {
            return GreaterThan(SqlExpression.Field(fieldName), SqlExpression.Constant(value));
        }

        public static CompareExpression GreaterThan<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return GreaterThan(Field<T>(field), Constant(value));
        }

        public static CompareExpression GreaterThanOrEqual(SqlExpression left, SqlExpression right)
        {
            return CompareExpression.Expression(ExpressionType.GreaterThanOrEqual, left, right);
        }

        public static CompareExpression GreaterThanOrEqual(string fieldName, object value)
        {
            return GreaterThanOrEqual(SqlExpression.Field(fieldName), SqlExpression.Constant(value));
        }

        public static CompareExpression GreaterThanOrEqual<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return GreaterThanOrEqual(Field<T>(field), Constant(value));
        }

        public static CompareExpression LessThan(SqlExpression left, SqlExpression right)
        {
            return CompareExpression.Expression(ExpressionType.LessThan, left, right);
        }

        public static CompareExpression LessThan(string fieldName, object value)
        {
            return LessThan(SqlExpression.Field(fieldName), SqlExpression.Constant(value));
        }

        public static CompareExpression LessThan<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return LessThan(Field<T>(field), Constant(value));
        }

        public static CompareExpression LessThanOrEqual(SqlExpression left, SqlExpression right)
        {
            return CompareExpression.Expression(ExpressionType.LessThanOrEqual, left, right);
        }

        public static CompareExpression LessThanOrEqual(string fieldName, object value)
        {
            return LessThanOrEqual(SqlExpression.Field(fieldName), SqlExpression.Constant(value));
        }

        public static CompareExpression LessThanOrEqual<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return LessThanOrEqual(Field<T>(field), Constant(value));
        }

        public static CompareExpression NotEqual(SqlExpression left, SqlExpression right)
        {
            return CompareExpression.Expression(ExpressionType.NotEqual, left, right);
        }

        public static CompareExpression NotEqual(string fieldName, object value)
        {
            return NotEqual(SqlExpression.Field(fieldName), SqlExpression.Constant(value));
        }

        public static CompareExpression NotEqual<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return NotEqual(Field<T>(field), Constant(value));
        }

        public static CompareExpression In(SqlExpression left, CollectionExpression right)
        {
            return CompareExpression.Expression(ExpressionType.In, left, right);
        }

        public static CompareExpression In(string fieldName, params object[] value)
        {
            return In(SqlExpression.Field(fieldName), new CollectionExpression(value.Select(x => Constant(x))));
        }

        public static CompareExpression In<T>(System.Linq.Expressions.Expression<Func<T, object>> field, params object[] values)
        {
            return In(Field<T>(field), new CollectionExpression(values.Select(x => Constant(x))));
        }

        public static CompareExpression Between(SqlExpression left, SqlExpression value1, SqlExpression value2)
        {
            return CompareExpression.Expression(ExpressionType.Between, left, new CollectionExpression() { value1, value2 });
        }

        public static CompareExpression Between(string fieldName, object value1, object value2)
        {
            return Between(SqlExpression.Field(fieldName), SqlExpression.Constant(value1), Constant(value2));
        }

        public static CompareExpression Between<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value1, object value2)
        {
            return Between(Field<T>(field), Constant(value1), Constant(value2));
        }

        public static CompareExpression Like(SqlExpression left, SqlExpression right, bool startWith, bool endWith)
        {
            return CompareExpression.ExpressionForLike(left, right, startWith, endWith);
        }

        public static CompareExpression Like(string fieldName, object value)
        {
            return Like(fieldName, value, true, true);
        }

        public static CompareExpression Like(string fieldName, object value, bool startWith, bool endWith)
        {
            return Like(SqlExpression.Field(fieldName), SqlExpression.Constant(value), startWith, endWith);
        }

        public static CompareExpression Like<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return Like(Field<T>(field), Constant(value), true, true);
        }

        public static CompareExpression Like<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value, bool startWith, bool endWith)
        {
            return Like(Field<T>(field), Constant(value), startWith, endWith);
        }

        public static ConstantExpression Constant(object value)
        {
            return ConstantExpression.Expression(value);
        }

        public static FunctionExpression Function(string name, SqlExpression expression)
        {
            return FunctionExpression.Expression(name, expression);
        }

        public static FunctionExpression Function(string name, params object[] parameters)
        {
            return FunctionExpression.Expression(name, parameters.Select(x => Constant(x)).ToArray());
        }

        public static ParameterExpression Parameter(string name, object value)
        {
            return ParameterExpression.Expression(name, value);
        }

        public static VariableNameExpression Variable(string name)
        {
            return VariableNameExpression.Variable(name);
        }

        public static DbNullExpression DbNull()
        {
            return DbNullExpression.Instance;
        }

        public static FieldExpression Field(string name)
        {
            return FieldExpression.Expression(name);
        }

        public static FieldExpression Field(string name, string alias)
        {
            return FieldExpression.Expression(name, alias);
        }

        public static FieldExpression Field(string name, string alias, string tableName)
        {
            return FieldExpression.Expression(name, alias, tableName);
        }

        public static FieldExpression Field<T>(System.Linq.Expressions.Expression<Func<T, object>> field)
        {
            return FieldExpression<T>.Expression(field);
        }

        public static FieldExpression Field<T>(System.Linq.Expressions.Expression<Func<T, object>> field, string alias)
        {
            return FieldExpression<T>.Expression(field, alias);
        }

        public static FieldExpression Field<T>(System.Linq.Expressions.Expression<Func<T, object>> field, string alias, string tableName)
        {
            return FieldExpression<T>.Expression(field, alias, tableName);
        }

        public static AliasExpression Alias(SqlExpression expression, string alias)
        {
            return AliasExpression.Expression(expression, alias);
        }
    }
}

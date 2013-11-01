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
    public abstract class Expression
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

        public static BinaryExpression Add(Expression left, Expression right)
        {
            return BinaryExpression.Expression(ExpressionType.Add, left, right);
        }

        public static BinaryExpression Divide(Expression left, Expression right)
        {
            return BinaryExpression.Expression(ExpressionType.Divide, left, right);
        }

        public static BinaryExpression Modulo(Expression left, Expression right)
        {
            return BinaryExpression.Expression(ExpressionType.Modulo, left, right);
        }

        public static BinaryExpression Multiply(Expression left, Expression right)
        {
            return BinaryExpression.Expression(ExpressionType.Multiply, left, right);
        }

        public static BinaryExpression Subtract(Expression left, Expression right)
        {
            return BinaryExpression.Expression(ExpressionType.Subtract, left, right);
        }

        public static ConditionExpression AndAlso(Expression left, Expression right)
        {
            return ConditionExpression.Condition(ExpressionType.AndAlso, left, right);
        }

        public static ConditionExpression OrElse(Expression left, Expression right)
        {
            return ConditionExpression.Condition(ExpressionType.OrElse, left, right);
        }

        public static CompareExpression Equal(Expression left, Expression right)
        {
            return CompareExpression.Expression(ExpressionType.Equal, left, right);
        }

        public static CompareExpression Equal<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return Equal(Field<T>(field), Constant(value));
        }

        public static CompareExpression IsNull(Expression left)
        {
            return CompareExpression.Expression(ExpressionType.IsNull, left, DbNullExpression.DbNull);
        }

        public static CompareExpression IsNull<T>(System.Linq.Expressions.Expression<Func<T, object>> field)
        {
            return IsNull(Field<T>(field));
        }

        public static CompareExpression IsNotNull(Expression left)
        {
            return CompareExpression.Expression(ExpressionType.IsNotNull, left, DbNullExpression.DbNull);
        }

        public static CompareExpression IsNotNull<T>(System.Linq.Expressions.Expression<Func<T, object>> field)
        {
            return IsNotNull(Field<T>(field));
        }

        public static CompareExpression GreaterThan(Expression left, Expression right)
        {
            return CompareExpression.Expression(ExpressionType.GreaterThan, left, right);
        }

        public static CompareExpression GreaterThan<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return GreaterThan(Field<T>(field), Constant(value));
        }

        public static CompareExpression GreaterThanOrEqual(Expression left, Expression right)
        {
            return CompareExpression.Expression(ExpressionType.GreaterThanOrEqual, left, right);
        }

        public static CompareExpression GreaterThanOrEqual<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return GreaterThanOrEqual(Field<T>(field), Constant(value));
        }

        public static CompareExpression LessThan(Expression left, Expression right)
        {
            return CompareExpression.Expression(ExpressionType.LessThan, left, right);
        }

        public static CompareExpression LessThan<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return LessThan(Field<T>(field), Constant(value));
        }

        public static CompareExpression LessThanOrEqual(Expression left, Expression right)
        {
            return CompareExpression.Expression(ExpressionType.LessThanOrEqual, left, right);
        }

        public static CompareExpression LessThanOrEqual<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return LessThanOrEqual(Field<T>(field), Constant(value));
        }

        public static CompareExpression NotEqual(Expression left, Expression right)
        {
            return CompareExpression.Expression(ExpressionType.NotEqual, left, right);
        }

        public static CompareExpression NotEqual<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return NotEqual(Field<T>(field), Constant(value));
        }

        public static CompareExpression In(Expression left, CollectionExpression right)
        {
            return CompareExpression.Expression(ExpressionType.In, left, right);
        }

        public static CompareExpression In<T>(System.Linq.Expressions.Expression<Func<T, object>> field, params object[] values)
        {
            return In(Field<T>(field), new CollectionExpression(values.Select(x => Constant(x))));
        }

        public static CompareExpression Between(Expression left, Expression value1, Expression value2)
        {
            return CompareExpression.Expression(ExpressionType.Between, left, new CollectionExpression() { value1, value2 });
        }

        public static CompareExpression Between<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value1, object value2)
        {
            return Between(Field<T>(field), Constant(value1), Constant(value2));
        }

        public static CompareExpression Like(Expression left, Expression right, bool startWith, bool endWith)
        {
            return CompareExpression.ExpressionForLike(left, right, startWith, endWith);
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

        public static DbNullExpression DbNull()
        {
            return DbNullExpression.DbNull;
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
    }
}

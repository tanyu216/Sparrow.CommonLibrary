using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Database.SqlBuilder;
using Sparrow.CommonLibrary.Mapper;
using Binary = System.Linq.Expressions.BinaryExpression;
using LqExpression = System.Linq.Expressions.Expression;
using LqExpressionType = System.Linq.Expressions.ExpressionType;

namespace Sparrow.CommonLibrary.Query
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

        private static bool IsEnumerator(Type type)
        {
            if (type.IsArray || type == typeof(IList) || type == typeof(ArrayList) || type == typeof(ICollection) || type == typeof(IEnumerable))
                return true;
            else if (type == typeof(object))
                return false;
            else
            {
                foreach (var baseType in type.GetInterfaces())
                    if (IsEnumerator(type))
                        return true;
            }
            return false;
        }

        internal static SqlExpression Expression(System.Linq.Expressions.Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            switch (expression.NodeType)
            {
                case LqExpressionType.AndAlso:
                    return AndAlso(Expression(((Binary)expression).Left), Expression(((Binary)expression).Right));

                case LqExpressionType.Call:
                    var methodCall = ((MethodCallExpression)expression);
                    if (methodCall.Method.Name == "Contains" && methodCall.Method.ReturnType == typeof(bool))
                    {
                        //System.String的Contants方法。
                        if (methodCall.Method.ReflectedType == typeof(System.String))
                        {
                            return Like(Expression(methodCall.Object), Expression(methodCall.Arguments[0]), true, true);
                        }
                        //System.Linq.Enumerable的Contants方法。注：这是一个扩展方法
                        if (methodCall.Method.ReflectedType == typeof(System.Linq.Enumerable))
                        {
                            if (methodCall.Arguments[1] is MemberExpression && ((MemberExpression)methodCall.Arguments[1]).Expression.NodeType == LqExpressionType.Parameter)
                            {
                                return In(Expression(methodCall.Arguments[1]), (CollectionExpression)Expression(methodCall.Arguments[0]));
                            }
                        }
                        //IList<>/IList的Contants方法
                        if (methodCall.Method.ReflectedType.GetInterfaces().Any(x => x == typeof(IList) || x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>)))
                        {
                            if (methodCall.Arguments[0] is MemberExpression && ((MemberExpression)methodCall.Arguments[0]).Expression.NodeType == LqExpressionType.Parameter)
                            {
                                return In(Expression(methodCall.Arguments[0]), (CollectionExpression)Expression(methodCall.Object));
                            }
                        }
                    }
                    else if (methodCall.Method.Name == "StartsWith" && methodCall.Method.ReturnType == typeof(bool))
                    {
                        return Like(Expression(methodCall.Object), Expression(methodCall.Arguments[0]), true, false);
                    }
                    else if (methodCall.Method.Name == "EndsWith" && methodCall.Method.ReturnType == typeof(bool))
                    {
                        return Like(Expression(methodCall.Object), Expression(methodCall.Arguments[0]), false, true);
                    }
                    else
                    {
                        if (methodCall.Method.ReturnType.IsValueType ||
                            methodCall.Method.ReturnType == typeof(System.String))
                        {
                            var obj = LqExpression.Lambda<Func<object>>(LqExpression.MakeUnary(LqExpressionType.Convert, methodCall, typeof(object))).Compile()();
                            return Constant(obj);
                        }
                        else if (IsEnumerator(methodCall.Method.ReturnType))
                        {
                            var obj = LqExpression.Lambda<Func<object>>(LqExpression.MakeUnary(LqExpressionType.Convert, methodCall, typeof(object))).Compile()();
                            var exps = new CollectionExpression();
                            foreach (object item in (IEnumerable)obj)
                            {
                                exps.Add(Constant(item));
                            }
                            return exps;
                        }
                    }

                    throw new NotSupportedException("不支持的函数调用。");

                case LqExpressionType.Equal:
                    var left = Expression(((Binary)expression).Left);
                    var right = Expression(((Binary)expression).Right);

                    if (left is CollectionExpression)
                        return In(right, (CollectionExpression)left);

                    if (right is CollectionExpression)
                        return In(left, (CollectionExpression)right);

                    return Equal(left, right);

                case LqExpressionType.GreaterThan:
                    return GreaterThan(Expression(((Binary)expression).Left), Expression(((Binary)expression).Right));

                case LqExpressionType.GreaterThanOrEqual:
                    return GreaterThanOrEqual(Expression(((Binary)expression).Left), Expression(((Binary)expression).Right));

                case LqExpressionType.LessThan:
                    return LessThan(Expression(((Binary)expression).Left), Expression(((Binary)expression).Right));

                case LqExpressionType.LessThanOrEqual:
                    return LessThanOrEqual(Expression(((Binary)expression).Left), Expression(((Binary)expression).Right));

                case LqExpressionType.NotEqual:
                    return NotEqual(Expression(((Binary)expression).Left), Expression(((Binary)expression).Right));

                case LqExpressionType.OrElse:
                    return OrElse(Expression(((Binary)expression).Left), Expression(((Binary)expression).Right));

                case LqExpressionType.Add:
                    return Add(Expression(((Binary)expression).Left), Expression(((Binary)expression).Right));

                case LqExpressionType.Divide:
                    return Divide(Expression(((Binary)expression).Left), Expression(((Binary)expression).Right));

                case LqExpressionType.Modulo:
                    return Modulo(Expression(((Binary)expression).Left), Expression(((Binary)expression).Right));

                case LqExpressionType.Multiply:
                    return Multiply(Expression(((Binary)expression).Left), Expression(((Binary)expression).Right));

                case LqExpressionType.Subtract:
                    return Subtract(Expression(((Binary)expression).Left), Expression(((Binary)expression).Right));

                case LqExpressionType.Constant:
                    return Constant(((System.Linq.Expressions.ConstantExpression)expression).Value);

                case LqExpressionType.Convert:
                    return Expression(((UnaryExpression)expression).Operand);

                case LqExpressionType.MemberAccess:

                    if (((MemberExpression)expression).Expression == null)
                    {
                        var obj = LqExpression.Lambda<Func<object>>(LqExpression.MakeUnary(LqExpressionType.Convert, expression, typeof(object))).Compile()();
                        return Constant(obj);
                    }

                    if (((MemberExpression)expression).Expression.NodeType == LqExpressionType.Parameter)
                    {
                        var propertyInfo = (PropertyInfo)PropertyExpression.ExtractMemberExpression(expression).Member;
                        var fieldInfo = Map.GetCheckedAccessor(propertyInfo.ReflectedType).MetaInfo[propertyInfo];
                        if (fieldInfo != null)
                            return Field(fieldInfo.PropertyName);

                        throw new NotSupportedException("实体属性未映射到字段。");
                    }

                    var value = LqExpression.Lambda<Func<object>>(LqExpression.MakeUnary(System.Linq.Expressions.ExpressionType.Convert, expression, typeof(object))).Compile()();
                    if (value is ICollection || (!(value is string) && value is IEnumerable))
                    {
                        var exps = new CollectionExpression();
                        foreach (object item in (IEnumerable)value)
                        {
                            exps.Add(Constant(item));
                        }
                        return exps;
                    }
                    else
                    {
                        return Constant(value);
                    }

                case LqExpressionType.NewArrayInit:
                    var array = (Array)LqExpression.Lambda<Func<object>>(LqExpression.MakeUnary(System.Linq.Expressions.ExpressionType.Convert, expression, typeof(object))).Compile()();
                    var arraycollection = new CollectionExpression(array.Length);
                    foreach (var item in array)
                        arraycollection.Add(Constant(item));
                    return arraycollection;

                case LqExpressionType.ListInit:
                    var list = (ICollection)LqExpression.Lambda<Func<object>>(LqExpression.MakeUnary(System.Linq.Expressions.ExpressionType.Convert, expression, typeof(object))).Compile()();
                    var listcollection = new CollectionExpression(list.Count);
                    foreach (var item in list)
                        listcollection.Add(Constant(item));
                    return listcollection;

            }

            throw new NotSupportedException(string.Format("不受支持的Lambda表达式类型：{0}", expression.NodeType));
        }

        public static LogicalBinaryExpression Expression<T>(System.Linq.Expressions.Expression<Func<T, bool>> logical)
        {
            if (logical == null)
                throw new ArgumentNullException("logical");

            if (!(logical.Body is System.Linq.Expressions.BinaryExpression) && !(logical.Body is System.Linq.Expressions.MethodCallExpression))
                throw new ArgumentException("logical不是一个有效的逻辑表达式。");

            Expression expression = logical.Body as System.Linq.Expressions.BinaryExpression;
            if (expression == null)
                expression = logical.Body as System.Linq.Expressions.MethodCallExpression;
            if (expression == null)
                throw new ArgumentException("logical不是一个有效的逻辑表达式。");

            var logicalExp = Expression(expression) as LogicalBinaryExpression;

            if (logicalExp == null)
                throw new ArgumentException("logical不是一个有效的逻辑表达式。");

            return logicalExp;
        }

        public static SimpleBinaryExpression Add(SqlExpression left, SqlExpression right)
        {
            return SimpleBinaryExpression.Expression(ExpressionType.Add, left, right);
        }

        public static SimpleBinaryExpression Divide(SqlExpression left, SqlExpression right)
        {
            return SimpleBinaryExpression.Expression(ExpressionType.Divide, left, right);
        }

        public static SimpleBinaryExpression Modulo(SqlExpression left, SqlExpression right)
        {
            return SimpleBinaryExpression.Expression(ExpressionType.Modulo, left, right);
        }

        public static SimpleBinaryExpression Multiply(SqlExpression left, SqlExpression right)
        {
            return SimpleBinaryExpression.Expression(ExpressionType.Multiply, left, right);
        }

        public static SimpleBinaryExpression Subtract(SqlExpression left, SqlExpression right)
        {
            return SimpleBinaryExpression.Expression(ExpressionType.Subtract, left, right);
        }

        public static LogicalBinaryExpression AndAlso(SqlExpression left, SqlExpression right)
        {
            return LogicalBinaryExpression.Expression(ExpressionType.AndAlso, left, right);
        }

        public static LogicalBinaryExpression OrElse(SqlExpression left, SqlExpression right)
        {
            return LogicalBinaryExpression.Expression(ExpressionType.OrElse, left, right);
        }

        public static LogicalBinaryExpression Equal(SqlExpression left, SqlExpression right)
        {
            if (left is DbNullExpression)
                return IsNull(right);

            if (right is DbNullExpression)
                return IsNull(left);

            return LogicalBinaryExpression.Expression(ExpressionType.Equal, left, right);
        }

        public static LogicalBinaryExpression Equal(string fieldName, object value)
        {
            if (value == null)
                return IsNull(fieldName);
            return Equal(SqlExpression.Field(fieldName), SqlExpression.Constant(value));
        }

        public static LogicalBinaryExpression Equal<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            if (value == null)
                return IsNull(field);
            return Equal(Field<T>(field), Constant(value));
        }

        public static LogicalBinaryExpression IsNull(SqlExpression left)
        {
            return LogicalBinaryExpression.Expression(ExpressionType.Is, left, DbNullExpression.Instance);
        }

        public static LogicalBinaryExpression IsNull(string fieldName)
        {
            return IsNull(SqlExpression.Field(fieldName));
        }

        public static LogicalBinaryExpression IsNull<T>(System.Linq.Expressions.Expression<Func<T, object>> field)
        {
            return IsNull(Field<T>(field));
        }

        public static LogicalBinaryExpression IsNotNull(SqlExpression left)
        {
            return LogicalBinaryExpression.Expression(ExpressionType.IsNot, left, DbNullExpression.Instance);
        }

        public static LogicalBinaryExpression IsNotNull(string fieldName)
        {
            return IsNotNull(SqlExpression.Field(fieldName));
        }

        public static LogicalBinaryExpression IsNotNull<T>(System.Linq.Expressions.Expression<Func<T, object>> field)
        {
            return IsNotNull(Field<T>(field));
        }

        public static LogicalBinaryExpression GreaterThan(SqlExpression left, SqlExpression right)
        {
            return LogicalBinaryExpression.Expression(ExpressionType.GreaterThan, left, right);
        }

        public static LogicalBinaryExpression GreaterThan(string fieldName, object value)
        {
            return GreaterThan(SqlExpression.Field(fieldName), SqlExpression.Constant(value));
        }

        public static LogicalBinaryExpression GreaterThan<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return GreaterThan(Field<T>(field), Constant(value));
        }

        public static LogicalBinaryExpression GreaterThanOrEqual(SqlExpression left, SqlExpression right)
        {
            return LogicalBinaryExpression.Expression(ExpressionType.GreaterThanOrEqual, left, right);
        }

        public static LogicalBinaryExpression GreaterThanOrEqual(string fieldName, object value)
        {
            return GreaterThanOrEqual(SqlExpression.Field(fieldName), SqlExpression.Constant(value));
        }

        public static LogicalBinaryExpression GreaterThanOrEqual<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return GreaterThanOrEqual(Field<T>(field), Constant(value));
        }

        public static LogicalBinaryExpression LessThan(SqlExpression left, SqlExpression right)
        {
            return LogicalBinaryExpression.Expression(ExpressionType.LessThan, left, right);
        }

        public static LogicalBinaryExpression LessThan(string fieldName, object value)
        {
            return LessThan(SqlExpression.Field(fieldName), SqlExpression.Constant(value));
        }

        public static LogicalBinaryExpression LessThan<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return LessThan(Field<T>(field), Constant(value));
        }

        public static LogicalBinaryExpression LessThanOrEqual(SqlExpression left, SqlExpression right)
        {
            return LogicalBinaryExpression.Expression(ExpressionType.LessThanOrEqual, left, right);
        }

        public static LogicalBinaryExpression LessThanOrEqual(string fieldName, object value)
        {
            return LessThanOrEqual(SqlExpression.Field(fieldName), SqlExpression.Constant(value));
        }

        public static LogicalBinaryExpression LessThanOrEqual<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return LessThanOrEqual(Field<T>(field), Constant(value));
        }

        public static LogicalBinaryExpression NotEqual(SqlExpression left, SqlExpression right)
        {
            if (left is DbNullExpression)
                return IsNotNull(right);

            if (right is DbNullExpression)
                return IsNotNull(left);

            return LogicalBinaryExpression.Expression(ExpressionType.NotEqual, left, right);
        }

        public static LogicalBinaryExpression NotEqual(string fieldName, object value)
        {
            if (value == null)
                return IsNotNull(fieldName);
            return NotEqual(SqlExpression.Field(fieldName), SqlExpression.Constant(value));
        }

        public static LogicalBinaryExpression NotEqual<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            if (value == null)
                return IsNotNull(field);
            return NotEqual(Field<T>(field), Constant(value));
        }

        public static LogicalBinaryExpression In(SqlExpression left, CollectionExpression right)
        {
            return LogicalBinaryExpression.Expression(ExpressionType.In, left, right);
        }

        public static LogicalBinaryExpression In(string fieldName, params object[] value)
        {
            return In(SqlExpression.Field(fieldName), new CollectionExpression(value.Select(x => Constant(x))));
        }

        public static LogicalBinaryExpression In<T>(System.Linq.Expressions.Expression<Func<T, object>> field, params object[] values)
        {
            return In(Field<T>(field), new CollectionExpression(values.Select(x => Constant(x))));
        }

        public static LogicalBinaryExpression Between(SqlExpression left, SqlExpression value1, SqlExpression value2)
        {
            return LogicalBinaryExpression.Expression(ExpressionType.Between, left, new CollectionExpression() { value1, value2 });
        }

        public static LogicalBinaryExpression Between(string fieldName, object value1, object value2)
        {
            return Between(SqlExpression.Field(fieldName), SqlExpression.Constant(value1), Constant(value2));
        }

        public static LogicalBinaryExpression Between<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value1, object value2)
        {
            return Between(Field<T>(field), Constant(value1), Constant(value2));
        }

        public static LogicalBinaryExpression Like(SqlExpression left, SqlExpression right, bool startWith, bool endWith)
        {
            if (startWith)
                right = SqlExpression.Add(right, WildcardsExpression.Instance);
            if (endWith)
                right = SqlExpression.Add(WildcardsExpression.Instance, right);

            return LogicalBinaryExpression.Expression(ExpressionType.Like, left, right);
        }

        public static LogicalBinaryExpression Like(string fieldName, object value)
        {
            return Like(fieldName, value, true, true);
        }

        public static LogicalBinaryExpression Like(string fieldName, object value, bool startWith, bool endWith)
        {
            return Like(SqlExpression.Field(fieldName), SqlExpression.Constant(value), startWith, endWith);
        }

        public static LogicalBinaryExpression Like<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value)
        {
            return Like(Field<T>(field), Constant(value), true, true);
        }

        public static LogicalBinaryExpression Like<T>(System.Linq.Expressions.Expression<Func<T, object>> field, object value, bool startWith, bool endWith)
        {
            return Like(Field<T>(field), Constant(value), startWith, endWith);
        }

        public static ConstantExpression Constant(object value)
        {
            if (value == null)
                return DbNull();
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

        public static WildcardsExpression Wildcards()
        {
            return WildcardsExpression.Instance;
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

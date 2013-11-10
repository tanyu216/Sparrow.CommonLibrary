using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Query
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// 生成条件表达式集合
        /// </summary>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static CollectionExpression Append(this CompareExpression expression1, CompareExpression expression2)
        {
            return new CollectionExpression() { expression1, expression2 };
        }

        /// <summary>
        /// 向表达式集合中添加比较运算表达式
        /// </summary>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static CollectionExpression Append(this CollectionExpression expression1, CompareExpression expression2)
        {
            expression1.Add(expression2);
            return expression1;
        }

        /// <summary>
        /// 生成条件表达式集合
        /// </summary>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static CollectionExpression<T> Append<T>(this CompareExpression expression1, CompareExpression expression2)
        {
            return new CollectionExpression<T>() { expression1, expression2 };
        }

        /// <summary>
        /// 向表达式集合中添加比较运算表达式
        /// </summary>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static CollectionExpression<T> Append<T>(this CollectionExpression<T> expression1, CompareExpression expression2)
        {
            expression1.Add(expression2);
            return expression1;
        }

        /// <summary>
        /// 向表达式集合添加一个比较“等于运算”表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static CollectionExpression<T> Equal<T, TValue>(this CollectionExpression<T> expression1, System.Linq.Expressions.Expression<Func<T, object>> field, TValue value)
        {
            if (value == null)
                expression1.Add(SqlExpression.IsNull<T>(field));
            else
                expression1.Add(SqlExpression.Equal<T>(field, value));
            return expression1;
        }

        /// <summary>
        /// 向表达式集合添加一个比较“大于运算”表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static CollectionExpression<T> GreaterThan<T, TValue>(this CollectionExpression<T> expression1, System.Linq.Expressions.Expression<Func<T, object>> field, TValue value)
        {
            expression1.Add(SqlExpression.GreaterThan<T>(field, value));
            return expression1;
        }

        /// <summary>
        /// 向表达式集合添加一个比较“等于或等于运算”表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static CollectionExpression<T> GreaterThanOrEqual<T, TValue>(this CollectionExpression<T> expression1, System.Linq.Expressions.Expression<Func<T, object>> field, TValue value)
        {
            expression1.Add(SqlExpression.GreaterThanOrEqual<T>(field, value));
            return expression1;
        }

        /// <summary>
        /// 向表达式集合添加一个比较“小于运算”表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static CollectionExpression<T> LessThan<T, TValue>(this CollectionExpression<T> expression1, System.Linq.Expressions.Expression<Func<T, object>> field, TValue value)
        {
            expression1.Add(SqlExpression.LessThan<T>(field, value));
            return expression1;
        }

        /// <summary>
        /// 向表达式集合添加一个比较“小于或等于运算”表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static CollectionExpression<T> LessThanOrEqual<T, TValue>(this CollectionExpression<T> expression1, System.Linq.Expressions.Expression<Func<T, object>> field, TValue value)
        {
            expression1.Add(SqlExpression.LessThanOrEqual<T>(field, value));
            return expression1;
        }

        /// <summary>
        /// 向表达式集合添加一个比较“不等于运算”表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static CollectionExpression<T> NotEqual<T, TValue>(this CollectionExpression<T> expression1, System.Linq.Expressions.Expression<Func<T, object>> field, TValue value)
        {
            if (value == null)
                expression1.Add(SqlExpression.IsNotNull<T>(field));
            else
                expression1.Add(SqlExpression.NotEqual<T>(field, value));
            return expression1;
        }

        /// <summary>
        /// 向表达式集合添加一个比较“包含运算”表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="field"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static CollectionExpression<T> In<T>(this CollectionExpression<T> expression1, System.Linq.Expressions.Expression<Func<T, object>> field, params object[] values)
        {
            expression1.Add(SqlExpression.In<T>(field, values));
            return expression1;
        }

        /// <summary>
        /// 向表达式集合添加一个比较“介于两者之间运算”表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="field"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static CollectionExpression<T> Between<T, TValue>(this CollectionExpression<T> expression1, System.Linq.Expressions.Expression<Func<T, object>> field, TValue value1, TValue value2)
        {
            expression1.Add(SqlExpression.Between<T>(field, value1, value2));
            return expression1;
        }

        /// <summary>
        /// 将表达式转换成条件“与”表达式
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static ConditionExpression ToAndAlso(this CollectionExpression collection)
        {
            if (collection.Count < 2)
            {
                return SqlExpression.AndAlso(collection[0], null);
            }

            ConditionExpression prev = SqlExpression.AndAlso(collection[0], collection[1]);
            for (var i = 2; i < collection.Count; i++)
            {
                prev = SqlExpression.AndAlso(prev, collection[i]);
            }
            return prev;
        }

        /// <summary>
        /// 将表达式转换成条件“或”表达式
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static ConditionExpression ToOrElse(this CollectionExpression collection)
        {
            if (collection.Count < 2)
            {
                return SqlExpression.OrElse(collection[0], null);
            }

            ConditionExpression prev = SqlExpression.OrElse(collection[0], collection[1]);
            for (var i = 2; i < collection.Count; i++)
            {
                prev = SqlExpression.OrElse(prev, collection[i]);
            }
            return prev;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sparrow.CommonLibrary.Query;

namespace Sparrow.CommonLibrary.Repository
{
    public interface IRepository<T>
    {
        #region Insert, Update, Save, Delete

        int Insert(T entity);
        int Insert(IEnumerable<T> entities);

        int Update(T entity);
        int Update(IEnumerable<T> entities);

        int Save(T entity);
        int Save(IEnumerable<T> entities);

        int Delete(T entity);
        int Delete(CompareExpression condition);
        int Delete(ConditionExpression condition);

        #endregion

        #region Query

        IList<T> GetList();
        IList<T> GetList(int startIndex, int rowCount);
        IList<T> GetList(CompareExpression condition);
        IList<T> GetList(ConditionExpression condition, int startIndex, int rowCount);

        T Get(object id);
        T Get(CompareExpression condition);
        T Get(ConditionExpression condition);

        #endregion

        #region Sum, Min, Max, Count, Avg, Count, Groupby

        TValue Sum<TValue>(Expression<Func<T, object>> field);
        TValue Sum<TValue>(Expression<Func<T, object>> field, CompareExpression condition);
        TValue Sum<TValue>(Expression<Func<T, object>> field, ConditionExpression condition);

        TValue Min<TValue>(Expression<Func<T, object>> field);
        TValue Min<TValue>(Expression<Func<T, object>> field, CompareExpression condition);
        TValue Min<TValue>(Expression<Func<T, object>> field, ConditionExpression condition);

        TValue Max<TValue>(Expression<Func<T, object>> field);
        TValue Max<TValue>(Expression<Func<T, object>> field, CompareExpression condition);
        TValue Max<TValue>(Expression<Func<T, object>> field, ConditionExpression condition);

        TValue Avg<TValue>(Expression<Func<T, object>> field);
        TValue Avg<TValue>(Expression<Func<T, object>> field, CompareExpression condition);
        TValue Avg<TValue>(Expression<Func<T, object>> field, ConditionExpression condition);

        int Count(Expression<Func<T, object>> field);
        int Count(Expression<Func<T, object>> field, CompareExpression condition);
        int Count(Expression<Func<T, object>> field, ConditionExpression condition);

        IDictionary<TKey, TValue> GroupbySum<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField);
        IDictionary<TKey, TValue> GroupbySum<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, CompareExpression condition);
        IDictionary<TKey, TValue> GroupbySum<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, ConditionExpression condition);

        IDictionary<TKey, TValue> GroupbyMin<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField);
        IDictionary<TKey, TValue> GroupbyMin<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, CompareExpression condition);
        IDictionary<TKey, TValue> GroupbyMin<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, ConditionExpression condition);

        IDictionary<TKey, TValue> GroupbyMax<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField);
        IDictionary<TKey, TValue> GroupbyMax<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, CompareExpression condition);
        IDictionary<TKey, TValue> GroupbyMax<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, ConditionExpression condition);

        IDictionary<TKey, TValue> GroupbyAvg<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField);
        IDictionary<TKey, TValue> GroupbyAvg<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, CompareExpression condition);
        IDictionary<TKey, TValue> GroupbyAvg<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, ConditionExpression condition);

        IDictionary<TKey, int> GroupbyCount<TKey>(Expression<Func<T, object>> field);
        IDictionary<TKey, int> GroupbyCount<TKey>(Expression<Func<T, object>> field, CompareExpression condition);
        IDictionary<TKey, int> GroupbyCount<TKey>(Expression<Func<T, object>> field, ConditionExpression condition);

        #endregion

    }
}

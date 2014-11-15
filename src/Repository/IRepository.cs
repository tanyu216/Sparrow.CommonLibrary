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
        int Delete(Expression<Func<T, bool>> logical);
        int Delete(LogicalBinaryExpression logical);

        #endregion

        #region Query

        IList<T> GetList();
        IList<T> GetList(int startIndex, int rowCount);
        IList<T> GetList(int startIndex, int rowCount, Expression<Func<T, object>> orderby, bool descending);
        IList<T> GetList(int startIndex, int rowCount, Expression<Func<T, object>> orderby, bool descending, Expression<Func<T, object>> orderby2, bool descending2);
        IList<T> GetList(int startIndex, int rowCount, Expression<Func<T, object>> orderby, bool descending, Expression<Func<T, object>> orderby2, bool descending2, Expression<Func<T, object>> orderby3, bool descending3);
        IList<T> GetList(Expression<Func<T, bool>> logical);
        IList<T> GetList(Expression<Func<T, bool>> logical, int startIndex, int rowCount);
        IList<T> GetList(Expression<Func<T, bool>> logical, int startIndex, int rowCount, Expression<Func<T, object>> orderby, bool descending);
        IList<T> GetList(Expression<Func<T, bool>> logical, int startIndex, int rowCount, Expression<Func<T, object>> orderby, bool descending, Expression<Func<T, object>> orderby2, bool descending2);
        IList<T> GetList(Expression<Func<T, bool>> logical, int startIndex, int rowCount, Expression<Func<T, object>> orderby, bool descending, Expression<Func<T, object>> orderby2, bool descending2, Expression<Func<T, object>> orderby3, bool descending3);
        
        T Get(object id);
        T Get(Expression<Func<T, bool>> logical);
        T Get(Expression<Func<T, bool>> logical, Expression<Func<T, object>> orderby, bool descending);
        T Get(Expression<Func<T, bool>> logical, Expression<Func<T, object>> orderby, bool descending, Expression<Func<T, object>> orderby2, bool descending2);
        T Get(Expression<Func<T, bool>> logical, Expression<Func<T, object>> orderby, bool descending, Expression<Func<T, object>> orderby2, bool descending2, Expression<Func<T, object>> orderby3, bool descending3);
        
        #endregion

        #region Sum, Min, Max, Count, Avg, Count, Groupby

        TValue Sum<TValue>(Expression<Func<T, object>> field);
        TValue Sum<TValue>(Expression<Func<T, object>> field, Expression<Func<T, bool>> logical);
        TValue Sum<TValue>(Expression<Func<T, object>> field, LogicalBinaryExpression logical);

        TValue Min<TValue>(Expression<Func<T, object>> field);
        TValue Min<TValue>(Expression<Func<T, object>> field, Expression<Func<T, bool>> logical);
        TValue Min<TValue>(Expression<Func<T, object>> field, LogicalBinaryExpression logical);

        TValue Max<TValue>(Expression<Func<T, object>> field);
        TValue Max<TValue>(Expression<Func<T, object>> field, Expression<Func<T, bool>> logical);
        TValue Max<TValue>(Expression<Func<T, object>> field, LogicalBinaryExpression logical);

        TValue Avg<TValue>(Expression<Func<T, object>> field);
        TValue Avg<TValue>(Expression<Func<T, object>> field, Expression<Func<T, bool>> logical);
        TValue Avg<TValue>(Expression<Func<T, object>> field, LogicalBinaryExpression logical);

        int Count(Expression<Func<T, object>> field);
        int Count(Expression<Func<T, object>> field, Expression<Func<T, bool>> logical);
        int Count(Expression<Func<T, object>> field, LogicalBinaryExpression logical);

        IDictionary<TKey, TValue> GroupbySum<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField);
        IDictionary<TKey, TValue> GroupbySum<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, Expression<Func<T, bool>> logical);
        IDictionary<TKey, TValue> GroupbySum<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, LogicalBinaryExpression logical);

        IDictionary<TKey, TValue> GroupbyMin<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField);
        IDictionary<TKey, TValue> GroupbyMin<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, Expression<Func<T, bool>> logical);
        IDictionary<TKey, TValue> GroupbyMin<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, LogicalBinaryExpression logical);

        IDictionary<TKey, TValue> GroupbyMax<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField);
        IDictionary<TKey, TValue> GroupbyMax<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, Expression<Func<T, bool>> logical);
        IDictionary<TKey, TValue> GroupbyMax<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, LogicalBinaryExpression logical);

        IDictionary<TKey, TValue> GroupbyAvg<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField);
        IDictionary<TKey, TValue> GroupbyAvg<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, Expression<Func<T, bool>> logical);
        IDictionary<TKey, TValue> GroupbyAvg<TKey, TValue>(Expression<Func<T, object>> keyField, Expression<Func<T, object>> valueField, LogicalBinaryExpression logical);

        IDictionary<TKey, int> GroupbyCount<TKey>(Expression<Func<T, object>> field);
        IDictionary<TKey, int> GroupbyCount<TKey>(Expression<Func<T, object>> field, Expression<Func<T, bool>> logical);
        IDictionary<TKey, int> GroupbyCount<TKey>(Expression<Func<T, object>> field, LogicalBinaryExpression logical);

        #endregion

    }
}

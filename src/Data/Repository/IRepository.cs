using Sparrow.CommonLibrary.Data.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Data.Repository
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
        int Delete(ConditionExpression<T> condition);

        #endregion

        #region Query

        IList<T> GetList();
        IList<T> GetList(ConditionExpression<T> condition);
        T Get(object id);
        T Get(ConditionExpression<T> condition);

        #endregion

        #region Sum, Min, Max, Count, Avg, Count

        double Sum(Expression<Func<T, object>> expression);
        double Sum(Expression<Func<T, object>> expression, ConditionExpression<T> condition);
        double Min(Expression<Func<T, object>> expression);
        double Min(Expression<Func<T, object>> expression, ConditionExpression<T> condition);
        double Max(Expression<Func<T, object>> expression);
        double Max(Expression<Func<T, object>> expression, ConditionExpression<T> condition);
        double Avg(Expression<Func<T, object>> expression);
        double Avg(Expression<Func<T, object>> expression, ConditionExpression<T> condition);
        long Count(Expression<Func<T, object>> expression);
        long Count(Expression<Func<T, object>> expression, ConditionExpression<T> condition);
        
        #endregion

    }
}

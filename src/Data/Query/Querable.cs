using Sparrow.CommonLibrary.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sparrow.CommonLibrary.Data.Query
{
    /// <summary>
    /// 查询表达式
    /// </summary>
    public class Queryable<T>
    {
        readonly DatabaseHelper database;

        public Queryable(DatabaseHelper database)
        {
            this.database = database;
        }

        public Queryable<T> Select(Expression<Func<T, object>> field)
        {
            return this;
        }

        public Queryable<T> Select(params Expression<Func<T, object>>[] fields)
        {
            return this;
        }

        public Queryable<T> Sum(Expression<Func<T, object>> field)
        {
            return this;
        }

        public Queryable<T> Min(Expression<Func<T, object>> field)
        {
            return this;
        }

        public Queryable<T> Max(Expression<Func<T, object>> field)
        {
            return this;
        }

        public Queryable<T> Avg(Expression<Func<T, object>> field)
        {
            return this;
        }

        public Queryable<T> Count(Expression<Func<T, object>> field)
        {
            return this;
        }

        public Queryable<T> Distinct()
        {
            return this;
        }

        public Queryable<T> Take(int top)
        {
            return this;
        }

        public Queryable<T> Where(Expression<Func<T, object>> field, Operator opt, object value)
        {
            return this;
        }

        public Queryable<T> GroupBy(Expression<Func<T, object>> field)
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

    }
}

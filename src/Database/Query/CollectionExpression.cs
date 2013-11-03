using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Database.Query;

namespace Sparrow.CommonLibrary.Database.Query
{
    /// <summary>
    /// 表达式集合
    /// </summary>
    public class CollectionExpression : SqlExpression, IList<SqlExpression>
    {
        private List<SqlExpression> list;

        public CollectionExpression()
            : this(4)
        {
        }

        public CollectionExpression(int capacity)
        {
            list = new List<SqlExpression>(capacity);
        }

        public CollectionExpression(IEnumerable<SqlExpression> collection)
        {
            list = new List<SqlExpression>(collection);
        }

        #region IList<Expression>

        public void Add(SqlExpression expression)
        {
            list.Add(expression);
        }

        public void AddRang(IEnumerable<SqlExpression> collection)
        {
            list.AddRange(collection);
        }

        public IEnumerator<SqlExpression> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public int IndexOf(SqlExpression item)
        {

            return list.IndexOf(item);
        }

        public void Insert(int index, SqlExpression item)
        {
            list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public SqlExpression this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(SqlExpression item)
        {
            return list.Contains(item);
        }

        public void CopyTo(SqlExpression[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(SqlExpression item)
        {
            return list.Remove(item);
        }

        #endregion

        #region override

        public override ExpressionType NodeType
        {
            get { return ExpressionType.Collection; }
        }

        public override string OutputSqlString(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
        {
            return string.Join(",", this.Select(x => x.OutputSqlString(builder, output)));
        }

        #endregion
    }

    public class CollectionExpression<T> : CollectionExpression
    {
        public CollectionExpression()
        {
        }

        public CollectionExpression(int capacity)
            : base(capacity)
        {
        }

        public CollectionExpression(IEnumerable<SqlExpression> collection)
            : base(collection)
        {
        }

    }
}

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
    public class CollectionExpression : Expression, IList<Expression>
    {
        private List<Expression> list;

        public CollectionExpression()
            : this(4)
        {
        }

        public CollectionExpression(int capacity)
        {
            list = new List<Expression>(capacity);
        }

        public CollectionExpression(IEnumerable<Expression> collection)
        {
            list = new List<Expression>(collection);
        }

        #region IList<Expression>

        public void Add(Expression expression)
        {
            list.Add(expression);
        }

        public void AddRang(IEnumerable<Expression> collection)
        {
            list.AddRange(collection);
        }

        public IEnumerator<Expression> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public int IndexOf(Expression item)
        {

            return list.IndexOf(item);
        }

        public void Insert(int index, Expression item)
        {
            list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public Expression this[int index]
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

        public bool Contains(Expression item)
        {
            return list.Contains(item);
        }

        public void CopyTo(Expression[] array, int arrayIndex)
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

        public bool Remove(Expression item)
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

        public CollectionExpression(IEnumerable<Expression> collection)
            : base(collection)
        {
        }

    }
}

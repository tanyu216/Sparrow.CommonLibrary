using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Data.Database;
using Sparrow.CommonLibrary.Data.SqlBuilder;

namespace Sparrow.CommonLibrary.Data.Query
{
    /// <summary>
    /// 条件表达式
    /// </summary>
    public class ConditionExpression : Expression
    {
        /// <summary>
        /// Left
        /// </summary>
        public Expression Left { get; private set; }

        /// <summary>
        /// right
        /// </summary>
        public Expression Right { get; private set; }

        /// <summary>
        /// 运算符
        /// </summary>
        public virtual string Operater { get; private set; }

        protected ConditionExpression(Expression left, Expression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");
            //
            Left = left;
            Right = right;
        }

        public ConditionExpression(Expression left, Expression right, string operater)
            : this(left, right)
        {
            if (string.IsNullOrEmpty(operater))
                throw new ArgumentNullException("operater");
            this.Operater = operater;
        }

        public override string Build(ISqlBuilder builder, ParameterCollection output)
        {
            // 运算符左边（colname(左) = 1（右））
            var condition = new StringBuilder();
            condition.Append(Left.Build(builder, output));
            condition.Append(' ').Append(this.Operater).Append(' ');
            condition.Append(Right.Build(builder, output));
            return condition.ToString();
        }
    }
}

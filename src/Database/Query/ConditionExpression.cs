using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Database.Query
{
    public abstract class ConditionExpression : BinaryExpression
    {

        internal static ConditionExpression Condition(ExpressionType nodeType, Expression left, Expression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            switch (nodeType)
            {
                case ExpressionType.AndAlso:
                    return new AndAlsoExpression() { Left = left, Right = right };
                case ExpressionType.OrElse:
                    return new OrElseExpression() { Left = left, Right = right };
                default:
                    throw new NotSupportedException(string.Format("不受支持的{0}", nodeType.ToString()));
            }
        }

        private class AndAlsoExpression : ConditionExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.AndAlso; }
            }

            public override string OutputSqlString(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
            {
                if (Right == null)
                    return Left.OutputSqlString(builder, output);
                return string.Concat(Left.OutputSqlString(builder, output), " AND ", Right.OutputSqlString(builder, output));
            }
        }

        private class OrElseExpression : ConditionExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.OrElse; }
            }

            public override string OutputSqlString(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
            {
                if (Right == null)
                    return Left.OutputSqlString(builder, output);
                return string.Concat("(", Left.OutputSqlString(builder, output), " OR ", Right.OutputSqlString(builder, output), ")");
            }
        }
    }

    public class ConditionExpression<T> : ConditionExpression
    {
        private readonly ConditionExpression _expression;

        protected ConditionExpression(ConditionExpression expression)
        {
            _expression = expression;
        }

        public override ExpressionType NodeType
        {
            get { return _expression.NodeType; }
        }

        public override string OutputSqlString(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
        {
            return _expression.OutputSqlString(builder, output);
        }
    }
}

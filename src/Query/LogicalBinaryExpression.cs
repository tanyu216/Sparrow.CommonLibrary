using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sparrow.CommonLibrary.Query
{
    public abstract class LogicalBinaryExpression : BinaryExpression
    {

        internal static LogicalBinaryExpression Expression(ExpressionType nodeType, SqlExpression left, SqlExpression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            switch (nodeType)
            {
                case ExpressionType.Equal:
                    return new EqualExpression() { Left = left, Right = right };
                case ExpressionType.Is:
                    return new IsExpression() { Left = left, Right = right };
                case ExpressionType.IsNot:
                    return new IsNotExpression() { Left = left, Right = right };
                case ExpressionType.GreaterThan:
                    return new GreaterThanExpression() { Left = left, Right = right };
                case ExpressionType.GreaterThanOrEqual:
                    return new GreaterThanOrEqualExpression() { Left = left, Right = right };
                case ExpressionType.LessThan:
                    return new LessThanExpression() { Left = left, Right = right };
                case ExpressionType.LessThanOrEqual:
                    return new LessThanOrEqualExpression() { Left = left, Right = right };
                case ExpressionType.NotEqual:
                    return new NotEqualExpression() { Left = left, Right = right };
                case ExpressionType.In:
                    if (!(right is CollectionExpression))
                        throw new ArgumentException("right不是一个集合表达式。");
                    return new InExpression() { Left = left, Right = (CollectionExpression)right };
                case ExpressionType.Between:
                    if (!(right is CollectionExpression))
                        throw new ArgumentException("right不是一个集合表达式。");
                    return new BetweenExpression() { Left = left, Right = (CollectionExpression)right };
                case ExpressionType.AndAlso:
                    return new AndAlsoExpression() { Left = left, Right = right };
                case ExpressionType.OrElse:
                    return new OrElseExpression() { Left = left, Right = right };
                default:
                    throw new NotSupportedException(string.Format("不受支持的{0}", nodeType.ToString()));
            }
        }

        private class BetweenExpression : LogicalBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.Between; }
            }

            protected override string Operator
            {
                get { return " BETWEEN "; }
            }

            public new CollectionExpression Right
            {
                get { return (CollectionExpression)base.Right; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("Right");
                    if (value.Count != 2)
                        throw new ArgumentException("Right集合元素必须为2个。");
                    base.Right = value;
                }
            }

            public override string OutputSqlString(Database.SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
            {
                return string.Concat(Left.OutputSqlString(builder, output), " BETWEEN ", Right[0].OutputSqlString(builder, output), " AND ", Right[1].OutputSqlString(builder, output));
            }
        }

        private class InExpression : LogicalBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.In; }
            }

            protected override string Operator
            {
                get { return " IN "; }
            }

            public override string OutputSqlString(Database.SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
            {
                return string.Concat(Left.OutputSqlString(builder, output), " IN (", Right.OutputSqlString(builder, output), ")");
            }
        }

        private class EqualExpression : LogicalBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.Equal; }
            }

            protected override string Operator
            {
                get { return "="; }
            }
        }

        private class IsExpression : LogicalBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.Is; }
            }

            protected override string Operator
            {
                get { return " IS "; }
            }
        }

        private class GreaterThanExpression : LogicalBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.GreaterThan; }
            }

            protected override string Operator
            {
                get { return ">"; }
            }
        }

        private class GreaterThanOrEqualExpression : LogicalBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.GreaterThanOrEqual; }
            }

            protected override string Operator
            {
                get { return ">="; }
            }

        }

        private class LessThanExpression : LogicalBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.LessThan; }
            }

            protected override string Operator
            {
                get { return "<"; }
            }

        }

        private class LessThanOrEqualExpression : LogicalBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.LessThanOrEqual; }
            }

            protected override string Operator
            {
                get { return "<="; }
            }
        }

        private class LikeExpression : LogicalBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.Like; }
            }

            protected override string Operator
            {
                get { return " LIKE "; }
            }
        }

        private class NotEqualExpression : LogicalBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.NotEqual; }
            }

            protected override string Operator
            {
                get { return "<>"; }
            }
        }

        private class IsNotExpression : LogicalBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.IsNot; }
            }

            protected override string Operator
            {
                get { return " IS NOT "; }
            }
        }

        private class AndAlsoExpression : LogicalBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.AndAlso; }
            }

            protected override string Operator
            {
                get { return " AND "; }
            }
        }

        private class OrElseExpression : LogicalBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.OrElse; }
            }

            protected override string Operator
            {
                get { return " OR "; }
            }

            public override string OutputSqlString(Database.SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
            {
                if (Right == null)
                    return Left.OutputSqlString(builder, output);
                return string.Concat("(", Left.OutputSqlString(builder, output), Operator, Right.OutputSqlString(builder, output), ")");
            }
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Query
{
    /// <summary>
    /// 条件比较表达式
    /// </summary>
    public abstract class CompareExpression : SqlExpression
    {
        public SqlExpression Left { get; protected set; }

        public SqlExpression Right { get; protected set; }

        protected abstract string Operator { get; }

        protected CompareExpression()
        {
        }

        internal static CompareExpression Expression(ExpressionType nodeType, SqlExpression left, SqlExpression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            switch (nodeType)
            {
                case ExpressionType.Equal:
                    return new EqualExpression() { Left = left, Right = right };
                case ExpressionType.IsNull:
                    return new EqualNullExpression() { Left = left, Right = right };
                case ExpressionType.IsNotNull:
                    return new EqualNullExpression() { Left = left, Right = right };
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
                    return new InExpression() { Left = left, Right = (CollectionExpression)right };
                case ExpressionType.Between:
                    return new BetweenExpression() { Left = left, Right = (CollectionExpression)right };
                default:
                    throw new NotSupportedException(string.Format("不受支持的{0}", nodeType.ToString()));
            }
        }

        internal static CompareExpression ExpressionForLike(SqlExpression left, SqlExpression right, bool startWith, bool endWith)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            return new LikeExpression() { Left = left, Right = right, StartWith = startWith, EndWith = endWith };
        }

        public override string OutputSqlString(Database.SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
        {
            return string.Concat(Left.OutputSqlString(builder, output), Operator, Right.OutputSqlString(builder, output));
        }

        private class BetweenExpression : CompareExpression
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

        private class InExpression : CompareExpression
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

        private class EqualExpression : CompareExpression
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

        private class EqualNullExpression : CompareExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.IsNull; }
            }

            protected override string Operator
            {
                get { return " IS "; }
            }

            public override string OutputSqlString(Database.SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
            {
                return string.Concat(Left.OutputSqlString(builder, output), Operator, Right.OutputSqlString(builder, output));
            }
        }

        private class GreaterThanExpression : CompareExpression
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

        private class GreaterThanOrEqualExpression : CompareExpression
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

        private class LessThanExpression : CompareExpression
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

        private class LessThanOrEqualExpression : CompareExpression
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

        private class LikeExpression : CompareExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.Like; }
            }

            protected override string Operator
            {
                get { return " LIKE "; }
            }

            public bool StartWith { get; set; }

            public bool EndWith { get; set; }

            public override string OutputSqlString(Database.SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
            {
                var condition = new StringBuilder();
                condition.Append(Left.OutputSqlString(builder, output));
                condition.Append(" LIKE ");
                if (StartWith)
                { condition.Append("'%'+"); }
                condition.Append(Right.OutputSqlString(builder, output));
                if (EndWith)
                { condition.Append("+'%'"); }
                return condition.ToString();
            }
        }

        private class NotEqualExpression : CompareExpression
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

        private class NotEqualNullExpression : CompareExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.IsNotNull; }
            }

            protected override string Operator
            {
                get { return " IS NOT "; }
            }

            public override string OutputSqlString(Database.SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
            {
                return string.Concat(Left.OutputSqlString(builder, output), Operator, Right.OutputSqlString(builder, output));
            }
        }
    }
}

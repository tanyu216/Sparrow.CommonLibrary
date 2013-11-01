using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Database.Query
{
    /// <summary>
    /// 二元运算表达式
    /// </summary>
    public abstract class BinaryExpression : Expression
    {
        public Expression Left { get; protected set; }

        public Expression Right { get; protected set; }

        internal static BinaryExpression Expression(ExpressionType nodeType, Expression left, Expression right)
        {
            switch (nodeType)
            {
                case ExpressionType.Add:
                    return new AddExpression() { Left = left, Right = right };
                case ExpressionType.Divide:
                    return new DivideExpression() { Left = left, Right = right };
                case ExpressionType.Modulo:
                    return new ModuloExpression() { Left = left, Right = right };
                case ExpressionType.Multiply:
                    return new MultiplyExpression() { Left = left, Right = right };
                case ExpressionType.Subtract:
                    return new SubtractExpression() { Left = left, Right = right };
                default:
                    throw new NotSupportedException(string.Format("不受支持的{0}", nodeType.ToString()));
            }
        }

        private class AddExpression : BinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.Add; }
            }

            public override string OutputSqlString(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
            {
                return string.Concat(Left.OutputSqlString(builder, output), "+", Right.OutputSqlString(builder, output));
            }
        }

        private class DivideExpression : BinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.Divide; }
            }

            public override string OutputSqlString(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
            {
                return string.Concat(Left.OutputSqlString(builder, output), "/", Right.OutputSqlString(builder, output));
            }
        }

        private class ModuloExpression : BinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.Modulo; }
            }

            public override string OutputSqlString(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
            {
                return string.Concat(Left.OutputSqlString(builder, output), "%", Right.OutputSqlString(builder, output));
            }
        }

        private class MultiplyExpression : BinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.Multiply; }
            }

            public override string OutputSqlString(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
            {
                return string.Concat(Left.OutputSqlString(builder, output), "*", Right.OutputSqlString(builder, output));
            }
        }

        private class SubtractExpression : BinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.Subtract; }
            }

            public override string OutputSqlString(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
            {
                return string.Concat(Left.OutputSqlString(builder, output), "-", Right.OutputSqlString(builder, output));
            }
        }
    }

    public class BinaryExpression<T> : BinaryExpression
    {
        private readonly BinaryExpression _expression;

        protected BinaryExpression(BinaryExpression expression)
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

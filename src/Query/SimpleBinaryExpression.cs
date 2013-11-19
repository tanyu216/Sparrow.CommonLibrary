using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Query
{
    public abstract class SimpleBinaryExpression : BinaryExpression
    {

        internal static SimpleBinaryExpression Expression(ExpressionType nodeType, SqlExpression left, SqlExpression right)
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

        private class AddExpression : SimpleBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.Add; }
            }

            protected override string Operator
            {
                get { return "+"; }
            }
        }

        private class DivideExpression : SimpleBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.Divide; }
            }

            protected override string Operator
            {
                get { return "/"; }
            }
        }

        private class ModuloExpression : SimpleBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.Modulo; }
            }

            protected override string Operator
            {
                get { return "%"; }
            }
        }

        private class MultiplyExpression : SimpleBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.Multiply; }
            }

            protected override string Operator
            {
                get { return "*"; }
            }
        }

        private class SubtractExpression : SimpleBinaryExpression
        {
            public override ExpressionType NodeType
            {
                get { return ExpressionType.Subtract; }
            }

            protected override string Operator
            {
                get { return "-"; }
            }
        }
    }
}

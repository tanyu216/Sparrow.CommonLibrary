using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Data.Query
{
    public class ConditionLikeExpression : ConditionExpression
    {
        public bool StartWith { get; private set; }

        public bool EndWith { get; private set; }

        public override string Operater
        {
            get
            {
                return "LIKE";
            }
        }

        public ConditionLikeExpression(Expression left, object value)
            : this(left, new ConstantExpression(value), true, true)
        {
        }

        public ConditionLikeExpression(Expression left, object value, bool startWith, bool endWith)
            : this(left, new ConstantExpression(value), startWith, endWith)
        {
        }

        public ConditionLikeExpression(Expression left, Expression right)
            : this(left, right, true, true)
        {
        }

        public ConditionLikeExpression(Expression left, Expression right, bool startWith, bool endWith)
            : base(left, right)
        {
            StartWith = startWith;
            EndWith = endWith;
        }

        public override string Build(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
        {
            var condition = new StringBuilder();
            condition.Append(Left.Build(builder, output));
            condition.Append(' ').Append(this.Operater).Append(' ');
            if (StartWith)
            { condition.Append("'%'+"); }
            condition.Append(Right.Build(builder, output));
            if (EndWith)
            { condition.Append("+'%'"); }
            return condition.ToString();
        }
    }
}

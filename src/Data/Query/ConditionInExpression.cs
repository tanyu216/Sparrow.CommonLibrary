using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Data.Query
{
    public class ConditionInExpression : ConditionExpression
    {
        public override string Operater
        {
            get
            {
                return "IN";
            }
        }

        public ConditionInExpression(Expression left, object[] values)
            : base(left, new ConstantsExpression(values))
        {
        }

        public ConditionInExpression(Expression left, ICollection values)
            : base(left, new ConstantsExpression(values))
        {
        }

        public override string Build(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
        {
            var condition = new StringBuilder();
            condition.Append(Left.Build(builder, output));
            condition.Append(' ').Append(this.Operater).Append('(');
            condition.Append(Right.Build(builder, output)).Append(')');
            return condition.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Data.Query
{
    public class ConditionBetweenExpression : ConditionExpression
    {
        public override string Operater
        {
            get
            {
                return "BETWEEN";
            }
        }

        public object Value1 { get; private set; }

        public object Value2 { get; private set; }

        public ConditionBetweenExpression(Expression left, object value1, object value2)
            : base(left, new ConstantsExpression(new[] { value1, value2 }))
        {
            if (value1 == null)
                throw new ArgumentNullException("value1");
            if (value2 == null)
                throw new ArgumentNullException("value2");
            Value1 = value1;
            Value2 = value2;
        }

        public override string Build(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
        {
            var p1 = output.Append("p", Value1, true).ParameterName;
            var p2 = output.Append("p", Value2, true).ParameterName;
            //
            var condition = new StringBuilder();
            condition.Append(Left.Build(builder, output));
            condition.Append(' ').Append(this.Operater).Append(' ');
            condition.Append(p1).Append(" AND ").Append(p2);
            return condition.ToString();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Data.Query
{
    public class ConstantExpression : Expression
    {
        public object Value { get; private set; }

        public ConstantExpression(object value)
        {
            this.Value = value;
        }

        public override string Build(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
        {
            return output.Append("p", Value, true).ParameterName;
        }
    }

    public class ConstantsExpression : Expression
    {
        public IEnumerable Values { get; private set; }

        public ConstantsExpression(object[] values)
        {
            this.Values = values;
        }

        public ConstantsExpression(ICollection values)
        {
            this.Values = values;
        }

        public override string Build(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
        {
            var sb = new StringBuilder();
            foreach (var value in Values)
                sb.Append(',').Append(output.Append("p", value, true).ParameterName);
            //
            sb.Remove(0, 1);
            return sb.ToString();
        }
    }
}

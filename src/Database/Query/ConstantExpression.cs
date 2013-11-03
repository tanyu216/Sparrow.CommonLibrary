using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Database.Query
{
    public class ConstantExpression : SqlExpression
    {
        public object Value { get; private set; }

        protected ConstantExpression(object value)
        {
            this.Value = value;
        }

        public override ExpressionType NodeType
        {
            get { return ExpressionType.Constant; }
        }

        public override string OutputSqlString(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
        {
            return output.Append("p", Value, true).ParameterName;
        }

        internal static ConstantExpression Expression(object value)
        {
            return new ConstantExpression(value);
        }
    }
}

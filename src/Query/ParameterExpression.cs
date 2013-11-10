using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Query
{
    public class ParameterExpression : SqlExpression
    {
        public string Name { get; protected set; }

        public object Value { get; protected set; }

        protected ParameterExpression(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            this.Name = name;
            this.Value = value;
        }

        public override ExpressionType NodeType
        {
            get { return ExpressionType.Parameter; }
        }

        public override string OutputSqlString(Database.SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
        {
            return output.Append(Name, Value).ParameterName;
        }

        internal static ParameterExpression Expression(string name, object value)
        {
            return new ParameterExpression(name, value);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Query
{
    public class VariableNameExpression : SqlExpression
    {
        public string Name { get; protected set; }

        protected VariableNameExpression(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            this.Name = name;
        }

        public override ExpressionType NodeType
        {
            get { return ExpressionType.VariableName; }
        }

        public override string OutputSqlString(Database.SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
        {
            return builder.BuildParameterName(Name);
        }

        internal static VariableNameExpression Expression(string name)
        {
            return new VariableNameExpression(name);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Data.Query
{
    public class VariableExpression : Expression
    {
        public string Name { get; private set; }

        public VariableExpression(string name)
        {
            this.Name = name;
        }

        public override string Build(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
        {
            return builder.BuildParameterName(Name);
        }
    }
}

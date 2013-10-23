using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Data.Query
{
    public class ParameterExpression : Expression
    {
        public string Name { get; private set; }
        public object Value { get; private set; }

        public ParameterExpression(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public override string Build(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
        {
            return output.Append(Name, Value).ParameterName;
        }
    }
}

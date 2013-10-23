using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Data.Database;
using Sparrow.CommonLibrary.Data.SqlBuilder;

namespace Sparrow.CommonLibrary.Data.Query
{
    public class FunctionExpression : Expression
    {
        public string Name { get; private set; }

        public Expression[] Expressions { get; private set; }

        public FunctionExpression(string name, Expression expression)
        {
            Name = name;
            Expressions = new[] { expression };
        }

        public FunctionExpression(string name, Expression[] exps)
        {
            Name = name;
            Expressions = exps;
        }

        private string BuildParameter(ISqlBuilder builder, ParameterCollection output)
        {
            if (Expressions.Length == 1)
                return Expressions[0].Build(builder, output);
            return string.Join(",", Expressions.Select(x => x.Build(builder, output)));
        }

        public override string Build(ISqlBuilder builder, ParameterCollection output)
        {
            return string.Concat(builder.BuildFuncName(Name), "(", BuildParameter(builder, output), ")");
        }
    }
}

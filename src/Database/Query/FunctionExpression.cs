using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Database.SqlBuilder;

namespace Sparrow.CommonLibrary.Database.Query
{
    public class FunctionExpression : SqlExpression
    {
        public string Schema { get; set; }

        public string Name { get; protected set; }

        public new string Alias { get; set; }

        public SqlExpression[] Arguments { get; protected set; }

        protected FunctionExpression(string name, SqlExpression arguments)
            : this(name, new[] { arguments })
        {
        }

        protected FunctionExpression(string name, params SqlExpression[] arguments)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            Name = name;
            Arguments = arguments;
        }

        private string BuildParameter(ISqlBuilder builder, ParameterCollection output)
        {
            if (Arguments.Length == 1)
                return Arguments[0].OutputSqlString(builder, output);
            return string.Join(",", Arguments.Select(x => x.OutputSqlString(builder, output)));
        }

        public override ExpressionType NodeType
        {
            get { return ExpressionType.Function; }
        }

        public override string OutputSqlString(ISqlBuilder builder, ParameterCollection output)
        {
            string funcName = null;
            if (string.IsNullOrEmpty(Schema))
                funcName = Name;
            else
                funcName = string.Concat(Schema, ".", Name);
            return string.Concat(builder.BuildFuncName(Name), "(", BuildParameter(builder, output), ")");
        }

        internal static FunctionExpression Expression(string name, params SqlExpression[] arguments)
        {
            return new FunctionExpression(name, arguments);
        }
    }
}

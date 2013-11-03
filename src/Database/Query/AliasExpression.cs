using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Database.Query
{
    public class AliasExpression : SqlExpression
    {
        public SqlExpression Exp { get; protected set; }

        public string Alias { get; protected set; }

        protected AliasExpression()
        {
        }

        public override ExpressionType NodeType
        {
            get { return ExpressionType.Alias; }
        }

        public override string OutputSqlString(SqlBuilder.ISqlBuilder builder, ParameterCollection output)
        {
            return string.Concat(Exp.OutputSqlString(builder, output), " AS ", Alias);
        }

        internal static AliasExpression Expression(SqlExpression expression, string alias)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (string.IsNullOrEmpty(alias))
                throw new ArgumentNullException("alias");

            return new AliasExpression() { Exp = expression, Alias = alias };
        }
    }
}

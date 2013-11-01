using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Database.Query
{
    public class DbNullExpression : Expression
    {
        public static readonly DbNullExpression DbNull;

        static DbNullExpression()
        {
            DbNull = new DbNullExpression();
        }

        private DbNullExpression()
        {
        }

        public override ExpressionType NodeType
        {
            get { return ExpressionType.Null; }
        }

        public override string OutputSqlString(SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
        {
            return "NULL";
        }
    }
}

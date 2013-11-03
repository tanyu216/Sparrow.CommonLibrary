using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Database.Query
{
    public class DbNullExpression : SqlExpression
    {
        public static readonly DbNullExpression Instance;

        static DbNullExpression()
        {
            Instance = new DbNullExpression();
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

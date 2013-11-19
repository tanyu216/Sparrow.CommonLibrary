using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Query
{
    public class WildcardsExpression : ConstantExpression
    {
        public static readonly WildcardsExpression Instance;

        static WildcardsExpression()
        {
            Instance = new WildcardsExpression();
        }

        private WildcardsExpression()
            : base("%")
        {
        }

        public override string OutputSqlString(Database.SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
        {
            return "'%'";
        }
    }
}

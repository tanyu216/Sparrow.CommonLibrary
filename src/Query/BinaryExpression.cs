using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Query
{
    /// <summary>
    /// 二元运算表达式
    /// </summary>
    public abstract class BinaryExpression : SqlExpression
    {
        public SqlExpression Left { get; protected set; }

        public SqlExpression Right { get; protected set; }

        protected abstract string Operator { get; }

        public override string OutputSqlString(Database.SqlBuilder.ISqlBuilder builder, Database.ParameterCollection output)
        {
            return string.Concat(Left.OutputSqlString(builder, output), Operator, Right.OutputSqlString(builder, output));
        }
    }

}

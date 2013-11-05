using System.Collections.Generic;
using System.Text;
using System.Linq;
using Sparrow.CommonLibrary.Mapper.Metadata;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Database.Query;
using Sparrow.CommonLibrary.Entity;
using System;

namespace Sparrow.CommonLibrary.Database.SqlBuilder
{
    /// <summary>
    /// SqlServer Formater
    /// </summary>
    public class SqlServerStatementBuilder : CommonBuilder
    {
        protected static readonly string SqlCharNoLock = " WITH(NOLOCK) ";
        protected static readonly string SqlCharRowLock = " WITH(ROWLOCK) ";
        protected static readonly string SqlCharUpdateLock = " WITH(UPLOCK) ";
        protected static readonly string SqlCharPageLock = " WITH(PAGELOCK) ";
        protected static readonly string SqlCharTabLock = " WITH(TABLOCK) ";

        /// <summary>
        /// 
        /// </summary>
        public static readonly SqlServerStatementBuilder Default = new SqlServerStatementBuilder();

        protected SqlServerStatementBuilder()
        {

        }

        /// <summary>
        /// Sql查询语句的锁选项
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static string LockOption(SqlOptions options)
        {
            var optVal = (int)options;
            if ((optVal & (int)SqlOptions.None) > 0)
                return SqlCharNoLock;
            if ((optVal & (int)SqlOptions.RowLock) > 0)
                return SqlCharRowLock;
            if ((optVal & (int)SqlOptions.UpdateLock) > 0)
                return SqlCharUpdateLock;
            if ((optVal & (int)SqlOptions.TableLock) > 0)
                return SqlCharTabLock;
            //
            return string.Empty;
        }

        public override string IncrementByQuery(string incrementName, string alias, SqlOptions options)
        {
            return string.Format("SELECT SCOPE_IDENTITY() AS {0} ", BuildAlias(alias));
        }

        public override string IncrementByParameter(string incrementName, string paramName, SqlOptions options)
        {
            return string.Format("SELECT {0}=SCOPE_IDENTITY() ", BuildParameterName(paramName));
        }

        public override string IfExistsFormat(string ifQuerySql, string ifTrueSql, string ifFalseSql, SqlOptions options)
        {
            var sql = new StringBuilder();
            // 
            if (string.IsNullOrWhiteSpace(ifTrueSql) || string.IsNullOrWhiteSpace(ifFalseSql))
            {
                if (string.IsNullOrWhiteSpace(ifTrueSql))
                {
                    sql.Append("IF EXISTS(").Append(ifQuerySql).AppendLine(")");
                    sql.AppendLine("BEGIN").AppendLine(ifTrueSql).AppendLine("END;");
                }
                else
                {
                    sql.Append("IF NOT EXISTS(").Append(ifQuerySql).AppendLine(")");
                    sql.AppendLine("BEGIN").AppendLine(ifFalseSql).AppendLine("END");
                }
            }
            else
            {
                sql.Append("IF EXISTS(").Append(ifQuerySql).AppendLine(")");
                sql.AppendLine("BEGIN").AppendLine(ifTrueSql).AppendLine("END");
                sql.AppendLine(" ELSE ");
                sql.AppendLine("BEGIN").AppendLine(ifFalseSql).AppendLine("END;");
            }
            // 
            return sql.ToString();
        }

        public override string QueryFormat(string topExpression, string fieldExpressions, string tableExpression, string conditionExpressions, string groupbyExpression, string havingExpression, string orderbyExpression, SqlOptions options)
        {
            if (string.IsNullOrEmpty(fieldExpressions))
                throw new ArgumentNullException("fieldExpressions");
            if (string.IsNullOrEmpty(tableExpression))
                throw new ArgumentNullException("tableExpression");

            //select [distinct][top(1)] {fieldExpressions} from {tableName} as {alias}
            var str = new StringBuilder().Append(SqlCharSelect);
            if ((options & SqlOptions.Distinct) > 0)
                str.Append(SqlCharDistinct);
            if (!string.IsNullOrEmpty(topExpression))
                str.Append(SqlCharTop).Append('(').Append(topExpression).Append(')');

            str.Append(fieldExpressions).Append(SqlCharFrom).Append(tableExpression).Append(LockOption(options));

            if (!string.IsNullOrEmpty(conditionExpressions))
                str.Append(SqlCharWhere).Append(conditionExpressions);

            if (!string.IsNullOrEmpty(groupbyExpression))
                str.Append(SqlCharGroupby).Append(groupbyExpression);

            if (!string.IsNullOrEmpty(havingExpression))
                str.Append(SqlCharHaving).Append(havingExpression);

            if (!string.IsNullOrEmpty(orderbyExpression))
                str.Append(SqlCharOrderby).Append(orderbyExpression);

            return str.ToString();
        }

    }
}

using System.Collections.Generic;
using System.Text;
using System.Linq;
using Sparrow.CommonLibrary.Mapper.Metadata;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Query;
using Sparrow.CommonLibrary.Entity;
using System;

namespace Sparrow.CommonLibrary.Database.SqlBuilder
{
    /// <summary>
    /// SqlServer Formater
    /// </summary>
    public class SqlServerStatementBuilder : CommonBuilder
    {
        protected static readonly string KeyWordNoLock = " WITH(NOLOCK) ";
        protected static readonly string KeyWordRowLock = " WITH(ROWLOCK) ";
        protected static readonly string KeyWordUpdateLock = " WITH(UPLOCK) ";
        protected static readonly string KeyWordPageLock = " WITH(PAGELOCK) ";
        protected static readonly string KeyWordTabLock = " WITH(TABLOCK) ";

        /// <summary>
        /// 
        /// </summary>
        public static readonly SqlServerStatementBuilder Default = new SqlServerStatementBuilder();

        public SqlServerStatementBuilder()
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
            if ((optVal & (int)SqlOptions.NoLock) > 0)
                return KeyWordNoLock;
            if ((optVal & (int)SqlOptions.RowLock) > 0)
                return KeyWordRowLock;
            if ((optVal & (int)SqlOptions.UpdateLock) > 0)
                return KeyWordUpdateLock;
            if ((optVal & (int)SqlOptions.TableLock) > 0)
                return KeyWordTabLock;
            //
            return string.Empty;
        }

        public override string IncrementByQuery(string incrementName, string alias, SqlOptions options)
        {
            return string.Format("SELECT SCOPE_IDENTITY() AS {0} ", BuildAlias(alias ?? incrementName));
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

            //select [distinct] [top(1)] {fieldExpressions} from {tableExpression}
            var sql = new StringBuilder(KeyWordSelect);
            if ((options & SqlOptions.Distinct) > 0)
                sql.Append(KeyWordDistinct);
            if (!string.IsNullOrEmpty(topExpression))
                sql.Append(KeyWordTop).Append('(').Append(topExpression).Append(')');

            sql.Append(fieldExpressions).Append(KeyWordFrom).Append(tableExpression).Append(LockOption(options));

            if (!string.IsNullOrEmpty(conditionExpressions))
                sql.Append(KeyWordWhere).Append(conditionExpressions);

            if (!string.IsNullOrEmpty(groupbyExpression))
            {
                sql.Append(KeyWordGroupby).Append(groupbyExpression);

                if (!string.IsNullOrEmpty(havingExpression))
                    sql.Append(KeyWordHaving).Append(havingExpression);
            }

            if (!string.IsNullOrEmpty(orderbyExpression))
                sql.Append(KeyWordOrderby).Append(orderbyExpression);

            return sql.ToString();
        }

        /// <summary>
        /// 有排序表达式的分页查询模板
        /// <para>
        /// select * from (
        ///      select row_number()over(order by {5:orderExpression})AS tt0_temp_rownumber,
        ///            {0:fieldExpressions} from {1:tableExpressions} {8:lock} [where {2:conditionExpression}] [group by {3:groupbyExpression} [having {4:havingExpression}]] 
        ///  )AS TT0
        /// where tt0_temp_rownumber &gt; {6:startIndex} and tt0_temp_rownumber &lt;= {6:startIndex}+{7:rowCount}}
        /// </para>
        /// </summary>
        private static readonly string tmplPageOfSql1 = "SELECT * FROM (SELECT ROW_NUMBER()OVER(ORDER BY {5})AS TT0_TEMP_ROWNUMBER,{0} from {1} {2} {3} {4}) AS TT0 WHERE TT0_TEMP_ROWNUMBER > {6} and TT0_TEMP_ROWNUMBER<={6}+{7}";

        /// <summary>
        /// 无排序表达式的分页查询模板
        /// <para>
        /// select * from (
        ///      select row_number()over(order by tt0_temp_column)AS tt0_temp_rownumber, *
        ///      from(
        ///            select top({6:startIndex}+{7:rowcount}) tt0_temp_column=0,{0:fieldExpressions} from {1:tableExpressions} {8:lock} [where {2:conditionExpression}] [group by {3:groupbyExpression} [having {4:havingExpression}]] 
        ///      )TT0
        ///  )AS TT1
        /// where tt0_temp_rownumber > {6:startIndex}
        /// </para>
        /// </summary>
        private static readonly string tmplPageOfSql2 = "SELECT * FROM (SELECT ROW_NUMBER()OVER(ORDER BY TT0_TEMP_COLUMN)AS TT0_TEMP_ROWNUMBER,* FROM(SELECT TOP({6}+{7}) TT0_TEMP_COLUMN=0,{0} FROM {1} {2} {3} {4}) AS TT0) AS TT1 WHERE TT0_TEMP_ROWNUMBER > {6}";

        public override string QueryFormat(string fieldExpressions, string tableExpression, string conditionExpressions, string groupbyExpression, string havingExpression, string orderbyExpression, int startIndex, int rowCount, SqlOptions options)
        {
            if (string.IsNullOrEmpty(fieldExpressions))
                throw new ArgumentNullException("fieldExpressions");
            if (string.IsNullOrEmpty(tableExpression))
                throw new ArgumentNullException("tableExpression");

            string tmpl = string.IsNullOrEmpty(orderbyExpression) ? tmplPageOfSql2 : tmplPageOfSql1;

            return string.Format(tmpl, fieldExpressions, tableExpression,
                string.IsNullOrEmpty(conditionExpressions) ? null : string.Concat(KeyWordWhere, conditionExpressions),
                string.IsNullOrEmpty(groupbyExpression) ? null : string.Concat(KeyWordGroupby, groupbyExpression),
                string.IsNullOrEmpty(groupbyExpression) || string.IsNullOrEmpty(havingExpression) ? null : string.Concat(KeyWordHaving, havingExpression),
                orderbyExpression,
                Constant(startIndex), Constant(rowCount), LockOption(options));
        }
    }
}

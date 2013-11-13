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
        protected static readonly string WordNoLock = " WITH(NOLOCK) ";
        protected static readonly string WordRowLock = " WITH(ROWLOCK) ";
        protected static readonly string WordUpdateLock = " WITH(UPLOCK) ";
        protected static readonly string WordPageLock = " WITH(PAGELOCK) ";
        protected static readonly string WordTabLock = " WITH(TABLOCK) ";

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
                return WordNoLock;
            if ((optVal & (int)SqlOptions.RowLock) > 0)
                return WordRowLock;
            if ((optVal & (int)SqlOptions.UpdateLock) > 0)
                return WordUpdateLock;
            if ((optVal & (int)SqlOptions.TableLock) > 0)
                return WordTabLock;
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
            var str = new StringBuilder().Append(WordSelect);
            if ((options & SqlOptions.Distinct) > 0)
                str.Append(WordDistinct);
            if (!string.IsNullOrEmpty(topExpression))
                str.Append(WordTop).Append('(').Append(topExpression).Append(')');

            str.Append(fieldExpressions).Append(WordFrom).Append(tableExpression).Append(LockOption(options));

            if (!string.IsNullOrEmpty(conditionExpressions))
                str.Append(WordWhere).Append(conditionExpressions);

            if (!string.IsNullOrEmpty(groupbyExpression))
                str.Append(WordGroupby).Append(groupbyExpression);

            if (!string.IsNullOrEmpty(havingExpression))
                str.Append(WordHaving).Append(havingExpression);

            if (!string.IsNullOrEmpty(orderbyExpression))
                str.Append(WordOrderby).Append(orderbyExpression);

            return str.ToString();
        }

        /// <summary>
        /// 有排序表达式的分页查询模板
        /// select * from (
        ///      select row_number()over(order by {5:orderExpression})AS tt0__rownumber,
        ///            {0:fieldExpressions} from {1:tableExpressions} [where {2:conditionExpression}] [group by {3:groupbyExpression} [having {4:havingExpression}]] 
        ///  )AS TT0
        /// where tt0__rownumber &gt; {6:startIndex} and tt0__rownumber &lt;= {6:startIndex}+{7:rowCount}}
        /// </summary>
        private static readonly string tmplPageOfSql1 = "SELECT * FROM (SELECT ROW_NUMBER()OVER(ORDER BY {5})AS TT0_TEMP_ROWNUMBER,{0} from {1} {2} {3} {4}) AS TT0 WHERE TT0_TEMP_ROWNUMBER > {6} and TT0_TEMP_ROWNUMBER<={6}+{7}";

        /// <summary>
        /// 无排序表达式的分页查询模板
        /// select * from (
        ///      select row_number()over(order by tt0_temp_column)AS tt0__rownumber, *
        ///      from(
        ///            select top({6:startIndex}+{7:rowcount}) tt0_temp_column=0,{0:fieldExpressions} from {1:tableExpressions} [where {2:conditionExpression}] [group by {3:groupbyExpression} [having {4:havingExpression}]] 
        ///      )TT0
        ///  )AS TT1
        /// where tt0__rownumber > {6:startIndex}
        /// </summary>
        private static readonly string tmplPageOfSql2 = "SELECT * FROM (SELECT ROW_NUMBER()OVER(ORDER BY TT0_TEMP_COLUMN)AS TT0_TEMP_ROWNUMBER,* FROM(SELECT TOP({6}+{7}) TT0_TEMP_COLUMN=0,{0} FROM {1} {2} {3} {4})TT0) AS TT1 WHERE TT0_TEMP_ROWNUMBER > {6}";

        public override string QueryFormat(string fieldExpressions, string tableExpression, string conditionExpressions, string groupbyExpression, string havingExpression, string orderbyExpression, int startIndex, int rowCount, SqlOptions options)
        {
            if (string.IsNullOrEmpty(fieldExpressions))
                throw new ArgumentNullException("fieldExpressions");
            if (string.IsNullOrEmpty(tableExpression))
                throw new ArgumentNullException("tableExpression");

            string tmpl = string.IsNullOrEmpty(orderbyExpression) ? tmplPageOfSql1 : tmplPageOfSql2;

            var nest1 = new StringBuilder().Append(WordSelect);
            nest1.Append("ROW_NUMBER()OVER(").Append(WordOrderby).Append(orderbyExpression).Append(")AS TT0_TEMP_ROWNUMBER,");
            nest1.Append(fieldExpressions);
            nest1.Append(WordFrom).Append(tableExpression);

            if (!string.IsNullOrEmpty(conditionExpressions))
                nest1.Append(WordWhere).Append(conditionExpressions);

            if (!string.IsNullOrEmpty(groupbyExpression))
            {
                nest1.Append(WordGroupby).Append(groupbyExpression);

                if (!string.IsNullOrEmpty(havingExpression))
                    nest1.Append(WordHaving).Append(havingExpression);
            }

            return new StringBuilder()
                .Append(WordSelect).Append(WordAllFields)
                .Append(WordFrom).Append('(').Append(nest1).Append(")AS TT0")
                .Append(WordWhere).Append("TT0_TEMP_ROWNUMBER>=").Append(Constant(startIndex + 1)).Append(" AND TT0_TEMP_ROWNUMBER<=").Append(Constant(startIndex + rowCount))
                .ToString();
        }
    }
}

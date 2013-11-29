using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Sparrow.CommonLibrary.Mapper.Metadata;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Query;
using Sparrow.CommonLibrary.Entity;

namespace Sparrow.CommonLibrary.Database.SqlBuilder
{
    /// <summary>
    /// Oracle Provider
    /// </summary>
    public class OracleStatementBuilder : CommonBuilder
    {
        protected static readonly string KeyWordRowLock = " FOR UPDATE ";
        protected static readonly string KeyWordUpdateLock = " FOR UPDATE ";
        protected static readonly string KeyWordTabLock = " IN EXCLUSIVE ";

        /// <summary>
        /// 
        /// </summary>
        public static readonly OracleStatementBuilder Default = new OracleStatementBuilder();

        public OracleStatementBuilder()
        {

        }

        public override string BuildParameterName(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException("parameterName");

            if (parameterName.StartsWith("@"))
            {
                return string.Concat(":", parameterName.Substring(1));
            }

            if (!parameterName.StartsWith(":"))
            {
                return string.Concat(":", parameterName);
            }
            return parameterName;
        }

        public override string Constant(object value)
        {
            if (value is DateTime)
                return string.Format("to_date('{0:yyyy/MM/dd HH:mm:ss}','YYYY/MM/DD HH24:MI:SS')", value);

            return base.Constant(value);
        }

        /// <summary>
        /// Sql查询语句的锁选项
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static string LockOption(SqlOptions options)
        {
            var optVal = (int)options;
            if ((optVal & (int)SqlOptions.RowLock) > 0)
                return KeyWordRowLock;
            if ((optVal & (int)SqlOptions.UpdateLock) > 0)
                return KeyWordUpdateLock;
            if ((optVal & (int)SqlOptions.TableLock) > 0)
                return KeyWordTabLock;
            //
            return string.Empty;
        }

        public override string InsertFormat(string tableName, IEnumerable<ItemValue<string, string>> fieldAndExpressions, string incrementField, string incrementName, SqlOptions options)
        {
            if (string.IsNullOrEmpty(incrementField))
                throw new ArgumentNullException("incrementField");
            if (string.IsNullOrEmpty(incrementName))
                throw new ArgumentNullException("incrementName");

            var fields = ExpressionsJoin(fieldAndExpressions.Select(x => BuildField(x.Item)));
            var values = ExpressionsJoin(fieldAndExpressions.Select(x => x.Value));
            // insert into {tableName}({fields})values({values})
            return new StringBuilder()
                .Append(KeyWordInsertInto).Append(BuildTableName(tableName))
                .Append("(").Append(BuildField(incrementField)).Append(',').Append(fields).Append(")")
                .Append(KeyWordValues).Append("(").Append(incrementName).Append(".NEXTVAL,").Append(values).Append(")")
                .ToString();
        }

        public override string IncrementByQuery(string incrementName, string alias, SqlOptions options)
        {
            return string.Format("SELECT {0}.CURRVAL {1} FROM DUAL", incrementName, BuildAlias(alias ?? incrementName));
        }

        public override string IncrementByParameter(string incrementName, string paramName, SqlOptions options)
        {
            return string.Format("{1}:={0}.CURRVAL ", incrementName, BuildParameterName(paramName));
        }

        public override string IfExistsFormat(string ifQuerySql, string ifTrueSql, string ifFalseSql, SqlOptions options)
        {
            throw new NotImplementedException();
        }

        public override string QueryFormat(string topExpression, string fieldExpressions, string tableExpression, string conditionExpressions, string groupbyExpression, string havingExpression, string orderbyExpression, SqlOptions options)
        {
            if (string.IsNullOrEmpty(fieldExpressions))
                throw new ArgumentNullException("fieldExpressions");
            if (string.IsNullOrEmpty(tableExpression))
                throw new ArgumentNullException("tableExpression");

            //select [distinct][top(1)] {fieldExpressions} from {tableExpression} [where {conditionExpression}] [group by {groupbyExpression} [having {havingExpression}]] [{lock} ]
            var sql = new StringBuilder(KeyWordSelect);
            if ((options & SqlOptions.Distinct) > 0)
                sql.Append(KeyWordDistinct);
            if (!string.IsNullOrEmpty(topExpression))
                sql.Append(KeyWordTop).Append('(').Append(topExpression).Append(')');

            sql.Append(fieldExpressions).Append(KeyWordFrom).Append(tableExpression);

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

            return sql.Append(LockOption(options)).ToString();
        }

        /// <summary>
        /// 分页查询模板
        /// <para>
        /// select * from (
        ///     select *,rownum AS tt0_temp_rownumber from(
        ///         select {0:feildExpressions} from {1:tableExpression} [where {2:conditionExpression}] [group by {3:groupbyExpression} [having {4:havingExpression}]] {8:lock} 
        ///     )as tt0 where rownum &lt;= {6:startIndex}+{7:rowcount}
        /// )as tt1 where tt0_temp_rownumber &gt; {6:startIndex}
        /// </para>
        /// </summary>
        private static readonly string tmplPageOfSql1 = "SELECT /*+ FIRST_ROWS */ * FROM (SELECT *,ROWNUM AS TT0_TEMP_ROWNUMBER FROM(SELECT {0} FROM {1} {2} {3} {4} {5} {8} )AS TT0 WHERE ROWNUM <= {6}+{7}) AS TT1WHERE TT0_TEMP_ROWNUMBER<{6}";

        public override string QueryFormat(string fieldExpressions, string tableExpression, string conditionExpressions, string groupbyExpression, string havingExpression, string orderbyExpression, int startIndex, int rowCount, SqlOptions options)
        {
            if (string.IsNullOrEmpty(fieldExpressions))
                throw new ArgumentNullException("fieldExpressions");
            if (string.IsNullOrEmpty(tableExpression))
                throw new ArgumentNullException("tableExpression");

            return string.Format(tmplPageOfSql1, fieldExpressions, tableExpression,
                string.IsNullOrEmpty(conditionExpressions) ? null : string.Concat(KeyWordWhere, conditionExpressions),
                string.IsNullOrEmpty(groupbyExpression) ? null : string.Concat(KeyWordGroupby, groupbyExpression),
                string.IsNullOrEmpty(groupbyExpression) || string.IsNullOrEmpty(havingExpression) ? null : string.Concat(KeyWordHaving, havingExpression),
                string.IsNullOrEmpty(orderbyExpression) ? null : string.Concat(KeyWordOrderby, orderbyExpression),
                Constant(startIndex), Constant(rowCount), LockOption(options));
        }
    }
}

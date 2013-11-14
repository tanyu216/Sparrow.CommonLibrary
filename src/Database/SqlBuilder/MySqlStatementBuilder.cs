using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Mapper.Metadata;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Query;
using Sparrow.CommonLibrary.Entity;
using System.Text.RegularExpressions;

namespace Sparrow.CommonLibrary.Database.SqlBuilder
{
    /// <summary>
    /// Mysql Provider
    /// </summary>
    public class MySqlStatementBuilder : CommonBuilder
    {
        private static readonly string KeyWordLimit = " LIMIT ";

        public static readonly MySqlStatementBuilder Default = new MySqlStatementBuilder();

        readonly static Regex testNameRegex = new Regex(@"^(`\w+`)(\.`\w+`)*$", RegexOptions.Compiled);
        readonly static Regex matchNamesRegex = new Regex(@"(\w+)", RegexOptions.Compiled);

        protected MySqlStatementBuilder()
        {

        }

        public override string BuildAlias(string alias)
        {
            return string.Concat("`", matchNamesRegex.Match(alias).Value, "`");
        }

        public override string BuildField(string field)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentException("field");

            if (testNameRegex.IsMatch(field))
                return field;

            var matchs = matchNamesRegex.Matches(field);
            if (matchs.Count == 1)
                return string.Concat("`", matchs[0].Value, "`");

            var str = new StringBuilder();
            foreach (Match match in matchs)
            {
                str.Append('.').Append('`').Append(match.Value).Append('`');
            }
            return str.Remove(0, 1).ToString();
        }

        public override string BuildTableName(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("tableName");

            return BuildField(tableName);
        }

        public override string IncrementByQuery(string incrementName, string alias, SqlOptions options)
        {
            return string.Format("SELECT LAST_INSERT_ID() AS {0}", BuildAlias(alias ?? incrementName));
        }

        public override string IncrementByParameter(string incrementName, string paramName, SqlOptions options)
        {
            return string.Concat("SELECT {0}:=LAST_INSERT_ID() ", BuildParameterName(paramName));
        }

        public override string IfExistsFormat(string ifQuerySql, string ifTrueSql, string ifFalseSql, SqlOptions options)
        {
            var sql = new StringBuilder();
            // 
            if (string.IsNullOrWhiteSpace(ifTrueSql) || string.IsNullOrWhiteSpace(ifFalseSql))
            {
                if (string.IsNullOrWhiteSpace(ifTrueSql))
                {
                    sql.Append("IF EXISTS(").Append(ifQuerySql).AppendLine(") THEN");
                    sql.AppendLine(ifTrueSql);
                    sql.AppendLine("END IF");
                }
                else
                {
                    sql.Append("IF NOT EXISTS(").Append(ifQuerySql).AppendLine(") THEN");
                    sql.AppendLine(ifFalseSql);
                    sql.AppendLine("END IF");
                }
            }
            else
            {
                sql.Append("IF EXISTS(").Append(ifQuerySql).AppendLine(") THEN");
                sql.AppendLine(ifTrueSql);
                sql.AppendLine(" ELSE ");
                sql.AppendLine(ifFalseSql);
                sql.AppendLine("END IF");
            }
            // 
            return sql.ToString();
        }

        public override string QueryFormat(string fieldExpressions, string tableExpression, string conditionExpressions, string groupbyExpression, string havingExpression, string orderbyExpression, int startIndex, int rowCount, SqlOptions options)
        {
            if (string.IsNullOrEmpty(fieldExpressions))
                throw new ArgumentNullException("fieldExpressions");
            if (string.IsNullOrEmpty(tableExpression))
                throw new ArgumentNullException("tableExpression");

            var sql = new StringBuilder(KeyWordSelect);
            if ((options & SqlOptions.Distinct) > 0)
                sql.Append(KeyWordDistinct);

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

            // limit 10,20
            sql.Append(KeyWordLimit).Append(Constant(startIndex)).Append(',').Append(Constant(startIndex + rowCount));

            return sql.ToString();
        }
    }
}

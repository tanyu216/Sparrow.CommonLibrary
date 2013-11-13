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
            throw new NotImplementedException();
        }

        public override string IncrementByParameter(string incrementName, string paramName, SqlOptions options)
        {
            throw new NotImplementedException();
        }

        public override string IfExistsFormat(string ifQuerySql, string ifTrueSql, string ifFalseSql, SqlOptions options)
        {
            throw new NotImplementedException();
        }
    }
}

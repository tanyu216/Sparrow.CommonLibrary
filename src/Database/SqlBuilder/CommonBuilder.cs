using Sparrow.CommonLibrary.Entity;
using Sparrow.CommonLibrary.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sparrow.CommonLibrary.Database.SqlBuilder
{
    /// <summary>
    /// 抽象的DML/DQL通用生成器
    /// </summary>
    public abstract class CommonBuilder : ISqlBuilder
    {
        #region SqlChar

        protected static readonly string SqlCharSelect = "SELECT ";
        protected static readonly string SqlCharDelete = "DELETE ";
        protected static readonly string SqlCharUpdate = "UPDATE ";
        protected static readonly string SqlCharInsertInto = "INSERT INTO ";
        protected static readonly string SqlCharSet = " SET ";
        protected static readonly string SqlCharFrom = " FROM ";
        protected static readonly string SqlCharAllFields = " * ";
        protected static readonly string SqlCharValues = " VALUES ";
        protected static readonly string SqlCharWhere = " WHERE ";
        protected static readonly string SqlCharGroupby = " GROUP BY ";
        protected static readonly string SqlCharHaving = " HAVING ";
        protected static readonly string SqlCharOrderby = " ORDER BY ";
        protected static readonly string SqlCharTop = " TOP ";
        protected static readonly string SqlCharDistinct = " DISTINCT ";

        #endregion

        readonly static Regex testNameRegex = new Regex(@"^(\[\w+\])(\.\[\w+\])*$");
        readonly static Regex matchNamesRegex = new Regex(@"(\w+)");

        public virtual string BuildAlias(string alias)
        {
            return string.Concat("[", matchNamesRegex.Match(alias).Value, "]");
        }

        public virtual string BuildField(string field)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentException("field");

            if (testNameRegex.IsMatch(field))
                return field;

            var matchs = matchNamesRegex.Matches(field);
            if (matchs.Count == 1)
                return string.Concat("[", matchs[0].Value, "]");

            var str = new StringBuilder();
            foreach (Match match in matchs)
            {
                str.Append('.').Append('[').Append(match.Value).Append(']');
            }
            return str.Remove(0, 1).ToString();
        }

        public virtual string BuildField(string field, string alias)
        {
            field = BuildField(field);
            if (string.IsNullOrEmpty(alias))
                return field;
            else
                return string.Concat(field, " AS ", BuildAlias(alias));
        }

        public virtual string BuildTableName(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("tableName");

            return BuildField(tableName);
        }

        public virtual string BuildTableName(string tableName, string alias)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("tableName");

            return BuildField(tableName, alias);
        }

        public virtual string BuildParameterName(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException("parameterName");

            if (!parameterName.StartsWith("@"))
            {
                return string.Concat("@", parameterName);
            }
            return parameterName;
        }

        public virtual string BuildFuncName(string functionName)
        {
            return functionName;
        }

        public virtual string BuildExpressionWithAlias(string expression, string alias)
        {
            if (string.IsNullOrEmpty(expression))
                throw new ArgumentNullException("expression");

            if (string.IsNullOrEmpty(alias))
                return expression;

            return string.Concat(expression, " AS ", BuildAlias(alias));
        }

        public virtual string Constant(object value)
        {
            if (value == null || value == DBNull.Value)
                return "NULL";
            if (value is string || value is char)
                return string.Concat("'", value.ToString(), "'");
            if (value is Int16 || value is Int32 || value is Int64 || value is UInt16 || value is UInt32 || value is UInt64)
                return value.ToString();
            if (value is decimal || value is double || value is float)
                return value.ToString();
            if (value is Timestamp)
                return value.ToString();
            if (value is DateTime)
                return ((DateTime)value).ToString("'yyyy-MM-dd HH:mm:ss'");
            if (value is char[])
                return new string((char[])value);
            if (value is byte)
                return value.ToString();
            if (value is bool)
                return (bool)value ? "true" : "false";
            if (value is byte[])
            {
                var content = new StringBuilder("0x");
                foreach (var b in (byte[])value)
                    content.Append(b.ToString("0x"));
            }
            //
            throw new ArgumentException(string.Format("不支持常量类型：{0}", value.GetType().FullName));
        }

        public virtual string ExpressionsJoin(IEnumerable<string> items)
        {
            if (items == null || items.Any() == false)
                throw new ArgumentNullException("names");

            var str = new StringBuilder();
            foreach (var item in items)
            {
                str.Append(',').Append(item);
            }
            return str.Remove(0, 1).ToString();
        }

        public abstract string IncrementByQuery(string incrementName, string alias, SqlOptions options);

        public abstract string IncrementByParameter(string incrementName, string paramName, SqlOptions options);

        public abstract string IfExistsFormat(string ifQuerySql, string ifTrueSql, string ifFalseSql, SqlOptions options);

        public virtual string InsertFormat(string tableName, IEnumerable<ItemValue<string, string>> fieldAndValueExpressions, SqlOptions options)
        {
            if (fieldAndValueExpressions == null || !fieldAndValueExpressions.Any())
                throw new ArgumentNullException("fieldAndValueExpressions");

            var fields = ExpressionsJoin(fieldAndValueExpressions.Select(x => BuildField(x.Item)));
            var values = ExpressionsJoin(fieldAndValueExpressions.Select(x => x.Value));
            // insert into {tableName}({fields})values({values})
            return new StringBuilder()
                .Append(SqlCharInsertInto).Append(BuildTableName(tableName))
                .Append("(").Append(fields).Append(")")
                .Append(SqlCharValues).Append("(").Append(values).Append(")")
                .ToString();
        }

        public virtual string InsertFormat(string tableName, IEnumerable<ItemValue<string, string>> fieldAndValueExpressions, string incrementField, string incrementName, SqlOptions options)
        {

            //默认自增长字段不需要处理
            return InsertFormat(tableName, fieldAndValueExpressions, options);
        }

        public virtual string UpdateFormat(string tableName, IEnumerable<ItemValue<string, string>> fieldAndValueExpressions, string condition, SqlOptions options)
        {
            if (fieldAndValueExpressions == null || !fieldAndValueExpressions.Any())
                throw new ArgumentNullException("fieldAndValueExpressions");

            var sql = new StringBuilder();
            // 1、生成UPDATE语句前半部分
            // 示例：UPDATE TableName SET
            sql.Append(SqlCharUpdate).Append(BuildTableName(tableName)).Append(SqlCharSet);

            // 2、生成中间set字段值部分
            // 示例：ColumnName=@parameterName,
            foreach (var item in fieldAndValueExpressions)
            {
                // 逐个加入成员字段
                sql.Append(BuildField(item.Item)).Append('=').Append(item.Value).Append(",");
            }
            // 移除生成sql语句时最后留下的一个逗号
            sql.Remove(sql.Length - 1, 1);

            // 3、加入修改条件
            if (!string.IsNullOrEmpty(condition))
                sql.Append(SqlCharWhere).Append(condition);

            // 4、返回
            return sql.ToString();
        }

        public virtual string DeleteFormat(string tableName, string condition, SqlOptions options)
        {
            //delete from {tableName} where {condition}
            var str = new StringBuilder().Append(SqlCharDelete).Append(BuildTableName(tableName));

            if (!string.IsNullOrEmpty(condition))
                str.Append(SqlCharWhere).Append(condition);

            return str.ToString();
        }

        public virtual string QueryFormat(string tableName, string fieldExpressions, string conditionExpressions, SqlOptions options)
        {
            return QueryFormat(tableName, null, fieldExpressions, conditionExpressions, options);
        }

        public virtual string QueryFormat(string tableName, string alias, string fieldExpressions, string conditionExpressions, SqlOptions options)
        {
            //select {fieldExpressions} from {tableName} as {alias} where {condition}
            var str = new StringBuilder().Append(SqlCharSelect).Append(fieldExpressions).Append(SqlCharFrom).Append(BuildTableName(tableName, alias));

            if (!string.IsNullOrEmpty(conditionExpressions))
                str.Append(SqlCharWhere).Append(conditionExpressions);

            return str.ToString();
        }

        public virtual string QueryFormat(string topExpression, string fieldExpressions, string tableExpression, string conditionExpressions, string groupbyExpression, string havingExpression, string orderbyExpression, SqlOptions options)
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

            str.Append(fieldExpressions).Append(SqlCharFrom).Append(tableExpression);

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

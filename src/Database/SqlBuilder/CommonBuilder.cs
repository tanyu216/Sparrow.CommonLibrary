﻿using Sparrow.CommonLibrary.Entity;
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
        #region Words

        protected static readonly string WordSelect = "SELECT ";
        protected static readonly string WordDelete = "DELETE ";
        protected static readonly string WordUpdate = "UPDATE ";
        protected static readonly string WordInsertInto = "INSERT INTO ";
        protected static readonly string WordSet = " SET ";
        protected static readonly string WordFrom = " FROM ";
        protected static readonly string WordAllFields = " * ";
        protected static readonly string WordValues = " VALUES ";
        protected static readonly string WordWhere = " WHERE ";
        protected static readonly string WordGroupby = " GROUP BY ";
        protected static readonly string WordHaving = " HAVING ";
        protected static readonly string WordOrderby = " ORDER BY ";
        protected static readonly string WordTop = " TOP ";
        protected static readonly string WordDistinct = " DISTINCT ";

        #endregion

        readonly static Regex testNameRegex = new Regex(@"^(\[\w+\])(\.\[\w+\])*$", RegexOptions.Compiled);
        readonly static Regex matchNamesRegex = new Regex(@"(\w+)", RegexOptions.Compiled);

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
            if (value is Int32 || value is Int64 || value is Int16 || value is UInt32 || value is UInt64 || value is UInt16)
                return value.ToString();
            if (value is decimal || value is double || value is float)
                return value.ToString();
            if (value is string || value is char)
                return string.Concat("'", value.ToString(), "'");
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
                .Append(WordInsertInto).Append(BuildTableName(tableName))
                .Append("(").Append(fields).Append(")")
                .Append(WordValues).Append("(").Append(values).Append(")")
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
            sql.Append(WordUpdate).Append(BuildTableName(tableName)).Append(WordSet);

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
                sql.Append(WordWhere).Append(condition);

            // 4、返回
            return sql.ToString();
        }

        public virtual string DeleteFormat(string tableName, string condition, SqlOptions options)
        {
            //delete from {tableName} where {condition}
            var str = new StringBuilder().Append(WordDelete).Append(BuildTableName(tableName));

            if (!string.IsNullOrEmpty(condition))
                str.Append(WordWhere).Append(condition);

            return str.ToString();
        }

        public virtual string QueryFormat(string topExpression, string fieldExpressions, string tableExpression, string conditionExpressions, string groupbyExpression, string havingExpression, string orderbyExpression, SqlOptions options)
        {
            if (string.IsNullOrEmpty(fieldExpressions))
                throw new ArgumentNullException("fieldExpressions");
            if (string.IsNullOrEmpty(tableExpression))
                throw new ArgumentNullException("tableExpression");

            //select [distinct][top(1)] {fieldExpressions} from {tableExpression} [where conditionExpression] [group by {groupbyExpression} [having {havingExpression}]] [order by {orderbyExpression}]
            var str = new StringBuilder().Append(WordSelect);
            if ((options & SqlOptions.Distinct) > 0)
                str.Append(WordDistinct);
            if (!string.IsNullOrEmpty(topExpression))
                str.Append(WordTop).Append('(').Append(topExpression).Append(')');

            str.Append(fieldExpressions).Append(WordFrom).Append(tableExpression);

            if (!string.IsNullOrEmpty(conditionExpressions))
                str.Append(WordWhere).Append(conditionExpressions);

            if (!string.IsNullOrEmpty(groupbyExpression))
            {
                str.Append(WordGroupby).Append(groupbyExpression);

                if (!string.IsNullOrEmpty(havingExpression))
                    str.Append(WordHaving).Append(havingExpression);
            }

            if (!string.IsNullOrEmpty(orderbyExpression))
                str.Append(WordOrderby).Append(orderbyExpression);

            return str.ToString();
        }


        private static readonly Regex orderReplace = new Regex(@"\s+(DESC|ASC)\s*($|,)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 默认的分页查询，问题：该查询的末页返回数据不足<paramref name="rowCount"/>时，将会返提取前一页的行补齐。需要手动处理
        /// </summary>
        /// <param name="fieldExpressions"></param>
        /// <param name="tableExpression"></param>
        /// <param name="conditionExpressions"></param>
        /// <param name="groupbyExpression"></param>
        /// <param name="havingExpression"></param>
        /// <param name="orderbyExpression"></param>
        /// <param name="startIndex"></param>
        /// <param name="rowCount"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public virtual string QueryFormat(string fieldExpressions, string tableExpression, string conditionExpressions, string groupbyExpression, string havingExpression, string orderbyExpression, int startIndex, int rowCount, SqlOptions options)
        {
            if (string.IsNullOrEmpty(fieldExpressions))
                throw new ArgumentNullException("fieldExpressions");
            if (string.IsNullOrEmpty(tableExpression))
                throw new ArgumentNullException("tableExpression");
            if (string.IsNullOrEmpty(orderbyExpression))
                throw new ArgumentNullException("orderbyExpression");

            // select top {rowCount} * from (select top ({startIndex}+{rowCount}) {fieldExpressions} from {tableExpressions} [where conditionExpression] [group by {groupbyExpression} [having havingExpression]] by order by {orderExpression}))
            var nest1 = new StringBuilder().Append(WordSelect);
            nest1.Append(WordTop).Append('(').Append(Constant(startIndex + rowCount)).Append(')');
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

            nest1.Append(WordOrderby).Append(orderbyExpression);

            var reverseOrderby = orderReplace.Replace(orderbyExpression, x =>
            {
                switch ((x.Groups[1].Value ?? string.Empty).ToUpper())
                {
                    case "ASC":
                        if ((x.Groups[2].Value ?? string.Empty) == ",")
                            return " DESC,";
                        return " DESC ";
                    case "DESC":
                        if ((x.Groups[2].Value ?? string.Empty) == ",")
                            return " ASC,";
                        return " ASC ";
                }
                return string.Empty;
            });

            var nest2 = new StringBuilder()
                .Append(WordSelect).Append(WordTop).Append('(').Append(Constant(rowCount)).Append(')')
                .Append(WordFrom).Append('(').Append(nest1.ToString()).Append(") AS TT0")
                .Append(WordOrderby).Append(reverseOrderby)
                .ToString();

            return new StringBuilder()
                .Append(WordSelect).Append(fieldExpressions)
                .Append(WordFrom).Append('(').Append(nest2).Append(") AS TT1")
                .Append(WordOrderby).Append(orderbyExpression)
                .ToString();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Data.Mapper.Metadata;
using Sparrow.CommonLibrary.Data.Database;
using Sparrow.CommonLibrary.Data.Query;
using Sparrow.CommonLibrary.Data.Entity;
using Sparrow.CommonLibrary.Data.Configuration;
using System.Collections.Concurrent;

namespace Sparrow.CommonLibrary.Data.SqlBuilder
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

        #endregion

        public virtual string BuildField(string field)
        {
            if (field.IndexOf('.') > -1)
                return string.Join(".", field.Split('.').Select(x => string.Concat("[", x, "]")));
            return string.Concat("[", field, "]");
        }

        public virtual string BuildTableName(IMetaInfo metaInfo)
        {
            if (metaInfo.Name.IndexOf('.') > -1)
                return string.Join(".", metaInfo.Name.Split('.').Select(x => string.Concat("[", x, "]")));
            return string.Concat("[", metaInfo.Name, "]");
        }

        public virtual string BuildParameterName(string parameterName)
        {
            if (!string.IsNullOrEmpty(parameterName) && !parameterName.StartsWith("@"))
            {
                return string.Concat("@", parameterName);
            }
            return parameterName;
        }

        public virtual string BuildFuncName(string functionName)
        {
            return functionName;
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

        public virtual string Delete(IMetaInfo metaInfo, IEnumerable<ItemValue> conditions, ParameterCollection output, SqlOptions options)
        {
            return string.Concat(SqlCharDelete, SqlCharFrom, this.BuildTableName(metaInfo), SqlCharWhere, Where(conditions, output, options));
        }

        public virtual string Delete(IMetaInfo metaInfo, ConditionExpressions expressions, ParameterCollection output, SqlOptions options)
        {
            return string.Concat(SqlCharDelete, SqlCharFrom, this.BuildTableName(metaInfo), SqlCharWhere, Where(expressions, output, options));
        }

        public virtual string Update(IMetaInfo metaInfo, IEnumerable<ItemValue> fields, IEnumerable<ItemValue> conditions, ParameterCollection output, SqlOptions options)
        {
            var sql = new StringBuilder();
            // 1、生成UPDATE语句前半部分
            // 示例：UPDATE TableName SET
            sql.Append(SqlCharUpdate).Append(BuildTableName(metaInfo)).Append(SqlCharSet);

            // 2、生成中间set字段值部分
            // 示例：ColumnName=@parameterName,
            foreach (var field in fields)
            {
                // 逐个加入成员字段
                sql.Append(BuildField(field.Item)).Append('=').Append(output.Append(field.Item, field.Value, true).ParameterName).Append(",");
            }
            // 移除生成sql语句时最后留下的一个逗号
            sql.Remove(sql.Length - 1, 1);

            // 3、加入修改条件
            sql.Append(SqlCharWhere).Append(Where(conditions, output, options));

            // 4、返回
            return sql.ToString();
        }

        public virtual string Update(IMetaInfo metaInfo, IEnumerable<ItemValue> fields, ConditionExpressions expressions, ParameterCollection output, SqlOptions options)
        {
            var sql = new StringBuilder();
            // 1、生成UPDATE语句前半部分
            // 示例：UPDATE TableName SET
            sql.Append(SqlCharUpdate).Append(BuildTableName(metaInfo)).Append(SqlCharSet);

            // 2、生成中间set字段值部分
            // 示例：ColumnName=@parameterName,
            foreach (var field in fields)
            {
                // 逐个加入成员字段
                sql.Append(BuildField(field.Item)).Append('=').Append(output.Append(field.Item, field.Value, true).ParameterName).Append(",");
            }
            // 移除生成sql语句时最后留下的一个逗号
            sql.Remove(sql.Length - 1, 1);

            // 3、加入修改条件
            sql.Append(SqlCharWhere).Append(Where(expressions, output, options));

            // 4、返回
            return sql.ToString();
        }

        public virtual string Insert(IMetaInfo metaInfo, IEnumerable<ItemValue> values, ParameterCollection output, SqlOptions options)
        {
            var sqlPart1 = new StringBuilder();
            var sqlPart2 = new StringBuilder();

            // 1、生成INSERT语句的前半部分
            // 示例：INSERT INTO TableName (
            sqlPart1.Append(SqlCharInsertInto).Append(BuildTableName(metaInfo)).Append('(');

            // 2、生成中间的字段部分
            // 示例：ColName1,ColName2
            // 3、生成参数部分
            // 示例：@Val1,@Val2
            foreach (var current in values)
            {
                // 默认情况下，Insert需要忽略增量标识列。像Oracle中序列的特殊用法需要重新实现。
                var metaDb = metaInfo as IMetaInfoForDbTable;
                if (metaDb != null && metaDb.Identity != null && metaDb.Identity.Name == current.Item)
                    continue;

                // 逐个加入语句的列元素
                sqlPart1.Append(BuildField(current.Item)).Append(',');
                // 逐个加入参数元素
                sqlPart2.Append(output.Append(current.Item, current.Value, true).ParameterName).Append(',');
            }
            sqlPart1.Remove(sqlPart1.Length - 1, 1);
            sqlPart2.Remove(sqlPart2.Length - 1, 1);
            // ) VALUES (
            sqlPart1.Append(')').Append(SqlCharValues).Append('(').Append(sqlPart2.ToString()).Append(')');

            // 4、返回
            return sqlPart1.ToString();
        }

        public virtual string Select(IMetaInfo metaInfo, IEnumerable<ItemValue<string, string>> fields, IEnumerable<ItemValue> conditions, ParameterCollection output, SqlOptions options)
        {
            return SelectStatmentFormate(metaInfo, FieldsJoin(fields), Where(conditions, output, options), options);
        }

        public virtual string Select(IMetaInfo metaInfo, IEnumerable<ItemValue<string, string>> fields, ConditionExpressions expressions, ParameterCollection output, SqlOptions options)
        {
            return SelectStatmentFormate(metaInfo, FieldsJoin(fields), Where(expressions, output, options), options);
        }

        public virtual string Select(IMetaInfo metaInfo, IEnumerable<string> fields, IEnumerable<ItemValue> conditions, ParameterCollection output, SqlOptions options)
        {
            return SelectStatmentFormate(metaInfo, FieldsJoin(fields), Where(conditions, output, options), options);
        }

        public virtual string Select(IMetaInfo metaInfo, IEnumerable<string> fields, ConditionExpressions expressions, ParameterCollection output, SqlOptions options)
        {
            return SelectStatmentFormate(metaInfo, FieldsJoin(fields), Where(expressions, output, options), options);
        }

        public virtual string Select(IMetaInfo metaInfo, IEnumerable<Expression> fields, ConditionExpressions expressions, ParameterCollection output, SqlOptions options)
        {
            return SelectStatmentFormate(metaInfo, string.Join(",", fields.Select(x => x.Build(this, output))), Where(expressions, output, options), options);
        }

        /// <summary>
        /// where条件
        /// </summary>
        /// <param name="conditions"> </param>
        /// <param name="output"> </param>
        /// <param name="options"> </param>
        /// <returns>生成的where条件</returns>
        public virtual string Where(IEnumerable<ItemValue> conditions, ParameterCollection output, SqlOptions options)
        {
            if (conditions == null)
                return string.Empty;

            switch (conditions.Count())
            {
                case 0:
                    return string.Empty;
                case 1:
                    var first = conditions.First();
                    return string.Concat(BuildField(first.Item), "=", output.Append(first.Item.Replace('.', '_'), first.Value, true).ParameterName);
                default:
                    var where = new StringBuilder();
                    foreach (var item in conditions)
                    {
                        where.Append(BuildField(item.Item)).Append("=").Append(output.Append(item.Item.Replace('.', '_'), item.Value, true).ParameterName).Append(" AND ");
                    }
                    where.Remove(where.Length - 5, 5);
                    return where.ToString();
            }
        }

        /// <summary>
        /// 依据Expressions生成where条件
        /// </summary>
        /// <param name="expressions">条件表达式</param>
        /// <param name="options"> </param>
        /// <param name="output">从Expressions对象中输出的值</param>
        /// <returns>生成的where条件</returns>
        public virtual string Where(ConditionExpressions expressions, ParameterCollection output, SqlOptions options)
        {
            if (expressions.Count == 0)
                return string.Empty;

            var condition = new StringBuilder();

            foreach (var exp in expressions)
            {
                condition.Append(exp.Build(this, output)).Append(" AND ");
            }
            condition.Remove(condition.Length - 5, 5);
            return condition.ToString();
        }

        #region abstract methods

        public abstract string IfExists(IMetaInfo metaInfo, string field, IEnumerable<ItemValue> conditions, string ifTrueSql, string ifFalseSql, ParameterCollection output, SqlOptions options);

        public abstract string IfExists(IMetaInfo metaInfo, string field, ConditionExpressions expressions, string ifTrueSql, string ifFalseSql, ParameterCollection output, SqlOptions options);

        public abstract string SelectForIncrement(IMetaInfoForDbTable metaInfo, string fieldName, SqlOptions options);

        #endregion

        #region SelectStatmentFormate

        /// <summary>
        /// 组装sql查询语句
        /// </summary>
        /// <returns></returns>
        protected abstract string SelectStatmentFormate(IMetaInfo metaInfo, string fields, string condition, SqlOptions options);

        #endregion

        #region GenerateFields

        /// <summary>
        /// 将fields转换成 col1 as colname1,col2 as colname2...
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        protected virtual string FieldsJoin(IEnumerable<ItemValue<string, string>> fields)
        {
            if (fields == null || fields.Any() == false)
                throw new ArgumentNullException("fields");

            var str = new StringBuilder();
            foreach (var field in fields)
            {
                str.Append(BuildField(field.Item)).Append(" AS ").Append(field.Value).Append(',');
            }
            str.Remove(str.Length - 1, 1);
            return str.ToString();
        }

        /// <summary>
        /// 将fields转换成 col1,col2,col3...
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        protected virtual string FieldsJoin(IEnumerable<string> fields)
        {
            if (fields == null || fields.Any() == false)
                throw new ArgumentNullException("fields");

            var str = new StringBuilder();
            foreach (var field in fields)
            {
                if (string.IsNullOrEmpty(field))
                    str.Append("*,");
                else
                    str.Append(BuildField(field)).Append(',');
            }
            str.Remove(str.Length - 1, 1);
            return str.ToString();
        }

        #endregion

        /// <summary>
        /// 验证主键值，如果值为空抛出一个异常
        /// </summary>
        /// <param name="val">主键值</param>
        /// <returns>主键值</returns>
        protected object CheckNull(object val)
        {
            if (val != DBNull.Value && val != null)
            {
                return val;
            }
            throw new ArgumentNullException("val", "主键出现空值。");
        }

    }
}

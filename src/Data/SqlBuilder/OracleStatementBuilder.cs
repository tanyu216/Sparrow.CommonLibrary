using System;
using System.Collections.Generic;
using System.Text;
using Sparrow.CommonLibrary.Data.Mapper.Metadata;
using Sparrow.CommonLibrary.Data.Database;
using Sparrow.CommonLibrary.Data.Query;
using Sparrow.CommonLibrary.Data.Entity;

namespace Sparrow.CommonLibrary.Data.SqlBuilder
{
    /// <summary>
    /// Oracle Provider
    /// </summary>
    public class OracleStatementBuilder : CommonBuilder
    {
        protected static readonly string SqlCharRowLock = " FOR UPDATE ";
        protected static readonly string SqlCharUpdateLock = " FOR UPDATE ";
        protected static readonly string SqlCharTabLock = " IN EXCLUSIVE ";

        /// <summary>
        /// 
        /// </summary>
        public static readonly OracleStatementBuilder Default = new OracleStatementBuilder();

        protected OracleStatementBuilder()
        {

        }

        public override string BuildParameterName(string parameterName)
        {
            if (!string.IsNullOrEmpty(parameterName) && !parameterName.StartsWith(":"))
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
                return SqlCharRowLock;
            if ((optVal & (int)SqlOptions.UpdateLock) > 0)
                return SqlCharUpdateLock;
            if ((optVal & (int)SqlOptions.TableLock) > 0)
                return SqlCharTabLock;
            //
            return string.Empty;
        }

        public override string Insert(IMetaInfo metaInfo, IEnumerable<ItemValue> values, ParameterCollection output, SqlOptions options)
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
                var metaDb = metaInfo as IMetaInfoForDbTable;
                if (metaDb != null && metaDb.Identity != null && metaDb.Identity.Name == current.Item)
                {
                    sqlPart1.Append(BuildField(current.Item)).Append(',');
                    sqlPart2.Append(metaDb.Identity.Name).Append(".NEXTVAL").Append(',');
                    continue;
                }

                // 逐个加入语句的列元素
                sqlPart1.Append(BuildField(current.Item)).Append(',');
                // 逐个加入参数元素
                sqlPart2.Append(output.Append(current.Item, current.Value, true).ParameterName).Append(',');
            }
            sqlPart1.Remove(sqlPart1.Length - 1, 1);
            sqlPart2.Remove(sqlPart2.Length - 1, 1);
            // ) VALUES (
            sqlPart1.Append(')').Append(SqlCharValues).Append('(').Append(sqlPart1.ToString()).Append(')');

            // 4、返回
            return sqlPart1.ToString();
        }

        public override string IfExists(IMetaInfo metaInfo, string field, IEnumerable<ItemValue> conditions, string ifTrueSql, string ifFalseSql, ParameterCollection output, SqlOptions options)
        {
            throw new NotImplementedException();
        }

        public override string IfExists(IMetaInfo metaInfo, string field, ConditionExpressions expressions, string ifTrueSql, string ifFalseSql, ParameterCollection output, SqlOptions options)
        {
            throw new NotImplementedException();
        }

        public override string SelectForIncrement(IMetaInfoForDbTable metaInfo, string fieldName, SqlOptions options)
        {
            return string.Format("SELECT {0}.CURRVAL {1} FROM DUAL;", metaInfo.Identity.Name, fieldName ?? metaInfo.Identity.Name);
        }

        protected override string SelectStatmentFormate(IMetaInfo metaInfo, string fields, string condition, SqlOptions options)
        {
            throw new NotImplementedException();
        }

    }
}

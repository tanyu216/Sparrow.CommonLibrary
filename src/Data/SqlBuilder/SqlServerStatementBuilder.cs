using System.Collections.Generic;
using System.Text;
using Sparrow.CommonLibrary.Data.Mapper.Metadata;
using Sparrow.CommonLibrary.Data.Database;
using Sparrow.CommonLibrary.Data.Query;
using Sparrow.CommonLibrary.Data.Entity;

namespace Sparrow.CommonLibrary.Data.SqlBuilder
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

        public override string IfExists(IMetaInfo metaInfo, string field, IEnumerable<ItemValue> conditions, string ifTrueSql, string ifFalseSql, ParameterCollection output, SqlOptions options)
        {
            if (string.IsNullOrEmpty(ifTrueSql) || string.IsNullOrEmpty(ifFalseSql))
                return string.Empty;

            var sql = new StringBuilder();
            var querySql = Select(metaInfo, new[] { field }, conditions, output, options);
            // 
            return IfExistsFormate(querySql, ifTrueSql, ifFalseSql);
        }

        public override string IfExists(IMetaInfo metaInfo, string field, ConditionExpressions expressions, string ifTrueSql, string ifFalseSql, ParameterCollection output, SqlOptions options)
        {
            if (string.IsNullOrEmpty(ifTrueSql) || string.IsNullOrEmpty(ifFalseSql))
                return string.Empty;

            var sql = new StringBuilder();
            var querySql = Select(metaInfo, new[] { field }, expressions, output, options);
            //
            return IfExistsFormate(querySql, ifTrueSql, ifFalseSql);
        }

        private string IfExistsFormate(string querySql, string ifTrueSql, string ifFalseSql)
        {
            var sql = new StringBuilder();
            // 
            if (string.IsNullOrWhiteSpace(ifTrueSql) || string.IsNullOrWhiteSpace(ifFalseSql))
            {
                if (string.IsNullOrWhiteSpace(ifTrueSql))
                {
                    sql.Append("IF EXISTS(").Append(querySql).AppendLine(")");
                    sql.AppendLine("BEGIN").AppendLine(ifTrueSql).AppendLine("END;");
                }
                else
                {
                    sql.Append("IF NOT EXISTS(").Append(querySql).AppendLine(")");
                    sql.AppendLine("BEGIN").AppendLine(ifFalseSql).AppendLine("END");
                }
            }
            else
            {
                sql.Append("IF EXISTS(").Append(querySql).AppendLine(")");
                sql.AppendLine("BEGIN").AppendLine(ifTrueSql).AppendLine("END");
                sql.AppendLine(" ELSE ");
                sql.AppendLine("BEGIN").AppendLine(ifFalseSql).AppendLine("END;");
            }
            // 
            return sql.ToString();
        }

        public override string SelectForIncrement(IMetaInfoForDbTable metaInfo, string fieldName, SqlOptions options)
        {
            return string.Format("SELECT SCOPE_IDENTITY() AS [{0}];", fieldName ?? metaInfo.Identity.Name);
        }

        protected override string SelectStatmentFormate(IMetaInfo metaInfo, string fields, string condition, SqlOptions options)
        {
            return string.Concat(SqlCharSelect, (string.IsNullOrEmpty(fields) ? SqlCharAllFields : fields), SqlCharFrom, BuildTableName(metaInfo), LockOption(options), (string.IsNullOrEmpty(condition) ? "" : SqlCharWhere), (string.IsNullOrEmpty(condition) ? "" : condition));
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

    }
}

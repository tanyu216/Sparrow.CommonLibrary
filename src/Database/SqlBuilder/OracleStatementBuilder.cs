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
        protected static readonly string WordRowLock = " FOR UPDATE ";
        protected static readonly string WordUpdateLock = " FOR UPDATE ";
        protected static readonly string WordTabLock = " IN EXCLUSIVE ";

        /// <summary>
        /// 
        /// </summary>
        public static readonly OracleStatementBuilder Default = new OracleStatementBuilder();

        protected OracleStatementBuilder()
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
                return WordRowLock;
            if ((optVal & (int)SqlOptions.UpdateLock) > 0)
                return WordUpdateLock;
            if ((optVal & (int)SqlOptions.TableLock) > 0)
                return WordTabLock;
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
                .Append(WordInsertInto).Append(BuildTableName(tableName))
                .Append("(").Append(BuildField(incrementField)).Append(',').Append(fields).Append(")")
                .Append(WordValues).Append("(").Append(incrementName).Append(".NEXTVAL,").Append(values).Append(")")
                .ToString();
        }

        public override string IncrementByQuery(string incrementName, string alias, SqlOptions options)
        {
            return string.Format("SELECT {0}.CURRVAL {1} FROM DUAL", incrementName, alias ?? incrementName);
        }

        public override string IncrementByParameter(string incrementName, string paramName, SqlOptions options)
        {
            return string.Format("SELECT {1}={0}.CURRVAL FROM DUAL", incrementName, BuildParameterName(paramName));
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

            //select [distinct][top(1)] {fieldExpressions} from {tableName} as {alias}
            var str = new StringBuilder().Append(WordSelect);
            if ((options & SqlOptions.Distinct) > 0)
                str.Append(WordDistinct);
            if (!string.IsNullOrEmpty(topExpression))
                str.Append(WordTop).Append('(').Append(topExpression).Append(')');

            str.Append(fieldExpressions).Append(WordFrom).Append(tableExpression);

            if (!string.IsNullOrEmpty(conditionExpressions))
                str.Append(WordWhere).Append(conditionExpressions);

            if (!string.IsNullOrEmpty(groupbyExpression))
                str.Append(WordGroupby).Append(groupbyExpression);

            if (!string.IsNullOrEmpty(havingExpression))
                str.Append(WordHaving).Append(havingExpression);

            if (!string.IsNullOrEmpty(orderbyExpression))
                str.Append(WordOrderby).Append(orderbyExpression);

            return str.Append(LockOption(options)).ToString();
        }

        public override string QueryFormat(string fieldExpressions, string tableExpression, string conditionExpressions, string groupbyExpression, string havingExpression, string orderbyExpression, int startIndex, int rowCount, SqlOptions options)
        {
            
        }
    }
}

using System.Collections.Generic;
using Sparrow.CommonLibrary.Mapper.Metadata;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Query;
using Sparrow.CommonLibrary.Entity;

namespace Sparrow.CommonLibrary.Database.SqlBuilder
{
    /// <summary>
    /// 生成DML/DQL脚本，实现该接口以支持不同的数据库标准。
    /// </summary>
    public interface ISqlBuilder
    {
        /// <summary>
        /// 参数、变量名称
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        string BuildParameterName(string parameterName);

        /// <summary>
        /// 生成别名
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        string BuildAlias(string alias);

        /// <summary>
        /// 成员字段
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        string BuildField(string field);

        /// <summary>
        /// 带有别名的成员字段
        /// </summary>
        /// <param name="field"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        string BuildField(string field, string alias);

        /// <summary>
        /// 表名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        string BuildTableName(string tableName);

        /// <summary>
        /// 表名
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        string BuildTableName(string tableName, string alias);

        /// <summary>
        /// 函数名
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        string BuildFuncName(string functionName);

        /// <summary>
        /// 向表达式后边增加别名
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        string BuildExpressionWithAlias(string expression, string alias);

        /// <summary>
        /// 将常量值转换为Sql格式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string Constant(object value);

        /// <summary>
        /// 将一组名称拼接成一个字符串
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        string ExpressionsJoin(IEnumerable<string> items);

        /// <summary>
        /// 生成查询“自动增长序列当前的序列”
        /// </summary>
        /// <param name="incrementName"></param>
        /// <param name="alias"> </param>
        /// <param name="options"> </param>
        /// <returns></returns>
        string IncrementByQuery(string incrementName, string alias, SqlOptions options);

        /// <summary>
        /// 生成查询“自动增长序列当前的序列”
        /// </summary>
        /// <param name="incrementName"></param>
        /// <param name="output"></param>
        /// <param name="paramName">查询的自动增长序列通过参数输出</param>
        /// <param name="options"></param>
        /// <returns></returns>
        string IncrementByParameter(string incrementName, string paramName, SqlOptions options);

        /// <summary>
        /// 格式化ifelse语句
        /// </summary>
        /// <param name="ifQuerySql"></param>
        /// <param name="ifTrueSql"></param>
        /// <param name="ifFalseSql"></param>
        /// <param name="output"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        string IfExistsFormat(string ifQuerySql, string ifTrueSql, string ifFalseSql, SqlOptions options);

        /// <summary>
        /// 格式化Insert语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldAndValueExpressions">字段名称和参数名称</param>
        /// <param name="output"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        string InsertFormat(string tableName, IEnumerable<ItemValue<string, string>> fieldAndValueExpressions, SqlOptions options);

        /// <summary>
        /// 格式化Insert语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldAndValueExpressions">字段名称和参数名称</param>
        /// <param name="incrementField"></param>
        /// <param name="incrementName"></param>
        /// <param name="output"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        string InsertFormat(string tableName, IEnumerable<ItemValue<string, string>> fieldAndValueExpressions, string incrementField, string incrementName, SqlOptions options);

        /// <summary>
        /// 格式化Update语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldAndValueExpressions">字段名称和参数名称</param>
        /// <param name="options"></param>
        /// <returns></returns>
        string UpdateFormat(string tableName, IEnumerable<ItemValue<string, string>> fieldAndValueExpressions, string condition, SqlOptions options);

        /// <summary>
        /// 格式化Delete语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="condition"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        string DeleteFormat(string tableName, string condition, SqlOptions options);

        /// <summary>
        /// 格式化Query语句
        /// </summary>
        /// <param name="topExpression"></param>
        /// <param name="fieldExpressions"></param>
        /// <param name="tableExpression"></param>
        /// <param name="conditionExpressions"></param>
        /// <param name="groupbyExpression"></param>
        /// <param name="havingExpression"></param>
        /// <param name="orderbyExpression"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        string QueryFormat(string topExpression, string fieldExpressions, string tableExpression, string conditionExpressions, string groupbyExpression, string havingExpression, string orderbyExpression, SqlOptions options);

        /// <summary>
        /// 格式化分页sql查询
        /// </summary>
        /// <param name="fieldExpressions"></param>
        /// <param name="tableExpression"></param>
        /// <param name="conditionExpressions"></param>
        /// <param name="groupbyExpression"></param>
        /// <param name="havingExpression"></param>
        /// <param name="orderbyExpression"></param>
        /// <param name="startIndex">从第0行开始的startIndex</param>
        /// <param name="rowCount">返回的总行数</param>
        /// <param name="options"></param>
        /// <returns></returns>
        string QueryFormat(string fieldExpressions, string tableExpression, string conditionExpressions, string groupbyExpression, string havingExpression, string orderbyExpression, int startIndex, int rowCount, SqlOptions options);
    }
}

using System.Collections.Generic;
using Sparrow.CommonLibrary.Data.Mapper.Metadata;
using Sparrow.CommonLibrary.Data.Database;
using Sparrow.CommonLibrary.Data.Query;
using Sparrow.CommonLibrary.Data.Entity;

namespace Sparrow.CommonLibrary.Data.SqlBuilder
{
    /// <summary>
    /// DML/DQL生成、格式化接口供应器，不同的数据标准由此接口下实现。
    /// </summary>
    public interface ISqlBuilder : INameBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaInfo"></param>
        /// <param name="conditions"></param>
        /// <param name="output"> </param>
        /// <param name="options"> </param>
        /// <returns></returns>
        string Delete(IMetaInfo metaInfo, IEnumerable<ItemValue> conditions, ParameterCollection output, SqlOptions options);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaInfo"></param>
        /// <param name="expressions"></param>
        /// <param name="options"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        string Delete(IMetaInfo metaInfo, ConditionExpressions expressions, ParameterCollection output, SqlOptions options);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaInfo"></param>
        /// <param name="fields"></param>
        /// <param name="conditions"></param>
        /// <param name="output"> </param>
        /// <param name="options"> </param>
        /// <returns></returns>
        string Update(IMetaInfo metaInfo, IEnumerable<ItemValue> fields, IEnumerable<ItemValue> conditions, ParameterCollection output, SqlOptions options);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaInfo"></param>
        /// <param name="fields"></param>
        /// <param name="expressions"></param>
        /// <param name="options"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        string Update(IMetaInfo metaInfo, IEnumerable<ItemValue> fields, ConditionExpressions expressions, ParameterCollection output, SqlOptions options);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaInfo"></param>
        /// <param name="values"></param>
        /// <param name="output"> </param>
        /// <param name="identity"> </param>
        /// <param name="options"> </param>
        /// <returns></returns>
        string Insert(IMetaInfo metaInfo, IEnumerable<ItemValue> values, ParameterCollection output, SqlOptions options);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaInfo"></param>
        /// <param name="field"></param>
        /// <param name="conditions"></param>
        /// <param name="output"> </param>
        /// <param name="ifTrueSql"></param>
        /// <param name="ifFalseSql"></param>
        /// <param name="options"> </param>
        /// <returns></returns>
        string IfExists(IMetaInfo metaInfo, string field, IEnumerable<ItemValue> conditions, string ifTrueSql, string ifFalseSql, ParameterCollection output, SqlOptions options);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaInfo"></param>
        /// <param name="field"></param>
        /// <param name="expressions"></param>
        /// <param name="ifTrueSql"></param>
        /// <param name="ifFalseSql"></param>
        /// <param name="options"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        string IfExists(IMetaInfo metaInfo, string field, ConditionExpressions expressions, string ifTrueSql, string ifFalseSql, ParameterCollection output, SqlOptions options);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaInfo"></param>
        /// <param name="fields"></param>
        /// <param name="conditions"></param>
        /// <param name="output"> </param>
        /// <param name="options"> </param>
        /// <returns></returns>
        string Select(IMetaInfo metaInfo, IEnumerable<ItemValue<string, string>> fields, IEnumerable<ItemValue> conditions, ParameterCollection output, SqlOptions options);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaInfo"></param>
        /// <param name="fields"></param>
        /// <param name="expressions"></param>
        /// <param name="options"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        string Select(IMetaInfo metaInfo, IEnumerable<ItemValue<string, string>> fields, ConditionExpressions expressions, ParameterCollection output, SqlOptions options);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaInfo"></param>
        /// <param name="fields"></param>
        /// <param name="conditions"></param>
        /// <param name="output"> </param>
        /// <param name="options"> </param>
        /// <returns></returns>
        string Select(IMetaInfo metaInfo, IEnumerable<string> fields, IEnumerable<ItemValue> conditions, ParameterCollection output, SqlOptions options);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaInfo"></param>
        /// <param name="fields"></param>
        /// <param name="expressions"></param>
        /// <param name="options"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        string Select(IMetaInfo metaInfo, IEnumerable<string> fields, ConditionExpressions expressions, ParameterCollection output, SqlOptions options);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaInfo"></param>
        /// <param name="fields"></param>
        /// <param name="expressions"></param>
        /// <param name="output"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        string Select(IMetaInfo metaInfo, IEnumerable<Expression> fields, ConditionExpressions expressions, ParameterCollection output, SqlOptions options);

        /// <summary>
        /// 生成Dql的Where条件
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="output"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        string Where(IEnumerable<ItemValue> conditions, ParameterCollection output, SqlOptions options);
        /// <summary>
        /// 生成Dql的Where条件
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="output"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        string Where(ConditionExpressions expressions, ParameterCollection output, SqlOptions options);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaInfo"></param>
        /// <param name="fieldName"> </param>
        /// <param name="options"> </param>
        /// <returns></returns>
        string SelectForIncrement(IMetaInfoForDbTable metaInfo, string fieldName, SqlOptions options);
    }
}

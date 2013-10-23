using Sparrow.CommonLibrary.Data.Mapper.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Data.SqlBuilder
{
    public interface INameBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        string BuildParameterName(string parameterName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        string BuildField(string field);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        string BuildTableName(IMetaInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        string BuildFuncName(string functionName);

        /// <summary>
        /// 将常量值转换为Sql格式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string Constant(object value);
    }
}

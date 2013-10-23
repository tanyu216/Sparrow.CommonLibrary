using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Data.Mapper.Metadata;
using Sparrow.CommonLibrary.Data.Database;
using Sparrow.CommonLibrary.Data.Query;
using Sparrow.CommonLibrary.Data.Entity;

namespace Sparrow.CommonLibrary.Data.SqlBuilder
{
    /// <summary>
    /// Mysql Provider
    /// </summary>
    public class MySqlStatementBuilder : CommonBuilder
    {
        public static readonly MySqlStatementBuilder Default = new MySqlStatementBuilder();

        protected MySqlStatementBuilder()
        {

        }

        public override string BuildField(string field)
        {
            if (field.IndexOf('.') > -1)
                return string.Join(".", field.Split('.').Select(x => string.Concat("`", x, "`")));
            return string.Concat("`", field, "`");
        }

        public override string BuildTableName(IMetaInfo metaInfo)
        {
            if (metaInfo.Name.IndexOf('.') > -1)
                return string.Join(".", metaInfo.Name.Split('.').Select(x => string.Concat("`", x, "`")));
            return string.Concat("`", metaInfo.Name, "`");
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
            throw new NotImplementedException();
        }

        protected override string SelectStatmentFormate(IMetaInfo metaInfo, string fields, string condition, SqlOptions options)
        {
            throw new NotImplementedException();
        }
    }
}

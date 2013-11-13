using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Entity;
using Sparrow.CommonLibrary.Mapper.Metadata;

namespace Sparrow.CommonLibrary.Database.SqlBuilder
{
    /// <summary>
    /// <see cref="ISqlBuilder"/>的扩展方法
    /// </summary>
    internal static class SqlBuilderExtensions
    {

        private static string TableName(IMetaInfo metaInfo)
        {
            if (metaInfo == null)
                throw new ArgumentNullException("metaInfo");

            if (string.IsNullOrEmpty(metaInfo.Name))
                throw new ArgumentException("metaInfo未配置一个有效称应用于TableName。");

            return metaInfo.Name;
        }

        /// <summary>
        /// where条件
        /// </summary>
        /// <param name="condition"> </param>
        /// <param name="output"> </param>
        /// <param name="options"> </param>
        /// <returns>生成的where条件</returns>
        public static string Where(this ISqlBuilder builder, IEnumerable<ItemValue> condition, ParameterCollection output, SqlOptions options)
        {
            if (condition == null || !condition.Any())
                return string.Empty;

            var where = new StringBuilder();
            foreach (var item in condition)
            {
                if (item.Value is ICollection)
                {
                    var values = ((ICollection)item.Value);
                    var list = new List<string>();
                    foreach (var value in values)
                        list.Add(output.Append("p", value, true).ParameterName);

                    where.Append(builder.BuildField(item.Item)).Append("IN(").Append(builder.ExpressionsJoin(list)).Append(") AND ");
                }
                else
                {
                    where.Append(builder.BuildField(item.Item)).Append("=").Append(output.Append("p", item.Value, true).ParameterName).Append(" AND ");
                }
            }
            where.Remove(where.Length - 5, 5);
            return where.ToString();
        }

        /// <summary>
        /// where条件
        /// </summary>
        /// <param name="condition"> </param>
        /// <param name="output"> </param>
        /// <param name="options"> </param>
        /// <returns>生成的where条件</returns>
        public static string Where(this ISqlBuilder builder, IDictionary<string, object> condition, ParameterCollection output, SqlOptions options)
        {
            if (condition == null || condition.Count == 0)
                return string.Empty;

            var where = new StringBuilder();
            foreach (var item in condition)
            {
                if (item.Value is ICollection)
                {
                    var values = ((ICollection)item.Value);
                    var list = new List<string>();
                    foreach (var value in values)
                        list.Add(output.Append("p", value, true).ParameterName);

                    where.Append(builder.BuildField(item.Key)).Append("IN(").Append(builder.ExpressionsJoin(list)).Append(") AND ");
                }
                else
                {
                    where.Append(builder.BuildField(item.Key)).Append("=").Append(output.Append("p", item.Value, true).ParameterName).Append(" AND ");
                }
            }
            where.Remove(where.Length - 5, 5);
            return where.ToString();
        }

        public static string Delete(this ISqlBuilder builder, IMetaInfo metaInfo, IEnumerable<ItemValue> condition, ParameterCollection output, SqlOptions options)
        {
            return builder.DeleteFormat(TableName(metaInfo), builder.Where(condition, output, options), options);
        }

        public static string Delete(this ISqlBuilder builder, IMetaInfo metaInfo, IDictionary<string, object> condition, ParameterCollection output, SqlOptions options)
        {
            return builder.DeleteFormat(TableName(metaInfo), builder.Where(condition, output, options), options);
        }

        public static string Update(this ISqlBuilder builder, IMetaInfo metaInfo, IEnumerable<ItemValue> fieldValues, IEnumerable<ItemValue> condition, ParameterCollection output, SqlOptions options)
        {
            return builder.UpdateFormat(TableName(metaInfo), fieldValues.Select(x => new ItemValue<string, string>(x.Item, output.Append("p", x.Value, true).ParameterName)), builder.Where(condition, output, options), options);
        }

        public static string Update(this ISqlBuilder builder, IMetaInfo metaInfo, IDictionary<string, object> fieldValues, IDictionary<string, object> condition, ParameterCollection output, SqlOptions options)
        {
            return builder.UpdateFormat(TableName(metaInfo), fieldValues.Select(x => new ItemValue<string, string>(x.Key, output.Append("p", x.Value, true).ParameterName)), builder.Where(condition, output, options), options);
        }

        public static string Insert(this ISqlBuilder builder, IMetaInfo metaInfo, IEnumerable<ItemValue> fieldValues, ParameterCollection output, SqlOptions options)
        {
            var list = new List<ItemValue<string, string>>();

            IMetaInfoForDbTable metaDb = metaInfo as IMetaInfoForDbTable;
            foreach (var item in fieldValues)
            {
                if (metaDb != null && metaDb.Identity != null && metaDb.Identity.FieldInfo.FieldName == item.Item)
                    continue;

                list.Add(new ItemValue<string, string>(item.Item, output.Append("p", item.Value, true).ParameterName));
            }
            if (metaDb != null && metaDb.Identity != null)
            {
                return builder.InsertFormat(TableName(metaInfo), list, metaDb.Identity.FieldInfo.FieldName, metaDb.Identity.Name, options);
            }
            else
            {
                return builder.InsertFormat(TableName(metaInfo), list, options);
            }
        }

        public static string Insert(this ISqlBuilder builder, IMetaInfo metaInfo, IDictionary<string, object> fieldValues, ParameterCollection output, SqlOptions options)
        {
            var list = new List<ItemValue<string, string>>();

            IMetaInfoForDbTable metaDb = metaInfo as IMetaInfoForDbTable;
            foreach (var item in fieldValues)
            {
                if (metaDb != null && metaDb.Identity != null && metaDb.Identity.FieldInfo.FieldName == item.Key)
                    continue;

                list.Add(new ItemValue<string, string>(item.Key, output.Append("p", item.Value, true).ParameterName));
            }
            if (metaDb != null && metaDb.Identity != null)
            {
                return builder.InsertFormat(TableName(metaInfo), list, metaDb.Identity.FieldInfo.FieldName, metaDb.Identity.Name, options);
            }
            else
            {
                return builder.InsertFormat(TableName(metaInfo), list, options);
            }
        }

        public static string Query(this ISqlBuilder builder, IMetaInfo metaInfo, IEnumerable<ItemValue<string, string>> fields, SqlOptions options)
        {
            return builder.QueryFormat(metaInfo, builder.ExpressionsJoin(fields.Select(x => builder.BuildField(x.Item, x.Value))), null, options);
        }

        public static string Query(this ISqlBuilder builder, IMetaInfo metaInfo, IDictionary<string, string> fields, SqlOptions options)
        {
            return builder.QueryFormat(metaInfo, builder.ExpressionsJoin(fields.Select(x => builder.BuildField(x.Key, x.Value))), null, options);
        }

        public static string Query(this ISqlBuilder builder, IMetaInfo metaInfo, IEnumerable<string> fields, SqlOptions options)
        {
            return builder.QueryFormat(metaInfo, builder.ExpressionsJoin(fields.Select(x => builder.BuildField(x))), null, options);
        }

        public static string Query(this ISqlBuilder builder, IMetaInfo metaInfo, IEnumerable<ItemValue<string, string>> fields, IEnumerable<ItemValue> condition, ParameterCollection output, SqlOptions options)
        {
            return builder.QueryFormat(metaInfo, builder.ExpressionsJoin(fields.Select(x => builder.BuildField(x.Item, x.Value))), builder.Where(condition, output, options), options);
        }

        public static string Query(this ISqlBuilder builder, IMetaInfo metaInfo, IDictionary<string, string> fields, IDictionary<string, object> condition, ParameterCollection output, SqlOptions options)
        {
            return builder.QueryFormat(metaInfo, builder.ExpressionsJoin(fields.Select(x => builder.BuildField(x.Key, x.Value))), builder.Where(condition, output, options), options);
        }

        public static string Query(this ISqlBuilder builder, IMetaInfo metaInfo, IEnumerable<string> fields, IEnumerable<ItemValue> condition, ParameterCollection output, SqlOptions options)
        {
            return builder.QueryFormat(metaInfo, builder.ExpressionsJoin(fields.Select(x => builder.BuildField(x))), builder.Where(condition, output, options), options);
        }

        public static string Query(this ISqlBuilder builder, IMetaInfo metaInfo, IEnumerable<string> fields, IDictionary<string, object> condition, ParameterCollection output, SqlOptions options)
        {
            return builder.QueryFormat(metaInfo, builder.ExpressionsJoin(fields.Select(x => builder.BuildField(x))), builder.Where(condition, output, options), options);
        }

        private static string QueryFormat(this ISqlBuilder builder, IMetaInfo metaInfo, string fieldExpressions, string conditionExpressions, SqlOptions options)
        {
            return builder.QueryFormat(null, fieldExpressions, builder.BuildTableName(TableName(metaInfo)), conditionExpressions, null, null, null, options);
        }

        public static string IncrementByQuery(this ISqlBuilder builder, IMetaInfo metaInfo, string alias, SqlOptions options)
        {
            var metaDb = metaInfo as IMetaInfoForDbTable;
            if (metaDb == null)
                throw new ArgumentException("metaInfo未实现接口" + typeof(IMetaInfoForDbTable).FullName);

            if (metaDb.Identity == null)
                throw new ArgumentException("metaInfo没有自动增长标识");

            return builder.IncrementByQuery(metaDb.Identity.Name, alias, options);
        }
    }
}

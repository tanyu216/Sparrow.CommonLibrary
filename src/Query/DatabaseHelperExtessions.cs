using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Database.SqlBuilder;
using Sparrow.CommonLibrary.Mapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Query
{
    public static class DatabaseHelperExtessions
    {
        public static Queryable<T> CreateQueryable<T>(this DatabaseHelper database)
        {
            return new Queryable<T>(database);
        }

        /// <summary>
        /// 生成数据查询Sql语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <param name="output"></param>
        /// <param name="options"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        private static string BuildDqlSql<T>(this DatabaseHelper database, ConditionExpression condition, ParameterCollection output, SqlOptions options, out IMapper<T> mapper)
        {
            mapper = Map.GetIMapper<T>();
            var metaInfo = mapper.MetaInfo;
            var fields = metaInfo.GetFieldNames();
            var builder = database.Builder;
            return database.Builder.QueryFormat(null, builder.BuildTableName(mapper.MetaInfo.Name), string.Join(",", fields.Select(x => builder.BuildField(x))), condition.OutputSqlString(builder, output), null, null, null, options);
        }

        /// <summary>
        /// 生成数据查询Sql语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <param name="output"></param>
        /// <param name="options"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        private static string BuildDqlSql<T>(this DatabaseHelper database, CompareExpression condition, ParameterCollection output, SqlOptions options, out IMapper<T> mapper)
        {
            mapper = Map.GetIMapper<T>();
            var metaInfo = mapper.MetaInfo;
            var fields = metaInfo.GetFieldNames();
            var builder = database.Builder;
            return database.Builder.QueryFormat(null, builder.BuildTableName(mapper.MetaInfo.Name), string.Join(",", fields.Select(x => builder.BuildField(x))), condition.OutputSqlString(builder, output), null, null, null, options);
        }

        #region ExecuteFirst<T>

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        public static T ExecuteFirst<T>(this DatabaseHelper database, CompareExpression condition)
        {
            return database.ExecuteFirst<T>(condition, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="options"> </param>
        public static T ExecuteFirst<T>(this DatabaseHelper database, CompareExpression condition, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = database.CreateParamterCollection();
            IMapper<T> mapper;
            var sql = database.BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            using (var reader = database.ExecuteReader(CommandType.Text, sql, parameters))
            {
                return mapper.MapSingle(reader);
            }
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        public static T ExecuteFirst<T>(this DatabaseHelper database, CompareExpression condition, DbTransaction transaction)
        {
            return database.ExecuteFirst<T>(condition, transaction, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        /// <param name="options"> </param>
        public static T ExecuteFirst<T>(this DatabaseHelper database, CompareExpression condition, DbTransaction transaction, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = database.CreateParamterCollection();
            IMapper<T> mapper;
            var sql = database.BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            using (var reader = database.ExecuteReader(CommandType.Text, sql, parameters, transaction))
            {
                return mapper.MapSingle(reader);
            }
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        public static T ExecuteFirst<T>(this DatabaseHelper database, ConditionExpression condition)
        {
            return database.ExecuteFirst<T>(condition, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="options"> </param>
        public static T ExecuteFirst<T>(this DatabaseHelper database, ConditionExpression condition, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = database.CreateParamterCollection();
            IMapper<T> mapper;
            var sql = database.BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            using (var reader = database.ExecuteReader(CommandType.Text, sql, parameters))
            {
                return mapper.MapSingle(reader);
            }
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        public static T ExecuteFirst<T>(this DatabaseHelper database, ConditionExpression condition, DbTransaction transaction)
        {
            return database.ExecuteFirst<T>(condition, transaction, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        /// <param name="options"> </param>
        public static T ExecuteFirst<T>(this DatabaseHelper database, ConditionExpression condition, DbTransaction transaction, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = database.CreateParamterCollection();
            IMapper<T> mapper;
            var sql = database.BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            using (var reader = database.ExecuteReader(CommandType.Text, sql, parameters, transaction))
            {
                return mapper.MapSingle(reader);
            }
        }

        #endregion

        #region ExecuteList<T>

        /// <summary>
        /// 执行条件表达式，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        public static IList<T> ExecuteList<T>(this DatabaseHelper database, CompareExpression condition)
        {
            return database.ExecuteList<T>(condition, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="options"> </param>
        public static IList<T> ExecuteList<T>(this DatabaseHelper database, CompareExpression condition, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("expressions");

            var parameters = database.CreateParamterCollection();
            IMapper<T> mapper;
            var sql = database.BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            using (var reader = database.ExecuteReader(CommandType.Text, sql, parameters))
            {
                return mapper.MapList(reader);
            }
        }

        /// <summary>
        /// 执行条件表达式，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        public static IList<T> ExecuteList<T>(this DatabaseHelper database, CompareExpression condition, DbTransaction transaction)
        {
            return database.ExecuteList<T>(condition, transaction, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        /// <param name="options"> </param>
        public static IList<T> ExecuteList<T>(this DatabaseHelper database, CompareExpression condition, DbTransaction transaction, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("expressions");

            var parameters = database.CreateParamterCollection();
            IMapper<T> mapper;
            var sql = database.BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            using (var reader = database.ExecuteReader(CommandType.Text, sql, parameters, transaction))
            {
                return mapper.MapList(reader);
            }
        }

        /// <summary>
        /// 执行条件表达式，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        public static IList<T> ExecuteList<T>(this DatabaseHelper database, ConditionExpression condition)
        {
            return database.ExecuteList<T>(condition, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="options"> </param>
        public static IList<T> ExecuteList<T>(this DatabaseHelper database, ConditionExpression condition, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("expressions");

            var parameters = database.CreateParamterCollection();
            IMapper<T> mapper;
            var sql = database.BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            using (var reader = database.ExecuteReader(CommandType.Text, sql, parameters))
            {
                return mapper.MapList(reader);
            }
        }

        /// <summary>
        /// 执行条件表达式，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        public static IList<T> ExecuteList<T>(this DatabaseHelper database, ConditionExpression condition, DbTransaction transaction)
        {
            return database.ExecuteList<T>(condition, transaction, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        /// <param name="options"> </param>
        public static IList<T> ExecuteList<T>(this DatabaseHelper database, ConditionExpression condition, DbTransaction transaction, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("expressions");

            var parameters = database.CreateParamterCollection();
            IMapper<T> mapper;
            var sql = database.BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            using (var reader = database.ExecuteReader(CommandType.Text, sql, parameters, transaction))
            {
                return mapper.MapList(reader);
            }
        }

        #endregion

        #region ExecuteReader<T>

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        public static IDataReader ExecuteReader<T>(this DatabaseHelper database, CompareExpression condition)
        {
            return database.ExecuteReader<T>(condition, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="options"> </param>
        public static IDataReader ExecuteReader<T>(this DatabaseHelper database, CompareExpression condition, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = database.CreateParamterCollection();
            IMapper<T> mapper;
            var sql = database.BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            return database.ExecuteReader(CommandType.Text, sql, parameters);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        public static IDataReader ExecuteReader<T>(this DatabaseHelper database, CompareExpression condition, DbTransaction transaction)
        {
            return database.ExecuteReader<T>(condition, transaction, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        /// <param name="options"> </param>
        public static IDataReader ExecuteReader<T>(this DatabaseHelper database, CompareExpression condition, DbTransaction transaction, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = database.CreateParamterCollection();
            IMapper<T> mapper;
            var sql = database.BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            return database.ExecuteReader(CommandType.Text, sql, parameters, transaction);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        public static IDataReader ExecuteReader<T>(this DatabaseHelper database, ConditionExpression condition)
        {
            return database.ExecuteReader<T>(condition, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="options"> </param>
        public static IDataReader ExecuteReader<T>(this DatabaseHelper database, ConditionExpression condition, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = database.CreateParamterCollection();
            IMapper<T> mapper;
            var sql = database.BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            return database.ExecuteReader(CommandType.Text, sql, parameters);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        public static IDataReader ExecuteReader<T>(this DatabaseHelper database, ConditionExpression condition, DbTransaction transaction)
        {
            return database.ExecuteReader<T>(condition, transaction, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        /// <param name="options"> </param>
        public static IDataReader ExecuteReader<T>(this DatabaseHelper database, ConditionExpression condition, DbTransaction transaction, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = database.CreateParamterCollection();
            IMapper<T> mapper;
            var sql = database.BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            return database.ExecuteReader(CommandType.Text, sql, parameters, transaction);
        }

        #endregion

        #region ExecuteScalar<T>

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        public static T ExecuteScalar<T>(this DatabaseHelper database, CompareExpression condition)
        {
            return database.ExecuteScalar<T>(condition, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="options"> </param>
        public static T ExecuteScalar<T>(this DatabaseHelper database, CompareExpression condition, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = database.CreateParamterCollection();
            IMapper<T> mapper;
            var sql = database.BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            return database.ExecuteScalar<T>(CommandType.Text, sql, parameters);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        public static T ExecuteScalar<T>(this DatabaseHelper database, CompareExpression condition, DbTransaction transaction)
        {
            return database.ExecuteScalar<T>(condition, transaction, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        /// <param name="options"> </param>
        public static T ExecuteScalar<T>(this DatabaseHelper database, CompareExpression condition, DbTransaction transaction, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = database.CreateParamterCollection();
            IMapper<T> mapper;
            var sql = database.BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            return database.ExecuteScalar<T>(CommandType.Text, sql, parameters, transaction);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        public static T ExecuteScalar<T>(this DatabaseHelper database, ConditionExpression condition)
        {
            return database.ExecuteScalar<T>(condition, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="options"> </param>
        public static T ExecuteScalar<T>(this DatabaseHelper database, ConditionExpression condition, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = database.CreateParamterCollection();
            IMapper<T> mapper;
            var sql = database.BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            return database.ExecuteScalar<T>(CommandType.Text, sql, parameters);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        public static T ExecuteScalar<T>(this DatabaseHelper database, ConditionExpression condition, DbTransaction transaction)
        {
            return database.ExecuteScalar<T>(condition, transaction, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        /// <param name="options"> </param>
        public static T ExecuteScalar<T>(this DatabaseHelper database, ConditionExpression condition, DbTransaction transaction, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = database.CreateParamterCollection();
            IMapper<T> mapper;
            var sql = database.BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            return database.ExecuteScalar<T>(CommandType.Text, sql, parameters, transaction);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Sparrow.CommonLibrary.Mapper;
using Sparrow.CommonLibrary.Database.Query;
using Sparrow.CommonLibrary.Database.SqlBuilder;

namespace Sparrow.CommonLibrary.Database
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DatabaseHelper
    {
        #region ExecuteTextFirst

        /// <summary>
        /// 执行sql语句，将结果集的第一行转换成类型<typeparamref name="T"/>实例对象并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">sql语句</param>
        /// <returns>返回类型<typeparamref name="T"/>的实例，如果数据库无返回的结集，则返回空。</returns>
        public T ExecuteFirst<T>(string commandText)
        {
            using (var reader = ExecuteReader(CommandType.Text, commandText, null))
            {
                return Map.Single<T, IDataReader>(reader);
            }
        }

        /// <summary>
        /// 执行sql语句，将结果集的第一行转换成类型<typeparamref name="T"/>实例对象并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <returns>返回类型<typeparamref name="T"/>的实例，如果数据库无返回的结集，则返回空。</returns>
        public T ExecuteFirst<T>(string commandText, ParameterCollection parameters)
        {
            using (var reader = ExecuteReader(CommandType.Text, commandText, parameters))
            {
                return Map.Single<T, IDataReader>(reader);
            }
        }

        /// <summary>
        /// 执行sql语句，将结果集的第一行转换成类型<typeparamref name="T"/>实例对象并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <param name="transaction"> </param>
        /// <returns>返回类型<typeparamref name="T"/>的实例，如果数据库无返回的结集，则返回空。</returns>
        public T ExecuteFirst<T>(string commandText, ParameterCollection parameters, DbTransaction transaction)
        {
            using (var reader = ExecuteReader(CommandType.Text, commandText, parameters, transaction))
            {
                return Map.Single<T, IDataReader>(reader);
            }
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        public T ExecuteFirst<T>(CompareExpression condition)
        {
            return ExecuteFirst<T>(condition, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="options"> </param>
        public T ExecuteFirst<T>(CompareExpression condition, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = CreateParamterCollection();
            IMapper<T> mapper;
            var sql = BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            using (var reader = ExecuteReader(CommandType.Text, sql, parameters))
            {
                return Map.Single<T, IDataReader>(reader);
            }
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        public T ExecuteFirst<T>(CompareExpression condition, DbTransaction transaction)
        {
            return ExecuteFirst<T>(condition, transaction, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        /// <param name="options"> </param>
        public T ExecuteFirst<T>(CompareExpression condition, DbTransaction transaction, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = CreateParamterCollection();
            IMapper<T> mapper;
            var sql = BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            using (var reader = ExecuteReader(CommandType.Text, sql, parameters, transaction))
            {
                return Map.Single<T, IDataReader>(reader);
            }
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        public T ExecuteFirst<T>(ConditionExpression condition)
        {
            return ExecuteFirst<T>(condition, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="options"> </param>
        public T ExecuteFirst<T>(ConditionExpression condition, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = CreateParamterCollection();
            IMapper<T> mapper;
            var sql = BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            using (var reader = ExecuteReader(CommandType.Text, sql, parameters))
            {
                return Map.Single<T, IDataReader>(reader);
            }
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        public T ExecuteFirst<T>(ConditionExpression condition, DbTransaction transaction)
        {
            return ExecuteFirst<T>(condition, transaction, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        /// <param name="options"> </param>
        public T ExecuteFirst<T>(ConditionExpression condition, DbTransaction transaction, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = CreateParamterCollection();
            IMapper<T> mapper;
            var sql = BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            using (var reader = ExecuteReader(CommandType.Text, sql, parameters, transaction))
            {
                return Map.Single<T, IDataReader>(reader);
            }
        }

        #endregion

        #region ExecuteTextList

        /// <summary>
        /// 执行sql语句，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">sql语句</param>
        /// <returns>IList&lt;<typeparamref name="T"/>&gt; 集合</returns>
        public IList<T> ExecuteList<T>(string commandText)
        {
            using (var reader = ExecuteReader(CommandType.Text, commandText, null))
            {
                return Map.List<T, IDataReader>(reader);
            }
        }

        /// <summary>
        /// 执行sql语句，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <returns>IList&lt;<typeparamref name="T"/>&gt; 集合</returns>
        public IList<T> ExecuteList<T>(string commandText, ParameterCollection parameters)
        {
            using (var reader = ExecuteReader(CommandType.Text, commandText, parameters))
            {
                return Map.List<T, IDataReader>(reader);
            }
        }

        /// <summary>
        /// 执行sql语句，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <param name="transaction"> </param>
        /// <returns>IList&lt;<typeparamref name="T"/>&gt; 集合</returns>
        public IList<T> ExecuteList<T>(string commandText, ParameterCollection parameters, DbTransaction transaction)
        {
            using (var reader = ExecuteReader(CommandType.Text, commandText, parameters, transaction))
            {
                return Map.List<T, IDataReader>(reader);
            }
        }

        /// <summary>
        /// 执行条件表达式，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        public IList<T> ExecuteList<T>(CompareExpression condition)
        {
            return ExecuteList<T>(condition, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="options"> </param>
        public IList<T> ExecuteList<T>(CompareExpression condition, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("expressions");

            var parameters = CreateParamterCollection();
            IMapper<T> mapper;
            var sql = BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            using (var reader = ExecuteReader(CommandType.Text, sql, parameters))
            {
                return Map.List<T, IDataReader>(reader);
            }
        }

        /// <summary>
        /// 执行条件表达式，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        public IList<T> ExecuteList<T>(CompareExpression condition, DbTransaction transaction)
        {
            return ExecuteList<T>(condition, transaction, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        /// <param name="options"> </param>
        public IList<T> ExecuteList<T>(CompareExpression condition, DbTransaction transaction, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("expressions");

            var parameters = CreateParamterCollection();
            IMapper<T> mapper;
            var sql = BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            using (var reader = ExecuteReader(CommandType.Text, sql, parameters, transaction))
            {
                return Map.List<T, IDataReader>(reader);
            }
        }

        /// <summary>
        /// 执行条件表达式，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        public IList<T> ExecuteList<T>(ConditionExpression condition)
        {
            return ExecuteList<T>(condition, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="options"> </param>
        public IList<T> ExecuteList<T>(ConditionExpression condition, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("expressions");

            var parameters = CreateParamterCollection();
            IMapper<T> mapper;
            var sql = BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            using (var reader = ExecuteReader(CommandType.Text, sql, parameters))
            {
                return Map.List<T, IDataReader>(reader);
            }
        }

        /// <summary>
        /// 执行条件表达式，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        public IList<T> ExecuteList<T>(ConditionExpression condition, DbTransaction transaction)
        {
            return ExecuteList<T>(condition, transaction, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        /// <param name="options"> </param>
        public IList<T> ExecuteList<T>(ConditionExpression condition, DbTransaction transaction, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("expressions");

            var parameters = CreateParamterCollection();
            IMapper<T> mapper;
            var sql = BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            using (var reader = ExecuteReader(CommandType.Text, sql, parameters, transaction))
            {
                return Map.List<T, IDataReader>(reader);
            }
        }

        #endregion

        #region ExecuteSprocList

        /// <summary>
        /// 执行存储过程，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">存储过程</param>
        /// <returns>IList&lt;<typeparamref name="T"/>&gt; 集合</returns>
        public IList<T> SprocExecuteList<T>(string commandText)
        {
            using (var reader = ExecuteReader(CommandType.StoredProcedure, commandText, null))
            {
                return Map.List<T, IDataReader>(reader);
            }
        }

        /// <summary>
        /// 执行存储过程，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">存储过程</param>
        /// <param name="parameters">存储过程的参数</param>
        /// <returns>IList&lt;<typeparamref name="T"/>&gt; 集合</returns>
        public IList<T> SprocExecuteList<T>(string commandText, ParameterCollection parameters)
        {
            using (var reader = ExecuteReader(CommandType.StoredProcedure, commandText, parameters))
            {
                return Map.List<T, IDataReader>(reader);
            }
        }

        /// <summary>
        /// 执行存储过程，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">存储过程</param>
        /// <param name="parameters">存储过程的参数</param>
        /// <param name="transaction"> </param>
        /// <returns>IList&lt;<typeparamref name="T"/>&gt; 集合</returns>
        public IList<T> SprocExecuteList<T>(string commandText, ParameterCollection parameters, DbTransaction transaction)
        {
            using (var reader = ExecuteReader(CommandType.StoredProcedure, commandText, parameters, transaction))
            {
                return Map.List<T, IDataReader>(reader);
            }
        }

        #endregion

    }
}

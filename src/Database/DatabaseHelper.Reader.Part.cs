using Sparrow.CommonLibrary.Query;
using Sparrow.CommonLibrary.Database.SqlBuilder;
using Sparrow.CommonLibrary.Mapper;
using System;
using System.Data;
using System.Data.Common;

namespace Sparrow.CommonLibrary.Database
{
    public partial class DatabaseHelper
    {

        /// <summary>
        /// 执行sql语句，返回IDataReader。
        /// </summary>
        /// <param name="commandText">sql语句</param>
        /// <returns>返回IDataReader</returns>
        public IDataReader ExecuteReader(string commandText)
        {
            return ExecuteReader(CommandType.Text, commandText, null);
        }

        /// <summary>
        /// 执行sql语句，返回IDataReader。
        /// </summary>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <returns>返回IDataReader</returns>
        public IDataReader ExecuteReader(string commandText, ParameterCollection parameters)
        {
            return ExecuteReader(CommandType.Text, commandText, parameters);
        }

        /// <summary>
        /// 执行sql语句，返回IDataReader。
        /// </summary>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <param name="transaction"> </param>
        /// <returns>返回IDataReader</returns>
        public IDataReader ExecuteReader(string commandText, ParameterCollection parameters, DbTransaction transaction)
        {
            return ExecuteReader(CommandType.Text, commandText, parameters, transaction);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        public IDataReader ExecuteReader<T>(CompareExpression condition)
        {
            return ExecuteReader<T>(condition, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="options"> </param>
        public IDataReader ExecuteReader<T>(CompareExpression condition, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = CreateParamterCollection();
            IMapper<T> mapper;
            var sql = BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            return ExecuteReader(CommandType.Text, sql, parameters);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        public IDataReader ExecuteReader<T>(CompareExpression condition, DbTransaction transaction)
        {
            return ExecuteReader<T>(condition, transaction, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        /// <param name="options"> </param>
        public IDataReader ExecuteReader<T>(CompareExpression condition, DbTransaction transaction, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = CreateParamterCollection();
            IMapper<T> mapper;
            var sql = BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            return ExecuteReader(CommandType.Text, sql, parameters, transaction);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        public IDataReader ExecuteReader<T>(ConditionExpression condition)
        {
            return ExecuteReader<T>(condition, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="options"> </param>
        public IDataReader ExecuteReader<T>(ConditionExpression condition, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = CreateParamterCollection();
            IMapper<T> mapper;
            var sql = BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            return ExecuteReader(CommandType.Text, sql, parameters);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        public IDataReader ExecuteReader<T>(ConditionExpression condition, DbTransaction transaction)
        {
            return ExecuteReader<T>(condition, transaction, SqlOptions.None);
        }

        /// <summary>
        /// 执行条件表达式，将结果集中的第一行数据转换实体对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">获取数据的查询条件表达式</param>
        /// <param name="transaction"> </param>
        /// <param name="options"> </param>
        public IDataReader ExecuteReader<T>(ConditionExpression condition, DbTransaction transaction, SqlOptions options)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var parameters = CreateParamterCollection();
            IMapper<T> mapper;
            var sql = BuildDqlSql(condition, parameters, options, out mapper);
            // 执行
            return ExecuteReader(CommandType.Text, sql, parameters, transaction);
        }

        /// <summary>
        /// 执行sql语句，返回IDataReader。
        /// </summary>
        /// <param name="batch">sql批次</param>
        /// <returns>受影响的行数</returns>
        /// <remarks><paramref name="batch"/> 中包含增量标识的实体对象，不会返回自增序列的值。</remarks>
        public IDataReader ExecuteReader(SqlBatch batch)
        {
            var command = BuildDbCommand(batch);
            return Executer.ExecuteReader(command);
        }

        /// <summary>
        /// 执行sql语句，返回IDataReader。
        /// </summary>
        /// <param name="batch">sql批次</param>
        /// <param name="transaction">事务处理</param>
        /// <returns>受影响的行数</returns>
        /// <remarks><paramref name="batch"/> 中包含增量标识的实体对象，不会返回自增序列的值。</remarks>
        public IDataReader ExecuteReader(SqlBatch batch, DbTransaction transaction)
        {
            var command = BuildDbCommand(batch);
            return Executer.ExecuteReader(command, transaction);
        }


        /// <summary>
        /// 执行存储过程，返回IDataReader。
        /// </summary>
        /// <param name="commandText">存储过程</param>
        /// <returns>返回IDataReader</returns>
        public IDataReader SprocExecuteReader(string commandText)
        {
            return ExecuteReader(CommandType.StoredProcedure, commandText, null);
        }

        /// <summary>
        /// 执行存储过程，返回IDataReader。
        /// </summary>
        /// <param name="commandText">存储过程</param>
        /// <param name="parameters">存储过程的参数</param>
        /// <returns>返回IDataReader</returns>
        public IDataReader SprocExecuteReader(string commandText, ParameterCollection parameters)
        {
            return ExecuteReader(CommandType.StoredProcedure, commandText, parameters);
        }

        /// <summary>
        /// 执行存储过程，返回IDataReader。
        /// </summary>
        /// <param name="commandText">存储过程</param>
        /// <param name="parameters">存储过程的参数</param>
        /// <param name="transaction"> </param>
        /// <returns>返回IDataReader</returns>
        public IDataReader SprocExecuteReader(string commandText, ParameterCollection parameters, DbTransaction transaction)
        {
            return ExecuteReader(CommandType.StoredProcedure, commandText, parameters, transaction);
        }
    }
}

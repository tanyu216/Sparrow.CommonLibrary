using System.Data;
using System.Data.Common;

namespace Sparrow.CommonLibrary.Database
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DatabaseHelper
    {

        /// <summary>
        /// 执行sql语句，不返回结果集。
        /// </summary>
        /// <param name="commandText">sql语句</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, (ParameterCollection)null);
        }

        /// <summary>
        /// 执行sql语句，不返回结果集。
        /// </summary>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string commandText, ParameterCollection parameters)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, parameters);
        }

        /// <summary>
        /// 执行sql语句，不返回结果集。
        /// </summary>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <param name="transaction"> </param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string commandText, ParameterCollection parameters, DbTransaction transaction)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, parameters, transaction);
        }

        /// <summary>
        /// 执行sql语句，不返回结果集。
        /// </summary>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string commandText, params object[] parameters)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, parameters);
        }

        /// <summary>
        /// 执行sql语句，不返回结果集。
        /// </summary>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <param name="transaction"> </param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string commandText, DbTransaction transaction, params object[] parameters)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, parameters, transaction);
        }

        /// <summary>
        /// 执行sql语句，不返回任何结果集。
        /// </summary>
        /// <param name="batch">sql批次</param>
        /// <returns>受影响的行数</returns>
        /// <remarks><paramref name="batch"/> 中包含增量标识的实体对象，不会返回自增序列的值。</remarks>
        public int ExecuteNonQuery(SqlBatch batch)
        {
            var command = BuildDbCommand(batch);
            return Executer.ExecuteNonQuery(command);
        }

        /// <summary>
        /// 执行sql语句，不返回任何结果集。
        /// </summary>
        /// <param name="batch">sql批次</param>
        /// <param name="transaction">事务处理</param>
        /// <returns>受影响的行数</returns>
        /// <remarks><paramref name="batch"/> 中包含增量标识的实体对象，不会返回自增序列的值。</remarks>
        public int ExecuteNonQuery(SqlBatch batch, DbTransaction transaction)
        {
            var command = BuildDbCommand(batch);
            return Executer.ExecuteNonQuery(command, transaction);
        }


        /// <summary>
        /// 执行存储过程，不返回结果集。
        /// </summary>
        /// <param name="commandText">存储过程</param>
        /// <returns>受影响的行数</returns>
        public int SprocExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(CommandType.StoredProcedure, commandText, (ParameterCollection)null);
        }

        /// <summary>
        /// 执行存储过程，不返回结果集。
        /// </summary>
        /// <param name="commandText">存储过程</param>
        /// <param name="parameters">存储过程的参数</param>
        /// <returns>受影响的行数</returns>
        public int SprocExecuteNonQuery(string commandText, ParameterCollection parameters)
        {
            return ExecuteNonQuery(CommandType.StoredProcedure, commandText, parameters);
        }

        /// <summary>
        /// 执行存储过程，不返回结果集。
        /// </summary>
        /// <param name="commandText">存储过程</param>
        /// <param name="parameters">存储过程的参数</param>
        /// <param name="transaction"> </param>
        /// <returns>受影响的行数</returns>
        public int SprocExecuteNonQuery(string commandText, ParameterCollection parameters, DbTransaction transaction)
        {
            return ExecuteNonQuery(CommandType.StoredProcedure, commandText, parameters, transaction);
        }
    }
}

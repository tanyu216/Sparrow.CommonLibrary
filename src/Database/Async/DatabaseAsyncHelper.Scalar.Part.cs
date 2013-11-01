using System;
using System.Data;
using System.Data.Common;

namespace Sparrow.CommonLibrary.Database.Async
{
    public static partial class DatabaseAsyncHelperExtensions
    {

        /// <summary>
        /// 执行sql语句，将标值返回。
        /// </summary>
        /// <param name="helper"> </param>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="commandText">sql语句</param>
        /// <returns></returns>
        public static IAsyncResult BeginExecuteScalar(this DatabaseHelper helper, AsyncCallback callback, object state, string commandText)
        {
            return helper.BeginExecuteScalar(callback, state, CommandType.Text, commandText, null);
        }

        /// <summary>
        /// 执行sql语句，将标值返回。
        /// </summary>
        /// <param name="helper"> </param>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql参数集合</param>
        /// <returns></returns>
        public static IAsyncResult BeginExecuteScalar(this DatabaseHelper helper, AsyncCallback callback, object state, string commandText, ParameterCollection parameters)
        {
            return helper.BeginExecuteScalar(callback, state, CommandType.Text, commandText, parameters);
        }

        /// <summary>
        /// 执行sql语句，将标值返回。
        /// </summary>
        /// <param name="helper"> </param>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql参数集合</param>
        /// <param name="transaction"> </param>
        /// <returns></returns>
        public static IAsyncResult BeginExecuteScalar(this DatabaseHelper helper, AsyncCallback callback, object state, string commandText, ParameterCollection parameters, DbTransaction transaction)
        {
            return helper.BeginExecuteScalar(callback, state, CommandType.Text, commandText, parameters, transaction);
        }

        /// <summary>
        /// 执行sql语句，将标值返回。
        /// </summary>
        /// <param name="helper"> </param>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="commandText">sql语句</param>
        /// <returns></returns>
        public static IAsyncResult BeginSprocExecuteScalar(this DatabaseHelper helper, AsyncCallback callback, object state, string commandText)
        {
            return helper.BeginExecuteScalar(callback, state, CommandType.StoredProcedure, commandText, null);
        }

        /// <summary>
        /// 执行存储过程，将标值返回。
        /// </summary>
        /// <param name="helper"> </param>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">存储过程的参数</param>
        /// <returns></returns>
        public static IAsyncResult BeginSprocExecuteScalar(this DatabaseHelper helper, AsyncCallback callback, object state, string commandText, ParameterCollection parameters)
        {
            return helper.BeginExecuteScalar(callback, state, CommandType.StoredProcedure, commandText, parameters);
        }

        /// <summary>
        /// 执行存储过程，将标值返回。
        /// </summary>
        /// <param name="helper"> </param>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="commandText">存储过程</param>
        /// <param name="parameters">存储过程的参数</param>
        /// <param name="transaction"> </param>
        /// <returns></returns>
        public static IAsyncResult BeginSprocExecuteScalar(this DatabaseHelper helper, AsyncCallback callback, object state, string commandText, ParameterCollection parameters, DbTransaction transaction)
        {
            return helper.BeginExecuteScalar(callback, state, CommandType.StoredProcedure, commandText, parameters, transaction);
        }
    }
}

﻿using System;
using System.Data;
using System.Data.Common;

namespace Sparrow.CommonLibrary.Data.Database.Async
{
    public static partial class DatabaseAsyncHelper
    {

        /// <summary>
        /// 执行sql语句，返回IDataReader。
        /// </summary>
        /// <param name="helper"> </param>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="commandText">sql语句</param>
        /// <returns></returns>
        public static IAsyncResult BeginExecuteReader(this DatabaseHelper helper, AsyncCallback callback, object state, string commandText)
        {
            return helper.BeginExecuteReader(callback, state, CommandType.Text, commandText, null);
        }

        /// <summary>
        /// 执行sql语句，返回IDataReader。
        /// </summary>
        /// <param name="helper"> </param>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <returns></returns>
        public static IAsyncResult BeginExecuteReader(this DatabaseHelper helper, AsyncCallback callback, object state, string commandText, ParameterCollection parameters)
        {
            return helper.BeginExecuteReader(callback, state, CommandType.Text, commandText, parameters);
        }

        /// <summary>
        /// 执行sql语句，返回IDataReader。
        /// </summary>
        /// <param name="helper"> </param>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <param name="transaction"> </param>
        /// <returns></returns>
        public static IAsyncResult BeginExecuteReader(this DatabaseHelper helper, AsyncCallback callback, object state, string commandText, ParameterCollection parameters, DbTransaction transaction)
        {
            return helper.BeginExecuteReader(callback, state, CommandType.Text, commandText, parameters, transaction);
        }

        /// <summary>
        /// 执行sql语句，返回IDataReader。
        /// </summary>
        /// <param name="helper"> </param>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="batch">sql批次</param>
        /// <returns></returns>
        /// <remarks><paramref name="batch"/> 中包含增量标识的实体对象，不会返回自增序列的值。</remarks>
        public static IAsyncResult BeginExecuteReader(this DatabaseHelper helper, AsyncCallback callback, object state, SqlBatch batch)
        {
            var command = helper.BuildDbCommand(batch);
            return helper.Executer.BeginExecuteReader(command, callback, state);
        }

        /// <summary>
        /// 执行sql语句，返回IDataReader。
        /// </summary>
        /// <param name="helper"> </param>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="batch">sql批次</param>
        /// <param name="transaction">事务处理</param>
        /// <returns></returns>
        /// <remarks><paramref name="batch"/> 中包含增量标识的实体对象，不会返回自增序列的值。</remarks>
        public static IAsyncResult BeginExecuteReader(this DatabaseHelper helper, AsyncCallback callback, object state, SqlBatch batch, DbTransaction transaction)
        {
            var command = helper.BuildDbCommand(batch);
            return helper.Executer.BeginExecuteReader(command, callback, state, transaction);
        }


        /// <summary>
        /// 执行存储过程，返回IDataReader。
        /// </summary>
        /// <param name="helper"> </param>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="commandText">存储过程</param>
        /// <returns></returns>
        public static IAsyncResult BeginSprocExecuteReader(this DatabaseHelper helper, AsyncCallback callback, object state, string commandText)
        {
            return helper.BeginExecuteReader(callback, state, CommandType.StoredProcedure, commandText, null);
        }

        /// <summary>
        /// 执行存储过程，返回IDataReader。
        /// </summary>
        /// <param name="helper"> </param>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="commandText">存储过程</param>
        /// <param name="parameters">存储过程的参数</param>
        /// <returns></returns>
        public static IAsyncResult BeginSprocExecuteReader(this DatabaseHelper helper, AsyncCallback callback, object state, string commandText, ParameterCollection parameters)
        {
            return helper.BeginExecuteReader(callback, state, CommandType.StoredProcedure, commandText, parameters);
        }

        /// <summary>
        /// 执行存储过程，返回IDataReader。
        /// </summary>
        /// <param name="helper"> </param>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="commandText">存储过程</param>
        /// <param name="parameters">存储过程的参数</param>
        /// <param name="transaction"> </param>
        /// <returns></returns>
        public static IAsyncResult BeginSprocExecuteReader(this DatabaseHelper helper, AsyncCallback callback, object state, string commandText, ParameterCollection parameters, DbTransaction transaction)
        {
            return helper.BeginExecuteReader(callback, state, CommandType.StoredProcedure, commandText, parameters, transaction);
        }
    }
}

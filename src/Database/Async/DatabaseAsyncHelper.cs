using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Sparrow.CommonLibrary.Entity;
using Sparrow.CommonLibrary.Database.SqlBuilder;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Mapper;
using Sparrow.CommonLibrary.Mapper.Metadata;

namespace Sparrow.CommonLibrary.Database.Async
{
    /// <summary>
    /// 辅助简化Sql语句、存储过程的异步执行，并支持实体与数据之间的映射。
    /// </summary>
    public static partial class DatabaseAsyncHelper
    {

        #region Basic

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="parameterCollection"></param>
        /// <param name="dbTransaction"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IAsyncResult BeginExecuteReader(this DatabaseHelper helper, AsyncCallback callback, object state, CommandType commandType, string commandText, ParameterCollection parameterCollection, DbTransaction dbTransaction = null)
        {
            var command = helper.BuildDbCommand(commandType, commandText, parameterCollection);
            if (dbTransaction == null)
                return helper.Executer.BeginExecuteReader(command, callback, state);
            //
            return helper.Executer.BeginExecuteReader(command, callback, state, dbTransaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="parameterCollection"></param>
        /// <param name="dbTransaction"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IAsyncResult BeginExecuteScalar(this DatabaseHelper helper, AsyncCallback callback, object state, CommandType commandType, string commandText, ParameterCollection parameterCollection, DbTransaction dbTransaction = null)
        {
            var command = helper.BuildDbCommand(commandType, commandText, parameterCollection);
            if (dbTransaction == null)
                return helper.Executer.BeginExecuteScalar(command, callback, state);
            //
            return helper.Executer.BeginExecuteScalar(command, callback, state, dbTransaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"> </param>
        /// <param name="state"> </param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="parameterCollection"></param>
        /// <param name="dbTransaction"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IAsyncResult BeginExecuteNonQuery(this DatabaseHelper helper, AsyncCallback callback, object state, CommandType commandType, string commandText, ParameterCollection parameterCollection, DbTransaction dbTransaction = null)
        {
            var command = helper.BuildDbCommand(commandType, commandText, parameterCollection);
            if (dbTransaction == null)
                return helper.Executer.BeginExecuteNonQuery(command, callback, state);
            //
            return helper.Executer.BeginExecuteNonQuery(command, callback, state, dbTransaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public static IDataReader EndExecuteReader(this DatabaseHelper helper, IAsyncResult asyncResult)
        {
            return helper.Executer.EndExecuteReader(asyncResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public static object EndExecuteScalar(this DatabaseHelper helper, IAsyncResult asyncResult)
        {
            return helper.Executer.EndExecuteScalar(asyncResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public static object EndExecuteScalar<T>(this DatabaseHelper helper, IAsyncResult asyncResult)
        {
            return Sparrow.CommonLibrary.Common.DbValueCast.Cast<T>(helper.Executer.EndExecuteScalar(asyncResult));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public static int EndExecuteNonQuery(this DatabaseHelper helper, IAsyncResult asyncResult)
        {
            return helper.Executer.EndExecuteNonQuery(asyncResult);
        }

        #endregion

    }
}

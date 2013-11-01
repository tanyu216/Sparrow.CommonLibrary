using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Xml;

namespace Sparrow.CommonLibrary.Database.DbCommon
{
    /// <summary>
    /// <paramref name="System.Data.Common.DbCommand"/> 执行器
    /// </summary>
    public interface ICommandExecuter
    {

        DbProvider DbProvider { get; }

        string ConnName { get; }

        ConnectionWrapper GetWrapperedConnection();

        DbConnection GetNotWrapperedConnection();

        #region Execute

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        IDataReader ExecuteReader(DbCommand command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        DataSet ExecuteDataSet(DbCommand command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        object ExecuteScalar(DbCommand command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        int ExecuteNonQuery(DbCommand command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dbTransaction"> </param>
        /// <returns></returns>
        IDataReader ExecuteReader(DbCommand command, DbTransaction dbTransaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dbTransaction"> </param>
        /// <returns></returns>
        DataSet ExecuteDataSet(DbCommand command, DbTransaction dbTransaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dbTransaction"> </param>
        /// <returns></returns>
        object ExecuteScalar(DbCommand command, DbTransaction dbTransaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dbTransaction"></param>
        /// <returns></returns>
        int ExecuteNonQuery(DbCommand command, DbTransaction dbTransaction);

        #endregion

        #region AsyncExecute

        /// <summary>
        /// 
        /// </summary>
        bool SupportsAsync { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="asyncCallback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        IAsyncResult BeginExecuteReader(DbCommand command, AsyncCallback asyncCallback, object state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="asyncCallback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        IAsyncResult BeginExecuteScalar(DbCommand command, AsyncCallback asyncCallback, object state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="asyncCallback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        IAsyncResult BeginExecuteNonQuery(DbCommand command, AsyncCallback asyncCallback, object state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="asyncCallback"></param>
        /// <param name="state"></param>
        /// <param name="dbTransaction"> </param>
        /// <returns></returns>
        IAsyncResult BeginExecuteReader(DbCommand command, AsyncCallback asyncCallback, object state, DbTransaction dbTransaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="asyncCallback"></param>
        /// <param name="state"></param>
        /// <param name="dbTransaction"> </param>
        /// <returns></returns>
        IAsyncResult BeginExecuteScalar(DbCommand command, AsyncCallback asyncCallback, object state, DbTransaction dbTransaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="asyncCallback"></param>
        /// <param name="state"></param>
        /// <param name="dbTransaction"></param>
        /// <returns></returns>
        IAsyncResult BeginExecuteNonQuery(DbCommand command, AsyncCallback asyncCallback, object state, DbTransaction dbTransaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        IDataReader EndExecuteReader(IAsyncResult asyncResult);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        object EndExecuteScalar(IAsyncResult asyncResult);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        int EndExecuteNonQuery(IAsyncResult asyncResult);

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;

namespace Sparrow.CommonLibrary.Database.DbCommon
{
    public class CommonExecuter : ICommandExecuter
    {
        public DbProvider DbProvider { get; private set; }
        public string ConnName { get; private set; }
        private readonly string _connectionString;

        internal protected CommonExecuter(string connName, string connectionString, DbProvider dbProvider)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            if (dbProvider == null)
                throw new ArgumentNullException("dbProvider");

            DbProvider = dbProvider;
            ConnName = connName;
            // test connection
            using (var conn = dbProvider.GetNotWrapperedConnection(connectionString))
            {
                _connectionString = connectionString;
            }
        }

        public ConnectionWrapper GetWrapperedConnection()
        {
            return DbProvider.GetWrapperedConnection(_connectionString);
        }

        public DbConnection GetNotWrapperedConnection()
        {
            return DbProvider.GetNotWrapperedConnection(_connectionString);
        }

        #region IDbCommandExecuter.Execute

        public System.Data.IDataReader ExecuteReader(System.Data.Common.DbCommand command)
        {
            using (var conn = GetWrapperedConnection())
            {
                PrepareCommand(command, conn);
                return new DataReaderWrapper(conn, command.ExecuteReader());
            }
        }

        public System.Data.DataSet ExecuteDataSet(System.Data.Common.DbCommand command)
        {
            using (var da = DbProvider.DbProviderFactory.CreateDataAdapter())
            {
                using (var conn = GetWrapperedConnection())
                {
                    PrepareCommand(command, conn);
                    da.SelectCommand = command;
                    var ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
        }

        public object ExecuteScalar(System.Data.Common.DbCommand command)
        {
            using (var conn = GetWrapperedConnection())
            {
                PrepareCommand(command, conn);
                return command.ExecuteScalar();
            }
        }

        public int ExecuteNonQuery(System.Data.Common.DbCommand command)
        {
            using (var conn = GetWrapperedConnection())
            {
                PrepareCommand(command, conn);
                return command.ExecuteNonQuery();
            }
        }

        public System.Data.IDataReader ExecuteReader(System.Data.Common.DbCommand command, System.Data.Common.DbTransaction dbTransaction)
        {
            PrepareCommand(command, dbTransaction);
            return command.ExecuteReader();
        }

        public System.Data.DataSet ExecuteDataSet(System.Data.Common.DbCommand command, System.Data.Common.DbTransaction dbTransaction)
        {
            PrepareCommand(command, dbTransaction);
            var da = DbProvider.DbProviderFactory.CreateDataAdapter();
            da.SelectCommand = command;
            var ds = new DataSet();
            da.Fill(ds);
            return ds;
        }

        public object ExecuteScalar(System.Data.Common.DbCommand command, System.Data.Common.DbTransaction dbTransaction)
        {
            PrepareCommand(command, dbTransaction);
            return command.ExecuteScalar();
        }

        public int ExecuteNonQuery(System.Data.Common.DbCommand command, System.Data.Common.DbTransaction dbTransaction)
        {
            PrepareCommand(command, dbTransaction);
            return command.ExecuteNonQuery();
        }

        #endregion

        #region IDbCommandExecuter.AsyncExecute

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 默认只有SqlServer受支持，如果想要支持其他数据库，需要继承类<see cref="CommonExecuter"/>并重写异步相关的方法。或重新实现接口<see cref=""/>IDbCommandExecuter。
        /// </remarks>
        public virtual bool SupportsAsync
        {
            get
            {
                // 默认只有SqlServer受支持，如果想要支持其他数据库，需要继承类CommandExecuter
                if (DbProvider.DbProviderFactory is System.Data.SqlClient.SqlClientFactory)
                    return true;
                return false;
            }
        }

        private void AsyncNotSupported()
        {
            throw new InvalidOperationException("异步操作不受支持。");
        }

        private IAsyncResult DoBeginExecute<T>(DbCommand command, DbTransaction dbTransaction, bool disposeCommand, bool closeConnection, Func<T, IAsyncResult> execute) where T : DbCommand
        {
            try
            {
                if (command is T)
                {
                    if (dbTransaction == null)
                        PrepareCommand(command, GetNotWrapperedConnection());
                    else
                        PrepareCommand(command, dbTransaction);
                    //
                    T cmd = (T)command;
                    var result = execute(cmd);
                    return new AsyncResult(result, command, disposeCommand, closeConnection);
                }
            }
            catch
            {
                if (command.Connection != null && command.Transaction == null)
                    command.Connection.Close();
                throw;
            }
            return null;
        }

        private bool DoEndExecute<T, TDoResult>(IAsyncResult asyncResult, Func<T, IAsyncResult, TDoResult> call, out TDoResult outResult) where T : DbCommand
        {
            var result = (AsyncResult)asyncResult;
            if (result.Command is T)
            {
                outResult = call((T)result.Command, result.InnerAsyncResult);
                return true;
            }
            outResult = default(TDoResult);
            return false;
        }

        private void CleanupAsyncResult(IAsyncResult asyncResult)
        {
            var result = (AsyncResult)asyncResult;
            if (result.DisposeCommand && result.Command != null)
                result.Command.Dispose();
            if (result.CloseConnection && result.Connection != null)
                result.Connection.Close();
        }

        public virtual IAsyncResult BeginExecuteReader(System.Data.Common.DbCommand command, AsyncCallback asyncCallback, object state)
        {
            IAsyncResult result = DoBeginExecute<SqlCommand>(
                command,
                null,
                false,
                false,
                x => x.BeginExecuteReader(asyncCallback, state, CommandBehavior.CloseConnection));

            if (result != null)
                return result;
            //
            AsyncNotSupported();
            return null;
        }

        public virtual IAsyncResult BeginExecuteScalar(System.Data.Common.DbCommand command, AsyncCallback asyncCallback, object state)
        {
            IAsyncResult result = DoBeginExecute<SqlCommand>(
                command,
                null,
                false,
                true,
                x => x.BeginExecuteReader(asyncCallback, state));

            if (result != null)
                return result;
            //
            AsyncNotSupported();
            return null;
        }

        public virtual IAsyncResult BeginExecuteNonQuery(System.Data.Common.DbCommand command, AsyncCallback asyncCallback, object state)
        {
            IAsyncResult result = DoBeginExecute<SqlCommand>(
                command,
                null,
                false,
                true,
                x => x.BeginExecuteNonQuery(asyncCallback, state));

            if (result != null)
                return result;
            //
            AsyncNotSupported();
            return null;
        }

        public virtual IAsyncResult BeginExecuteReader(System.Data.Common.DbCommand command, AsyncCallback asyncCallback, object state, System.Data.Common.DbTransaction dbTransaction)
        {
            var closeConnection = dbTransaction == null;
            IAsyncResult result = DoBeginExecute<SqlCommand>(
                command,
                null,
                false,
                closeConnection,
                x => x.BeginExecuteReader(asyncCallback, state, x.Transaction == null ? CommandBehavior.CloseConnection : CommandBehavior.Default));

            if (result != null)
                return result;
            //
            AsyncNotSupported();
            return null;
        }

        public virtual IAsyncResult BeginExecuteScalar(System.Data.Common.DbCommand command, AsyncCallback asyncCallback, object state, System.Data.Common.DbTransaction dbTransaction)
        {
            var closeConnection = dbTransaction == null;
            IAsyncResult result = DoBeginExecute<SqlCommand>(
                command,
                dbTransaction,
                false,
                closeConnection,
                x => x.BeginExecuteReader(asyncCallback, state));

            if (result != null)
                return result;
            //
            AsyncNotSupported();
            return null;
        }

        public virtual IAsyncResult BeginExecuteNonQuery(System.Data.Common.DbCommand command, AsyncCallback asyncCallback, object state, System.Data.Common.DbTransaction dbTransaction)
        {
            var closeConnection = dbTransaction == null;
            IAsyncResult result = DoBeginExecute<SqlCommand>(
                command,
                dbTransaction,
                false,
                closeConnection,
                x => x.BeginExecuteNonQuery(asyncCallback, state));

            if (result != null)
                return result;
            //
            AsyncNotSupported();
            return null;
        }

        public virtual System.Data.IDataReader EndExecuteReader(IAsyncResult asyncResult)
        {
            try
            {
                IDataReader outResult;
                if (DoEndExecute<SqlCommand, IDataReader>(asyncResult, (x, y) => x.EndExecuteReader(y), out outResult))
                {
                    return outResult;
                }
            }
            catch
            {
                //正常情况下：AsyncResult.CloseConnection==false，CleanupAsyncResult不会关闭连接
                var result = ((AsyncResult)asyncResult);
                if (result.Command.Transaction == null)
                    result.Command.Connection.Close();
                throw;
            }
            finally
            {
                CleanupAsyncResult(asyncResult);
            }
            //
            AsyncNotSupported();
            return null;
        }

        public virtual object EndExecuteScalar(IAsyncResult asyncResult)
        {
            try
            {
                IDataReader outResult;
                if (DoEndExecute<SqlCommand, IDataReader>(asyncResult, (x, y) => x.EndExecuteReader(y), out outResult))
                {
                    using (outResult)
                    {
                        if (!outResult.Read() || outResult.FieldCount == 0)
                            return null;
                        return outResult.GetValue(0);
                    }
                }
            }
            finally
            {
                CleanupAsyncResult(asyncResult);
            }
            //
            AsyncNotSupported();
            return null;
        }

        public virtual int EndExecuteNonQuery(IAsyncResult asyncResult)
        {
            try
            {
                int outResult;
                if (DoEndExecute<SqlCommand, int>(asyncResult, (x, y) => x.EndExecuteNonQuery(y), out outResult))
                    return outResult;
            }
            finally
            {
                CleanupAsyncResult(asyncResult);
            }
            //
            AsyncNotSupported();
            return 0;
        }

        #endregion

        #region Static

        protected static void PrepareCommand(DbCommand command, ConnectionWrapper connection)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (connection == null)
                throw new ArgumentNullException("connection");
            //
            command.Connection = connection.Connection;
        }

        protected static void PrepareCommand(DbCommand command, DbTransaction dbTransaction)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (dbTransaction == null)
                throw new ArgumentNullException("dbTransaction");
            //
            command.Connection = dbTransaction.Connection;
            command.Transaction = dbTransaction;
        }

        protected static void PrepareCommand(DbCommand command, DbConnection connection)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (connection == null)
                throw new ArgumentNullException("connection");
            //
            command.Connection = connection;
        }

        #endregion
    }
}

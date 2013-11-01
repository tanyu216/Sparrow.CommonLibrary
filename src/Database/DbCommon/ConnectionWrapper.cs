using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;

namespace Sparrow.CommonLibrary.Database.DbCommon
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class ConnectionWrapper : IDisposable
    {
        private int refCount;

        protected ConnectionWrapper(DbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            Connection = connection;
            refCount = 1;
        }

        public DbConnection Connection { get; private set; }

        public bool IsDisposed
        {
            get { return refCount == 0; }
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                int count = Interlocked.Decrement(ref refCount);
                if (count == 0)
                {
                    Connection.Dispose();
                    Connection = null;
                    GC.SuppressFinalize(this);
                }
            }
        }

        #endregion

        public ConnectionWrapper AddReference()
        {
            Interlocked.Increment(ref refCount);
            return this;
        }

        #region static

        private static readonly ConcurrentDictionary<Transaction, Dictionary<string, ConnectionWrapper>> tranConnections;

        static ConnectionWrapper()
        {
            tranConnections = new ConcurrentDictionary<Transaction, Dictionary<string, ConnectionWrapper>>();
        }

        private static DbConnection CreateConnection(string connectionString, DbProviderFactory dbProvider)
        {
            DbConnection conn = null;
            try
            {
                conn = dbProvider.CreateConnection();
                conn.ConnectionString = connectionString;
                conn.Open();
            }
            catch
            {
                if (conn != null)
                    conn.Close();
                throw;
            }
            return conn;
        }

        public static ConnectionWrapper GetConnection(string connectionString, DbProviderFactory dbProvider)
        {
            Transaction currentTran = Transaction.Current;
            if (currentTran == null)
                return new ConnectionWrapper(CreateConnection(connectionString, dbProvider));

            // tranConnections是一个多线程安全的字典对象，尽量必免使用锁带来的性能损耗。
            Dictionary<string, ConnectionWrapper> connWrappers =
                tranConnections.GetOrAdd(currentTran, x =>
                                                        {
                                                            x.TransactionCompleted += OnTransactionCompleted;
                                                            return new Dictionary<string, ConnectionWrapper>();
                                                        });

            // connWrappers通常不会在并发环境下操作，也就不会发生锁竞争，对于锁带来的性能损耗可以忽略。此处上一把锁主要是为了安全起见。
            ConnectionWrapper connWrapper;
            lock (connWrappers)
            {
                if (!connWrappers.TryGetValue(connectionString, out connWrapper))
                {
                    connWrapper = new ConnectionWrapper(CreateConnection(connectionString, dbProvider));
                    connWrappers.Add(connectionString, connWrapper);
                }
                connWrapper.AddReference();
            }

            return connWrapper;
        }

        private static void OnTransactionCompleted(object sender, TransactionEventArgs e)
        {
            Dictionary<string, ConnectionWrapper> connWrappers;

            if (!tranConnections.TryRemove(e.Transaction, out connWrappers))
                return;

            lock (connWrappers)
            {
                foreach (var connWrapper in connWrappers.Values)
                {
                    connWrapper.Dispose();
                }
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Database.DbCommon
{
    public class AsyncResult : IAsyncResult
    {
        public AsyncResult(IAsyncResult innerAsyncResult, DbCommand command, bool disposeCommand, bool closeConnection)
        {
            _innerAsyncResult = innerAsyncResult;
            _command = command;
            _disposeCommand = disposeCommand;
            _closeConnection = closeConnection;
        }

        #region IAsyncResult

        public object AsyncState
        {
            get { return _innerAsyncResult.AsyncState; }
        }

        public System.Threading.WaitHandle AsyncWaitHandle
        {
            get { return _innerAsyncResult.AsyncWaitHandle; }
        }

        public bool CompletedSynchronously
        {
            get { return _innerAsyncResult.CompletedSynchronously; }
        }

        public bool IsCompleted
        {
            get { return _innerAsyncResult.IsCompleted; }
        }

        #endregion

        private readonly IAsyncResult _innerAsyncResult;
        public IAsyncResult InnerAsyncResult
        {
            get { return _innerAsyncResult; }
        }

        private readonly bool _disposeCommand;
        public bool DisposeCommand
        {
            get { return _disposeCommand; }
        }

        private readonly DbCommand _command;
        public DbCommand Command
        {
            get { return _command; }
        }

        private readonly bool _closeConnection;
        public bool CloseConnection
        {
            get { return _closeConnection; }
        }

        public DbConnection Connection
        {
            get { return _command.Connection; }
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Database.DbCommon
{
    /// <summary>
    /// 数据库驱动
    /// </summary>
    public class DbProvider
    {
        private static readonly ConcurrentDictionary<string, DbProvider> dbProviders;
        /// <summary>
        /// 
        /// </summary>
        public DbProviderFactory DbProviderFactory { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProviderName { get; private set; }

        static DbProvider()
        {
            dbProviders = new ConcurrentDictionary<string, DbProvider>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="factory"></param>
        protected DbProvider(string providerName, DbProviderFactory factory)
        {
            this.ProviderName = providerName;
            this.DbProviderFactory = factory;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static DbProviderFactory GetDbProviderFactory(string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException("providerName");
            //
            var provider = DbProviderFactories.GetFactory(providerName);
            if (provider == null)
                throw new NotSupportedException(string.Format("不受支持的提供程序：{0}。", providerName));
            return provider;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static DbProvider GetDbProvider(string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException("providerName");

            return dbProviders.GetOrAdd(providerName, x => new DbProvider(x, GetDbProviderFactory(x)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public ConnectionWrapper GetWrapperedConnection(string connectionString)
        {
            return ConnectionWrapper.GetConnection(connectionString, DbProviderFactory);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public DbConnection GetNotWrapperedConnection(string connectionString)
        {
            var conn = DbProviderFactory.CreateConnection();
            conn.ConnectionString = connectionString;
            try
            {
                conn.Open();
                return conn;
            }
            catch
            {
                conn.Dispose();
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DbParameter CreateParameter()
        {
            return DbProviderFactory.CreateParameter();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public DbCommand CreateDbCommand(System.Data.CommandType commandType)
        {
            var cmd = DbProviderFactory.CreateCommand();
            cmd.CommandType = commandType;
            return cmd;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public DbCommand CreateDbCommand(System.Data.CommandType commandType, string commandText)
        {
            var cmd = DbProviderFactory.CreateCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = commandText;
            return cmd;
        }

    }
}

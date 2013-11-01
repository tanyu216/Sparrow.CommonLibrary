using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Database.DbCommon
{
    public class DbProvider
    {
        private static readonly ConcurrentDictionary<string, DbProvider> dbProviders;

        public DbProviderFactory DbProviderFactory { get; private set; }

        public string ProviderName { get; private set; }

        static DbProvider()
        {
            dbProviders = new ConcurrentDictionary<string, DbProvider>();
        }

        protected DbProvider(string providerName, DbProviderFactory factory)
        {
            this.ProviderName = providerName;
            this.DbProviderFactory = factory;
        }

        public static DbProviderFactory GetDbProviderFactory(string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException("providerName");
            //
            foreach (DataRow row in DbProviderFactories.GetFactoryClasses().Rows)
            {
                if (row[2].ToString() == providerName)
                {
                    var type = Type.GetType(row[3].ToString());
                    return (DbProviderFactory)type.GetField("Instance").GetValue(null);
                }
            }
            throw new NotSupportedException(string.Format("不受支持的提供程序：{0}。", providerName));
        }

        public static DbProvider GetDbProvider(string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException("providerName");

            return dbProviders.GetOrAdd(providerName, x => new DbProvider(x, GetDbProviderFactory(x)));
        }

        public ConnectionWrapper GetWrapperedConnection(string connectionString)
        {
            return ConnectionWrapper.GetConnection(connectionString, DbProviderFactory);
        }

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

        public DbParameter CreateParameter()
        {
            return DbProviderFactory.CreateParameter();
        }

        public DbCommand CreateDbCommand(System.Data.CommandType commandType)
        {
            var cmd = DbProviderFactory.CreateCommand();
            cmd.CommandType = commandType;
            return cmd;
        }

        public DbCommand CreateDbCommand(System.Data.CommandType commandType, string commandText)
        {
            var cmd = DbProviderFactory.CreateCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = commandText;
            return cmd;
        }

    }
}

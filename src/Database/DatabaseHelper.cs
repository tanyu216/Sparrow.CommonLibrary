using Sparrow.CommonLibrary.Database.DbCommon;
using Sparrow.CommonLibrary.Entity;
using Sparrow.CommonLibrary.Mapper;
using Sparrow.CommonLibrary.Mapper.Metadata;
using Sparrow.CommonLibrary.Query;
using Sparrow.CommonLibrary.Database.SqlBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Sparrow.CommonLibrary.Common;
using System.Configuration;

namespace Sparrow.CommonLibrary.Database
{
    /// <summary>
    /// 辅助简化Sql语句、存储过程的执行，并支持实体与数据之间的映射。
    /// </summary>
    public partial class DatabaseHelper
    {
        #region Properties

        private string _connectionString;
        public DbProvider DbProvider { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ISqlBuilder Builder { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public EntityToSqlStatement EntityToSql { get; private set; }

        #endregion

        #region ctor/init

        static DatabaseHelper()
        {
            _databaseHelpers = new ConcurrentDictionary<string, DatabaseHelper>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="dbProvider">数据库操作对象提供器</param>
        /// <param name="builder">sql语句生成</param>
        protected DatabaseHelper()
        {
        }

        private void Init(string connectionString, DbProvider dbProvider, ISqlBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException("connectionString");
            if (dbProvider == null)
                throw new ArgumentNullException("dbProvider");
            if (builder == null)
                throw new ArgumentNullException("builder");

            _connectionString = connectionString;
            DbProvider = dbProvider;
            Builder = builder;
            EntityToSql = EntityToSqlStatement.Create(Builder);
        }

        #endregion

        #region Build/Create

        /// <summary>
        /// 生成command对象
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="parameterCollection"></param>
        /// <returns></returns>
        internal protected DbCommand BuildDbCommand(CommandType commandType, string commandText, ParameterCollection parameterCollection)
        {
            if (commandText == null) throw new ArgumentNullException("commandText");
            var command = DbProvider.CreateDbCommand(commandType, commandText);

            //
            if (parameterCollection != null)
                command.Parameters.AddRange(parameterCollection.ToArray());
            //
            return command;
        }

        /// <summary>
        /// 生成command对象
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="parameterCollection"></param>
        /// <returns></returns>
        internal protected DbCommand BuildDbCommand(CommandType commandType, string commandText, object[] parameters)
        {
            if (commandText == null) throw new ArgumentNullException("commandText");

            var plist = CreateParamterCollection(parameters.Length);
            var pnames = plist.Fill(parameters);
            var sql = string.Format(commandText, pnames);
            return BuildDbCommand(commandType, sql, plist);
        }

        /// <summary>
        /// 生成command对象
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        internal protected DbCommand BuildDbCommand(SqlBatch batch)
        {
            var sql = new StringBuilder();
            var paramters = CreateParamterCollection();
            EntityExplain entityExplain = null;
            //统一生成sql之后统一去执行
            foreach (var item in batch)
            {
                if (item.ItemType == ItemCommandType.Text)
                {
                    var sqlStat = (string)item.Command;
                    if (item.Parameters == null || item.Parameters.Length == 0)
                    {
                        sql.Append(sqlStat).AppendLine();
                    }
                    else
                    {
                        var paras = paramters.Fill(item.Parameters);
                        sql.AppendFormat(sqlStat, paras).AppendLine();
                    }
                    continue;
                }

                // 实体转换成sql
                var entity = item.Command;
                var entyExpl = entity as IEntityExplain;
                if (entyExpl == null)
                {
                    if (entityExplain == null)
                        entityExplain = new EntityExplain(entity);
                    else
                        entityExplain.Switch(entity);
                }
                bool hasIncrement;
                var innerSql = EntityToSql.GenerateInsertOrUpdate(entyExpl ?? entityExplain, paramters, false, out hasIncrement);
                if (string.IsNullOrEmpty(innerSql))
                    continue;

                sql.AppendLine(innerSql);
            }
            // 没有sql语句，则直接退出。
            if (sql.Length == 0)
                return null;
            //
            return BuildDbCommand(CommandType.Text, sql.ToString(), paramters);
        }

        /// <summary>
        /// 创建一个参数集合的实例对象
        /// </summary>
        /// <returns></returns>
        public virtual ParameterCollection CreateParamterCollection()
        {
            return new ParameterCollection(Builder, DbProvider);
        }

        /// <summary>
        /// 创建一个参数集合的实例对象
        /// </summary>
        /// <param name="capacity"></param>
        /// <returns></returns>
        public virtual ParameterCollection CreateParamterCollection(int capacity)
        {
            return new ParameterCollection(Builder, DbProvider, capacity);
        }

        #endregion

        #region Connection

        public ConnectionWrapper GetWrapperedConnection()
        {
            return DbProvider.GetWrapperedConnection(_connectionString);
        }

        public DbConnection GetNotWrapperedConnection()
        {
            return DbProvider.GetNotWrapperedConnection(_connectionString);
        }

        #endregion

        #region Execute

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual System.Data.IDataReader ExecuteReader(System.Data.Common.DbCommand command)
        {
            using (var conn = GetWrapperedConnection())
            {
                PrepareCommand(command, conn);
                return new DataReaderWrapper(conn, command.ExecuteReader());
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual System.Data.DataSet ExecuteDataSet(System.Data.Common.DbCommand command)
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

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual object ExecuteScalar(System.Data.Common.DbCommand command)
        {
            using (var conn = GetWrapperedConnection())
            {
                PrepareCommand(command, conn);
                return command.ExecuteScalar();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual int ExecuteNonQuery(System.Data.Common.DbCommand command)
        {
            using (var conn = GetWrapperedConnection())
            {
                PrepareCommand(command, conn);
                return command.ExecuteNonQuery();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual System.Data.IDataReader ExecuteReader(System.Data.Common.DbCommand command, System.Data.Common.DbTransaction dbTransaction)
        {
            PrepareCommand(command, dbTransaction);
            return command.ExecuteReader();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual System.Data.DataSet ExecuteDataSet(System.Data.Common.DbCommand command, System.Data.Common.DbTransaction dbTransaction)
        {
            PrepareCommand(command, dbTransaction);
            var da = DbProvider.DbProviderFactory.CreateDataAdapter();
            da.SelectCommand = command;
            var ds = new DataSet();
            da.Fill(ds);
            return ds;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual object ExecuteScalar(System.Data.Common.DbCommand command, System.Data.Common.DbTransaction dbTransaction)
        {
            PrepareCommand(command, dbTransaction);
            return command.ExecuteScalar();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual int ExecuteNonQuery(System.Data.Common.DbCommand command, System.Data.Common.DbTransaction dbTransaction)
        {
            PrepareCommand(command, dbTransaction);
            return command.ExecuteNonQuery();
        }

        #endregion

        #region PrepareCommand

        internal protected static void PrepareCommand(DbCommand command, ConnectionWrapper connection)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (connection == null)
                throw new ArgumentNullException("connection");
            //
            command.Connection = connection.Connection;
        }

        internal protected static void PrepareCommand(DbCommand command, DbTransaction dbTransaction)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (dbTransaction == null)
                throw new ArgumentNullException("dbTransaction");
            //
            command.Connection = dbTransaction.Connection;
            command.Transaction = dbTransaction;
        }

        internal protected static void PrepareCommand(DbCommand command, DbConnection connection)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (connection == null)
                throw new ArgumentNullException("connection");
            //
            command.Connection = connection;
        }

        #endregion

        #region Hide BaseMethod

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private new bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private new int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private new string ToString()
        {
            return base.ToString();
        }

        #endregion

        #region Decrypt

        private static Func<string, string> decrypter;

        /// <summary>
        /// 自定义链接字符串解密方法
        /// </summary>
        /// <param name="decrypter">解密方法</param>
        public static void SetDecrypter(Func<string, string> decrypter)
        {
            DatabaseHelper.decrypter = decrypter;
        }

        /// <summary>
        /// 链接字符串解密
        /// </summary>
        /// <param name="connectionString">密文</param>
        /// <returns>解密成功后的明文</returns>
        private static string Decrypt(string connectionString)
        {
            if (decrypter != null)
                return decrypter(connectionString);
            return connectionString;
        }

        #endregion

        #region GetDatabaseHelper

        private static readonly ConcurrentDictionary<string, DatabaseHelper> _databaseHelpers;

        private static ISqlBuilder GetISqlBuilder(string connName, string providerName)
        {
            if (!string.IsNullOrEmpty(connName))
            {
                var builder = Configuration.DatabaseSettings.Settings.GetISqlBuilder(connName);
                if (builder != null)
                    return builder;
            }
            if (!string.IsNullOrEmpty(providerName))
            {
                var builder = Configuration.DatabaseSettings.Settings.GetISqlBuilder(providerName);
                if (builder != null)
                    return builder;
            }
            throw new System.Configuration.ConfigurationErrorsException(string.Format("未配置{0}/{1}对应的{2}实例。", connName, providerName, typeof(ISqlBuilder)));
        }

        private static DatabaseHelper CreateDatabaseHelper(string connName, string connString, string providerName)
        {
            var dbProvider = DbProvider.GetDbProvider(providerName);
            DatabaseHelper db = null;
            Type dbType = null;
            if (!string.IsNullOrEmpty(connName))
            {
                dbType = Configuration.DatabaseSettings.Settings.GetDatabaseHelperType(connName);
            }
            if (dbType == null && !string.IsNullOrEmpty(providerName))
            {
                dbType = Configuration.DatabaseSettings.Settings.GetDatabaseHelperType(providerName);
            }
            if (dbType != null)
            {
                db = (DatabaseHelper)Activator.CreateInstance(dbType);
            }

            if (db == null)
            {
                db = new DatabaseHelper();
            }
            db.Init(Decrypt(connString), dbProvider, GetISqlBuilder(connName, dbProvider.ProviderName));
            return db;
        }

        public static DatabaseHelper GetHelper(string connectionName)
        {
            return _databaseHelpers.GetOrAdd(connectionName, x =>
            {
                var connString = ConfigurationManager.ConnectionStrings[connectionName];

                if (connString == null)
                    throw new ConfigurationErrorsException(string.Format("未找到{0}连接字符串配置。", connectionName));
                if (string.IsNullOrEmpty(connString.ConnectionString))
                    throw new ConfigurationErrorsException(string.Format("{0}未配置连接字符串。", connectionName));
                if (string.IsNullOrEmpty(connString.ProviderName))
                    throw new ConfigurationErrorsException("连接字符串未配置providerName。");

                return CreateDatabaseHelper(connectionName, connString.ConnectionString, connString.ProviderName);
            });
        }

        public static DatabaseHelper GetHelper(string connectionString, string providerName)
        {
            return _databaseHelpers.GetOrAdd(connectionString, x =>
            {
                if (string.IsNullOrEmpty(connectionString))
                    throw new ArgumentNullException("connectionString");
                if (string.IsNullOrEmpty(providerName))
                    throw new ArgumentNullException("providerName");

                return CreateDatabaseHelper(null, connectionString, providerName);
            });
        }

        #endregion
    }
}
